#pragma once

#include "CoreMinimal.h"
#include "Styling/SlateTypes.h"
#include "Styling/SlateBrush.h"
#include "Fonts/SlateFontInfo.h"

/**
 * Shared look for every Slate widget in the MMO interface: dark slate panels, bronze trim,
 * parchment-white text. Change the palette here and every menu, panel and popup follows.
 */
namespace EVUIStyle
{
	// Palette
	extern const FLinearColor PanelFill;
	extern const FLinearColor PanelFillDark;
	extern const FLinearColor TitleFill;
	extern const FLinearColor Trim;          // bronze frame
	extern const FLinearColor TrimBright;
	extern const FLinearColor Accent;        // headings / active tab text
	extern const FLinearColor Text;
	extern const FLinearColor TextDim;
	extern const FLinearColor TextDisabled;
	extern const FLinearColor RowHover;
	extern const FLinearColor RowSelected;
	extern const FLinearColor Good;
	extern const FLinearColor Bad;
	extern const FLinearColor Shadow;

	FSlateFontInfo Font(int32 Size, bool bBold = false);

	/** Rounded panel with a bronze outline. */
	const FSlateBrush* PanelBrush();
	/** Darker inset area (lists, content wells). */
	const FSlateBrush* WellBrush();
	/** Title bar strip. */
	const FSlateBrush* TitleBrush();
	/** Dropdown menu body. */
	const FSlateBrush* MenuBrush();
	/** Solid white; tint with ColorAndOpacity / BorderBackgroundColor. */
	const FSlateBrush* WhiteBrush();
	/** Fully transparent (click catchers, invisible borders). */
	const FSlateBrush* NoBrush();

	/** Big bronze button (game menu, dialogs). */
	const FButtonStyle& Button();
	/** Flat row button used inside menus, category trees and lists. */
	const FButtonStyle& FlatButton();
	/** Highlighted variant of FlatButton (selected category / active tab). */
	const FButtonStyle& FlatButtonSelected();
	/** Square slot used by the micro menu. */
	const FButtonStyle& SlotButton();
	const FButtonStyle& SlotButtonActive();

	const FCheckBoxStyle& CheckBox();
	const FSliderStyle& Slider();
	const FEditableTextBoxStyle& TextBox();
	const FScrollBarStyle& ScrollBar();
}
