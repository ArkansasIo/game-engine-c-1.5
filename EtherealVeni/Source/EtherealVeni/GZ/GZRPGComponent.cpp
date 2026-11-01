#include "GZRPGComponent.h"
#include "GZAudioSubsystem.h"
#include "EthVeni/EthVeni.h"
#include "Dom/JsonObject.h"
#include "Serialization/JsonReader.h"
#include "Serialization/JsonSerializer.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "Engine/GameInstance.h"

UGZRPGComponent::UGZRPGComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
	UnknownItem.Id = TEXT("unknown");
	UnknownItem.Name = TEXT("Unknown");
	UnknownItem.Type = TEXT("material");
}

FString UGZRPGComponent::Pretty(const FString& Id)
{
	FString Out = Id.Replace(TEXT("_"), TEXT(" "));
	bool bCap = true;
	for (int32 i = 0; i < Out.Len(); ++i)
	{
		if (bCap && FChar::IsAlpha(Out[i]))
		{
			Out[i] = FChar::ToUpper(Out[i]);
			bCap = false;
		}
		if (Out[i] == TEXT(' '))
		{
			bCap = true;
		}
	}
	return Out;
}

void UGZRPGComponent::SetupItems()
{
	auto Add = [this](const TCHAR* Id, const TCHAR* Type, int32 Price, FLinearColor Color, float Heal = 0.f, float Mana = 0.f, int32 Atk = 0, int32 Def = 0)
	{
		FGZItemDef D;
		D.Id = Id;
		D.Name = Pretty(Id);
		D.Type = Type;
		D.BasePrice = Price;
		D.Color = Color;
		D.Heal = Heal;
		D.Mana = Mana;
		D.Attack = Atk;
		D.Defense = Def;
		Items.Add(D.Id, D);
	};
	const FLinearColor Ore(0.45f, 0.42f, 0.40f), Metal(0.70f, 0.72f, 0.78f), Wood(0.55f, 0.36f, 0.18f), Herb(0.25f, 0.65f, 0.25f),
		Food(0.85f, 0.65f, 0.30f), Potion(0.85f, 0.20f, 0.30f), Weapon(0.35f, 0.55f, 0.95f), Armor(0.55f, 0.45f, 0.80f), Gem(0.30f, 0.85f, 0.90f);

	Add(TEXT("iron_ore"), TEXT("material"), 8, Ore);
	Add(TEXT("iron_ingot"), TEXT("material"), 22, Metal);
	Add(TEXT("steel_ingot"), TEXT("material"), 48, Metal * 1.1f);
	Add(TEXT("oak_log"), TEXT("material"), 7, Wood);
	Add(TEXT("wood_handle"), TEXT("material"), 11, Wood * 1.2f);
	Add(TEXT("oak_handle"), TEXT("material"), 19, Wood * 1.4f);
	Add(TEXT("leather_strip"), TEXT("material"), 9, FLinearColor(0.6f, 0.4f, 0.25f));
	Add(TEXT("red_herb"), TEXT("material"), 5, Herb);
	Add(TEXT("herbal_extract"), TEXT("material"), 15, Herb * 1.3f);
	Add(TEXT("clear_water"), TEXT("material"), 2, FLinearColor(0.35f, 0.6f, 0.95f));
	Add(TEXT("grain"), TEXT("material"), 3, Food);
	Add(TEXT("salt"), TEXT("material"), 2, FLinearColor(0.9f, 0.9f, 0.9f));
	Add(TEXT("fish"), TEXT("material"), 6, FLinearColor(0.4f, 0.7f, 0.85f));
	Add(TEXT("crystal_gem"), TEXT("material"), 120, Gem);
	Add(TEXT("healing_tonic"), TEXT("consumable"), 30, Potion, 70.f, 0.f);
	Add(TEXT("mana_draught"), TEXT("consumable"), 34, FLinearColor(0.3f, 0.35f, 0.95f), 0.f, 50.f);
	Add(TEXT("traveler_bread"), TEXT("consumable"), 12, Food, 30.f, 10.f);
	Add(TEXT("grilled_fish"), TEXT("consumable"), 18, Food * 1.1f, 45.f, 0.f);
	Add(TEXT("iron_sword"), TEXT("weapon"), 140, Weapon, 0, 0, 12, 0);
	Add(TEXT("steel_sword"), TEXT("weapon"), 340, Weapon * 1.2f, 0, 0, 26, 0);
	Add(TEXT("hunting_bow"), TEXT("weapon"), 160, Wood * 1.5f, 0, 0, 14, 0);
	Add(TEXT("leather_armor"), TEXT("armor"), 120, Armor, 0, 0, 0, 8);
	Add(TEXT("iron_armor"), TEXT("armor"), 320, Armor * 1.2f, 0, 0, 0, 18);

	MarketGoods = {TEXT("clear_water"), TEXT("grain"), TEXT("salt"), TEXT("iron_ore"), TEXT("oak_log"), TEXT("red_herb"), TEXT("leather_strip"),
		TEXT("iron_ingot"), TEXT("healing_tonic"), TEXT("mana_draught"), TEXT("traveler_bread"), TEXT("iron_sword"), TEXT("hunting_bow"), TEXT("leather_armor"), TEXT("crystal_gem")};
}

