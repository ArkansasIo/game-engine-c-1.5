#include "UI/SEVRoot.h"
#include "UI/SEVMenu.h"
#include "UI/SEVPanels.h"
#include "UI/EVUISubsystem.h"
#include "UI/EVUIStyle.h"

#include "Framework/Application/SlateApplication.h"
#include "Widgets/SOverlay.h"
#include "Widgets/SBoxPanel.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Layout/SConstraintCanvas.h"
#include "Widgets/Layout/SDPIScaler.h"
#include "Widgets/Text/STextBlock.h"

namespace
{
	constexpr double MessageLifetime = 4.0;
	constexpr double MessageFade = 1.0;

	FAnchors AnchorsFor(EEVPanelArea Area)
	{
		switch (Area)
		{
		case EEVPanelArea::Left: return FAnchors(0.f, 0.f);
		case EEVPanelArea::Right: return FAnchors(1.f, 0.f);
		default: return FAnchors(0.5f, 0.45f);
		}
	}

	FVector2D AlignmentFor(EEVPanelArea Area)
	{
		switch (Area)
		{
		case EEVPanelArea::Left: return FVector2D(0.f, 0.f);
		case EEVPanelArea::Right: return FVector2D(1.f, 0.f);
		default: return FVector2D(0.5f, 0.5f);
		}
	}
}

void SEVRoot::Construct(const FArguments& InArgs, UEVUISubsystem* InUI)
{
	UI = InUI;
	TWeakObjectPtr<UEVUISubsystem> WeakUI(InUI);

	auto InterfaceVisibility = [WeakUI]()
	{
		return (WeakUI.IsValid() && WeakUI->IsInterfaceVisible()) ? EVisibility::SelfHitTestInvisible : EVisibility::Collapsed;
	};

	SetVisibility(EVisibility::SelfHitTestInvisible);

	ChildSlot
	[
		SNew(SDPIScaler)
		.DPIScale_Lambda([WeakUI]() { return WeakUI.IsValid() ? WeakUI->GetUIScale() : 1.f; })
		[
			SNew(SOverlay)
			.Visibility(EVisibility::SelfHitTestInvisible)

			// Movable panels
			+ SOverlay::Slot()
			[
				SAssignNew(PanelCanvas, SConstraintCanvas)
				.Visibility_Lambda(InterfaceVisibility)
			]

			// Micro menu, bottom right
			+ SOverlay::Slot()
			.HAlign(HAlign_Right)
			.VAlign(VAlign_Bottom)
			.Padding(FMargin(0.f, 0.f, 16.f, 14.f))
			[
				SAssignNew(MicroMenuHost, SBox)
				.Visibility_Lambda([WeakUI]()
				{
					const bool bShow = WeakUI.IsValid() && WeakUI->IsInterfaceVisible()
						&& (!WeakUI->FindSetting(TEXT("interface.show_micromenu")) || WeakUI->GetSettingBool(TEXT("interface.show_micromenu")));
					return bShow ? EVisibility::SelfHitTestInvisible : EVisibility::Collapsed;
				})
			]

			// Messages, top centre
			+ SOverlay::Slot()
			.HAlign(HAlign_Center)
			.VAlign(VAlign_Top)
			.Padding(FMargin(0.f, 120.f, 0.f, 0.f))
			[
				SAssignNew(MessageList, SVerticalBox)
				.Visibility(EVisibility::HitTestInvisible)
			]

			// Popup dialog with dimmed backdrop
			+ SOverlay::Slot()
			[
				SNew(SBorder)
				.BorderImage(EVUIStyle::WhiteBrush())
				.BorderBackgroundColor(FLinearColor(0.f, 0.f, 0.f, 0.45f))
				.Visibility_Lambda([this]() { return bPopupOpen ? EVisibility::Visible : EVisibility::Collapsed; })
				.OnMouseButtonDown_Lambda([](const FGeometry&, const FPointerEvent&) { return FReply::Handled(); })   // modal: nothing behind it is clickable
				.HAlign(HAlign_Center)
				.VAlign(VAlign_Center)
				[
					SAssignNew(PopupHost, SBox)
				]
			]

			// Click catcher: any click outside a dropdown closes all dropdowns.
			+ SOverlay::Slot()
			[
				SNew(SBorder)
				.BorderImage(EVUIStyle::NoBrush())
				.Visibility_Lambda([this]() { return MenuLevels.Num() > 0 ? EVisibility::Visible : EVisibility::Collapsed; })
				.OnMouseButtonDown_Lambda([this](const FGeometry&, const FPointerEvent&)
				{
					CloseMenus(0);
					return FReply::Handled();
				})
			]

			// Dropdown menus
			+ SOverlay::Slot()
			[
				SAssignNew(MenuCanvas, SConstraintCanvas)
				.Visibility(EVisibility::SelfHitTestInvisible)
			]
		]
	];

	RebuildMicroMenu();
	RebuildPanels();
}

