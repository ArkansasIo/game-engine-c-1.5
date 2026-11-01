#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "GZRPGComponent.generated.h"

/** Static definition of an item (data-driven from goonzu_world.json recipe/resource ids). */
struct FGZItemDef
{
	FString Id;
	FString Name;
	FString Type;        // material | consumable | weapon | armor | food
	int32 BasePrice = 10;
	float Heal = 0.f;
	float Mana = 0.f;
	int32 Attack = 0;
	int32 Defense = 0;
	FLinearColor Color = FLinearColor(0.5f, 0.5f, 0.5f);
};

struct FGZSkillState
{
	FString Id;
	FString Name;
	FString Category;
	int32 Level = 1;
	float Xp = 0.f;        // 0..100 toward next level
	int32 BasicCap = 30;
	int32 Cap = 70;
};

struct FGZRecipe
{
	FString Id;
	FString Output;
	int32 OutputCount = 1;
	FString Skill;
	int32 Difficulty = 1;
	TArray<TPair<FString, int32>> Materials;
	bool bProcessing = false;   // resource chain step (raw -> processed)
};

struct FGZTownInfo
{
	int32 Id = 0;
	FString Name;
	FString Biome;
	float SalesTax = 0.05f;
	int64 Treasury = 0;
	FString Head;
	int32 PlayerShares = 0;
	int32 VotedCandidate = -1;
	TArray<FString> Candidates;
};

struct FGZQuest
{
	FString Title;
	FString Desc;
	FString Kind;     // gather | craft | kill | visit | luma
	FString Target;
	int32 Goal = 1;
	int32 Progress = 0;
	int32 RewardLuma = 0;
	int32 RewardExp = 0;
	bool bDone = false;
};

struct FGZChatLine
{
	FString Channel;
	FString Text;
	FLinearColor Color = FLinearColor::White;
};

/**
 * All RPG state for the player: attributes, production skills, inventory,
 * Luma currency, recipes (from goonzu_world.json), market pricing, towns and quests.
 */
UCLASS(ClassGroup = (GoonZu), meta = (BlueprintSpawnableComponent))
class ETHEREALVENI_API UGZRPGComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	UGZRPGComponent();
	virtual void BeginPlay() override;
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	// ---- Character ----
	FString CharacterName = TEXT("Traveler");
	int32 Level = 1;
	float Exp = 0.f;
	float ExpToNext = 100.f;
	float HP = 200.f;
	float MaxHP = 200.f;
	float MP = 80.f;
	float MaxMP = 80.f;
	int64 Luma = 1500;
	int32 BonusPoints = 0;
	TArray<FString> AttributeNames;
	TArray<int32> AttributeValues;

	FString EquippedWeapon;
	FString EquippedArmor;
	TArray<FString> QuickSlots;   // F1..F5

	// ---- World data ----
	FString WorldName = TEXT("Luminara");
	FString Currency = TEXT("Luma");
	FString PriceFormula;
	TArray<FGZSkillState> Skills;
	TArray<FGZRecipe> Recipes;
	TArray<FGZTownInfo> Towns;
	TArray<FString> OfficeLines;
	TArray<FGZQuest> Quests;
	TArray<FString> MarketGoods;
	TArray<FString> InventoryOrder;
	TMap<FString, int32> Inventory;
	TArray<FGZChatLine> Chat;

	// ---- Location (set by the character every tick) ----
	FString CurrentZone = TEXT("Wilderness");
	bool bInTown = false;

	// ---- API ----
	void AddChat(const FString& Channel, const FString& Text, const FLinearColor& Color = FLinearColor::White);
	const FGZItemDef& GetItem(const FString& Id) const;
	int32 Count(const FString& Item) const;
	void AddItem(const FString& Item, int32 N);
	bool RemoveItem(const FString& Item, int32 N);
	FGZSkillState* FindSkill(const FString& Id);
	int32 SkillLevel(const FString& Id) const;
	void GiveSkillXp(const FString& Id, float Amount);
	void GiveExp(float Amount);
	void GiveLuma(int64 Amount);
	float CraftChance(const FGZRecipe& R) const;
	bool HasMaterials(const FGZRecipe& R) const;
	bool Craft(int32 RecipeIndex);
	int32 GetBuyPrice(const FString& Item) const;
	int32 GetSellPrice(const FString& Item) const;
	bool Buy(const FString& Item);
	bool Sell(const FString& Item);
	void UseItem(const FString& Item);
	void UseQuickSlot(int32 Index);
	void RaiseAttribute(int32 Index);
	int32 GetAttack() const;
	int32 GetDefense() const;
	int32 Attr(const FString& Name) const;
	FGZTownInfo* FindTown(const FString& Name);
	const FGZTownInfo* FindTown(const FString& Name) const;
	void BuyShare();
	void Vote(int32 Candidate);
	void OnVisitZone(const FString& Zone);
	void OnKill(const FString& MonsterKind);
	static FString Pretty(const FString& Id);

	float GatherCooldown = 0.f;
	float AttackCooldown = 0.f;
	float CombatTimer = 0.f;

private:
	void LoadWorldJson();
	void SetupItems();
	void SetupQuests();
	void Progress(const FString& Kind, const FString& Target, int32 Amount);
	float RegionMultiplier(const FString& Item) const;
	float TaxRate() const;

	TMap<FString, FGZItemDef> Items;
	TMap<FString, float> Demand;
	TMap<FString, float> Supply;
	FGZItemDef UnknownItem;
	TSet<FString> VisitedZones;
};
