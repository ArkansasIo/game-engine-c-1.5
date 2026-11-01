#include "UI/EVUIStyle.h"
#include "Styling/CoreStyle.h"
#include "Brushes/SlateColorBrush.h"
#include "Brushes/SlateNoResource.h"
#include "Brushes/SlateRoundedBoxBrush.h"

namespace EVUIStyle
{
	const FLinearColor PanelFill(0.030f, 0.036f, 0.052f, 0.94f);
	const FLinearColor PanelFillDark(0.012f, 0.015f, 0.024f, 0.92f);
	const FLinearColor TitleFill(0.16f, 0.095f, 0.035f, 0.98f);
	const FLinearColor Trim(0.47f, 0.35f, 0.17f, 1.f);
	const FLinearColor TrimBright(0.92f, 0.72f, 0.36f, 1.f);
	const FLinearColor Accent(1.00f, 0.82f, 0.36f, 1.f);
	const FLinearColor Text(0.96f, 0.93f, 0.86f, 1.f);
	const FLinearColor TextDim(0.66f, 0.68f, 0.74f, 1.f);
	const FLinearColor TextDisabled(0.38f, 0.38f, 0.40f, 1.f);
	const FLinearColor RowHover(0.30f, 0.22f, 0.10f, 0.85f);
	const FLinearColor RowSelected(0.46f, 0.32f, 0.11f, 0.95f);
	const FLinearColor Good(0.45f, 0.95f, 0.50f, 1.f);
	const FLinearColor Bad(1.00f, 0.42f, 0.38f, 1.f);
	const FLinearColor Shadow(0.f, 0.f, 0.f, 0.55f);

	FSlateFontInfo Font(int32 Size, bool bBold)
	{
		return FCoreStyle::GetDefaultFontStyle(bBold ? TEXT("Bold") : TEXT("Regular"), Size);
	}

	const FSlateBrush* PanelBrush()
	{
		static const FSlateRoundedBoxBrush Brush(PanelFill, 6.f, Trim, 1.5f);
		return &Brush;
	}

	const FSlateBrush* WellBrush()
	{
		static const FSlateRoundedBoxBrush Brush(PanelFillDark, 4.f, FLinearColor(0.f, 0.f, 0.f, 0.8f), 1.f);
		return &Brush;
	}

	const FSlateBrush* TitleBrush()
	{
		static const FSlateRoundedBoxBrush Brush(TitleFill, FVector4(6.f, 6.f, 0.f, 0.f));
		return &Brush;
	}

	const FSlateBrush* MenuBrush()
	{
		static const FSlateRoundedBoxBrush Brush(FLinearColor(0.02f, 0.024f, 0.036f, 0.97f), 4.f, Trim, 1.f);
		return &Brush;
	}

	const FSlateBrush* WhiteBrush()
	{
		static const FSlateColorBrush Brush(FLinearColor::White);
		return &Brush;
	}

	const FSlateBrush* NoBrush()
	{
		static const FSlateNoResource Brush;
		return &Brush;
	}

	static FButtonStyle MakeButton(const FLinearColor& Normal, const FLinearColor& Hovered, const FLinearColor& Pressed, const FLinearColor& Outline, float Radius, const FMargin& Padding)
	{
		FButtonStyle Style;
		Style.SetNormal(FSlateRoundedBoxBrush(Normal, Radius, Outline, 1.f));
		Style.SetHovered(FSlateRoundedBoxBrush(Hovered, Radius, TrimBright, 1.f));
		Style.SetPressed(FSlateRoundedBoxBrush(Pressed, Radius, TrimBright, 1.f));
		Style.SetDisabled(FSlateRoundedBoxBrush(FLinearColor(0.06f, 0.06f, 0.07f, 0.9f), Radius, FLinearColor(0.2f, 0.2f, 0.2f, 1.f), 1.f));
		Style.SetNormalForeground(FSlateColor(Text));
		Style.SetHoveredForeground(FSlateColor(FLinearColor::White));
		Style.SetPressedForeground(FSlateColor(FLinearColor::White));
		Style.SetDisabledForeground(FSlateColor(TextDisabled));
		Style.SetNormalPadding(Padding);
		Style.SetPressedPadding(FMargin(Padding.Left, Padding.Top + 1.f, Padding.Right, Padding.Bottom - 1.f));
		return Style;
	}

	const FButtonStyle& Button()
	{
		static const FButtonStyle Style = MakeButton(
			FLinearColor(0.20f, 0.13f, 0.06f, 0.96f), FLinearColor(0.34f, 0.22f, 0.09f, 1.f), FLinearColor(0.48f, 0.31f, 0.10f, 1.f),
			Trim, 4.f, FMargin(14.f, 6.f));
		return Style;
	}