float SEVRoot::GetScale() const
{
	return UI.IsValid() ? UI->GetUIScale() : 1.f;
}

void SEVRoot::Tick(const FGeometry& AllottedGeometry, const double InCurrentTime, const float InDeltaTime)
{
	SCompoundWidget::Tick(AllottedGeometry, InCurrentTime, InDeltaTime);
	CachedGeometry = AllottedGeometry;

	const double Now = FSlateApplication::Get().GetCurrentTime();
	const int32 Before = Messages.Num();
	Messages.RemoveAll([Now](const FMessage& M) { return Now - M.Time > MessageLifetime; });
	if (Messages.Num() != Before) RebuildMessages();
}

FVector2D SEVRoot::AbsoluteToLayer(const FVector2D& Absolute) const
{
	return CachedGeometry.AbsoluteToLocal(Absolute) / GetScale();
}

FVector2D SEVRoot::GetCursorPosition() const
{
	return AbsoluteToLayer(FSlateApplication::Get().GetCursorPos());
}

FVector2D SEVRoot::GetLayerSize() const
{
	return CachedGeometry.GetLocalSize() / GetScale();
}

// ---------------------------------------------------------------- panels

void SEVRoot::RebuildPanels()
{
	UEVUISubsystem* S = UI.Get();
	if (!S || !PanelCanvas.IsValid()) return;

	const TArray<FName>& Order = S->GetOpenPanelOrder();

	// Forget windows that were closed so they rebuild fresh next time.
	for (auto It = PanelWidgets.CreateIterator(); It; ++It)
	{
		if (!Order.Contains(It.Key())) It.RemoveCurrent();
	}
	if (Order == BuiltPanelOrder) return;
	BuiltPanelOrder = Order;

	PanelCanvas->ClearChildren();
	TWeakObjectPtr<UEVUISubsystem> WeakUI(S);
	for (FName PanelId : Order)
	{
		const FEVPanelDefinition* Def = S->FindPanel(PanelId);
		if (!Def) continue;

		TSharedPtr<SWidget>& Widget = PanelWidgets.FindOrAdd(PanelId);
		if (!Widget.IsValid())
		{
			Widget = SNew(SEVWindow, S, PanelId);
		}

		PanelCanvas->AddSlot()
			.Anchors(AnchorsFor(Def->Area))
			.Alignment(AlignmentFor(Def->Area))
			.AutoSize(true)
			.Offset_Lambda([WeakUI, PanelId]()
			{
				const FVector2D O = WeakUI.IsValid() ? WeakUI->GetPanelOffset(PanelId) : FVector2D::ZeroVector;
				return FMargin(O.X, O.Y, 0.f, 0.f);
			})
			[
				Widget.ToSharedRef()
			];
	}
}

void SEVRoot::RebuildMicroMenu()
{
	if (MicroMenuHost.IsValid() && UI.IsValid())
	{
		MicroMenuHost->SetContent(SNew(SEVMicroMenu, UI.Get()));
	}
}

// ---------------------------------------------------------------- dropdowns

bool SEVRoot::IsMenuLevelShowing(int32 Level, FName MenuId) const
{
	return MenuLevels.IsValidIndex(Level) && MenuLevels[Level].MenuId == MenuId;
}

