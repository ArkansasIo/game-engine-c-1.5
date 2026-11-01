#include "UI/SEVOptions.h"
#include "UI/SEVPanels.h"
#include "UI/SEVRoot.h"
#include "UI/EVUISubsystem.h"
#include "UI/EVUIStyle.h"

#include "Widgets/SBoxPanel.h"
#include "Widgets/Images/SImage.h"
#include "Widgets/Input/SButton.h"
#include "Widgets/Input/SCheckBox.h"
#include "Widgets/Input/SSlider.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Layout/SScrollBox.h"
#include "Widgets/Layout/SSpacer.h"
#include "Widgets/Text/STextBlock.h"

#define LOCTEXT_NAMESPACE "EVUI"

namespace
{
	const FName KeyBindingsPage(TEXT("KeyBindings"));
	constexpr float LabelWidth = 230.f;

	FString FormatSlider(const FEVSettingDefinition& S, float V)
	{
		if (S.bPercent) return FString::Printf(TEXT("%d%%"), FMath::RoundToInt(V * 100.f));
		return S.Step >= 1.f ? FString::Printf(TEXT("%d"), FMath::RoundToInt(V)) : FString::Printf(TEXT("%.2f"), V);
	}
}

void SEVOptions::Construct(const FArguments& InArgs, UEVUISubsystem* InUI)
{
	UI = InUI;
	TWeakPtr<SEVOptions> WeakThis = SharedThis(this);

	auto BottomButton = [](const FText& Label, TFunction<FReply()> OnClick, TAttribute<bool> Enabled = true)
	{
		return SNew(SBox).MinDesiredWidth(104.f).HeightOverride(30.f)
			[
				SNew(SButton)
				.ButtonStyle(&EVUIStyle::Button())
				.IsFocusable(false)
				.HAlign(HAlign_Center).VAlign(VAlign_Center)
				.IsEnabled(Enabled)
				.OnClicked_Lambda([OnClick]() { return OnClick(); })
				[
					SNew(STextBlock).Text(Label).Font(EVUIStyle::Font(11, true))
				]
			];
	};

	ChildSlot
	[
		SNew(SVerticalBox)
		+ SVerticalBox::Slot().FillHeight(1.f)
		[
			SNew(SHorizontalBox)
			// Category tree
			+ SHorizontalBox::Slot().AutoWidth()
			[
				SNew(SBox).WidthOverride(210.f)
				[
					SNew(SBorder).BorderImage(EVUIStyle::WellBrush()).Padding(4.f)
					[
						SNew(SScrollBox).ScrollBarStyle(&EVUIStyle::ScrollBar())
						+ SScrollBox::Slot()[SAssignNew(Tree, SVerticalBox)]
					]
				]
			]
			// Page
			+ SHorizontalBox::Slot().FillWidth(1.f).Padding(10.f, 0.f, 0.f, 0.f)
			[
				SNew(SBorder).BorderImage(EVUIStyle::WellBrush()).Padding(FMargin(14.f, 10.f))
				[
					SAssignNew(Page, SBox)
				]
			]
		]
		+ SVerticalBox::Slot().AutoHeight().Padding(0.f, 10.f, 0.f, 0.f)
		[
			SNew(SHorizontalBox)
			+ SHorizontalBox::Slot().AutoWidth()
			[
				BottomButton(LOCTEXT("Defaults", "Defaults"), [WeakThis]()
				{
					if (TSharedPtr<SEVOptions> Self = WeakThis.Pin()) Self->RestoreDefaults();
					return FReply::Handled();
				})
			]
			+ SHorizontalBox::Slot().FillWidth(1.f).VAlign(VAlign_Center).Padding(12.f, 0.f)
			[
				SNew(STextBlock)
				.Font(EVUIStyle::Font(9))
				.ColorAndOpacity(EVUIStyle::TextDim)
				.Text_Lambda([WeakThis]()
				{
					const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
					return (Self.IsValid() && Self->HasPending()) ? LOCTEXT("Unsaved", "You have unapplied changes.") : FText::GetEmpty();
				})
			]
			+ SHorizontalBox::Slot().AutoWidth().Padding(4.f, 0.f)
			[
				BottomButton(LOCTEXT("Okay", "Okay"), [WeakThis]()
				{
					if (TSharedPtr<SEVOptions> Self = WeakThis.Pin())
					{
						Self->Apply();
						if (Self->UI.IsValid()) Self->UI->ClosePanel(TEXT("options"));
					}
					return FReply::Handled();
				})
			]
			+ SHorizontalBox::Slot().AutoWidth().Padding(4.f, 0.f)
			[
				BottomButton(LOCTEXT("Cancel", "Cancel"), [WeakThis]()
				{
					if (TSharedPtr<SEVOptions> Self = WeakThis.Pin())
					{
						Self->Pending.Reset();
						if (Self->UI.IsValid()) Self->UI->ClosePanel(TEXT("options"));
					}
					return FReply::Handled();
				})
			]
			+ SHorizontalBox::Slot().AutoWidth().Padding(4.f, 0.f, 0.f, 0.f)
			[
				BottomButton(LOCTEXT("Apply", "Apply"), [WeakThis]()
				{
					if (TSharedPtr<SEVOptions> Self = WeakThis.Pin()) Self->Apply();
					return FReply::Handled();
				}, TAttribute<bool>::CreateLambda([WeakThis]()
				{
					const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
					return Self.IsValid() && Self->HasPending();
				}))
			]
		]
	];

	FName Start = InUI->GetRequestedOptionsCategory();
	const TArray<FEVOptionsCategory>& Cats = InUI->GetOptionsCategories();
	if (!Cats.ContainsByPredicate([Start](const FEVOptionsCategory& C) { return C.Id == Start; }))
	{
		const FEVOptionsCategory* FirstLeaf = Cats.FindByPredicate([](const FEVOptionsCategory& C) { return !C.ParentId.IsNone(); });
		Start = FirstLeaf ? FirstLeaf->Id : (Cats.Num() ? Cats[0].Id : NAME_None);
	}
	SelectCategory(Start);
}

