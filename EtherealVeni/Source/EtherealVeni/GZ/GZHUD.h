#pragma once

#include "CoreMinimal.h"
#include "GameFramework/HUD.h"
#include "GZHUD.generated.h"

class UTexture2D;
class UFont;
class UGZRPGComponent;
class AGZIsoCharacter;

UENUM(BlueprintType)
enum class EGZWindow : uint8
{
	None,
	Character,
	Inventory,
	Skills,
	Manufacture,
	Market,
	Town,
	Quests,
	WorldMap,
	Community,
	Help,
	GameMenu,
	Count
};

UENUM(BlueprintType)
enum class EGZMenuAction : uint8
{
	ExpandGroup,
	OpenWindow,
	Resume,
	CloseAll,
	Quit
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FGZMenuEntry
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	FName Id = NAME_None;

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	FName ParentId = NAME_None;

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	FText Label;

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	FText Description;

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	EGZWindow Window = EGZWindow::None;

	UPROPERTY(BlueprintReadOnly, Category = "UI|Menus")
	EGZMenuAction Action = EGZMenuAction::ExpandGroup;
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FGZMenuWindowStateChanged, EGZWindow, Window, bool, bIsOpen);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FGZMenuGroupChanged, FName, GroupId);

/**
 * GoonZu-style interface drawn entirely in code: status panel, minimap, target frame,
 * chat log, F1-F5 quick slots, menu bar and draggable windows (Character, Inventory,
 * Skills, Manufacture, Market, Town/Election, Quests, World Map, Community, Help, Menu).
 * Icons/portraits are textures rendered inside Unreal by the Luminara builder script.
 */
UCLASS()
class ETHEREALVENI_API AGZHUD : public AHUD
{
	GENERATED_BODY()

public:
	AGZHUD();
	virtual void DrawHUD() override;

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void ToggleWindow(EGZWindow W);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void OpenWindow(EGZWindow W);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void CloseWindow(EGZWindow W);

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	bool IsOpen(EGZWindow W) const { return OpenWindows.Contains(W); }

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	TArray<FGZMenuEntry> GetMenuEntries() const;

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool SetExpandedMenuGroup(FName GroupId);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool ActivateMenuEntryById(FName EntryId);

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	FName GetExpandedMenuGroup() const { return ExpandedMenuGroup; }

	UPROPERTY(BlueprintAssignable, Category = "UI|Menus")
	FGZMenuWindowStateChanged OnWindowStateChanged;

	UPROPERTY(BlueprintAssignable, Category = "UI|Menus")
	FGZMenuGroupChanged OnMenuGroupChanged;

	UFUNCTION(BlueprintPure, Category = "UI")
	bool IsMouseOverUI() const { return bMouseOverUI; }

	UFUNCTION(BlueprintCallable, Category = "UI")
	void AddFloatingText(const FVector& WorldLocation, const FString& Text, const FLinearColor& Color);

	// ---- hooks used by the MMO interface (UI/EVUIDefaultContent) ----

	/** Closes the most recently opened HUD window. False when none is open. */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool CloseTopmostWindow();

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void CloseAllWindows();

	static constexpr int32 NumChatFilters = 6;
	static FString GetChatFilterName(int32 Filter);
	void SetChatFilter(int32 Filter) { ChatFilter = FMath::Clamp(Filter, 0, NumChatFilters - 1); }
	int32 GetChatFilter() const { return ChatFilter; }

	/** Raid-style marker on the current target: 0 = none, 1..NumTargetMarkers. */
	static constexpr int32 NumTargetMarkers = 8;
	static FText GetMarkerName(int32 Marker);
	static FLinearColor GetMarkerColor(int32 Marker);
	void SetTargetMarker(int32 Marker);
	int32 GetTargetMarker() const;

protected:
	virtual void BeginPlay() override;

private:
	struct FButton
	{
		FBox2D Box;
		FString Id;
		int32 Layer = 0;
	};
	struct FFloating
	{
		FVector World;
		FString Text;
		FLinearColor Color;
		float Age = 0.f;
	};

