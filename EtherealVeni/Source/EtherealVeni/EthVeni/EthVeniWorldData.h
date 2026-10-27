#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "EthVeniWorldData.generated.h"

/**
 * Loads and exposes the GoonZu world definition from
 * Content/GoonZuGame/goonzu_world.json (towns, skills, recipes, resource chains).
 */
USTRUCT(BlueprintType)
struct FTown
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly)
	int32 Id = 0;

	UPROPERTY(BlueprintReadOnly)
	FString Name;

	UPROPERTY(BlueprintReadOnly)
	FString Biome;

	UPROPERTY(BlueprintReadOnly)
	float SalesTax = 0.f;

	UPROPERTY(BlueprintReadOnly)
	int32 Treasury = 0;
};

USTRUCT(BlueprintType)
struct FSkill
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly)
	FString Id;

	UPROPERTY(BlueprintReadOnly)
	FString Name;

	UPROPERTY(BlueprintReadOnly)
	FString Category;

	UPROPERTY(BlueprintReadOnly)
	int32 BasicCap = 0;

	UPROPERTY(BlueprintReadOnly)
	int32 ExperienceCap = 0;
};

USTRUCT(BlueprintType)
struct FRecipe
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly)
	FString Id;

	UPROPERTY(BlueprintReadOnly)
	FString Output;

	UPROPERTY(BlueprintReadOnly)
	FString Skill;

	UPROPERTY(BlueprintReadOnly)
	int32 Difficulty = 0;

	UPROPERTY(BlueprintReadOnly)
	TMap<FString, int32> Materials;
};

UCLASS(BlueprintType)
class ETHEREALVENI_API UEthVeniWorldData : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	/** Parses the world JSON. Returns false on failure; see log for details. */
	UFUNCTION(BlueprintCallable, Category = "Ethereal Veni")
	bool LoadWorldData(const FString& InJsonString);

	/** Loads goonzu_world.json from project Content. */
	UFUNCTION(BlueprintCallable, Category = "Ethereal Veni")
	bool LoadWorldDataFromContent();

	UFUNCTION(BlueprintPure, Category = "Ethereal Veni")
	const TArray<FSkill>& GetSkills() const { return Skills; }

	UFUNCTION(BlueprintPure, Category = "Ethereal Veni")
	const TArray<FTown>& GetTowns() const { return Towns; }

private:
	UPROPERTY()
	TArray<FSkill> Skills;

	UPROPERTY()
	TArray<FTown> Towns;

	TArray<FRecipe> Recipes;
};