void SEVOptions::SelectCategory(FName CategoryId)
{
	Current = CategoryId;
	// Make sure the selected category is visible in the tree.
	if (UI.IsValid())
	{
		if (const FEVOptionsCategory* C = UI->GetOptionsCategories().FindByPredicate([CategoryId](const FEVOptionsCategory& X) { return X.Id == CategoryId; }))
		{
			Collapsed.Remove(C->ParentId);
		}
	}
	RebuildTree();
	RebuildPage();
}

void SEVOptions::RebuildTree()
{
	UEVUISubsystem* S = UI.Get();
	if (!S || !Tree.IsValid()) return;
	Tree->ClearChildren();
	TWeakPtr<SEVOptions> WeakThis = SharedThis(this);

	const TArray<FEVOptionsCategory>& Cats = S->GetOptionsCategories();
	auto AddRow = [&](const FEVOptionsCategory& C, int32 Depth, bool bHasChildren)
	{
		const FName Id = C.Id;
		const bool bSelected = Current == Id;
		const bool bOpen = !Collapsed.Contains(Id);
		FText Label = C.Label;
		if (bHasChildren) Label = FText::Format(INVTEXT("{0} {1}"), FText::FromString(bOpen ? TEXT("−") : TEXT("+")), C.Label);

		Tree->AddSlot().AutoHeight().Padding(0.f, 1.f)
		[
			SNew(SButton)
			.ButtonStyle(bSelected ? &EVUIStyle::FlatButtonSelected() : &EVUIStyle::FlatButton())
			.IsFocusable(false)
			.ContentPadding(FMargin(8.f + Depth * 16.f, 4.f, 6.f, 4.f))
			.OnClicked_Lambda([WeakThis, Id, bHasChildren]()
			{
				if (TSharedPtr<SEVOptions> Self = WeakThis.Pin())
				{
					if (bHasChildren && Self->Current == Id)
					{
						if (Self->Collapsed.Contains(Id)) Self->Collapsed.Remove(Id); else Self->Collapsed.Add(Id);
						Self->RebuildTree();
					}
					else
					{
						Self->SelectCategory(Id);
					}
				}
				return FReply::Handled();
			})
			[
				SNew(STextBlock)
				.Text(Label)
				.Font(EVUIStyle::Font(Depth == 0 ? 11 : 10, Depth == 0))
				.ColorAndOpacity(bSelected ? EVUIStyle::Accent : (Depth == 0 ? EVUIStyle::Text : EVUIStyle::TextDim))
			]
		];
	};

	for (const FEVOptionsCategory& Top : Cats)
	{
		if (!Top.ParentId.IsNone()) continue;
		const bool bHasChildren = Cats.ContainsByPredicate([&Top](const FEVOptionsCategory& C) { return C.ParentId == Top.Id; });
		AddRow(Top, 0, bHasChildren);
		if (bHasChildren && !Collapsed.Contains(Top.Id))
		{
			for (const FEVOptionsCategory& Child : Cats)
			{
				if (Child.ParentId == Top.Id) AddRow(Child, 1, false);
			}
		}
	}
}

