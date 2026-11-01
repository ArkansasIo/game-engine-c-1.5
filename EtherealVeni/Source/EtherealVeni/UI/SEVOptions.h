#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

class UEVUISubsystem;
class SBox;
class SVerticalBox;
struct FEVSettingDefinition;

/**
 * Options window: collapsible category tree on the left, the selected category's page on the right,
 * Defaults / Okay / Cancel / Apply along the bottom. Edits are held as pending values until applied.
 * The "KeyBindings" page lists every registered binding with two rebindable slots.
 */
class ETHEREALVENI_API SEVOptions : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVOptions) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI);

private:
	void SelectCategory(FName CategoryId);
	void RebuildTree();
	void RebuildPage();
	TSharedRef<SWidget> MakeSettingRow(const FEVSettingDefinition& Setting);
	TSharedRef<SWidget> MakeKeyBindingsPage();
	TSharedRef<SWidget> MakeCategoryIndexPage(FName CategoryId);

	FString GetValue(FName Key) const;
	void SetPending(FName Key, const FString& Value);
	void Apply();
	void RestoreDefaults();
	bool HasPending() const { return Pending.Num() > 0; }

	TWeakObjectPtr<UEVUISubsystem> UI;
	TSharedPtr<SVerticalBox> Tree;
	TSharedPtr<SBox> Page;
	FName Current;
	TSet<FName> Collapsed;
	TMap<FName, FString> Pending;
};