	const FButtonStyle& FlatButton()
	{
		static const FButtonStyle Style = []()
		{
			FButtonStyle S = MakeButton(FLinearColor::Transparent, RowHover, RowSelected, FLinearColor::Transparent, 3.f, FMargin(8.f, 3.f));
			S.SetHovered(FSlateRoundedBoxBrush(RowHover, 3.f));
			S.SetPressed(FSlateRoundedBoxBrush(RowSelected, 3.f));
			S.SetDisabled(FSlateNoResource());
			return S;
		}();
		return Style;
	}

	const FButtonStyle& FlatButtonSelected()
	{
		static const FButtonStyle Style = []()
		{
			FButtonStyle S = FlatButton();
			S.SetNormal(FSlateRoundedBoxBrush(RowSelected, 3.f, TrimBright, 1.f));
			S.SetHovered(FSlateRoundedBoxBrush(RowSelected, 3.f, TrimBright, 1.f));
			return S;
		}();
		return Style;
	}

	const FButtonStyle& SlotButton()
	{
		static const FButtonStyle Style = MakeButton(
			FLinearColor(0.05f, 0.05f, 0.07f, 0.95f), FLinearColor(0.20f, 0.15f, 0.08f, 1.f), FLinearColor(0.34f, 0.24f, 0.09f, 1.f),
			Trim, 4.f, FMargin(2.f));
		return Style;
	}

	const FButtonStyle& SlotButtonActive()
	{
		static const FButtonStyle Style = []()
		{
			FButtonStyle S = SlotButton();
			S.SetNormal(FSlateRoundedBoxBrush(FLinearColor(0.38f, 0.26f, 0.08f, 1.f), 4.f, TrimBright, 1.5f));
			S.SetHovered(FSlateRoundedBoxBrush(FLinearColor(0.46f, 0.32f, 0.10f, 1.f), 4.f, TrimBright, 1.5f));
			return S;
		}();
		return Style;
	}

	const FCheckBoxStyle& CheckBox()
	{
		static const FCheckBoxStyle Style = []()
		{
			FCheckBoxStyle S = FCoreStyle::Get().GetWidgetStyle<FCheckBoxStyle>("Checkbox");
			S.SetForegroundColor(FSlateColor(Accent));
			S.SetHoveredForegroundColor(FSlateColor(TrimBright));
			S.SetCheckedForegroundColor(FSlateColor(Accent));
			S.SetCheckedHoveredForegroundColor(FSlateColor(TrimBright));
			S.SetBorderBackgroundColor(FSlateColor(Trim));
			return S;
		}();
		return Style;
	}

	const FSliderStyle& Slider()
	{
		static const FSliderStyle Style = []()
		{
			FSliderStyle S = FCoreStyle::Get().GetWidgetStyle<FSliderStyle>("Slider");
			S.SetNormalBarImage(FSlateRoundedBoxBrush(FLinearColor(0.12f, 0.10f, 0.07f, 1.f), 2.f, Trim, 1.f));
			S.SetHoveredBarImage(FSlateRoundedBoxBrush(FLinearColor(0.18f, 0.14f, 0.08f, 1.f), 2.f, TrimBright, 1.f));
			S.SetNormalThumbImage(FSlateRoundedBoxBrush(Accent, 5.f, FVector2D(10.f, 16.f)));
			S.SetHoveredThumbImage(FSlateRoundedBoxBrush(FLinearColor::White, 5.f, FVector2D(10.f, 16.f)));
			S.SetBarThickness(4.f);
			return S;
		}();
		return Style;
	}

	const FEditableTextBoxStyle& TextBox()
	{
		static const FEditableTextBoxStyle Style = []()
		{
			FEditableTextBoxStyle S = FCoreStyle::Get().GetWidgetStyle<FEditableTextBoxStyle>("NormalEditableTextBox");
			S.SetBackgroundImageNormal(FSlateRoundedBoxBrush(PanelFillDark, 3.f, Trim, 1.f));
			S.SetBackgroundImageHovered(FSlateRoundedBoxBrush(PanelFillDark, 3.f, TrimBright, 1.f));
			S.SetBackgroundImageFocused(FSlateRoundedBoxBrush(PanelFillDark, 3.f, Accent, 1.f));
			S.SetForegroundColor(FSlateColor(Text));
			S.SetFocusedForegroundColor(FSlateColor(FLinearColor::White));
			S.SetFont(Font(11));
			return S;
		}();
		return Style;
	}

	const FScrollBarStyle& ScrollBar()
	{
		static const FScrollBarStyle Style = []()
		{
			FScrollBarStyle S = FCoreStyle::Get().GetWidgetStyle<FScrollBarStyle>("ScrollBar");
			S.SetNormalThumbImage(FSlateRoundedBoxBrush(Trim, 3.f));
			S.SetHoveredThumbImage(FSlateRoundedBoxBrush(TrimBright, 3.f));
			S.SetDraggedThumbImage(FSlateRoundedBoxBrush(Accent, 3.f));
			return S;
		}();
		return Style;
	}
}