FString SEVOptions::GetValue(FName Key) const
{
	if (const FString* P = Pending.Find(Key)) return *P;
	return UI.IsValid() ? UI->GetSettingString(Key) : FString();
}

void SEVOptions::SetPending(FName Key, const FString& Value)
{
	if (UI.IsValid() && UI->GetSettingString(Key) == Value) Pending.Remove(Key);
	else Pending.Add(Key, Value);
}

void SEVOptions::Apply()
{
	UEVUISubsystem* S = UI.Get();
	if (!S || Pending.Num() == 0) return;
	// Presets first so individual quality overrides applied in the same batch win.
	TArray<FName> Keys;
	Pending.GetKeys(Keys);
	Keys.Sort([](const FName& A, const FName& B) { return (A == TEXT("gfx.preset")) > (B == TEXT("gfx.preset")); });
	const TMap<FName, FString> Batch = Pending;
	Pending.Reset();
	for (FName Key : Keys) S->SetSetting(Key, Batch[Key]);
	S->ShowMessage(LOCTEXT("Applied", "Settings applied"), EVUIStyle::Good);
	RebuildPage();
}

void SEVOptions::RestoreDefaults()
{
	UEVUISubsystem* S = UI.Get();
	if (!S) return;
	const FEVOptionsCategory* Cat = S->GetOptionsCategories().FindByPredicate([this](const FEVOptionsCategory& C) { return C.Id == Current; });
	if (Cat && Cat->Page == KeyBindingsPage)
	{
		TWeakPtr<SEVOptions> WeakThis = SharedThis(this);
		S->ShowConfirmationNative(LOCTEXT("ResetKeysTitle", "Reset Key Bindings"),
			LOCTEXT("ResetKeysMsg", "Restore every key binding to its default?"),
			[WeakThis]()
			{
				if (TSharedPtr<SEVOptions> Self = WeakThis.Pin())
				{
					if (Self->UI.IsValid()) Self->UI->ResetAllBindings();
					Self->RebuildPage();
				}
			}, LOCTEXT("Reset", "Reset"));
		return;
	}
	for (const FEVSettingDefinition* Setting : S->GetSettingsInCategory(Current))
	{
		SetPending(Setting->Key, Setting->DefaultValue);
	}
	RebuildPage();
}

void SEVOptions::RebuildPage()
{
	UEVUISubsystem* S = UI.Get();
	if (!S || !Page.IsValid()) return;
	const FEVOptionsCategory* Cat = S->GetOptionsCategories().FindByPredicate([this](const FEVOptionsCategory& C) { return C.Id == Current; });
	if (!Cat)
	{
		Page->SetContent(EVUIWidgets::MakeParagraph(LOCTEXT("NoCats", "No option categories registered."), true));
		return;
	}
	if (Cat->Page == KeyBindingsPage)
	{
		Page->SetContent(MakeKeyBindingsPage());
		return;
	}

	const TArray<const FEVSettingDefinition*> Items = S->GetSettingsInCategory(Current);
	if (Items.Num() == 0)
	{
		Page->SetContent(MakeCategoryIndexPage(Current));
		return;
	}

	TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
	Box->AddSlot().AutoHeight()
	[
		SNew(STextBlock).Text(Cat->Label).Font(EVUIStyle::Font(16, true)).ColorAndOpacity(EVUIStyle::Accent)
	];
	if (!Cat->Description.IsEmpty())
	{
		Box->AddSlot().AutoHeight().Padding(0.f, 2.f, 0.f, 8.f)[EVUIWidgets::MakeParagraph(Cat->Description, true)];
	}

	FString LastSection = TEXT("\x01");
	for (const FEVSettingDefinition* Setting : Items)
	{
		if (Setting->Section.ToString() != LastSection)
		{
			LastSection = Setting->Section.ToString();
			if (!LastSection.IsEmpty())
			{
				Box->AddSlot().AutoHeight().Padding(0.f, 10.f, 0.f, 0.f)[EVUIWidgets::MakeSectionHeader(Setting->Section)];
			}
		}
		Box->AddSlot().AutoHeight().Padding(0.f, 3.f)[MakeSettingRow(*Setting)];
	}

	Page->SetContent(
		SNew(SScrollBox).ScrollBarStyle(&EVUIStyle::ScrollBar())
		+ SScrollBox::Slot().Padding(0.f, 0.f, 10.f, 0.f)[Box]);
}

