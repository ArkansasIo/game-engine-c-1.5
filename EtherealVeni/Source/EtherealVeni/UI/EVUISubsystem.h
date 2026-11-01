#pragma once

#include "CoreMinimal.h"
#include "Subsystems/LocalPlayerSubsystem.h"
#include "UI/EVUITypes.h"
#include "EVUISubsystem.generated.h"

class SEVRoot;
class SWidget;
class UTexture2D;
class UUserWidget;
class UGameViewportClient;
class APlayerController;
struct FSlateBrush;
struct FKeyEvent;
class FEVUIInputProcessor;

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FEVUIActionEvent, FName, ActionId);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FEVUIPanelEvent, FName, PanelId, bool, bIsOpen);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FEVUISettingEvent, FName, Key, const FString&, Value);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FEVUIMenuEvent, FName, MenuId, FName, ItemId);

/**
 * The interface API. One instance per local player.
 *
 * Owns every menu, sub-menu, panel, setting and key binding, draws them through a Slate
 * overlay on the game viewport, and routes hotkeys / Escape. Game code talks to it by id:
 *
 *   UEVUISubsystem* UI = UEVUISubsystem::Get(PC);
 *   UI->RegisterAction("guild.invite", [..]{ ... });
 *   UI->AddMenuItem("unit.target", FEVMenuItem::MakeAction("invite", INVTEXT("Invite"), "guild.invite"));
 *   UI->OpenContextMenu("unit.target", TEXT("Goblin"));
 *
 * Blueprints get the same calls (Category "UI") plus events for actions, panels and settings.
 * Default MMO content (micro menu, Game Menu, Options tree, unit / minimap / chat menus,
 * key bindings) is registered by EVUIDefaultContent.
 */
UCLASS()
class ETHEREALVENI_API UEVUISubsystem : public ULocalPlayerSubsystem
{
	GENERATED_BODY()

public:
	static UEVUISubsystem* Get(const APlayerController* PC);

	/** First local player's interface. */
	UFUNCTION(BlueprintPure, Category = "UI", meta = (WorldContext = "WorldContextObject", DisplayName = "Get MMO Interface"))
	static UEVUISubsystem* GetInterface(const UObject* WorldContextObject);

	/** Reads a checkbox setting without caring whether the UI exists yet. */
	static bool GetBoolSettingFor(const APlayerController* PC, FName Key, bool bDefault);

	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	// ------------------------------------------------------------------ viewport

	/** Adds the interface overlay to the game viewport. Safe to call every BeginPlay. */
	UFUNCTION(BlueprintCallable, Category = "UI")
	void AttachToViewport();

	UFUNCTION(BlueprintCallable, Category = "UI")
	void DetachFromViewport();

	UFUNCTION(BlueprintPure, Category = "UI")
	bool IsAttached() const;

	/** Hide / show the whole interface (default binding Alt+Z). */
	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetInterfaceVisible(bool bVisible);

	UFUNCTION(BlueprintPure, Category = "UI")
	bool IsInterfaceVisible() const { return bInterfaceVisible; }

	UFUNCTION(BlueprintPure, Category = "UI")
	float GetUIScale() const;

	// ------------------------------------------------------------------ actions

	/** Native action. IsChecked lights micro-menu buttons and check marks; IsEnabled greys items out. */
	void RegisterAction(FName ActionId, TFunction<void()> Execute, TFunction<bool()> IsChecked = nullptr, TFunction<bool()> IsEnabled = nullptr);

	UFUNCTION(BlueprintCallable, Category = "UI|Actions")
	void UnregisterAction(FName ActionId);

	/** Runs the native handler (if any) and always broadcasts OnAction so Blueprints can implement actions too. */
	UFUNCTION(BlueprintCallable, Category = "UI|Actions")
	bool ExecuteAction(FName ActionId);

	UFUNCTION(BlueprintPure, Category = "UI|Actions")
	bool IsActionChecked(FName ActionId) const;

	UFUNCTION(BlueprintPure, Category = "UI|Actions")
	bool IsActionEnabled(FName ActionId) const;

