#include "UI/SEVPanels.h"
#include "UI/SEVRoot.h"
#include "UI/EVUISubsystem.h"
#include "UI/EVUIStyle.h"

#include "Brushes/SlateRoundedBoxBrush.h"
#include "Widgets/SBoxPanel.h"
#include "Widgets/SOverlay.h"
#include "Widgets/Images/SImage.h"
#include "Widgets/Input/SButton.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Layout/SSpacer.h"
#include "Widgets/Layout/SWidgetSwitcher.h"
#include "Widgets/Text/STextBlock.h"

#define LOCTEXT_NAMESPACE "EVUI"

namespace
{
	constexpr float TitleBarHeight = 34.f;

	const FSlateBrush* TabActiveBrush()
	{
		static const FSlateRoundedBoxBrush B(EVUIStyle::RowSelected, FVector4(0.f, 0.f, 5.f, 5.f), EVUIStyle::TrimBright, 1.f);
		return &B;
	}
	const FSlateBrush* TabIdleBrush()
	{
		static const FSlateRoundedBoxBrush B(EVUIStyle::PanelFillDark, FVector4(0.f, 0.f, 5.f, 5.f), EVUIStyle::Trim, 1.f);
		return &B;
	}
	const FSlateBrush* ActiveGlow()
	{
		static const FSlateRoundedBoxBrush B(FLinearColor::Transparent, 4.f, EVUIStyle::Accent, 2.f);
		return &B;
	}
}

// ====================================================================== helpers

namespace EVUIWidgets
{
	TSharedRef<SWidget> MakeSectionHeader(const FText& Text)
	{
		return SNew(SVerticalBox)
			+ SVerticalBox::Slot().AutoHeight()
			[
				SNew(STextBlock).Text(Text).Font(EVUIStyle::Font(12, true)).ColorAndOpacity(EVUIStyle::Accent)
			]
			+ SVerticalBox::Slot().AutoHeight().Padding(0.f, 3.f, 0.f, 6.f)
			[
				SNew(SBox).HeightOverride(1.f)[SNew(SImage).Image(EVUIStyle::WhiteBrush()).ColorAndOpacity(EVUIStyle::Trim)]
			];
	}

	TSharedRef<SWidget> MakeParagraph(const FText& Text, bool bDim)
	{
		return SNew(STextBlock)
			.Text(Text)
			.Font(EVUIStyle::Font(10))
			.ColorAndOpacity(bDim ? EVUIStyle::TextDim : EVUIStyle::Text)
			.AutoWrapText(true);
	}

	TSharedRef<SWidget> MakeConfirmPopup(UEVUISubsystem* UI, const FText& Title, const FText& Message, const FText& ConfirmLabel, TFunction<void()> OnConfirm)
	{
		TWeakObjectPtr<UEVUISubsystem> WeakUI(UI);
		return SNew(SBox)
			.WidthOverride(380.f)
			[
				SNew(SBorder)
				.BorderImage(EVUIStyle::PanelBrush())
				.Padding(FMargin(18.f, 14.f))
				.OnMouseButtonDown_Lambda([](const FGeometry&, const FPointerEvent&) { return FReply::Handled(); })
				[
					SNew(SVerticalBox)
					+ SVerticalBox::Slot().AutoHeight().HAlign(HAlign_Center)
					[
						SNew(STextBlock).Text(Title).Font(EVUIStyle::Font(14, true)).ColorAndOpacity(EVUIStyle::Accent)
					]
					+ SVerticalBox::Slot().AutoHeight().Padding(0.f, 10.f, 0.f, 16.f)
					[
						SNew(STextBlock).Text(Message).Font(EVUIStyle::Font(11)).ColorAndOpacity(EVUIStyle::Text)
						.AutoWrapText(true).Justification(ETextJustify::Center)
					]
					+ SVerticalBox::Slot().AutoHeight().HAlign(HAlign_Center)
					[
						SNew(SHorizontalBox)
						+ SHorizontalBox::Slot().AutoWidth().Padding(6.f, 0.f)
						[
							SNew(SBox).MinDesiredWidth(120.f)
							[
								SNew(SButton)
								.ButtonStyle(&EVUIStyle::Button())
								.IsFocusable(false)
								.HAlign(HAlign_Center)
								.OnClicked_Lambda([WeakUI, OnConfirm]()
								{
									if (WeakUI.IsValid()) WeakUI->ClosePopup();
									if (OnConfirm) OnConfirm();
									return FReply::Handled();
								})
								[
									SNew(STextBlock).Text(ConfirmLabel).Font(EVUIStyle::Font(11, true))
								]
							]
						]
						+ SHorizontalBox::Slot().AutoWidth().Padding(6.f, 0.f)
						[
							SNew(SBox).MinDesiredWidth(120.f)
							[
								SNew(SButton)
								.ButtonStyle(&EVUIStyle::Button())
								.IsFocusable(false)
								.HAlign(HAlign_Center)
								.OnClicked_Lambda([WeakUI]()
								{
									if (WeakUI.IsValid()) WeakUI->ClosePopup();
									return FReply::Handled();
								})
								[
									SNew(STextBlock).Text(LOCTEXT("Cancel", "Cancel")).Font(EVUIStyle::Font(11, true))
								]
							]
						]
					]
				]
			];
	}
}