TSharedRef<SWidget> SEVOptions::MakeCategoryIndexPage(FName CategoryId)
{
	UEVUISubsystem* S = UI.Get();
	TWeakPtr<SEVOptions> WeakThis = SharedThis(this);
	const FEVOptionsCategory* Cat = S->GetOptionsCategories().FindByPredicate([CategoryId](const FEVOptionsCategory& C) { return C.Id == CategoryId; });

	TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
	Box->AddSlot().AutoHeight()[SNew(STextBlock).Text(Cat ? Cat->Label : FText()).Font(EVUIStyle::Font(16, true)).ColorAndOpacity(EVUIStyle::Accent)];
	if (Cat && !Cat->Description.IsEmpty())
	{
		Box->AddSlot().AutoHeight().Padding(0.f, 2.f, 0.f, 10.f)[EVUIWidgets::MakeParagraph(Cat->Description, true)];
	}
	for (const FEVOptionsCategory& Child : S->GetOptionsCategories())
	{
		if (Child.ParentId != CategoryId) continue;
		const FName Id = Child.Id;
		Box->AddSlot().AutoHeight().Padding(0.f, 3.f)
		[
			SNew(SButton)
			.ButtonStyle(&EVUIStyle::FlatButton())
			.IsFocusable(false)
			.OnClicked_Lambda([WeakThis, Id]() { if (TSharedPtr<SEVOptions> Self = WeakThis.Pin()) Self->SelectCategory(Id); return FReply::Handled(); })
			[
				SNew(SVerticalBox)
				+ SVerticalBox::Slot().AutoHeight()[SNew(STextBlock).Text(Child.Label).Font(EVUIStyle::Font(12, true)).ColorAndOpacity(EVUIStyle::Text)]
				+ SVerticalBox::Slot().AutoHeight()[EVUIWidgets::MakeParagraph(Child.Description, true)]
			]
		];
	}
	return Box;
}

