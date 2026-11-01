#include "UI/SEVMenu.h"
#include "UI/SEVRoot.h"
#include "UI/EVUISubsystem.h"
#include "UI/EVUIStyle.h"

#include "Brushes/SlateRoundedBoxBrush.h"
#include "Widgets/SBoxPanel.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Images/SImage.h"
#include "Widgets/Text/STextBlock.h"

namespace
{
	const FSlateBrush* CheckBoxEmpty()
	{
		static const FSlateRoundedBoxBrush B(FLinearColor(0.f, 0.f, 0.f, 0.6f), 2.f, EVUIStyle::Trim, 1.f, FVector2D(12.f, 12.f));
		return &B;
	}
	const FSlateBrush* CheckBoxFull()
	{
		static const FSlateRoundedBoxBrush B(EVUIStyle::Accent, 2.f, EVUIStyle::TrimBright, 1.f, FVector2D(12.f, 12.f));
		return &B;
	}
	const FSlateBrush* RadioEmpty()
	{
		static const FSlateRoundedBoxBrush B(FLinearColor(0.f, 0.f, 0.f, 0.6f), 6.f, EVUIStyle::Trim, 1.f, FVector2D(12.f, 12.f));
		return &B;
	}
	const FSlateBrush* RadioFull()
	{
		static const FSlateRoundedBoxBrush B(EVUIStyle::Accent, 6.f, EVUIStyle::TrimBright, 1.f, FVector2D(12.f, 12.f));
		return &B;
	}
	const FSlateBrush* RowHighlight()
	{
		static const FSlateRoundedBoxBrush B(EVUIStyle::RowHover, 3.f);
		return &B;
	}
}

/** A single clickable row. Highlight comes from the owning panel's selection. */
class SEVMenuRow : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVMenuRow) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, TSharedRef<SEVMenuPanel> InOwner, UEVUISubsystem* InUI, int32 InIndex)
	{
		Owner = InOwner;
		UI = InUI;
		Index = InIndex;
		const FEVMenuItem& Item = InOwner->GetItem(InIndex);
		const FEVMenuItem ItemCopy = Item;
		TWeakObjectPtr<UEVUISubsystem> WeakUI(InUI);
		TWeakPtr<SEVMenuPanel> WeakOwner = InOwner;
		const int32 RowIndex = InIndex;

		auto IsEnabled = [WeakUI, ItemCopy]() { return WeakUI.IsValid() && WeakUI->IsMenuItemEnabled(ItemCopy); };

		// Left column: check box / radio dot.
		TSharedRef<SWidget> Mark = SNew(SBox).WidthOverride(18.f);
		if (Item.Kind == EEVMenuItemKind::Toggle || Item.Kind == EEVMenuItemKind::Radio)
		{
			const bool bRadio = Item.Kind == EEVMenuItemKind::Radio;
			Mark = SNew(SBox).WidthOverride(18.f).HAlign(HAlign_Left).VAlign(VAlign_Center)
				[
					SNew(SBox).WidthOverride(12.f).HeightOverride(12.f)
					[
						SNew(SImage).Image_Lambda([WeakUI, ItemCopy, bRadio]()
						{
							const bool bOn = WeakUI.IsValid() && WeakUI->IsMenuItemChecked(ItemCopy);
							return bRadio ? (bOn ? RadioFull() : RadioEmpty()) : (bOn ? CheckBoxFull() : CheckBoxEmpty());
						})
					]
				];
		}

		TSharedRef<SHorizontalBox> Line = SNew(SHorizontalBox)
			+ SHorizontalBox::Slot().AutoWidth().VAlign(VAlign_Center)[Mark];

		if (const FSlateBrush* Icon = InUI->GetIconBrush(Item.IconName))
		{
			Line->AddSlot().AutoWidth().VAlign(VAlign_Center).Padding(0.f, 0.f, 6.f, 0.f)
			[
				SNew(SBox).WidthOverride(18.f).HeightOverride(18.f)[SNew(SImage).Image(Icon)]
			];
		}

		const FLinearColor Custom = Item.TextColor;
		Line->AddSlot().FillWidth(1.f).VAlign(VAlign_Center)
		[
			SNew(STextBlock)
			.Text(Item.Label)
			.Font(EVUIStyle::Font(11))
			.ColorAndOpacity_Lambda([IsEnabled, Custom]()
			{
				if (!IsEnabled()) return FSlateColor(EVUIStyle::TextDisabled);
				return FSlateColor(Custom.A > 0.f ? Custom : EVUIStyle::Text);
			})
		];

		if (Item.Kind == EEVMenuItemKind::Submenu)
		{
			Line->AddSlot().AutoWidth().VAlign(VAlign_Center).Padding(14.f, 0.f, 0.f, 0.f)
			[
				SNew(STextBlock).Text(FText::FromString(TEXT("›"))).Font(EVUIStyle::Font(13, true)).ColorAndOpacity(EVUIStyle::Accent)
			];
		}
		else if (!Item.ActionId.IsNone())
		{
			const FName ActionId = Item.ActionId;
			Line->AddSlot().AutoWidth().VAlign(VAlign_Center).Padding(18.f, 0.f, 0.f, 0.f)
			[
				SNew(STextBlock)
				.Text_Lambda([WeakUI, ActionId]() { return WeakUI.IsValid() ? WeakUI->GetShortcutText(ActionId) : FText::GetEmpty(); })
				.Font(EVUIStyle::Font(9))
				.ColorAndOpacity(EVUIStyle::TextDim)
			];
		}

		ChildSlot
		[
			SNew(SBorder)
			.BorderImage_Lambda([WeakOwner, RowIndex, IsEnabled]()
			{
				const TSharedPtr<SEVMenuPanel> P = WeakOwner.Pin();
				return (P.IsValid() && P->GetSelected() == RowIndex && IsEnabled()) ? RowHighlight() : EVUIStyle::NoBrush();
			})
			.Padding(FMargin(8.f, 4.f, 10.f, 4.f))
			[
				Line
			]
		];

		if (!Item.Tooltip.IsEmpty())
		{
			SetToolTipText(Item.Tooltip);
		}
	}

	virtual void OnMouseEnter(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		SCompoundWidget::OnMouseEnter(MyGeometry, MouseEvent);
		if (TSharedPtr<SEVMenuPanel> P = Owner.Pin()) P->OnRowHovered(Index);
	}

	virtual FReply OnMouseButtonDown(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		return FReply::Handled();
	}

	virtual FReply OnMouseButtonUp(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		if (MouseEvent.GetEffectingButton() == EKeys::LeftMouseButton || MouseEvent.GetEffectingButton() == EKeys::RightMouseButton)
		{
			if (TSharedPtr<SEVMenuPanel> P = Owner.Pin()) P->OnRowClicked(Index);
		}
		return FReply::Handled();
	}