	// ---- drawing helpers ----
	void Panel(float X, float Y, float W, float H, const FLinearColor& Bg, bool bGoldFrame = true);
	void Frame(float X, float Y, float W, float H, const FLinearColor& C, float T = 1.f);
	void Txt(const FString& T, float X, float Y, const FLinearColor& C = FLinearColor::White, float Size = 1.f, bool bShadow = true);
	void TxtC(const FString& T, float CX, float Y, const FLinearColor& C = FLinearColor::White, float Size = 1.f);
	float TxtW(const FString& T, float Size = 1.f);
	bool Button(const FString& Id, float X, float Y, float W, float H, const FString& Label, bool bEnabled = true, bool bActive = false, float Size = 0.9f);
	void Bar(float X, float Y, float W, float H, float Frac, const FLinearColor& C, const FString& Label);
	void Icon(const FString& IconId, float X, float Y, float Size, const FLinearColor& Fallback, const FString& FallbackText);
	void ItemSlot(const FString& Item, float X, float Y, float Size, int32 Count, const FString& ButtonId);
	void Hit(const FString& Id, float X, float Y, float W, float H);
	UTexture2D* Tex(const FString& Name);
	bool BeginWindow(EGZWindow W, const FString& Title, float W0, float H0, float& OutX, float& OutY);

	// ---- fixed HUD ----
	void DrawStatus();
	void DrawMinimap();
	void DrawTargetFrame();
	void DrawChat();
	void DrawQuickBar();
	void DrawMenuBar();
	void DrawPrompt();
	void DrawFloating(float Dt);
	void DrawClock();

	// ---- windows ----
	void DrawCharacter();
	void DrawInventory();
	void DrawSkills();
	void DrawManufacture();
	void DrawMarket();
	void DrawTown();
	void DrawQuests();
	void DrawWorldMap();
	void DrawCommunity();
	void DrawHelp();
	void DrawGameMenu();
	void DrawMapContents(float X, float Y, float W, float H, float WorldRadius, bool bLabels);

	void HandleClick(const FString& Id);
	FString WindowTitle(EGZWindow W) const;
	void ActivateMenuEntry(const FGZMenuEntry& Entry);

	UGZRPGComponent* RPG() const;
	AGZIsoCharacter* Player() const;

	UPROPERTY()
	TMap<FString, TObjectPtr<UTexture2D>> TextureCache;
	TSet<FString> MissingTextures;

	TArray<EGZWindow> OpenWindows;
	TMap<uint8, FVector2D> WindowPos;
	FName ExpandedMenuGroup = TEXT("player");
	TArray<FButton> Buttons;        // built this frame
	TArray<FButton> PrevButtons;    // from last frame, used for clicks
	TArray<FBox2D> UIRects;
	TArray<FBox2D> PrevUIRects;
	TArray<FFloating> Floating;

	EGZWindow CurrentWindow = EGZWindow::None;
	int32 CurrentLayer = 0;
	float S = 1.f;
	FVector2D Mouse = FVector2D::ZeroVector;
	bool bMouseOverUI = false;
	bool bMouseDown = false;
	int32 DragWindow = -1;
	FVector2D DragOffset = FVector2D::ZeroVector;
	int32 SelectedRecipe = 0;
	int32 RecipeScroll = 0;
	int32 MarketScroll = 0;
	int32 ChatFilter = 0;
	int32 InventoryMode = 0;   // 0 use/equip, 1 assign to quick slot
	int32 AssignSlot = 0;
	float LastTime = 0.f;

	// MMO interface integration (cached once per frame from UEVUISubsystem settings).
	void ReadInterfaceSettings();
	void HandleContextClicks();
	int32 MarkerFor(const AActor* Actor) const;
	bool bSlateUI = false;
	bool bShowClock = true, bShowMinimap = true, bShowTarget = true, bShowChat = true, bFloatingText = true;
	bool bTrackNpcs = true, bTrackMonsters = true, bTrackResources = true, bSystemChat = true, bTextShadow = true, bTooltips = true, bWorldMarker = true;
	float HudScale = 1.f, PanelOpacity = 1.f, MinimapRadius = 6000.f;
	int32 PlayerStatus = 0;
	FBox2D StatusRect = FBox2D(ForceInit), MinimapRect = FBox2D(ForceInit), TargetRect = FBox2D(ForceInit), ChatRect = FBox2D(ForceInit);
	TMap<TWeakObjectPtr<AActor>, int32> TargetMarkers;
};