// ====================================================================== window frame

void SEVWindow::Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InPanelId)
{
	UI = InUI;
	PanelId = InPanelId;
	const FEVPanelDefinition* Def = InUI->FindPanel(InPanelId);
	check(Def);

	TWeakObjectPtr<UEVUISubsystem> WeakUI(InUI);
	const FName Id = InPanelId;

	// Content: one page per tab (or a single page).
	TSharedRef<SWidgetSwitcher> Switcher = SNew(SWidgetSwitcher)
		.WidgetIndex_Lambda([this]() { return GetActiveTabIndex(); });
	if (Def->Tabs.Num() == 0)
	{
		TabIds.Add(NAME_None);
		Switcher->AddSlot()[InUI->BuildPanelContent(Id, NAME_None)];
	}
	for (const FEVPanelTab& Tab : Def->Tabs)
	{
		TabIds.Add(Tab.TabId);
		Switcher->AddSlot()[InUI->BuildPanelContent(Id, Tab.TabId)];
	}

	TSharedRef<SVerticalBox> Body = SNew(SVerticalBox)
		// Title bar
		+ SVerticalBox::Slot().AutoHeight()
		[
			SNew(SBox).HeightOverride(TitleBarHeight)
			[
				SNew(SBorder)
				.BorderImage(EVUIStyle::TitleBrush())
				.Padding(FMargin(12.f, 0.f, 4.f, 0.f))
				.VAlign(VAlign_Center)
				[
					SNew(SHorizontalBox)
					+ SHorizontalBox::Slot().FillWidth(1.f).VAlign(VAlign_Center)
					[
						SNew(STextBlock).Text(Def->Title).Font(EVUIStyle::Font(13, true)).ColorAndOpacity(EVUIStyle::Accent)
						.ShadowOffset(FVector2D(1.f, 1.f)).ShadowColorAndOpacity(EVUIStyle::Shadow)
					]
					+ SHorizontalBox::Slot().AutoWidth().VAlign(VAlign_Center)
					[
						SNew(SBox).WidthOverride(26.f).HeightOverride(24.f)
						[
							SNew(SButton)
							.ButtonStyle(&EVUIStyle::FlatButton())
							.IsFocusable(false)
							.HAlign(HAlign_Center).VAlign(VAlign_Center)
							.ToolTipText(LOCTEXT("CloseTip", "Close (Esc)"))
							.OnClicked_Lambda([WeakUI, Id]() { if (WeakUI.IsValid()) WeakUI->ClosePanel(Id); return FReply::Handled(); })
							[
								SNew(STextBlock).Text(INVTEXT("X")).Font(EVUIStyle::Font(11, true)).ColorAndOpacity(EVUIStyle::Text)
							]
						]
					]
				]
			]
		]
		+ SVerticalBox::Slot().AutoHeight()
		[
			SNew(SBox).HeightOverride(1.f)[SNew(SImage).Image(EVUIStyle::WhiteBrush()).ColorAndOpacity(EVUIStyle::TrimBright)]
		];

	if (Def->Size.Y > 0.f)
	{
		Body->AddSlot().FillHeight(1.f).Padding(12.f, 10.f)[Switcher];
	}
	else
	{
		Body->AddSlot().AutoHeight().Padding(12.f, 10.f)[Switcher];
	}

	TSharedRef<SWidget> Framed = SNew(SBorder)
		.BorderImage(EVUIStyle::PanelBrush())
		.Padding(FMargin(1.f))
		[
			Body
		];

	// Tabs hang under the frame.
	TSharedRef<SVerticalBox> Outer = SNew(SVerticalBox);
	if (Def->Size.Y > 0.f)
	{
		Outer->AddSlot().FillHeight(1.f)[Framed];
	}
	else
	{
		Outer->AddSlot().AutoHeight()[Framed];
	}

	if (Def->Tabs.Num() > 1)
	{
		TSharedRef<SHorizontalBox> Strip = SNew(SHorizontalBox);
		for (int32 i = 0; i < Def->Tabs.Num(); ++i)
		{
			const FName TabId = Def->Tabs[i].TabId;
			Strip->AddSlot().AutoWidth().Padding(i == 0 ? 14.f : 2.f, 0.f, 0.f, 0.f)
			[
				SNew(SButton)
				.ButtonStyle(&EVUIStyle::FlatButton())
				.ContentPadding(FMargin(0.f))
				.IsFocusable(false)
				.OnClicked_Lambda([WeakUI, Id, TabId]() { if (WeakUI.IsValid()) WeakUI->SetPanelTab(Id, TabId); return FReply::Handled(); })
				[
					SNew(SBorder)
					.BorderImage_Lambda([this, i]() { return GetActiveTabIndex() == i ? TabActiveBrush() : TabIdleBrush(); })
					.Padding(FMargin(16.f, 5.f, 16.f, 7.f))
					[
						SNew(STextBlock)
						.Text(Def->Tabs[i].Label)
						.Font(EVUIStyle::Font(10, true))
						.ColorAndOpacity_Lambda([this, i]() { return FSlateColor(GetActiveTabIndex() == i ? EVUIStyle::Accent : EVUIStyle::TextDim); })
					]
				]
			];
		}
		Outer->AddSlot().AutoHeight()[Strip];
	}

	ChildSlot
	[
		SNew(SBox)
		.WidthOverride(Def->Size.X)
		.HeightOverride(Def->Size.Y > 0.f ? FOptionalSize(Def->Size.Y) : FOptionalSize())
		[
			Outer
		]
	];
}