TSharedRef<SWidget> SEVOptions::MakeSettingRow(const FEVSettingDefinition& Setting)
{
	TWeakPtr<SEVOptions> WeakThis = SharedThis(this);
	const FName Key = Setting.Key;
	const FEVSettingDefinition Def = Setting;

	auto IsChanged = [WeakThis, Key]()
	{
		const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
		return Self.IsValid() && Self->Pending.Contains(Key);
	};

	TSharedRef<SWidget> Control = SNullWidget::NullWidget;
	switch (Setting.Type)
	{
	case EEVSettingType::Checkbox:
		Control = SNew(SCheckBox)
			.Style(&EVUIStyle::CheckBox())
			.IsFocusable(false)
			.IsChecked_Lambda([WeakThis, Key]()
			{
				const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
				return (Self.IsValid() && Self->GetValue(Key) == TEXT("1")) ? ECheckBoxState::Checked : ECheckBoxState::Unchecked;
			})
			.OnCheckStateChanged_Lambda([WeakThis, Key](ECheckBoxState State)
			{
				if (TSharedPtr<SEVOptions> Self = WeakThis.Pin()) Self->SetPending(Key, State == ECheckBoxState::Checked ? TEXT("1") : TEXT("0"));
			});
		break;

	case EEVSettingType::Slider:
		Control = SNew(SHorizontalBox)
			+ SHorizontalBox::Slot().FillWidth(1.f).VAlign(VAlign_Center)
			[
				SNew(SSlider)
				.Style(&EVUIStyle::Slider())
				.IsFocusable(false)
				.MinValue(Def.Min)
				.MaxValue(Def.Max)
				.StepSize(Def.Step)
				.MouseUsesStep(true)
				.Value_Lambda([WeakThis, Key]()
				{
					const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
					return Self.IsValid() ? FCString::Atof(*Self->GetValue(Key)) : 0.f;
				})
				.OnValueChanged_Lambda([WeakThis, Key, Def](float V)
				{
					const float Snapped = FMath::Clamp(FMath::GridSnap(V, Def.Step), Def.Min, Def.Max);
					if (TSharedPtr<SEVOptions> Self = WeakThis.Pin()) Self->SetPending(Key, FString::SanitizeFloat(Snapped));
				})
			]
			+ SHorizontalBox::Slot().AutoWidth().VAlign(VAlign_Center).Padding(10.f, 0.f, 0.f, 0.f)
			[
				SNew(SBox).WidthOverride(48.f)
				[
					SNew(STextBlock)
					.Font(EVUIStyle::Font(10, true))
					.ColorAndOpacity(EVUIStyle::Text)
					.Text_Lambda([WeakThis, Key, Def]()
					{
						const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
						return FText::FromString(Self.IsValid() ? FormatSlider(Def, FCString::Atof(*Self->GetValue(Key))) : FString());
					})
				]
			];
		break;

	case EEVSettingType::Choice:
	{
		TSharedPtr<SButton> Button;
		Control = SAssignNew(Button, SButton)
			.ButtonStyle(&EVUIStyle::Button())
			.IsFocusable(false)
			.ContentPadding(FMargin(10.f, 4.f))
			[
				SNew(SHorizontalBox)
				+ SHorizontalBox::Slot().FillWidth(1.f)
				[
					SNew(STextBlock)
					.Font(EVUIStyle::Font(10))
					.Text_Lambda([WeakThis, Key, Def]()
					{
						const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
						const int32 Index = Self.IsValid() ? FCString::Atoi(*Self->GetValue(Key)) : 0;
						return Def.Choices.IsValidIndex(Index) ? Def.Choices[Index] : LOCTEXT("Custom", "Custom");
					})
				]
				+ SHorizontalBox::Slot().AutoWidth().Padding(8.f, 0.f, 0.f, 0.f)
				[
					SNew(STextBlock).Text(FText::FromString(TEXT("▾"))).Font(EVUIStyle::Font(10)).ColorAndOpacity(EVUIStyle::Accent)
				]
			];
		TWeakPtr<SButton> WeakButton = Button;
		Button->SetOnClicked(FOnClicked::CreateLambda([WeakThis, WeakButton, Key, Def]()
		{
			const TSharedPtr<SEVOptions> Self = WeakThis.Pin();
			const TSharedPtr<SButton> B = WeakButton.Pin();
			UEVUISubsystem* S = Self.IsValid() ? Self->UI.Get() : nullptr;
			if (!S || !B.IsValid() || !S->GetRoot().IsValid()) return FReply::Handled();
			const FGeometry& Geo = B->GetTickSpaceGeometry();
			const FVector2D Below = S->GetRoot()->AbsoluteToLayer(Geo.LocalToAbsolute(FVector2D(0.f, Geo.GetLocalSize().Y)));
			const float Width = Geo.GetLocalSize().X;
			S->OpenChoiceMenu(Def.Choices, FCString::Atoi(*Self->GetValue(Key)), [WeakThis, Key](int32 Picked)
			{
				if (TSharedPtr<SEVOptions> Me = WeakThis.Pin()) Me->SetPending(Key, FString::FromInt(Picked));
			}, Below + FVector2D(0.f, 2.f), Width);
			return FReply::Handled();
		}));
		break;
	}
	}

	return SNew(SHorizontalBox)
		.ToolTipText(Setting.Tooltip)
		+ SHorizontalBox::Slot().AutoWidth().VAlign(VAlign_Center)
		[
			SNew(SBox).WidthOverride(LabelWidth)
			[
				SNew(STextBlock)
				.Text(Setting.Label)
				.Font(EVUIStyle::Font(11))
				.ColorAndOpacity_Lambda([IsChanged]() { return FSlateColor(IsChanged() ? EVUIStyle::Accent : EVUIStyle::Text); })
			]
		]
		+ SHorizontalBox::Slot().FillWidth(1.f).VAlign(VAlign_Center).HAlign(Setting.Type == EEVSettingType::Checkbox ? HAlign_Left : HAlign_Fill)
		[
			SNew(SBox).MaxDesiredWidth(300.f)[Control]
		];
}