	UPROPERTY(BlueprintAssignable, Category = "UI|Actions")
	FEVUIActionEvent OnAction;

	// ------------------------------------------------------------------ menus

	/** Adds or replaces a menu. Items of kind Submenu reference other menus by id. */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void RegisterMenu(const FEVMenuDefinition& Menu);

	/** Inserts an item (after InsertAfter, or at the end). Creates the menu if needed. */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool AddMenuItem(FName MenuId, const FEVMenuItem& Item, FName InsertAfter = NAME_None);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool RemoveMenuItem(FName MenuId, FName ItemId);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool SetMenuItemEnabled(FName MenuId, FName ItemId, bool bEnabled);

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	bool GetMenu(FName MenuId, FEVMenuDefinition& OutMenu) const;

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	TArray<FName> GetMenuIds() const;

	const FEVMenuDefinition* FindMenu(FName MenuId) const { return Menus.Find(MenuId); }

	/** Opens a dropdown at the mouse cursor. Context is shown in the title ("{Context}") and readable by actions. */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool OpenContextMenu(FName MenuId, const FString& Context);

	/** Opens a dropdown with its top-left at Position (Slate units, after UI Scale). */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	bool OpenMenuAt(FName MenuId, FVector2D Position, const FString& Context);

	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void CloseMenus();

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	bool IsAnyMenuOpen() const;

	UFUNCTION(BlueprintPure, Category = "UI|Menus")
	FString GetMenuContext() const { return MenuContext; }

	/** Same as clicking the item. */
	UFUNCTION(BlueprintCallable, Category = "UI|Menus")
	void ActivateMenuItem(FName MenuId, FName ItemId);

	bool IsMenuItemChecked(const FEVMenuItem& Item) const;
	bool IsMenuItemEnabled(const FEVMenuItem& Item) const;
	FText GetShortcutText(FName ActionId) const;
	FText ResolveMenuTitle(const FEVMenuDefinition& Menu) const;

	/** Dropdown of plain choices (used by Options combo boxes). */
	void OpenChoiceMenu(const TArray<FText>& Choices, int32 Selected, TFunction<void(int32)> OnPicked, FVector2D Position, float MinWidth);

	UPROPERTY(BlueprintAssignable, Category = "UI|Menus")
	FEVUIMenuEvent OnMenuItemActivated;

	// ------------------------------------------------------------------ panels

	/** Also registers the action "panel.<PanelId>" that toggles it. */
	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void RegisterPanel(const FEVPanelDefinition& Panel);

	/** Native content for one tab (TabId NAME_None for single-page panels). */
	void RegisterPanelContent(FName PanelId, FName TabId, TFunction<TSharedRef<SWidget>()> Factory);

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void OpenPanel(FName PanelId, FName TabId = NAME_None);

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void ClosePanel(FName PanelId);

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void TogglePanel(FName PanelId);

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void CloseAllPanels();

	UFUNCTION(BlueprintPure, Category = "UI|Panels")
	bool IsPanelOpen(FName PanelId) const { return OpenPanels.Contains(PanelId); }

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void SetPanelTab(FName PanelId, FName TabId);

	UFUNCTION(BlueprintPure, Category = "UI|Panels")
	FName GetPanelTab(FName PanelId) const;

	UFUNCTION(BlueprintPure, Category = "UI|Panels")
	TArray<FName> GetOpenPanels() const { return OpenPanels; }

	UFUNCTION(BlueprintCallable, Category = "UI|Panels")
	void ResetPanelPositions();

	const FEVPanelDefinition* FindPanel(FName PanelId) const { return Panels.Find(PanelId); }
	TSharedRef<SWidget> BuildPanelContent(FName PanelId, FName TabId);
	FVector2D GetPanelOffset(FName PanelId) const { return PanelOffsets.FindRef(PanelId); }
	void MovePanel(FName PanelId, FVector2D Delta);
	void BringPanelToFront(FName PanelId);

	UPROPERTY(BlueprintAssignable, Category = "UI|Panels")
	FEVUIPanelEvent OnPanelChanged;