int32 SEVWindow::GetActiveTabIndex() const
{
	const FName Tab = UI.IsValid() ? UI->GetPanelTab(PanelId) : NAME_None;
	const int32 Index = TabIds.IndexOfByKey(Tab);
	return Index == INDEX_NONE ? 0 : Index;
}

FReply SEVWindow::OnMouseButtonDown(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent)
{
	if (UI.IsValid())
	{
		UI->CloseMenus();
		UI->BringPanelToFront(PanelId);
	}
	if (MouseEvent.GetEffectingButton() == EKeys::LeftMouseButton)
	{
		const FVector2D Local = MyGeometry.AbsoluteToLocal(MouseEvent.GetScreenSpacePosition());
		if (Local.Y <= TitleBarHeight)
		{
			bDragging = true;
			return FReply::Handled().CaptureMouse(SharedThis(this));
		}
	}
	// Swallow clicks so they never reach the world behind the window.
	return FReply::Handled();
}

FReply SEVWindow::OnMouseButtonUp(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent)
{
	if (bDragging)
	{
		bDragging = false;
		return FReply::Handled().ReleaseMouseCapture();
	}
	return FReply::Handled();
}

FReply SEVWindow::OnMouseMove(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent)
{
	if (bDragging && HasMouseCapture() && UI.IsValid())
	{
		UI->MovePanel(PanelId, MouseEvent.GetCursorDelta() / FMath::Max(MyGeometry.Scale, 0.01f));
		return FReply::Handled();
	}
	return FReply::Unhandled();
}