private:
	TWeakPtr<SEVMenuPanel> Owner;
	TWeakObjectPtr<UEVUISubsystem> UI;
	int32 Index = 0;
};

// ====================================================================== panel

void SEVMenuPanel::Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InMenuId, int32 InLevel)
{
	UI = InUI;
	MenuId = InMenuId;
	Level = InLevel;

	const FEVMenuDefinition* Def = InUI ? InUI->FindMenu(InMenuId) : nullptr;
	if (Def) Items = Def->Items;

	TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);

	const FText Title = Def ? InUI->ResolveMenuTitle(*Def) : FText::GetEmpty();
	if (!Title.IsEmpty())
	{
		Box->AddSlot().AutoHeight().Padding(10.f, 4.f, 10.f, 2.f)
		[
			SNew(STextBlock).Text(Title).Font(EVUIStyle::Font(12, true)).ColorAndOpacity(EVUIStyle::Accent)
		];
		Box->AddSlot().AutoHeight().Padding(6.f, 3.f)
		[
			SNew(SBox).HeightOverride(1.f)[SNew(SImage).Image(EVUIStyle::WhiteBrush()).ColorAndOpacity(EVUIStyle::Trim)]
		];
	}

	Rows.SetNum(Items.Num());
	for (int32 i = 0; i < Items.Num(); ++i)
	{
		const FEVMenuItem& Item = Items[i];
		switch (Item.Kind)
		{
		case EEVMenuItemKind::Separator:
			Box->AddSlot().AutoHeight().Padding(6.f, 4.f)
			[
				SNew(SBox).HeightOverride(1.f)[SNew(SImage).Image(EVUIStyle::WhiteBrush()).ColorAndOpacity(FLinearColor(EVUIStyle::Trim.R, EVUIStyle::Trim.G, EVUIStyle::Trim.B, 0.55f))]
			];
			break;
		case EEVMenuItemKind::Header:
			Box->AddSlot().AutoHeight().Padding(10.f, 5.f, 10.f, 1.f)
			[
				SNew(STextBlock).Text(Item.Label).Font(EVUIStyle::Font(9, true)).ColorAndOpacity(EVUIStyle::Accent)
			];
			break;
		default:
		{
			TSharedRef<SEVMenuRow> Row = SNew(SEVMenuRow, SharedThis(this), InUI, i);
			Rows[i] = Row;
			Box->AddSlot().AutoHeight()[Row];
			break;
		}
		}
	}

	ChildSlot
	[
		SNew(SBorder)
		.BorderImage(EVUIStyle::MenuBrush())
		.Padding(FMargin(3.f, 5.f))
		[
			SNew(SBox).MinDesiredWidth(Def ? Def->MinWidth : 190.f)
			[
				Box
			]
		]
	];
}