void SEVRoot::OpenMenu(FName MenuId, FVector2D Position, int32 Level, float FlipX)
{
	UEVUISubsystem* S = UI.Get();
	if (!S || !MenuCanvas.IsValid() || !S->FindMenu(MenuId)) return;

	Level = FMath::Clamp(Level, 0, MenuLevels.Num());
	CloseMenus(Level);

	TSharedRef<SEVMenuPanel> Panel = SNew(SEVMenuPanel, S, MenuId, Level);
	Panel->SlatePrepass(1.f);
	const FVector2D Size = Panel->GetDesiredSize();
	const FVector2D View = GetLayerSize();

	FVector2D Pos = Position;
	if (Pos.X + Size.X > View.X)
	{
		Pos.X = (FlipX >= 0.f) ? FlipX - Size.X : View.X - Size.X - 4.f;
	}
	if (Pos.Y + Size.Y > View.Y)
	{
		Pos.Y = View.Y - Size.Y - 4.f;
	}
	Pos.X = FMath::Max(Pos.X, 4.f);
	Pos.Y = FMath::Max(Pos.Y, 4.f);

	MenuCanvas->AddSlot()
		.Offset(FMargin(Pos.X, Pos.Y, 0.f, 0.f))
		.AutoSize(true)
		[
			Panel
		];

	MenuLevels.Add({MenuId, Panel});
	if (Level == 0) FocusMenuLevel(0);
}

void SEVRoot::CloseMenus(int32 FromLevel)
{
	bool bClosedAny = false;
	while (MenuLevels.Num() > FromLevel)
	{
		const FMenuLevel Last = MenuLevels.Pop();
		if (Last.Widget.IsValid() && MenuCanvas.IsValid())
		{
			MenuCanvas->RemoveSlot(Last.Widget.ToSharedRef());
		}
		bClosedAny = true;
	}
	if (bClosedAny && MenuLevels.Num() == 0 && FSlateApplication::IsInitialized())
	{
		FSlateApplication::Get().SetAllUserFocusToGameViewport();
	}
}

void SEVRoot::FocusMenuLevel(int32 Level)
{
	if (MenuLevels.IsValidIndex(Level) && MenuLevels[Level].Widget.IsValid())
	{
		FSlateApplication::Get().SetKeyboardFocus(MenuLevels[Level].Widget, EFocusCause::SetDirectly);
	}
}

// ---------------------------------------------------------------- popup & messages

void SEVRoot::ShowPopup(TSharedRef<SWidget> Popup)
{
	if (!PopupHost.IsValid()) return;
	PopupHost->SetContent(Popup);
	bPopupOpen = true;
}

void SEVRoot::ClosePopup()
{
	if (PopupHost.IsValid()) PopupHost->SetContent(SNullWidget::NullWidget);
	bPopupOpen = false;
	if (FSlateApplication::IsInitialized()) FSlateApplication::Get().SetAllUserFocusToGameViewport();
}

void SEVRoot::AddMessage(const FText& Text, const FLinearColor& Color)
{
	FMessage M;
	M.Text = Text;
	M.Color = Color;
	M.Time = FSlateApplication::Get().GetCurrentTime();
	Messages.Add(M);
	while (Messages.Num() > 4) Messages.RemoveAt(0);
	RebuildMessages();
}

void SEVRoot::RebuildMessages()
{
	if (!MessageList.IsValid()) return;
	MessageList->ClearChildren();
	for (const FMessage& M : Messages)
	{
		const double Born = M.Time;
		const FLinearColor Color = M.Color;
		MessageList->AddSlot().AutoHeight().HAlign(HAlign_Center).Padding(0.f, 1.f)
		[
			SNew(STextBlock)
			.Text(M.Text)
			.Font(EVUIStyle::Font(15, true))
			.ShadowOffset(FVector2D(1.f, 1.f))
			.ShadowColorAndOpacity(FLinearColor(0.f, 0.f, 0.f, 0.9f))
			.ColorAndOpacity_Lambda([Born, Color]()
			{
				const double Age = FSlateApplication::Get().GetCurrentTime() - Born;
				const float Alpha = (float)FMath::Clamp((MessageLifetime - Age) / MessageFade, 0.0, 1.0);
				return FSlateColor(FLinearColor(Color.R, Color.G, Color.B, Alpha));
			})
		];
	}
}
