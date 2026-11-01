#pragma once

#include "CoreMinimal.h"
#include "InputCoreTypes.h"
#include "EVUITypes.generated.h"

class UUserWidget;

/**
 * Data model for the Ethereal Veni MMO interface.
 *
 * Everything the player can click is described by data and registered with UEVUISubsystem:
 *   - Menus      : named lists of items. Items can open further menus (cascading sub-menus),
 *                  run actions, or toggle / select settings (checkbox and radio items).
 *   - Panels     : movable windows (Options, Help & Support, Game Menu ...) with optional tabs.
 *   - Micro menu : the row of buttons in the bottom-right corner.
 *   - Settings   : typed options shown in the Options window and saved to GameUserSettings.ini.
 *   - Bindings   : rebindable key chords that run actions.
 */

UENUM(BlueprintType)
enum class EEVMenuItemKind : uint8
{
	/** Runs ActionId and closes the menu. */
	Action,
	/** Opens SubmenuId beside this item on hover / Right arrow. */
	Submenu,
	/** Checkbox. Flips the bool setting SettingKey, or runs ActionId. Menu stays open. */
	Toggle,
	/** Radio option. Sets SettingKey to RadioValue, or runs ActionId. */
	Radio,
	/** Thin divider line. */
	Separator,
	/** Non-clickable section title. */
	Header
};

/** Where a panel docks when it opens. Panels sharing a docking area replace each other. */
UENUM(BlueprintType)
enum class EEVPanelArea : uint8
{
	Left,
	Center,
	Right,
	/** Never replaces anything; stays where the player last dragged it. */
	Free
};

UENUM(BlueprintType)
enum class EEVSettingType : uint8
{
	Checkbox,
	Slider,
	Choice
};

/** A key plus modifiers, e.g. Ctrl+Shift+K. */
USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVKeyChord
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FKey Key;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	bool bShift = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	bool bCtrl = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	bool bAlt = false;

	FEVKeyChord() = default;
	FEVKeyChord(const FKey& InKey, bool bInShift = false, bool bInCtrl = false, bool bInAlt = false)
		: Key(InKey), bShift(bInShift), bCtrl(bInCtrl), bAlt(bInAlt) {}

	bool IsValid() const { return Key.IsValid(); }
	bool operator==(const FEVKeyChord& O) const { return Key == O.Key && bShift == O.bShift && bCtrl == O.bCtrl && bAlt == O.bAlt; }
	bool operator!=(const FEVKeyChord& O) const { return !(*this == O); }

	/** Stable text used in the ini file, e.g. "Ctrl+Shift+K". */
	FString ToConfigString() const;
	static FEVKeyChord FromConfigString(const FString& In);
	/** Text for the UI, e.g. "Ctrl+Shift+K" or "Not Bound". */
	FText ToDisplayText() const;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVMenuItem
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName Id = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	EEVMenuItemKind Kind = EEVMenuItemKind::Action;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FText Label;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FText Tooltip;

	/** Action to run (Action / Toggle / Radio). Its key binding is shown on the right. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName ActionId = NAME_None;

	/** Menu opened by a Submenu item. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName SubmenuId = NAME_None;

	/** Setting driven by a Toggle (bool) or Radio (value) item. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName SettingKey = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FString RadioValue;

	/** Icon texture /Game/GoonZu/Icons/T_<IconName>. Optional. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName IconName = NAME_None;

	/** Alpha 0 = default text colour. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FLinearColor TextColor = FLinearColor(1.f, 1.f, 1.f, 0.f);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	bool bEnabled = true;

	static FEVMenuItem MakeAction(FName Id, const FText& Label, FName ActionId, const FText& Tooltip = FText::GetEmpty());
	static FEVMenuItem MakeSubmenu(FName Id, const FText& Label, FName SubmenuId, const FText& Tooltip = FText::GetEmpty());
	static FEVMenuItem MakeToggle(FName Id, const FText& Label, FName SettingKey, const FText& Tooltip = FText::GetEmpty());
	static FEVMenuItem MakeRadio(FName Id, const FText& Label, FName SettingKey, const FString& Value);
	static FEVMenuItem MakeSeparator();
	static FEVMenuItem MakeHeader(const FText& Label);
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVMenuDefinition
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FName MenuId = NAME_None;

	/** Optional title drawn at the top of the dropdown. "{Context}" is replaced by the menu's context tag. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	FText Title;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	TArray<FEVMenuItem> Items;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Menu")
	float MinWidth = 190.f;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVPanelTab
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	FName TabId = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	FText Label;

	/** UMG content for this tab. Native code can use UEVUISubsystem::RegisterPanelContent instead. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	TSubclassOf<UUserWidget> WidgetClass;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVPanelDefinition
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	FName PanelId = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	FText Title;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	EEVPanelArea Area = EEVPanelArea::Center;

	/** Size in Slate units (before UI Scale). Height 0 = fit content. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	FVector2D Size = FVector2D(560.f, 460.f);

	/** Tabs along the bottom edge. Leave empty for a single page (TabId NAME_None). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	TArray<FEVPanelTab> Tabs;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	bool bCloseOnEscape = true;

	/** When this panel replaced another one in the same area, closing it re-opens that one
	 *  (Options opened from the Game Menu goes back to the Game Menu). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Panels")
	bool bReturnToPrevious = false;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVMicroButton
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FName Id = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FText Label;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FText Tooltip;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FName IconName = NAME_None;

	/** Short text drawn when the icon texture is missing. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FText Glyph;

	/** Left-click. The button lights up while this action reports "checked". */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FName ActionId = NAME_None;

	/** Right-click dropdown. Optional. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|MicroMenu")
	FName ContextMenuId = NAME_None;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVOptionsCategory
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FName Id = NAME_None;

	/** NAME_None for a top-level category. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FName ParentId = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FText Label;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FText Description;

	/** Special pages: "KeyBindings". NAME_None = page generated from the category's settings. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FName Page = NAME_None;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVSettingDefinition
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FName Key = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FName CategoryId = NAME_None;

	/** Sub-heading inside the category page. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FText Section;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FText Label;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FText Tooltip;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	EEVSettingType Type = EEVSettingType::Checkbox;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	float Min = 0.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	float Max = 1.f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	float Step = 0.05f;

	/** Slider shows Value*100 with a % sign. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	bool bPercent = false;

	/** Choice labels; the stored value is the index. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	TArray<FText> Choices;

	/** "1"/"0" for checkboxes, a number for sliders, an index for choices. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Options")
	FString DefaultValue;
};

USTRUCT(BlueprintType)
struct ETHEREALVENI_API FEVKeyBindingDefinition
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FName ActionId = NAME_None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FText Label;

	/** Group heading in the Key Bindings page, e.g. "Interface Panels". */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FText Group;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FEVKeyChord DefaultPrimary;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UI|Bindings")
	FEVKeyChord DefaultSecondary;
};