	// ------------------------------------------------------------------ micro menu

	UFUNCTION(BlueprintCallable, Category = "UI|MicroMenu")
	void RegisterMicroButton(const FEVMicroButton& Button);

	UFUNCTION(BlueprintPure, Category = "UI|MicroMenu")
	const TArray<FEVMicroButton>& GetMicroButtons() const { return MicroButtons; }

	// ------------------------------------------------------------------ popups & messages

	/** Yes/No dialog. Confirm runs ConfirmActionId. */
	UFUNCTION(BlueprintCallable, Category = "UI|Popups")
	void ShowConfirmation(FText Title, FText Message, FName ConfirmActionId, FText ConfirmLabel);

	void ShowConfirmationNative(const FText& Title, const FText& Message, TFunction<void()> OnConfirm, const FText& ConfirmLabel = FText());

	UFUNCTION(BlueprintCallable, Category = "UI|Popups")
	void ClosePopup();

	UFUNCTION(BlueprintPure, Category = "UI|Popups")
	bool IsPopupOpen() const;

	/** Short message in the top-centre message area (fades out). */
	UFUNCTION(BlueprintCallable, Category = "UI|Popups")
	void ShowMessage(FText Message, FLinearColor Color);

	// ------------------------------------------------------------------ options & settings

	UFUNCTION(BlueprintCallable, Category = "UI|Options")
	void RegisterOptionsCategory(const FEVOptionsCategory& Category);

	UFUNCTION(BlueprintCallable, Category = "UI|Options")
	void RegisterSetting(const FEVSettingDefinition& Setting);

	/** Native hook run whenever the value changes (and once at startup). */
	void RegisterSettingApplier(FName Key, TFunction<void(const FString&)> Apply);

	/** Settings whose source of truth lives elsewhere (GameUserSettings) read their value through this. */
	void RegisterSettingReader(FName Key, TFunction<FString()> Read);

	UFUNCTION(BlueprintPure, Category = "UI|Options")
	FString GetSettingString(FName Key) const;

	UFUNCTION(BlueprintPure, Category = "UI|Options")
	float GetSettingFloat(FName Key) const;

	UFUNCTION(BlueprintPure, Category = "UI|Options")
	bool GetSettingBool(FName Key) const;

	UFUNCTION(BlueprintPure, Category = "UI|Options")
	int32 GetSettingInt(FName Key) const;

	/** Stores, applies, saves and broadcasts. */
	UFUNCTION(BlueprintCallable, Category = "UI|Options")
	void SetSetting(FName Key, const FString& Value);

	UFUNCTION(BlueprintCallable, Category = "UI|Options")
	void ResetSetting(FName Key);

	const FEVSettingDefinition* FindSetting(FName Key) const;
	const TArray<FEVOptionsCategory>& GetOptionsCategories() const { return Categories; }
	TArray<const FEVSettingDefinition*> GetSettingsInCategory(FName CategoryId) const;

	/** Opens the Options window on a category (e.g. "graphics.display", "keybindings"). */
	UFUNCTION(BlueprintCallable, Category = "UI|Options")
	void OpenOptions(FName CategoryId);

	FName GetRequestedOptionsCategory() const { return RequestedOptionsCategory; }

	UPROPERTY(BlueprintAssignable, Category = "UI|Options")
	FEVUISettingEvent OnSettingChanged;

	// ------------------------------------------------------------------ key bindings

	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	void RegisterKeyBinding(const FEVKeyBindingDefinition& Binding);

	UFUNCTION(BlueprintPure, Category = "UI|Bindings")
	FEVKeyChord GetBinding(FName ActionId, int32 Slot) const;

	/** Binds and returns the action that previously owned the chord (now unbound), if any. */
	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	FName SetBinding(FName ActionId, int32 Slot, FEVKeyChord Chord);

	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	void ClearBinding(FName ActionId, int32 Slot);

	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	void ResetAllBindings();

	UFUNCTION(BlueprintPure, Category = "UI|Bindings")
	FName FindActionForChord(const FEVKeyChord& Chord) const;

