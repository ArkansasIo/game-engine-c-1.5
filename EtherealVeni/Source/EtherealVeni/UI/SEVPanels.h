#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

class UEVUISubsystem;
class SButton;

/**
 * Movable window frame: title bar (drag to move, X to close), content area and an optional
 * tab strip along the bottom. Content per tab comes from UEVUISubsystem::BuildPanelContent.
 */
class ETHEREALVENI_API SEVWindow : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVWindow) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InPanelId);

	virtual FReply OnMouseButtonDown(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override;
	virtual FReply OnMouseButtonUp(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override;
	virtual FReply OnMouseMove(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override;
	virtual FReply OnMouseWheel(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override { return FReply::Handled(); }

private:
	int32 GetActiveTabIndex() const;

	TWeakObjectPtr<UEVUISubsystem> UI;
	FName PanelId;
	TArray<FName> TabIds;
	bool bDragging = false;
};

/** The Escape menu: one large button per item of the "game_menu" menu definition. */
class ETHEREALVENI_API SEVGameMenu : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVGameMenu) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InMenuId);

private:
	TWeakObjectPtr<UEVUISubsystem> UI;
	FName MenuId;
};

/** Bottom-right button row. Left-click runs the button's action, right-click opens its dropdown. */
class ETHEREALVENI_API SEVMicroMenu : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVMicroMenu) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI);
};

namespace EVUIWidgets
{
	/** Yes / Cancel dialog shown by UEVUISubsystem::ShowConfirmation. */
	TSharedRef<SWidget> MakeConfirmPopup(UEVUISubsystem* UI, const FText& Title, const FText& Message, const FText& ConfirmLabel, TFunction<void()> OnConfirm);

	/** Gold section heading with an underline, used on pages and in panels. */
	TSharedRef<SWidget> MakeSectionHeader(const FText& Text);

	/** Wrapped body text. */
	TSharedRef<SWidget> MakeParagraph(const FText& Text, bool bDim = false);
}
