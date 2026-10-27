#include "EthVeniWorldData.h"
#include "EthVeni.h"
#include "JsonObjectConverter.h"
#include "Misc/Paths.h"
#include "Misc/FileHelper.h"
#include "HAL/PlatformFileManager.h"

void UEthVeniWorldData::Initialize(FSubsystemCollectionBase &Collection)
{
	Super::Initialize(Collection);
	UE_LOG(LogEthVeni, Log, TEXT("UEthVeniWorldData initialized"));
}

bool UEthVeniWorldData::LoadWorldDataFromContent()
{
	const FString FilePath = FPaths::Combine(FPaths::ProjectContentDir(), TEXT("GoonZuGame/goonzu_world.json"));
	FString Json;
	if (!FFileHelper::LoadFileToString(Json, *FilePath))
	{
		UE_LOG(LogEthVeni, Warning, TEXT("Could not read world data: %s"), *FilePath);
		return false;
	}
	return LoadWorldData(Json);
}

bool UEthVeniWorldData::LoadWorldData(const FString &InJsonString)
{
	TSharedPtr<FJsonObject> Root;
	TSharedRef<class TJsonReader<>> Reader = TJsonReaderFactory<>::Create(InJsonString);
	if (!FJsonSerializer::Deserialize(Reader, Root) || !Root.IsValid())
	{
		UE_LOG(LogEthVeni, Error, TEXT("goonzu_world.json is not valid JSON"));
		return false;
	}

	Skills.Reset();
	Towns.Reset();
	Recipes.Reset();

	// Skills
	const TArray<TSharedPtr<FJsonValue>> *SkillArr = nullptr;
	if (Root->TryGetArrayField(TEXT("skills"), SkillArr) && SkillArr)
	{
		for (const TSharedPtr<FJsonValue> &V : *SkillArr)
		{
			const TSharedPtr<FJsonObject> O = V->AsObject();
			if (!O)
				continue;
			FSkill S;
			O->TryGetStringField(TEXT("id"), S.Id);
			O->TryGetStringField(TEXT("name"), S.Name);
			O->TryGetStringField(TEXT("category"), S.Category);
			double Cap = 0;
			if (O->TryGetNumberField(TEXT("basicCap"), Cap))
				S.BasicCap = (int32)Cap;
			if (O->TryGetNumberField(TEXT("experienceCap"), Cap))
				S.ExperienceCap = (int32)Cap;
			Skills.Add(S);
		}
	}

	// Towns
	const TArray<TSharedPtr<FJsonValue>> *TownArr = nullptr;
	if (Root->TryGetArrayField(TEXT("towns"), TownArr) && TownArr)
	{
		for (const TSharedPtr<FJsonValue> &V : *TownArr)
		{
			const TSharedPtr<FJsonObject> O = V->AsObject();
			if (!O)
				continue;
			FTown T;
			double Num = 0;
			if (O->TryGetNumberField(TEXT("id"), Num))
				T.Id = (int32)Num;
			O->TryGetStringField(TEXT("name"), T.Name);
			O->TryGetStringField(TEXT("biome"), T.Biome);
			if (O->TryGetNumberField(TEXT("salesTax"), Num))
				T.SalesTax = (float)Num;
			if (O->TryGetNumberField(TEXT("treasury"), Num))
				T.Treasury = (int32)Num;
			Towns.Add(T);
		}
	}

	// Recipes
	const TArray<TSharedPtr<FJsonValue>> *RecipeArr = nullptr;
	if (Root->TryGetArrayField(TEXT("recipes"), RecipeArr) && RecipeArr)
	{
		for (const TSharedPtr<FJsonValue> &V : *RecipeArr)
		{
			const TSharedPtr<FJsonObject> O = V->AsObject();
			if (!O)
				continue;
			FRecipe R;
			O->TryGetStringField(TEXT("id"), R.Id);
			O->TryGetStringField(TEXT("output"), R.Output);
			O->TryGetStringField(TEXT("skill"), R.Skill);
			double Num = 0;
			if (O->TryGetNumberField(TEXT("difficulty"), Num))
				R.Difficulty = (int32)Num;
			const TSharedPtr<FJsonObject> *Mats;
			if (O->TryGetObjectField(TEXT("materials"), Mats) && Mats->IsValid())
			{
				for (const TPair<FString, TSharedPtr<FJsonValue>> &Pair : (*Mats)->Values)
				{
					if (Pair.Value.IsValid() && Pair.Value->Type == EJson::Number)
					{
						R.Materials.Add(Pair.Key, (int32)Pair.Value->AsNumber());
					}
				}
			}
			Recipes.Add(MoveTemp(R));
		}
	}

	UE_LOG(LogEthVeni, Log, TEXT("Loaded world data: %d skills, %d towns, %d recipes"), Skills.Num(), Towns.Num(), Recipes.Num());
	return true;
}
