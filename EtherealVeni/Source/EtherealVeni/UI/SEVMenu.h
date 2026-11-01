#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"
#include "UI/EVUITypes.h"

class UEVUISubsystem;

/**
 * One level of a cascading dropdown. Rows can be actions, checkboxes, radio options, headers,
 * separators or sub-menus; hovering (or Right arrow on) a sub-menu row opens the next level
 * beside it through SEVRoot. Keyboard: Up/Down select, Right/Enter open, Left back, Enter/Space activate.
 */
class ETHEREALVENI_API SEVMenuPanel : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVMenuPanel) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InMenuId, int32 InLevel);

	virtual bool SupportsKeyboardFocus() const override { return true; }
	virtual FReply OnKeyDown(const FGeometry& MyGeometry, const FKeyEvent& InKeyEvent) override;
	virtual FReply OnMouseButtonDown(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override { return FReply::Handled(); }
	virtual FReply OnMouseButtonUp(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override { return FReply::Handled(); }

	FName GetMenuId() const { return MenuId; }
	int32 GetLevel() const { return Level; }
	int32 GetSelected() const { return Selected; }
	const FEVMenuItem& GetItem(int32 Index) const { return Items[Index]; }

	void OnRowHovered(int32 Index);
	void OnRowClicked(int32 Index);

private:
	bool IsSelectable(int32 Index) const;
	void MoveSelection(int32 Dir);
	void OpenSubmenu(int32 Index, bool bFocusChild);

	TWeakObjectPtr<UEVUISubsystem> UI;
	FName MenuId;
	int32 Level = 0;
	int32 Selected = INDEX_NONE;
	int32 OpenSubIndex = INDEX_NONE;
	TArray<FEVMenuItem> Items;
	TArray<TSharedPtr<SWidget>> Rows;
};