	const TArray<FEVKeyBindingDefinition>& GetKeyBindings() const { return BindingDefs; }

	/** The next key press is assigned to ActionId (Escape cancels). */
	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	void BeginKeyCapture(FName ActionId, int32 Slot);

	UFUNCTION(BlueprintCallable, Category = "UI|Bindings")
	void CancelKeyCapture();

	bool IsCapturing(FName ActionId, int32 Slot) const { return CaptureAction == ActionId && CaptureSlot == Slot; }

	/** Keys hard-wired to gameplay (movement, attack ...). Key capture refuses them with Reason. */
	void RegisterReservedKey(const FKey& Key, const FText& Reason);

	// ------------------------------------------------------------------ input

	/** Handlers asked (newest first) to consume Escape before the Game Menu opens. */
	void RegisterEscapeHandler(TFunction<bool()> Handler);

	/** Closes the topmost thing (menu, popup, panel, game window) or opens the Game Menu. */
	UFUNCTION(BlueprintCallable, Category = "UI")
	bool HandleEscape();

	/** Called by the Slate input pre-processor. Returns true when the key was consumed. */
	bool HandleKeyDown(const FKeyEvent& Event);

	// ------------------------------------------------------------------ helpers for widgets

	const FSlateBrush* GetIconBrush(FName IconName);
	TSharedPtr<SEVRoot> GetRoot() const { return Root; }
	APlayerController* GetPlayerController() const;
	const TArray<FName>& GetOpenPanelOrder() const { return OpenPanels; }

private:
	struct FActionHandler
	{
		TFunction<void()> Execute;
		TFunction<bool()> IsChecked;
		TFunction<bool()> IsEnabled;
	};

	void LoadSettings();
	void SaveSettings();
	void LoadBindings();
	void SaveBindings();
	void ApplySetting(FName Key, const FString& Value);
	void ApplyAllSettings();
	void NotifyPanelsChanged();
	void ClosePanelInternal(FName PanelId, bool bAllowReturn);
	FVector2D DefaultPanelOffset(const FEVPanelDefinition& Panel) const;
	bool IsTextInputFocused() const;
	bool IsGameViewportFocused() const;

	TMap<FName, FActionHandler> Actions;
	TMap<FName, FEVMenuDefinition> Menus;
	TMap<FName, FEVPanelDefinition> Panels;
	TMap<FName, TMap<FName, TFunction<TSharedRef<SWidget>()>>> PanelContent;
	TArray<FName> OpenPanels;                 // z-order, last is on top
	TMap<FName, FName> PanelTabs;
	TMap<FName, FVector2D> PanelOffsets;
	TMap<FName, FName> ReturnToPanel;
	TArray<FEVMicroButton> MicroButtons;

	TArray<FEVOptionsCategory> Categories;
	TArray<FEVSettingDefinition> Settings;
	TMap<FName, FString> SettingValues;
	TMap<FName, TFunction<void(const FString&)>> SettingAppliers;
	TMap<FName, TFunction<FString()>> SettingReaders;
	FName RequestedOptionsCategory;

	TArray<FEVKeyBindingDefinition> BindingDefs;
	TMap<FName, TArray<FEVKeyChord>> Bindings;   // two slots per action
	FName CaptureAction;
	int32 CaptureSlot = -1;
	TMap<FKey, FText> ReservedKeys;

	TArray<TFunction<bool()>> EscapeHandlers;
	FString MenuContext;
	bool bInterfaceVisible = true;
	bool bLoading = false;

	TSharedPtr<SEVRoot> Root;
	TSharedPtr<FEVUIInputProcessor> InputProcessor;
	TWeakObjectPtr<UGameViewportClient> AttachedViewport;

	UPROPERTY()
	TMap<FName, TObjectPtr<UTexture2D>> IconTextures;
	TMap<FName, TSharedPtr<FSlateBrush>> IconBrushes;
	TSet<FName> MissingIcons;

	UPROPERTY()
	TArray<TObjectPtr<UUserWidget>> PanelUserWidgets;
};