void UGZRPGComponent::LoadWorldJson()
{
	const FString Path = FPaths::Combine(FPaths::ProjectContentDir(), TEXT("GoonZuGame/goonzu_world.json"));
	FString Json;
	TSharedPtr<FJsonObject> Root;
	if (!FFileHelper::LoadFileToString(Json, *Path))
	{
		UE_LOG(LogEthVeni, Warning, TEXT("GoonZu: could not read %s"), *Path);
	}
	else
	{
		TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(Json);
		if (!FJsonSerializer::Deserialize(Reader, Root))
		{
			Root.Reset();
		}
	}

	if (Root.IsValid())
	{
		const TSharedPtr<FJsonObject>* WorldObj = nullptr;
		if (Root->TryGetObjectField(TEXT("world"), WorldObj) && WorldObj)
		{
			(*WorldObj)->TryGetStringField(TEXT("displayName"), WorldName);
		}

		const TArray<TSharedPtr<FJsonValue>>* Arr = nullptr;
		if (Root->TryGetArrayField(TEXT("attributes"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				AttributeNames.Add(Pretty(V->AsString()));
			}
		}
		if (Root->TryGetArrayField(TEXT("skills"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				const TSharedPtr<FJsonObject> O = V->AsObject();
				if (!O.IsValid()) continue;
				FGZSkillState S;
				O->TryGetStringField(TEXT("id"), S.Id);
				O->TryGetStringField(TEXT("name"), S.Name);
				O->TryGetStringField(TEXT("category"), S.Category);
				double N = 0;
				if (O->TryGetNumberField(TEXT("basicCap"), N)) S.BasicCap = (int32)N;
				if (O->TryGetNumberField(TEXT("experienceCap"), N)) S.Cap = (int32)N;
				Skills.Add(S);
			}
		}
		if (Root->TryGetArrayField(TEXT("towns"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				const TSharedPtr<FJsonObject> O = V->AsObject();
				if (!O.IsValid()) continue;
				FGZTownInfo T;
				double N = 0;
				if (O->TryGetNumberField(TEXT("id"), N)) T.Id = (int32)N;
				O->TryGetStringField(TEXT("name"), T.Name);
				O->TryGetStringField(TEXT("biome"), T.Biome);
				if (O->TryGetNumberField(TEXT("salesTax"), N)) T.SalesTax = (float)N;
				if (O->TryGetNumberField(TEXT("treasury"), N)) T.Treasury = (int64)N;
				Towns.Add(T);
			}
		}
		if (Root->TryGetArrayField(TEXT("recipes"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				const TSharedPtr<FJsonObject> O = V->AsObject();
				if (!O.IsValid()) continue;
				FGZRecipe R;
				O->TryGetStringField(TEXT("id"), R.Id);
				O->TryGetStringField(TEXT("output"), R.Output);
				O->TryGetStringField(TEXT("skill"), R.Skill);
				double N = 0;
				if (O->TryGetNumberField(TEXT("difficulty"), N)) R.Difficulty = (int32)N;
				const TSharedPtr<FJsonObject>* Mats = nullptr;
				if (O->TryGetObjectField(TEXT("materials"), Mats) && Mats)
				{
					for (const auto& Pair : (*Mats)->Values)
					{
						R.Materials.Add(TPair<FString, int32>(Pair.Key, (int32)Pair.Value->AsNumber()));
					}
				}
				Recipes.Add(R);
			}
		}
		if (Root->TryGetArrayField(TEXT("resourceChains"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				const TSharedPtr<FJsonObject> O = V->AsObject();
				if (!O.IsValid()) continue;
				FGZRecipe R;
				FString Raw;
				O->TryGetStringField(TEXT("raw"), Raw);
				O->TryGetStringField(TEXT("processed"), R.Output);
				O->TryGetStringField(TEXT("skill"), R.Skill);
				R.Id = TEXT("process_") + R.Output;
				R.Difficulty = 2;
				R.Materials.Add(TPair<FString, int32>(Raw, 2));
				R.bProcessing = true;
				Recipes.Insert(R, 0);
			}
		}
		if (Root->TryGetArrayField(TEXT("offices"), Arr))
		{
			for (const TSharedPtr<FJsonValue>& V : *Arr)
			{
				const TSharedPtr<FJsonObject> O = V->AsObject();
				if (!O.IsValid()) continue;
				FString Id, Scope;
				double Days = 0;
				O->TryGetStringField(TEXT("id"), Id);
				O->TryGetStringField(TEXT("scope"), Scope);
				O->TryGetNumberField(TEXT("termDays"), Days);
				OfficeLines.Add(FString::Printf(TEXT("%s  (%s office, %d-day term)"), *Pretty(Id), *Scope, (int32)Days));
			}
		}
		const TSharedPtr<FJsonObject>* Econ = nullptr;
		if (Root->TryGetObjectField(TEXT("economy"), Econ) && Econ)
		{
			(*Econ)->TryGetStringField(TEXT("currency"), Currency);
			(*Econ)->TryGetStringField(TEXT("priceFormula"), PriceFormula);
		}
	}

	// Fallbacks so the game is always playable.
	if (AttributeNames.Num() == 0)
	{
		AttributeNames = {TEXT("Strength"), TEXT("Agility"), TEXT("Vitality"), TEXT("Wisdom"), TEXT("Dexterity"), TEXT("Luck")};
	}
	if (Towns.Num() == 0)
	{
		FGZTownInfo T;
		T.Name = TEXT("Emberfall");
		T.Biome = TEXT("temperate");
		Towns.Add(T);
	}

	// Skills referenced by recipes / chains that the JSON doesn't define (e.g. woodworking).
	for (const FGZRecipe& R : Recipes)
	{
		if (!FindSkill(R.Skill))
		{
			FGZSkillState S;
			S.Id = R.Skill;
			S.Name = Pretty(R.Skill);
			S.Category = TEXT("production");
			Skills.Add(S);
		}
	}

	// Extra recipes that close the production loops.
	auto AddRecipe = [this](const TCHAR* Id, const TCHAR* Out, const TCHAR* Skill, int32 Diff, bool bProc,
		const TCHAR* M1, int32 N1, const TCHAR* M2 = nullptr, int32 N2 = 0)
	{
		FGZRecipe R;
		R.Id = Id;
		R.Output = Out;
		R.Skill = Skill;
		R.Difficulty = Diff;
		R.bProcessing = bProc;
		R.Materials.Add(TPair<FString, int32>(FString(M1), N1));
		if (M2)
		{
			R.Materials.Add(TPair<FString, int32>(FString(M2), N2));
		}
		Recipes.Add(R);
	};
	AddRecipe(TEXT("smelt_steel"), TEXT("steel_ingot"), TEXT("blacksmithing"), 10, true, TEXT("iron_ingot"), 2);
	AddRecipe(TEXT("carve_oak_handle"), TEXT("oak_handle"), TEXT("woodworking"), 6, true, TEXT("oak_log"), 3);
	AddRecipe(TEXT("mana_draught"), TEXT("mana_draught"), TEXT("alchemy"), 8, false, TEXT("herbal_extract"), 1, TEXT("clear_water"), 1);
	AddRecipe(TEXT("grilled_fish"), TEXT("grilled_fish"), TEXT("cooking"), 5, false, TEXT("fish"), 1, TEXT("salt"), 1);
	AddRecipe(TEXT("hunting_bow"), TEXT("hunting_bow"), TEXT("blacksmithing"), 8, false, TEXT("oak_handle"), 2, TEXT("leather_strip"), 2);
	AddRecipe(TEXT("leather_armor"), TEXT("leather_armor"), TEXT("blacksmithing"), 7, false, TEXT("leather_strip"), 6);
	AddRecipe(TEXT("iron_armor"), TEXT("iron_armor"), TEXT("blacksmithing"), 18, false, TEXT("iron_ingot"), 6, TEXT("leather_strip"), 3);
	if (!FindSkill(TEXT("woodworking")))
	{
		FGZSkillState S;
		S.Id = TEXT("woodworking");
		S.Name = TEXT("Woodworking");
		S.Category = TEXT("production");
		Skills.Add(S);
	}
	if (OfficeLines.Num() == 0)
	{
		OfficeLines.Add(TEXT("Town Head  (town office, 30-day term)"));
	}
	if (PriceFormula.IsEmpty())
	{
		PriceFormula = TEXT("BasePrice * DemandMultiplier * SupplyMultiplier * RegionMultiplier * TaxMultiplier");
	}

	// Matches goonzu_world.json order: Frostmere, Emberfall, Greenhollow.
	static const TCHAR* Heads[] = {TEXT("Mayor Haneul"), TEXT("Mayor Seol"), TEXT("Mayor Dara"), TEXT("Mayor Bitna")};
	static const TCHAR* Cands[] = {TEXT("Merchant Yuri"), TEXT("Smith Doyun"), TEXT("Sage Mirae"), TEXT("Captain Jun"), TEXT("Farmer Areum")};
	for (int32 i = 0; i < Towns.Num(); ++i)
	{
		Towns[i].Head = Heads[i % 4];
		Towns[i].Candidates = {Cands[i % 5], Cands[(i + 2) % 5], Cands[(i + 4) % 5]};
	}
}

void UGZRPGComponent::SetupQuests()
{
	auto Q = [this](const TCHAR* Title, const TCHAR* Desc, const TCHAR* Kind, const TCHAR* Target, int32 Goal, int32 Luma, int32 Xp)
	{
		FGZQuest Qs;
		Qs.Title = Title;
		Qs.Desc = Desc;
		Qs.Kind = Kind;
		Qs.Target = Target;
		Qs.Goal = Goal;
		Qs.RewardLuma = Luma;
		Qs.RewardExp = Xp;
		Quests.Add(Qs);
	};
	Q(TEXT("First Ore"), TEXT("Mine 6 Iron Ore at a vein near Frostmere."), TEXT("gather"), TEXT("iron_ore"), 6, 200, 60);
	Q(TEXT("Smelter's Apprentice"), TEXT("Process ore into 3 Iron Ingots (Manufacture window)."), TEXT("craft"), TEXT("iron_ingot"), 3, 250, 80);
	Q(TEXT("A Blade of Your Own"), TEXT("Manufacture an Iron Sword and equip it."), TEXT("craft"), TEXT("iron_sword"), 1, 400, 150);
	Q(TEXT("Slime Control"), TEXT("Defeat 5 Slimes in the Emberfall meadows."), TEXT("kill"), TEXT("Slime"), 5, 180, 90);
	Q(TEXT("Herbalist"), TEXT("Gather 8 Red Herbs in the Forgotten Forest."), TEXT("gather"), TEXT("red_herb"), 8, 220, 70);
	Q(TEXT("Grand Tour"), TEXT("Visit Frostmere, Emberfall and Greenhollow."), TEXT("visit"), TEXT("town"), 3, 300, 120);
	Q(TEXT("Merchant Prince"), TEXT("Hold 5,000 Luma at once."), TEXT("luma"), TEXT("luma"), 5000, 500, 200);
	Q(TEXT("Dragon's Shadow"), TEXT("Defeat the Ember Drake in the Dragon Lair."), TEXT("kill"), TEXT("Ember Drake"), 1, 3000, 1500);
}

void UGZRPGComponent::BeginPlay()
{
	Super::BeginPlay();
	SetupItems();
	LoadWorldJson();
	SetupQuests();

	AttributeValues.Init(5, AttributeNames.Num());
	QuickSlots = {TEXT("healing_tonic"), TEXT("mana_draught"), TEXT("traveler_bread"), TEXT("grilled_fish"), TEXT("")};

	AddItem(TEXT("healing_tonic"), 5);
	AddItem(TEXT("traveler_bread"), 4);
	AddItem(TEXT("iron_ore"), 2);
	AddItem(TEXT("leather_strip"), 1);
	AddItem(TEXT("wood_handle"), 1);

	for (const FString& G : MarketGoods)
	{
		Demand.Add(G, 1.f);
		Supply.Add(G, 1.f);
	}

	AddChat(TEXT("System"), FString::Printf(TEXT("Welcome to %s! Currency: %s. %d skills, %d recipes, %d towns loaded from goonzu_world.json."),
		*WorldName, *Currency, Skills.Num(), Recipes.Num(), Towns.Num()), FLinearColor(1.f, 0.85f, 0.3f));
	AddChat(TEXT("System"), TEXT("WASD / click to move, Q/E rotate camera, wheel zoom. Space attack, G gather, E talk."), FLinearColor(0.7f, 0.9f, 1.f));
	AddChat(TEXT("System"), TEXT("P toggles the 8-bit render mode."), FLinearColor(0.7f, 0.9f, 1.f));
	AddChat(TEXT("System"), TEXT("Windows: C Character, I Inventory, K Skills, U Manufacture, B Market, N Town, L Quests, M Map, J Community, F10 Menu."), FLinearColor(0.7f, 0.9f, 1.f));
}

void UGZRPGComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	GatherCooldown = FMath::Max(0.f, GatherCooldown - DeltaTime);
	AttackCooldown = FMath::Max(0.f, AttackCooldown - DeltaTime);
	CombatTimer = FMath::Max(0.f, CombatTimer - DeltaTime);
	const float Regen = CombatTimer > 0.f ? 0.5f : 4.f;
	HP = FMath::Min(MaxHP, HP + Regen * DeltaTime * (1.f + Attr(TEXT("Vitality")) * 0.05f));
	MP = FMath::Min(MaxMP, MP + Regen * 0.5f * DeltaTime * (1.f + Attr(TEXT("Wisdom")) * 0.05f));
	// Market slowly drifts back to equilibrium.
	for (auto& D : Demand) D.Value = FMath::FInterpTo(D.Value, 1.f, DeltaTime, 0.02f);
	for (auto& S : Supply) S.Value = FMath::FInterpTo(S.Value, 1.f, DeltaTime, 0.02f);
}

void UGZRPGComponent::AddChat(const FString& Channel, const FString& Text, const FLinearColor& Color)
{
	FGZChatLine L;
	L.Channel = Channel;
	L.Text = Text;
	L.Color = Color;
	Chat.Add(L);
	if (Chat.Num() > 60)
	{
		Chat.RemoveAt(0);
	}
}

const FGZItemDef& UGZRPGComponent::GetItem(const FString& Id) const
{
	if (const FGZItemDef* D = Items.Find(Id))
	{
		return *D;
	}
	return UnknownItem;
}

int32 UGZRPGComponent::Count(const FString& Item) const
{
	const int32* N = Inventory.Find(Item);
	return N ? *N : 0;
}

void UGZRPGComponent::AddItem(const FString& Item, int32 N)
{
	if (N <= 0 || Item.IsEmpty()) return;
	if (!Items.Contains(Item))
	{
		FGZItemDef D;
		D.Id = Item;
		D.Name = Pretty(Item);
		D.Type = TEXT("material");
		Items.Add(Item, D);
	}
	int32& C = Inventory.FindOrAdd(Item);
	C += N;
	InventoryOrder.AddUnique(Item);
	Progress(TEXT("gather"), Item, N);
}

bool UGZRPGComponent::RemoveItem(const FString& Item, int32 N)
{
	int32* C = Inventory.Find(Item);
	if (!C || *C < N) return false;
	*C -= N;
	if (*C <= 0)
	{
		Inventory.Remove(Item);
		InventoryOrder.Remove(Item);
		if (EquippedWeapon == Item) EquippedWeapon.Reset();
		if (EquippedArmor == Item) EquippedArmor.Reset();
	}
	return true;
}

FGZSkillState* UGZRPGComponent::FindSkill(const FString& Id)
{
	return Skills.FindByPredicate([&Id](const FGZSkillState& S) { return S.Id == Id; });
}

int32 UGZRPGComponent::SkillLevel(const FString& Id) const
{
	const FGZSkillState* S = Skills.FindByPredicate([&Id](const FGZSkillState& X) { return X.Id == Id; });
	return S ? S->Level : 0;
}

void UGZRPGComponent::GiveSkillXp(const FString& Id, float Amount)
{
	FGZSkillState* S = FindSkill(Id);
	if (!S || S->Level >= S->Cap) return;
	// Past the basic cap, training slows down (GoonZu-style basic vs experience levels).
	const float Mult = S->Level >= S->BasicCap ? 0.35f : 1.f;
	S->Xp += Amount * Mult;
	while (S->Xp >= 100.f && S->Level < S->Cap)
	{
		S->Xp -= 100.f;
		S->Level++;
		AddChat(TEXT("System"), FString::Printf(TEXT("%s increased to %d!"), *S->Name, S->Level), FLinearColor(0.5f, 1.f, 0.5f));
	}
}

void UGZRPGComponent::GiveExp(float Amount)
{
	Exp += Amount;
	bool bLeveledUp = false;
	while (Exp >= ExpToNext)
	{
		Exp -= ExpToNext;
		Level++;
		bLeveledUp = true;
		ExpToNext = FMath::RoundToFloat(ExpToNext * 1.35f);
		BonusPoints += 3;
		MaxHP += 25.f;
		MaxMP += 10.f;
		HP = MaxHP;
		MP = MaxMP;
		AddChat(TEXT("System"), FString::Printf(TEXT("Level up! You are now level %d (+3 attribute points)."), Level), FLinearColor(1.f, 0.9f, 0.2f));
	}
	if (bLeveledUp)
	{
		if (UGameInstance* GameInstance = GetWorld() ? GetWorld()->GetGameInstance() : nullptr)
		{
			if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
			{
				Audio->PlaySoundEffectById(TEXT("level_up"));
			}
		}
	}
}

void UGZRPGComponent::GiveLuma(int64 Amount)
{
	Luma = FMath::Max<int64>(0, Luma + Amount);
	Progress(TEXT("luma"), TEXT("luma"), 0);
}

int32 UGZRPGComponent::Attr(const FString& Name) const
{
	const int32 Idx = AttributeNames.IndexOfByKey(Name);
	return (Idx != INDEX_NONE && AttributeValues.IsValidIndex(Idx)) ? AttributeValues[Idx] : 0;
}

void UGZRPGComponent::RaiseAttribute(int32 Index)
{
	if (BonusPoints <= 0 || !AttributeValues.IsValidIndex(Index)) return;
	AttributeValues[Index]++;
	BonusPoints--;
	if (AttributeNames[Index] == TEXT("Vitality")) MaxHP += 10.f;
	if (AttributeNames[Index] == TEXT("Wisdom")) MaxMP += 6.f;
}

int32 UGZRPGComponent::GetAttack() const
{
	const int32 Weapon = EquippedWeapon.IsEmpty() ? 0 : GetItem(EquippedWeapon).Attack;
	return 6 + Attr(TEXT("Strength")) * 2 + Attr(TEXT("Dexterity")) + Weapon + SkillLevel(TEXT("sword")) / 2;
}

int32 UGZRPGComponent::GetDefense() const
{
	const int32 Armor = EquippedArmor.IsEmpty() ? 0 : GetItem(EquippedArmor).Defense;
	return 2 + Attr(TEXT("Vitality")) + Attr(TEXT("Agility")) / 2 + Armor;
}

float UGZRPGComponent::CraftChance(const FGZRecipe& R) const
{
	const int32 Lvl = SkillLevel(R.Skill);
	return FMath::Clamp(0.6f + (Lvl - R.Difficulty) * 0.04f + Attr(TEXT("Dexterity")) * 0.005f + Attr(TEXT("Luck")) * 0.003f, 0.1f, 0.98f);
}

bool UGZRPGComponent::HasMaterials(const FGZRecipe& R) const
{
	for (const auto& M : R.Materials)
	{
		if (Count(M.Key) < M.Value) return false;
	}
	return true;
}

bool UGZRPGComponent::Craft(int32 RecipeIndex)
{
	if (!Recipes.IsValidIndex(RecipeIndex)) return false;
	const FGZRecipe R = Recipes[RecipeIndex];
	if (!HasMaterials(R))
	{
		AddChat(TEXT("System"), FString::Printf(TEXT("Not enough materials for %s."), *Pretty(R.Output)), FLinearColor(1.f, 0.5f, 0.4f));
		return false;
	}
	for (const auto& M : R.Materials)
	{
		RemoveItem(M.Key, M.Value);
	}
	const float Chance = CraftChance(R);
	GiveSkillXp(R.Skill, 12.f + R.Difficulty * 2.f);
	if (FMath::FRand() > Chance)
	{
		AddChat(TEXT("System"), FString::Printf(TEXT("Manufacture failed: %s (%.0f%% chance). Materials lost."), *Pretty(R.Output), Chance * 100.f), FLinearColor(1.f, 0.45f, 0.35f));
		return false;
	}
	static const TCHAR* Qualities[] = {TEXT("Common"), TEXT("Fine"), TEXT("Superior"), TEXT("Masterwork")};
	const float Roll = FMath::FRand() + SkillLevel(R.Skill) * 0.01f;
	const int32 Q = Roll > 1.15f ? 3 : Roll > 0.9f ? 2 : Roll > 0.6f ? 1 : 0;
	AddItem(R.Output, R.OutputCount);
	GiveExp(4.f + R.Difficulty);
	Progress(TEXT("craft"), R.Output, R.OutputCount);
	if (UGameInstance* GameInstance = GetWorld() ? GetWorld()->GetGameInstance() : nullptr)
	{
		if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
		{
			Audio->PlaySoundEffectById(TEXT("craft_item"));
		}
	}
	AddChat(TEXT("System"), FString::Printf(TEXT("Manufactured %s [%s]."), *Pretty(R.Output), R.bProcessing ? TEXT("Processed") : Qualities[Q]), FLinearColor(0.5f, 1.f, 0.6f));
	return true;
}

FGZTownInfo* UGZRPGComponent::FindTown(const FString& Name)
{
	return Towns.FindByPredicate([&Name](const FGZTownInfo& T) { return T.Name == Name; });
}

const FGZTownInfo* UGZRPGComponent::FindTown(const FString& Name) const
{
	return Towns.FindByPredicate([&Name](const FGZTownInfo& T) { return T.Name == Name; });
}

float UGZRPGComponent::TaxRate() const
{
	const FGZTownInfo* T = FindTown(CurrentZone);
	return T ? T->SalesTax : 0.06f;
}

float UGZRPGComponent::RegionMultiplier(const FString& Item) const
{
	const FGZTownInfo* T = FindTown(CurrentZone);
	if (!T) return 1.f;
	const FString& Type = GetItem(Item).Type;
	if (T->Biome == TEXT("snow"))
	{
		// Food is scarce in the north, ore is plentiful.
		if (Type == TEXT("consumable")) return 1.2f;
		if (Item.Contains(TEXT("iron")) || Item.Contains(TEXT("ore"))) return 0.85f;
	}
	if (T->Biome == TEXT("forest"))
	{
		if (Item.Contains(TEXT("oak")) || Item.Contains(TEXT("herb")) || Item.Contains(TEXT("wood"))) return 0.8f;
		if (Type == TEXT("weapon")) return 1.15f;
	}
	if (T->Biome == TEXT("temperate"))
	{
		if (Item == TEXT("grain") || Item == TEXT("salt")) return 0.85f;
	}
	return 1.f;
}

int32 UGZRPGComponent::GetBuyPrice(const FString& Item) const
{
	const float D = Demand.Contains(Item) ? Demand[Item] : 1.f;
	const float S = Supply.Contains(Item) ? Supply[Item] : 1.f;
	return FMath::Max(1, FMath::RoundToInt(GetItem(Item).BasePrice * D * (1.f / S) * RegionMultiplier(Item) * (1.f + TaxRate())));
}

int32 UGZRPGComponent::GetSellPrice(const FString& Item) const
{
	const float D = Demand.Contains(Item) ? Demand[Item] : 1.f;
	const float S = Supply.Contains(Item) ? Supply[Item] : 1.f;
	return FMath::Max(1, FMath::RoundToInt(GetItem(Item).BasePrice * 0.6f * D * (1.f / S) * RegionMultiplier(Item) * (1.f - TaxRate())));
}

bool UGZRPGComponent::Buy(const FString& Item)
{
	if (!bInTown)
	{
		AddChat(TEXT("System"), TEXT("The market is only open inside a town."), FLinearColor(1.f, 0.5f, 0.4f));
		return false;
	}
	const int32 Price = GetBuyPrice(Item);
	if (Luma < Price)
	{
		AddChat(TEXT("System"), TEXT("Not enough Luma."), FLinearColor(1.f, 0.5f, 0.4f));
		return false;
	}
	Luma -= Price;
	if (FGZTownInfo* T = FindTown(CurrentZone))
	{
		T->Treasury += FMath::RoundToInt(Price * T->SalesTax);
	}
	{ float& Dm = Demand.FindOrAdd(Item); if (Dm <= 0.f) Dm = 1.f; Dm += 0.03f; }
	AddItem(Item, 1);
	GiveSkillXp(TEXT("trading"), 3.f);
	return true;
}

bool UGZRPGComponent::Sell(const FString& Item)
{
	if (!bInTown)
	{
		AddChat(TEXT("System"), TEXT("The market is only open inside a town."), FLinearColor(1.f, 0.5f, 0.4f));
		return false;
	}
	if (!RemoveItem(Item, 1)) return false;
	const int32 Price = GetSellPrice(Item);
	if (FGZTownInfo* T = FindTown(CurrentZone))
	{
		T->Treasury += FMath::RoundToInt(Price * T->SalesTax);
	}
	{ float& Sp = Supply.FindOrAdd(Item); if (Sp <= 0.f) Sp = 1.f; Sp += 0.03f; }
	GiveLuma(Price);
	GiveSkillXp(TEXT("trading"), 4.f);
	return true;
}

void UGZRPGComponent::UseItem(const FString& Item)
{
	const FGZItemDef& D = GetItem(Item);
	if (Count(Item) <= 0) return;
	if (D.Type == TEXT("consumable"))
	{
		RemoveItem(Item, 1);
		HP = FMath::Min(MaxHP, HP + D.Heal);
		MP = FMath::Min(MaxMP, MP + D.Mana);
		AddChat(TEXT("System"), FString::Printf(TEXT("Used %s."), *D.Name), FLinearColor(0.8f, 0.8f, 1.f));
	}
	else if (D.Type == TEXT("weapon"))
	{
		EquippedWeapon = Item;
		AddChat(TEXT("System"), FString::Printf(TEXT("Equipped %s (Attack +%d)."), *D.Name, D.Attack), FLinearColor(0.8f, 0.8f, 1.f));
	}
	else if (D.Type == TEXT("armor"))
	{
		EquippedArmor = Item;
		AddChat(TEXT("System"), FString::Printf(TEXT("Equipped %s (Defense +%d)."), *D.Name, D.Defense), FLinearColor(0.8f, 0.8f, 1.f));
	}
}

void UGZRPGComponent::UseQuickSlot(int32 Index)
{
	if (QuickSlots.IsValidIndex(Index) && !QuickSlots[Index].IsEmpty())
	{
		UseItem(QuickSlots[Index]);
	}
}

void UGZRPGComponent::BuyShare()
{
	FGZTownInfo* T = FindTown(CurrentZone);
	if (!T)
	{
		AddChat(TEXT("System"), TEXT("Town shares can only be bought inside a town."), FLinearColor(1.f, 0.5f, 0.4f));
		return;
	}
	const int32 Cost = 500;
	if (Luma < Cost)
	{
		AddChat(TEXT("System"), TEXT("A share costs 500 Luma."), FLinearColor(1.f, 0.5f, 0.4f));
		return;
	}
	Luma -= Cost;
	T->Treasury += Cost;
	T->PlayerShares++;
	AddChat(TEXT("Town"), FString::Printf(TEXT("You now hold %d share(s) of %s. Shareholders may vote for Town Head."), T->PlayerShares, *T->Name), FLinearColor(0.6f, 0.9f, 1.f));
}

void UGZRPGComponent::Vote(int32 Candidate)
{
	FGZTownInfo* T = FindTown(CurrentZone);
	if (!T || !T->Candidates.IsValidIndex(Candidate)) return;
	if (T->PlayerShares <= 0)
	{
		AddChat(TEXT("Town"), TEXT("Only shareholders can vote. Buy a share first."), FLinearColor(1.f, 0.5f, 0.4f));
		return;
	}
	T->VotedCandidate = Candidate;
	AddChat(TEXT("Town"), FString::Printf(TEXT("You voted for %s with %d share(s)."), *T->Candidates[Candidate], T->PlayerShares), FLinearColor(0.6f, 0.9f, 1.f));
}

void UGZRPGComponent::OnVisitZone(const FString& Zone)
{
	if (FindTown(Zone) && !VisitedZones.Contains(Zone))
	{
		VisitedZones.Add(Zone);
		Progress(TEXT("visit"), TEXT("town"), 1);
	}
}

void UGZRPGComponent::OnKill(const FString& MonsterKind)
{
	Progress(TEXT("kill"), MonsterKind, 1);
}

void UGZRPGComponent::Progress(const FString& Kind, const FString& Target, int32 Amount)
{
	for (FGZQuest& Q : Quests)
	{
		if (Q.bDone || Q.Kind != Kind) continue;
		const int32 PreviousProgress = Q.Progress;
		if (Kind == TEXT("luma"))
		{
			Q.Progress = (int32)FMath::Min<int64>(Luma, Q.Goal);
		}
		else if (Q.Target == Target)
		{
			Q.Progress = FMath::Min(Q.Goal, Q.Progress + Amount);
		}
		if (Q.Progress >= Q.Goal)
		{
			Q.bDone = true;
			Luma += Q.RewardLuma;
			AddChat(TEXT("Quest"), FString::Printf(TEXT("Quest complete: %s  (+%d Luma, +%d EXP)"), *Q.Title, Q.RewardLuma, Q.RewardExp), FLinearColor(1.f, 0.8f, 0.3f));
			GiveExp(Q.RewardExp);
			if (UGameInstance* GameInstance = GetWorld() ? GetWorld()->GetGameInstance() : nullptr)
			{
				if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
				{
					Audio->PlaySoundEffectById(TEXT("quest_complete"));
				}
			}
		}
		else if (Q.Progress > PreviousProgress)
		{
			if (UGameInstance* GameInstance = GetWorld() ? GetWorld()->GetGameInstance() : nullptr)
			{
				if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
				{
					Audio->PlaySoundEffectById(TEXT("quest_progress"), 0.6f);
				}
			}
		}
	}
}