// ====================================================================== game menu

void SEVGameMenu::Construct(const FArguments& InArgs, UEVUISubsystem* InUI, FName InMenuId)
{
	UI = InUI;
	MenuId = InMenuId;
	TWeakObjectPtr<UEVUISubsystem> WeakUI(InUI);
	const FEVMenuDefinition* Def = InUI->FindMenu(InMenuId);

	TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
	if (Def)
	{
		for (const FEVMenuItem& Item : Def->Items)
		{
			if (Item.Kind == EEVMenuItemKind::Separator)
			{
				Box->AddSlot().AutoHeight()[SNew(SSpacer).Size(FVector2D(1.f, 10.f))];
				continue;
			}
			if (Item.Kind == EEVMenuItemKind::Header)
			{
				Box->AddSlot().AutoHeight().Padding(2.f, 6.f, 2.f, 2.f)
				[
					SNew(STextBlock).Text(Item.Label).Font(EVUIStyle::Font(9, true)).ColorAndOpacity(EVUIStyle::TextDim)
				];
				continue;
			}

			const FEVMenuItem ItemCopy = Item;
			const FName Menu = InMenuId;
			TSharedPtr<SButton> Button;
			TSharedRef<SHorizontalBox> Label = SNew(SHorizontalBox)
				+ SHorizontalBox::Slot().FillWidth(1.f).HAlign(HAlign_Center)
				[
					SNew(STextBlock).Text(Item.Label).Font(EVUIStyle::Font(12, true))
					.ColorAndOpacity(Item.TextColor.A > 0.f ? FSlateColor(Item.TextColor) : FSlateColor::UseForeground())
				];
			if (Item.Kind == EEVMenuItemKind::Submenu)
			{
				Label->AddSlot().AutoWidth()[SNew(STextBlock).Text(FText::FromString(TEXT("›"))).Font(EVUIStyle::Font(13, true)).ColorAndOpacity(EVUIStyle::Accent)];
			}

			Box->AddSlot().AutoHeight().Padding(0.f, 3.f)
			[
				SNew(SBox).HeightOverride(34.f)
				[
					SAssignNew(Button, SButton)
					.ButtonStyle(&EVUIStyle::Button())
					.IsFocusable(false)
					.VAlign(VAlign_Center)
					.ToolTipText(Item.Tooltip)
					.IsEnabled_Lambda([WeakUI, ItemCopy]() { return WeakUI.IsValid() && WeakUI->IsMenuItemEnabled(ItemCopy); })
					[
						Label
					]
				]
			];

			TWeakPtr<SButton> WeakButton = Button;
			Button->SetOnClicked(FOnClicked::CreateLambda([WeakUI, WeakButton, ItemCopy, Menu]()
			{
				UEVUISubsystem* S = WeakUI.Get();
				if (!S) return FReply::Handled();
				if (ItemCopy.Kind == EEVMenuItemKind::Submenu)
				{
					TSharedPtr<SEVRoot> Root = S->GetRoot();
					TSharedPtr<SButton> B = WeakButton.Pin();
					if (Root.IsValid() && B.IsValid())
					{
						const FGeometry& Geo = B->GetTickSpaceGeometry();
						const FVector2D Right = Root->AbsoluteToLayer(Geo.LocalToAbsolute(FVector2D(Geo.GetLocalSize().X, 0.f)));
						const FVector2D Left = Root->AbsoluteToLayer(Geo.LocalToAbsolute(FVector2D::ZeroVector));
						Root->OpenMenu(ItemCopy.SubmenuId, FVector2D(Right.X + 4.f, Right.Y), 0, Left.X - 4.f);
					}
				}
				else
				{
					S->ActivateMenuItem(Menu, ItemCopy.Id);
				}
				return FReply::Handled();
			}));
		}
	}

	ChildSlot
	[
		SNew(SBox).Padding(FMargin(6.f, 2.f, 6.f, 4.f))
		[
			Box
		]
	];
}

// ====================================================================== micro menu