bool SEVMenuPanel::IsSelectable(int32 Index) const
{
	return Items.IsValidIndex(Index) && Rows.IsValidIndex(Index) && Rows[Index].IsValid() && UI.IsValid() && UI->IsMenuItemEnabled(Items[Index]);
}

void SEVMenuPanel::MoveSelection(int32 Dir)
{
	if (Items.Num() == 0) return;
	int32 I = Selected;
	for (int32 Step = 0; Step < Items.Num(); ++Step)
	{
		I = (I == INDEX_NONE) ? (Dir > 0 ? 0 : Items.Num() - 1) : (I + Dir + Items.Num()) % Items.Num();
		if (IsSelectable(I))
		{
			Selected = I;
			return;
		}
	}
}

void SEVMenuPanel::OpenSubmenu(int32 Index, bool bFocusChild)
{
	UEVUISubsystem* S = UI.Get();
	TSharedPtr<SEVRoot> Root = S ? S->GetRoot() : nullptr;
	if (!Root.IsValid() || !IsSelectable(Index) || Items[Index].Kind != EEVMenuItemKind::Submenu) return;

	if (OpenSubIndex != Index || !Root->IsMenuLevelShowing(Level + 1, Items[Index].SubmenuId))
	{
		const FGeometry& RowGeo = Rows[Index]->GetTickSpaceGeometry();
		const FGeometry& MyGeo = GetTickSpaceGeometry();
		const FVector2D TopRight = Root->AbsoluteToLayer(RowGeo.LocalToAbsolute(FVector2D(MyGeo.GetLocalSize().X, 0.f)));
		const FVector2D PanelRight = Root->AbsoluteToLayer(MyGeo.LocalToAbsolute(FVector2D(MyGeo.GetLocalSize().X, 0.f)));
		const FVector2D PanelLeft = Root->AbsoluteToLayer(MyGeo.LocalToAbsolute(FVector2D::ZeroVector));
		Root->OpenMenu(Items[Index].SubmenuId, FVector2D(PanelRight.X - 2.f, TopRight.Y - 5.f), Level + 1, PanelLeft.X + 2.f);
		OpenSubIndex = Index;
	}
	if (bFocusChild) Root->FocusMenuLevel(Level + 1);
}

void SEVMenuPanel::OnRowHovered(int32 Index)
{
	if (!IsSelectable(Index))
	{
		Selected = INDEX_NONE;
		return;
	}
	Selected = Index;
	UEVUISubsystem* S = UI.Get();
	TSharedPtr<SEVRoot> Root = S ? S->GetRoot() : nullptr;
	if (!Root.IsValid()) return;

	if (Items[Index].Kind == EEVMenuItemKind::Submenu)
	{
		OpenSubmenu(Index, false);
	}
	else
	{
		Root->CloseMenus(Level + 1);
		OpenSubIndex = INDEX_NONE;
	}
}

void SEVMenuPanel::OnRowClicked(int32 Index)
{
	if (!IsSelectable(Index)) return;
	if (Items[Index].Kind == EEVMenuItemKind::Submenu)
	{
		OpenSubmenu(Index, true);
		return;
	}
	if (UEVUISubsystem* S = UI.Get())
	{
		S->ActivateMenuItem(MenuId, Items[Index].Id);
	}
}

FReply SEVMenuPanel::OnKeyDown(const FGeometry& MyGeometry, const FKeyEvent& InKeyEvent)
{
	const FKey Key = InKeyEvent.GetKey();
	UEVUISubsystem* S = UI.Get();
	TSharedPtr<SEVRoot> Root = S ? S->GetRoot() : nullptr;

	if (Key == EKeys::Down) { MoveSelection(1); return FReply::Handled(); }
	if (Key == EKeys::Up) { MoveSelection(-1); return FReply::Handled(); }
	if (Key == EKeys::Right)
	{
		if (IsSelectable(Selected) && Items[Selected].Kind == EEVMenuItemKind::Submenu) OpenSubmenu(Selected, true);
		return FReply::Handled();
	}
	if (Key == EKeys::Left)
	{
		if (Root.IsValid() && Level > 0)
		{
			Root->CloseMenus(Level);
			Root->FocusMenuLevel(Level - 1);
		}
		return FReply::Handled();
	}
	if (Key == EKeys::Enter || Key == EKeys::SpaceBar)
	{
		OnRowClicked(Selected);
		return FReply::Handled();
	}
	return FReply::Unhandled();
}
