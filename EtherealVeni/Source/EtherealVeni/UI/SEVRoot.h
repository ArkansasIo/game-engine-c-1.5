#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

class UEVUISubsystem;
class SConstraintCanvas;
class SBox;
class SVerticalBox;
class SEVMenuPanel;

/**
 * Viewport overlay that hosts the whole interface. Layers, bottom to top:
 *   panels (movable windows) -> micro menu -> message area -> popup dialog -> menu click-catcher -> dropdown menus
 * Everything inside is scaled by the "ui.scale" setting.
 */
class ETHEREALVENI_API SEVRoot : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(SEVRoot) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs, UEVUISubsystem* InUI);
	virtual void Tick(const FGeometry& AllottedGeometry, const double InCurrentTime, const float InDeltaTime) override;

	/** Re-syncs the panel layer with the subsystem's open panels and z-order. */
	void RebuildPanels();
	void RebuildMicroMenu();

	/**
	 * Opens MenuId as dropdown level Level (0 = root, closes everything at Level and deeper first).
	 * FlipX is the x to align the menu's right edge to when it doesn't fit on the right (sub-menus open to the left).
	 */
	void OpenMenu(FName MenuId, FVector2D Position, int32 Level, float FlipX = -1.f);
	void CloseMenus(int32 FromLevel = 0);
	bool IsMenuOpen() const { return MenuLevels.Num() > 0; }
	bool IsMenuLevelShowing(int32 Level, FName MenuId) const;
	void FocusMenuLevel(int32 Level);

	void ShowPopup(TSharedRef<SWidget> Popup);
	void ClosePopup();
	bool IsPopupOpen() const { return bPopupOpen; }

	void AddMessage(const FText& Text, const FLinearColor& Color);

	/** Mouse cursor in the scaled layer's coordinates. */
	FVector2D GetCursorPosition() const;
	/** Converts an absolute (desktop) position to the scaled layer's coordinates. */
	FVector2D AbsoluteToLayer(const FVector2D& Absolute) const;
	FVector2D GetLayerSize() const;

private:
	struct FMenuLevel
	{
		FName MenuId;
		TSharedPtr<SEVMenuPanel> Widget;
	};
	struct FMessage
	{
		FText Text;
		FLinearColor Color;
		double Time = 0.0;
	};

	float GetScale() const;
	void RebuildMessages();

	TWeakObjectPtr<UEVUISubsystem> UI;
	TSharedPtr<SConstraintCanvas> PanelCanvas;
	TSharedPtr<SConstraintCanvas> MenuCanvas;
	TSharedPtr<SBox> MicroMenuHost;
	TSharedPtr<SBox> PopupHost;
	TSharedPtr<SVerticalBox> MessageList;
	TMap<FName, TSharedPtr<SWidget>> PanelWidgets;
	TArray<FName> BuiltPanelOrder;
	TArray<FMenuLevel> MenuLevels;
	TArray<FMessage> Messages;
	bool bPopupOpen = false;
	FGeometry CachedGeometry;
};