void SEVMicroMenu::Construct(const FArguments& InArgs, UEVUISubsystem* InUI)
{
	TWeakObjectPtr<UEVUISubsystem> WeakUI(InUI);
	TSharedRef<SHorizontalBox> Row = SNew(SHorizontalBox);

	for (const FEVMicroButton& Btn : InUI->GetMicroButtons())
	{
		const FName ActionId = Btn.ActionId;
		const FName ContextMenuId = Btn.ContextMenuId;
		const FText Label = Btn.Label;
		const FText Tip = Btn.Tooltip;
		const FSlateBrush* Icon = InUI->GetIconBrush(Btn.IconName);

		TSharedRef<SWidget> Face = Icon
			? StaticCastSharedRef<SWidget>(SNew(SImage).Image(Icon))
			: StaticCastSharedRef<SWidget>(SNew(SBox).HAlign(HAlign_Center).VAlign(VAlign_Center)
				[
					SNew(STextBlock).Text(Btn.Glyph.IsEmpty() ? FText::FromString(Label.ToString().Left(1)) : Btn.Glyph)
					.Font(EVUIStyle::Font(14, true)).ColorAndOpacity(EVUIStyle::Accent)
				]);

		Row->AddSlot().AutoWidth().Padding(2.f, 0.f)
		[
			SNew(SBorder)
			.BorderImage(EVUIStyle::NoBrush())
			.Padding(0.f)
			.OnMouseButtonDown_Lambda([WeakUI, ContextMenuId, Label](const FGeometry&, const FPointerEvent& E)
			{
				if (E.GetEffectingButton() == EKeys::RightMouseButton && WeakUI.IsValid() && !ContextMenuId.IsNone())
				{
					WeakUI->OpenContextMenu(ContextMenuId, Label.ToString());
					return FReply::Handled();
				}
				return FReply::Unhandled();
			})
			[
				SNew(SBox).WidthOverride(40.f).HeightOverride(40.f)
				[
					SNew(SOverlay)
					+ SOverlay::Slot()
					[
						SNew(SButton)
						.ButtonStyle(&EVUIStyle::SlotButton())
						.IsFocusable(false)
						.ContentPadding(FMargin(3.f))
						.ToolTipText_Lambda([WeakUI, ActionId, Label, Tip, ContextMenuId]()
						{
							if (!WeakUI.IsValid() || !WeakUI->GetBoolSettingFor(WeakUI->GetPlayerController(), TEXT("interface.tooltips"), true)) return FText::GetEmpty();
							const FText Key = WeakUI->GetShortcutText(ActionId);
							FText Head = Key.IsEmpty() ? Label : FText::Format(INVTEXT("{0}  ({1})"), Label, Key);
							if (!Tip.IsEmpty()) Head = FText::Format(INVTEXT("{0}\n{1}"), Head, Tip);
							if (!ContextMenuId.IsNone()) Head = FText::Format(LOCTEXT("RightClickMore", "{0}\nRight-click for more options."), Head);
							return Head;
						})
						.OnClicked_Lambda([WeakUI, ActionId]() { if (WeakUI.IsValid()) WeakUI->ExecuteAction(ActionId); return FReply::Handled(); })
						[
							Face
						]
					]
					// Lit frame while the window it opens is showing.
					+ SOverlay::Slot()
					[
						SNew(SImage)
						.Visibility(EVisibility::HitTestInvisible)
						.Image(ActiveGlow())
						.ColorAndOpacity_Lambda([WeakUI, ActionId]()
						{
							return FSlateColor(FLinearColor(1.f, 1.f, 1.f, (WeakUI.IsValid() && WeakUI->IsActionChecked(ActionId)) ? 1.f : 0.f));
						})
					]
				]
			]
		];
	}

	ChildSlot
	[
		SNew(SBorder)
		.BorderImage(EVUIStyle::PanelBrush())
		.Padding(FMargin(5.f))
		.OnMouseButtonDown_Lambda([](const FGeometry&, const FPointerEvent&) { return FReply::Handled(); })
		[
			Row
		]
	];
}

#undef LOCTEXT_NAMESPACE