TSharedRef<SWidget> SEVOptions::MakeKeyBindingsPage()
{
	UEVUISubsystem* S = UI.Get();
	TWeakObjectPtr<UEVUISubsystem> WeakUI(S);

	TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
	Box->AddSlot().AutoHeight()[SNew(STextBlock).Text(LOCTEXT("KeyBindings", "Key Bindings")).Font(EVUIStyle::Font(16, true)).ColorAndOpacity(EVUIStyle::Accent)];
	Box->AddSlot().AutoHeight().Padding(0.f, 2.f, 0.f, 6.f)
	[
		EVUIWidgets::MakeParagraph(LOCTEXT("KeyHelp", "Click a slot and press a key (Ctrl / Alt / Shift combinations work). Esc cancels, right-click clears. Bindings are saved immediately."), true)
	];

	// Column headings
	Box->AddSlot().AutoHeight().Padding(0.f, 4.f)
	[
		SNew(SHorizontalBox)
		+ SHorizontalBox::Slot().FillWidth(1.f)[SNew(STextBlock).Text(LOCTEXT("Command", "Command")).Font(EVUIStyle::Font(9, true)).ColorAndOpacity(EVUIStyle::TextDim)]
		+ SHorizontalBox::Slot().AutoWidth()[SNew(SBox).WidthOverride(148.f).Padding(4.f, 0.f)[SNew(STextBlock).Text(LOCTEXT("Key1", "Key 1")).Font(EVUIStyle::Font(9, true)).ColorAndOpacity(EVUIStyle::TextDim)]]
		+ SHorizontalBox::Slot().AutoWidth()[SNew(SBox).WidthOverride(148.f).Padding(4.f, 0.f)[SNew(STextBlock).Text(LOCTEXT("Key2", "Key 2")).Font(EVUIStyle::Font(9, true)).ColorAndOpacity(EVUIStyle::TextDim)]]
	];

	FString LastGroup = TEXT("\x01");
	for (const FEVKeyBindingDefinition& B : S->GetKeyBindings())
	{
		if (B.Group.ToString() != LastGroup)
		{
			LastGroup = B.Group.ToString();
			Box->AddSlot().AutoHeight().Padding(0.f, 10.f, 0.f, 0.f)[EVUIWidgets::MakeSectionHeader(B.Group)];
		}

		const FName ActionId = B.ActionId;
		auto SlotButton = [WeakUI, ActionId](int32 Slot)
		{
			return SNew(SBox).WidthOverride(148.f).HeightOverride(26.f).Padding(4.f, 0.f)
				[
					SNew(SBorder)
					.BorderImage(EVUIStyle::NoBrush())
					.Padding(0.f)
					.OnMouseButtonDown_Lambda([WeakUI, ActionId, Slot](const FGeometry&, const FPointerEvent& E)
					{
						if (E.GetEffectingButton() == EKeys::RightMouseButton && WeakUI.IsValid())
						{
							WeakUI->CancelKeyCapture();
							WeakUI->ClearBinding(ActionId, Slot);
							return FReply::Handled();
						}
						return FReply::Unhandled();
					})
					[
						SNew(SButton)
						.ButtonStyle(&EVUIStyle::Button())
						.IsFocusable(false)
						.HAlign(HAlign_Center).VAlign(VAlign_Center)
						.ContentPadding(FMargin(4.f, 2.f))
						.OnClicked_Lambda([WeakUI, ActionId, Slot]()
						{
							if (WeakUI.IsValid())
							{
								if (WeakUI->IsCapturing(ActionId, Slot)) WeakUI->CancelKeyCapture();
								else WeakUI->BeginKeyCapture(ActionId, Slot);
							}
							return FReply::Handled();
						})
						[
							SNew(STextBlock)
							.Font(EVUIStyle::Font(10))
							.Text_Lambda([WeakUI, ActionId, Slot]()
							{
								if (!WeakUI.IsValid()) return FText::GetEmpty();
								if (WeakUI->IsCapturing(ActionId, Slot)) return LOCTEXT("PressKey", "Press a key...");
								const FEVKeyChord C = WeakUI->GetBinding(ActionId, Slot);
								return C.IsValid() ? C.ToDisplayText() : FText::GetEmpty();
							})
							.ColorAndOpacity_Lambda([WeakUI, ActionId, Slot]()
							{
								return FSlateColor((WeakUI.IsValid() && WeakUI->IsCapturing(ActionId, Slot)) ? EVUIStyle::Accent : EVUIStyle::Text);
							})
						]
					]
				];
		};

		Box->AddSlot().AutoHeight().Padding(0.f, 2.f)
		[
			SNew(SHorizontalBox)
			+ SHorizontalBox::Slot().FillWidth(1.f).VAlign(VAlign_Center)
			[
				SNew(STextBlock).Text(B.Label).Font(EVUIStyle::Font(11)).ColorAndOpacity(EVUIStyle::Text)
			]
			+ SHorizontalBox::Slot().AutoWidth()[SlotButton(0)]
			+ SHorizontalBox::Slot().AutoWidth()[SlotButton(1)]
		];
	}

	return SNew(SScrollBox).ScrollBarStyle(&EVUIStyle::ScrollBar())
		+ SScrollBox::Slot().Padding(0.f, 0.f, 10.f, 0.f)[Box];
}

#undef LOCTEXT_NAMESPACE
