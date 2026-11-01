#include "UI/EVUISubsystem.h"
#include "UI/SEVRoot.h"
#include "UI/SEVPanels.h"
#include "UI/EVUIStyle.h"
#include "UI/EVUIDefaultContent.h"
#include "EthVeni/EthVeni.h"

#include "Engine/Engine.h"
#include "Engine/GameViewportClient.h"
#include "Engine/LocalPlayer.h"
#include "Engine/Texture2D.h"
#include "Engine/World.h"
#include "GameFramework/PlayerController.h"
#include "Blueprint/UserWidget.h"
#include "Framework/Application/IInputProcessor.h"
#include "Framework/Application/SlateApplication.h"
#include "Misc/ConfigCacheIni.h"
#include "Widgets/SViewport.h"
#include "Widgets/Text/STextBlock.h"

#define LOCTEXT_NAMESPACE "EVUI"

namespace
{
	const TCHAR* ConfigSection = TEXT("/Script/EtherealVeni.EVUI");
	const FName ChoiceMenuId(TEXT("__choice"));
	const FName GameMenuPanel(TEXT("game_menu"));
	const FName OptionsPanel(TEXT("options"));

	bool IsModifierKey(const FKey& Key)
	{
		return Key == EKeys::LeftShift || Key == EKeys::RightShift || Key == EKeys::LeftControl || Key == EKeys::RightControl
			|| Key == EKeys::LeftAlt || Key == EKeys::RightAlt || Key == EKeys::LeftCommand || Key == EKeys::RightCommand;
	}
}

// ====================================================================== data helpers

FString FEVKeyChord::ToConfigString() const
{
	if (!IsValid()) return TEXT("None");
	FString S;
	if (bCtrl) S += TEXT("Ctrl+");
	if (bAlt) S += TEXT("Alt+");
	if (bShift) S += TEXT("Shift+");
	return S + Key.GetFName().ToString();
}

FEVKeyChord FEVKeyChord::FromConfigString(const FString& In)
{
	FEVKeyChord C;
	TArray<FString> Parts;
	In.TrimStartAndEnd().ParseIntoArray(Parts, TEXT("+"));
	for (int32 i = 0; i < Parts.Num(); ++i)
	{
		const FString& P = Parts[i];
		if (i < Parts.Num() - 1)
		{
			if (P == TEXT("Ctrl")) C.bCtrl = true;
			else if (P == TEXT("Alt")) C.bAlt = true;
			else if (P == TEXT("Shift")) C.bShift = true;
		}
		else if (P != TEXT("None"))
		{
			C.Key = FKey(FName(*P));
		}
	}
	return C;
}

FText FEVKeyChord::ToDisplayText() const
{
	if (!IsValid()) return LOCTEXT("NotBound", "Not Bound");
	FString S;
	if (bCtrl) S += TEXT("Ctrl+");
	if (bAlt) S += TEXT("Alt+");
	if (bShift) S += TEXT("Shift+");
	return FText::FromString(S + Key.GetDisplayName().ToString());
}

FEVMenuItem FEVMenuItem::MakeAction(FName Id, const FText& Label, FName ActionId, const FText& Tooltip)
{
	FEVMenuItem I;
	I.Id = Id; I.Label = Label; I.ActionId = ActionId; I.Tooltip = Tooltip; I.Kind = EEVMenuItemKind::Action;
	return I;
}

FEVMenuItem FEVMenuItem::MakeSubmenu(FName Id, const FText& Label, FName SubmenuId, const FText& Tooltip)
{
	FEVMenuItem I;
	I.Id = Id; I.Label = Label; I.SubmenuId = SubmenuId; I.Tooltip = Tooltip; I.Kind = EEVMenuItemKind::Submenu;
	return I;
}

FEVMenuItem FEVMenuItem::MakeToggle(FName Id, const FText& Label, FName SettingKey, const FText& Tooltip)
{
	FEVMenuItem I;
	I.Id = Id; I.Label = Label; I.SettingKey = SettingKey; I.Tooltip = Tooltip; I.Kind = EEVMenuItemKind::Toggle;
	return I;
}

FEVMenuItem FEVMenuItem::MakeRadio(FName Id, const FText& Label, FName SettingKey, const FString& Value)
{
	FEVMenuItem I;
	I.Id = Id; I.Label = Label; I.SettingKey = SettingKey; I.RadioValue = Value; I.Kind = EEVMenuItemKind::Radio;
	return I;
}

FEVMenuItem FEVMenuItem::MakeSeparator()
{
	static int32 Counter = 0;
	FEVMenuItem I;
	I.Id = FName(*FString::Printf(TEXT("__sep%d"), ++Counter));
	I.Kind = EEVMenuItemKind::Separator;
	return I;
}

FEVMenuItem FEVMenuItem::MakeHeader(const FText& Label)
{
	static int32 Counter = 0;
	FEVMenuItem I;
	I.Id = FName(*FString::Printf(TEXT("__hdr%d"), ++Counter));
	I.Label = Label;
	I.Kind = EEVMenuItemKind::Header;
	return I;
}

// ====================================================================== input pre-processor

class FEVUIInputProcessor : public IInputProcessor
{
public:
	explicit FEVUIInputProcessor(UEVUISubsystem* InUI) : UI(InUI) {}

	virtual void Tick(const float DeltaTime, FSlateApplication& SlateApp, TSharedRef<ICursor> Cursor) override {}

	virtual bool HandleKeyDownEvent(FSlateApplication& SlateApp, const FKeyEvent& InKeyEvent) override
	{
		UEVUISubsystem* S = UI.Get();
		return S && S->HandleKeyDown(InKeyEvent);
	}

private:
	TWeakObjectPtr<UEVUISubsystem> UI;
};

// ====================================================================== lifetime

UEVUISubsystem* UEVUISubsystem::Get(const APlayerController* PC)
{
	const ULocalPlayer* LP = PC ? PC->GetLocalPlayer() : nullptr;
	return LP ? LP->GetSubsystem<UEVUISubsystem>() : nullptr;
}

UEVUISubsystem* UEVUISubsystem::GetInterface(const UObject* WorldContextObject)
{
	UWorld* World = GEngine ? GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull) : nullptr;
	return World ? Get(World->GetFirstPlayerController()) : nullptr;
}

bool UEVUISubsystem::GetBoolSettingFor(const APlayerController* PC, FName Key, bool bDefault)
{
	const UEVUISubsystem* UI = Get(PC);
	return (UI && UI->FindSetting(Key)) ? UI->GetSettingBool(Key) : bDefault;
}

void UEVUISubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);

	bLoading = true;
	EVUIDefaultContent::Register(*this);
	LoadSettings();
	LoadBindings();
	bLoading = false;

	if (FSlateApplication::IsInitialized() && !IsRunningDedicatedServer())
	{
		InputProcessor = MakeShared<FEVUIInputProcessor>(this);
		FSlateApplication::Get().RegisterInputPreProcessor(InputProcessor);
	}
	UE_LOG(LogEthVeni, Log, TEXT("EVUI: %d menus, %d panels, %d settings, %d key bindings registered"),
		Menus.Num(), Panels.Num(), Settings.Num(), BindingDefs.Num());
}

void UEVUISubsystem::Deinitialize()
{
	DetachFromViewport();
	if (InputProcessor.IsValid() && FSlateApplication::IsInitialized())
	{
		FSlateApplication::Get().UnregisterInputPreProcessor(InputProcessor);
	}
	InputProcessor.Reset();
	Actions.Reset();
	EscapeHandlers.Reset();
	Super::Deinitialize();
}

APlayerController* UEVUISubsystem::GetPlayerController() const
{
	const ULocalPlayer* LP = GetLocalPlayer();
	return LP ? LP->PlayerController.Get() : nullptr;
}

// ====================================================================== viewport

void UEVUISubsystem::AttachToViewport()
{
	ULocalPlayer* LP = GetLocalPlayer();
	UGameViewportClient* VC = LP ? LP->ViewportClient.Get() : nullptr;
	if (!VC || !FSlateApplication::IsInitialized()) return;

	DetachFromViewport();
	Root = SNew(SEVRoot, this);
	VC->AddViewportWidgetForPlayer(LP, Root.ToSharedRef(), 50);
	AttachedViewport = VC;
	ApplyAllSettings();
	Root->RebuildPanels();
}

void UEVUISubsystem::DetachFromViewport()
{
	CancelKeyCapture();
	if (Root.IsValid())
	{
		UGameViewportClient* VC = AttachedViewport.Get();
		ULocalPlayer* LP = GetLocalPlayer();
		if (VC && LP)
		{
			VC->RemoveViewportWidgetForPlayer(LP, Root.ToSharedRef());
		}
	}
	Root.Reset();
	AttachedViewport.Reset();
	PanelUserWidgets.Reset();
}

bool UEVUISubsystem::IsAttached() const
{
	return Root.IsValid() && AttachedViewport.IsValid();
}

void UEVUISubsystem::SetInterfaceVisible(bool bVisible)
{
	if (bInterfaceVisible == bVisible) return;
	bInterfaceVisible = bVisible;
	if (!bVisible) CloseMenus();
	ShowMessage(bVisible ? LOCTEXT("UIShown", "Interface shown") : FText::Format(LOCTEXT("UIHidden", "Interface hidden - press {0} or Esc to show it"), GetShortcutText(TEXT("ui.toggle_interface"))),
		FLinearColor(1.f, 0.9f, 0.6f));
}

float UEVUISubsystem::GetUIScale() const
{
	return FindSetting(TEXT("ui.scale")) ? FMath::Clamp(GetSettingFloat(TEXT("ui.scale")), 0.5f, 2.f) : 1.f;
}

// ====================================================================== actions

void UEVUISubsystem::RegisterAction(FName ActionId, TFunction<void()> Execute, TFunction<bool()> IsChecked, TFunction<bool()> IsEnabled)
{
	FActionHandler& H = Actions.FindOrAdd(ActionId);
	H.Execute = MoveTemp(Execute);
	H.IsChecked = MoveTemp(IsChecked);
	H.IsEnabled = MoveTemp(IsEnabled);
}

void UEVUISubsystem::UnregisterAction(FName ActionId)
{
	Actions.Remove(ActionId);
}

bool UEVUISubsystem::ExecuteAction(FName ActionId)
{
	if (ActionId.IsNone() || !IsActionEnabled(ActionId)) return false;
	if (const FActionHandler* H = Actions.Find(ActionId))
	{
		if (H->Execute)
		{
			// Copy: the handler may re-register actions (choice menus do).
			const TFunction<void()> Run = H->Execute;
			Run();
		}
	}
	OnAction.Broadcast(ActionId);
	return true;
}

bool UEVUISubsystem::IsActionChecked(FName ActionId) const
{
	const FActionHandler* H = Actions.Find(ActionId);
	return H && H->IsChecked && H->IsChecked();
}

bool UEVUISubsystem::IsActionEnabled(FName ActionId) const
{
	const FActionHandler* H = Actions.Find(ActionId);
	return !H || !H->IsEnabled || H->IsEnabled();
}

// ====================================================================== menus

void UEVUISubsystem::RegisterMenu(const FEVMenuDefinition& Menu)
{
	if (Menu.MenuId.IsNone()) return;
	Menus.Add(Menu.MenuId, Menu);
}

bool UEVUISubsystem::AddMenuItem(FName MenuId, const FEVMenuItem& Item, FName InsertAfter)
{
	if (MenuId.IsNone()) return false;
	FEVMenuDefinition& Menu = Menus.FindOrAdd(MenuId);
	Menu.MenuId = MenuId;
	Menu.Items.RemoveAll([&Item](const FEVMenuItem& E) { return E.Id == Item.Id; });
	const int32 After = InsertAfter.IsNone() ? INDEX_NONE : Menu.Items.IndexOfByPredicate([InsertAfter](const FEVMenuItem& E) { return E.Id == InsertAfter; });
	if (After == INDEX_NONE) Menu.Items.Add(Item);
	else Menu.Items.Insert(Item, After + 1);
	return true;
}

bool UEVUISubsystem::RemoveMenuItem(FName MenuId, FName ItemId)
{
	FEVMenuDefinition* Menu = Menus.Find(MenuId);
	return Menu && Menu->Items.RemoveAll([ItemId](const FEVMenuItem& E) { return E.Id == ItemId; }) > 0;
}

bool UEVUISubsystem::SetMenuItemEnabled(FName MenuId, FName ItemId, bool bEnabled)
{
	FEVMenuDefinition* Menu = Menus.Find(MenuId);
	FEVMenuItem* Item = Menu ? Menu->Items.FindByPredicate([ItemId](const FEVMenuItem& E) { return E.Id == ItemId; }) : nullptr;
	if (!Item) return false;
	Item->bEnabled = bEnabled;
	return true;
}

bool UEVUISubsystem::GetMenu(FName MenuId, FEVMenuDefinition& OutMenu) const
{
	if (const FEVMenuDefinition* M = Menus.Find(MenuId))
	{
		OutMenu = *M;
		return true;
	}
	return false;
}

TArray<FName> UEVUISubsystem::GetMenuIds() const
{
	TArray<FName> Ids;
	Menus.GetKeys(Ids);
	return Ids;
}

bool UEVUISubsystem::OpenContextMenu(FName MenuId, const FString& Context)
{
	return Root.IsValid() && OpenMenuAt(MenuId, Root->GetCursorPosition() + FVector2D(4.f, 4.f), Context);
}

bool UEVUISubsystem::OpenMenuAt(FName MenuId, FVector2D Position, const FString& Context)
{
	if (!Root.IsValid() || !Menus.Contains(MenuId))
	{
		UE_LOG(LogEthVeni, Warning, TEXT("EVUI: cannot open menu '%s'"), *MenuId.ToString());
		return false;
	}
	if (!bInterfaceVisible) SetInterfaceVisible(true);
	MenuContext = Context;
	Root->OpenMenu(MenuId, Position, 0);
	return true;
}

void UEVUISubsystem::CloseMenus()
{
	if (Root.IsValid()) Root->CloseMenus(0);
}

bool UEVUISubsystem::IsAnyMenuOpen() const
{
	return Root.IsValid() && Root->IsMenuOpen();
}

bool UEVUISubsystem::IsMenuItemChecked(const FEVMenuItem& Item) const
{
	if (Item.Kind == EEVMenuItemKind::Toggle)
	{
		return Item.SettingKey.IsNone() ? IsActionChecked(Item.ActionId) : GetSettingBool(Item.SettingKey);
	}
	if (Item.Kind == EEVMenuItemKind::Radio)
	{
		if (Item.SettingKey.IsNone()) return IsActionChecked(Item.ActionId);
		const FString Value = GetSettingString(Item.SettingKey);
		if (Value.IsNumeric() && Item.RadioValue.IsNumeric())
		{
			return FMath::IsNearlyEqual(FCString::Atof(*Value), FCString::Atof(*Item.RadioValue), 0.001f);
		}
		return Value == Item.RadioValue;
	}
	return false;
}

bool UEVUISubsystem::IsMenuItemEnabled(const FEVMenuItem& Item) const
{
	if (!Item.bEnabled) return false;
	if (Item.Kind == EEVMenuItemKind::Separator || Item.Kind == EEVMenuItemKind::Header) return false;
	if (Item.Kind == EEVMenuItemKind::Submenu) return Menus.Contains(Item.SubmenuId);
	return Item.ActionId.IsNone() || IsActionEnabled(Item.ActionId);
}

FText UEVUISubsystem::GetShortcutText(FName ActionId) const
{
	const FEVKeyChord Chord = GetBinding(ActionId, 0);
	return Chord.IsValid() ? Chord.ToDisplayText() : FText::GetEmpty();
}

FText UEVUISubsystem::ResolveMenuTitle(const FEVMenuDefinition& Menu) const
{
	if (Menu.Title.IsEmpty()) return FText::GetEmpty();
	FString T = Menu.Title.ToString();
	T.ReplaceInline(TEXT("{Context}"), *MenuContext);
	return FText::FromString(T);
}

void UEVUISubsystem::ActivateMenuItem(FName MenuId, FName ItemId)
{
	const FEVMenuDefinition* Menu = Menus.Find(MenuId);
	const FEVMenuItem* Found = Menu ? Menu->Items.FindByPredicate([ItemId](const FEVMenuItem& E) { return E.Id == ItemId; }) : nullptr;
	if (!Found || !IsMenuItemEnabled(*Found)) return;

	const FEVMenuItem Item = *Found;   // menus may be re-registered by the action
	OnMenuItemActivated.Broadcast(MenuId, ItemId);

	switch (Item.Kind)
	{
	case EEVMenuItemKind::Toggle:
		if (!Item.SettingKey.IsNone()) SetSetting(Item.SettingKey, GetSettingBool(Item.SettingKey) ? TEXT("0") : TEXT("1"));
		ExecuteAction(Item.ActionId);
		break;   // checkbox menus stay open
	case EEVMenuItemKind::Radio:
		CloseMenus();
		if (!Item.SettingKey.IsNone()) SetSetting(Item.SettingKey, Item.RadioValue);
		ExecuteAction(Item.ActionId);
		break;
	case EEVMenuItemKind::Action:
		CloseMenus();
		ExecuteAction(Item.ActionId);
		break;
	default:
		break;
	}
}

void UEVUISubsystem::OpenChoiceMenu(const TArray<FText>& Choices, int32 Selected, TFunction<void(int32)> OnPicked, FVector2D Position, float MinWidth)
{
	FEVMenuDefinition Menu;
	Menu.MenuId = ChoiceMenuId;
	Menu.MinWidth = MinWidth;
	for (int32 i = 0; i < Choices.Num(); ++i)
	{
		const FName Act(*FString::Printf(TEXT("__choice.%d"), i));
		RegisterAction(Act, [OnPicked, i]() { if (OnPicked) OnPicked(i); }, [i, Selected]() { return i == Selected; });
		FEVMenuItem Item;
		Item.Id = FName(*FString::Printf(TEXT("c%d"), i));
		Item.Kind = EEVMenuItemKind::Radio;
		Item.Label = Choices[i];
		Item.ActionId = Act;
		Menu.Items.Add(Item);
	}
	RegisterMenu(Menu);
	if (Root.IsValid()) Root->OpenMenu(ChoiceMenuId, Position, 0);
}

// ====================================================================== panels

void UEVUISubsystem::RegisterPanel(const FEVPanelDefinition& Panel)
{
	if (Panel.PanelId.IsNone()) return;
	Panels.Add(Panel.PanelId, Panel);
	const FName PanelId = Panel.PanelId;
	RegisterAction(FName(*(TEXT("panel.") + PanelId.ToString())),
		[this, PanelId]() { TogglePanel(PanelId); },
		[this, PanelId]() { return IsPanelOpen(PanelId); });
}

void UEVUISubsystem::RegisterPanelContent(FName PanelId, FName TabId, TFunction<TSharedRef<SWidget>()> Factory)
{
	PanelContent.FindOrAdd(PanelId).Add(TabId, MoveTemp(Factory));
}

FVector2D UEVUISubsystem::DefaultPanelOffset(const FEVPanelDefinition& Panel) const
{
	switch (Panel.Area)
	{
	case EEVPanelArea::Left: return FVector2D(24.f, 150.f);
	case EEVPanelArea::Right: return FVector2D(-24.f, 150.f);
	default: return FVector2D::ZeroVector;
	}
}

void UEVUISubsystem::OpenPanel(FName PanelId, FName TabId)
{
	const FEVPanelDefinition* Def = Panels.Find(PanelId);
	if (!Def)
	{
		UE_LOG(LogEthVeni, Warning, TEXT("EVUI: unknown panel '%s'"), *PanelId.ToString());
		return;
	}
	if (!TabId.IsNone()) PanelTabs.Add(PanelId, TabId);
	else if (!PanelTabs.Contains(PanelId) && Def->Tabs.Num() > 0) PanelTabs.Add(PanelId, Def->Tabs[0].TabId);

	if (!bInterfaceVisible) bInterfaceVisible = true;
	CloseMenus();

	if (OpenPanels.Contains(PanelId))
	{
		BringPanelToFront(PanelId);
		return;
	}

	// Docked panels replace whatever occupies the same area.
	if (Def->Area != EEVPanelArea::Free)
	{
		const TArray<FName> Snapshot = OpenPanels;
		for (FName Other : Snapshot)
		{
			const FEVPanelDefinition* OtherDef = Panels.Find(Other);
			if (OtherDef && OtherDef->Area == Def->Area)
			{
				if (Def->bReturnToPrevious) ReturnToPanel.Add(PanelId, Other);
				ClosePanelInternal(Other, false);
			}
		}
	}

	if (!PanelOffsets.Contains(PanelId)) PanelOffsets.Add(PanelId, DefaultPanelOffset(*Def));
	OpenPanels.Add(PanelId);
	NotifyPanelsChanged();
	OnPanelChanged.Broadcast(PanelId, true);
}

void UEVUISubsystem::ClosePanel(FName PanelId)
{
	ClosePanelInternal(PanelId, true);
}

void UEVUISubsystem::ClosePanelInternal(FName PanelId, bool bAllowReturn)
{
	if (OpenPanels.Remove(PanelId) == 0) return;
	if (PanelId == OptionsPanel) CancelKeyCapture();
	NotifyPanelsChanged();
	OnPanelChanged.Broadcast(PanelId, false);

	FName ReturnTo;
	if (ReturnToPanel.RemoveAndCopyValue(PanelId, ReturnTo) && bAllowReturn && Panels.Contains(ReturnTo))
	{
		OpenPanel(ReturnTo);
	}
}

void UEVUISubsystem::TogglePanel(FName PanelId)
{
	if (IsPanelOpen(PanelId)) ClosePanel(PanelId);
	else OpenPanel(PanelId);
}

void UEVUISubsystem::CloseAllPanels()
{
	ReturnToPanel.Reset();
	const TArray<FName> Snapshot = OpenPanels;
	for (int32 i = Snapshot.Num() - 1; i >= 0; --i)
	{
		ClosePanelInternal(Snapshot[i], false);
	}
}

void UEVUISubsystem::SetPanelTab(FName PanelId, FName TabId)
{
	PanelTabs.Add(PanelId, TabId);
}

FName UEVUISubsystem::GetPanelTab(FName PanelId) const
{
	return PanelTabs.FindRef(PanelId);
}

void UEVUISubsystem::ResetPanelPositions()
{
	PanelOffsets.Reset();
	for (FName Id : OpenPanels)
	{
		if (const FEVPanelDefinition* Def = Panels.Find(Id)) PanelOffsets.Add(Id, DefaultPanelOffset(*Def));
	}
}

void UEVUISubsystem::MovePanel(FName PanelId, FVector2D Delta)
{
	FVector2D& Offset = PanelOffsets.FindOrAdd(PanelId);
	Offset += Delta;
	// Keep the title bar reachable.
	if (Root.IsValid())
	{
		const FVector2D View = Root->GetLayerSize();
		Offset.X = FMath::Clamp(Offset.X, -View.X, View.X);
		Offset.Y = FMath::Clamp(Offset.Y, -View.Y, View.Y);
	}
}

void UEVUISubsystem::BringPanelToFront(FName PanelId)
{
	if (OpenPanels.Num() == 0 || OpenPanels.Last() == PanelId || !OpenPanels.Contains(PanelId)) return;
	OpenPanels.Remove(PanelId);
	OpenPanels.Add(PanelId);
	NotifyPanelsChanged();
}

void UEVUISubsystem::NotifyPanelsChanged()
{
	if (Root.IsValid()) Root->RebuildPanels();
}

TSharedRef<SWidget> UEVUISubsystem::BuildPanelContent(FName PanelId, FName TabId)
{
	if (const TMap<FName, TFunction<TSharedRef<SWidget>()>>* Tabs = PanelContent.Find(PanelId))
	{
		if (const TFunction<TSharedRef<SWidget>()>* Factory = Tabs->Find(TabId))
		{
			if (*Factory) return (*Factory)();
		}
	}
	if (const FEVPanelDefinition* Def = Panels.Find(PanelId))
	{
		const FEVPanelTab* Tab = Def->Tabs.FindByPredicate([TabId](const FEVPanelTab& T) { return T.TabId == TabId; });
		APlayerController* PC = GetPlayerController();
		if (Tab && Tab->WidgetClass && PC)
		{
			if (UUserWidget* W = CreateWidget<UUserWidget>(PC, Tab->WidgetClass))
			{
				PanelUserWidgets.Add(W);
				return W->TakeWidget();
			}
		}
	}
	return SNew(STextBlock)
		.Text(FText::Format(LOCTEXT("NoContent", "No content registered for panel '{0}' tab '{1}'.\nUse RegisterPanelContent (C++) or set the tab's WidgetClass (UMG)."),
			FText::FromName(PanelId), FText::FromName(TabId)))
		.ColorAndOpacity(FLinearColor(0.7f, 0.7f, 0.75f))
		.AutoWrapText(true);
}

// ====================================================================== micro menu, popups, messages

void UEVUISubsystem::RegisterMicroButton(const FEVMicroButton& Button)
{
	MicroButtons.RemoveAll([&Button](const FEVMicroButton& B) { return B.Id == Button.Id; });
	MicroButtons.Add(Button);
	if (Root.IsValid()) Root->RebuildMicroMenu();
}

void UEVUISubsystem::ShowConfirmation(FText Title, FText Message, FName ConfirmActionId, FText ConfirmLabel)
{
	TWeakObjectPtr<UEVUISubsystem> WeakThis(this);
	ShowConfirmationNative(Title, Message, [WeakThis, ConfirmActionId]()
	{
		if (UEVUISubsystem* S = WeakThis.Get()) S->ExecuteAction(ConfirmActionId);
	}, ConfirmLabel);
}

void UEVUISubsystem::ShowConfirmationNative(const FText& Title, const FText& Message, TFunction<void()> OnConfirm, const FText& ConfirmLabel)
{
	if (!Root.IsValid()) return;
	CloseMenus();
	Root->ShowPopup(EVUIWidgets::MakeConfirmPopup(this, Title, Message,
		ConfirmLabel.IsEmpty() ? LOCTEXT("Okay", "Okay") : ConfirmLabel, MoveTemp(OnConfirm)));
}

void UEVUISubsystem::ClosePopup()
{
	if (Root.IsValid()) Root->ClosePopup();
}

bool UEVUISubsystem::IsPopupOpen() const
{
	return Root.IsValid() && Root->IsPopupOpen();
}

void UEVUISubsystem::ShowMessage(FText Message, FLinearColor Color)
{
	if (Root.IsValid()) Root->AddMessage(Message, Color);
}

// ====================================================================== settings

void UEVUISubsystem::RegisterOptionsCategory(const FEVOptionsCategory& Category)
{
	Categories.RemoveAll([&Category](const FEVOptionsCategory& C) { return C.Id == Category.Id; });
	Categories.Add(Category);
}

void UEVUISubsystem::RegisterSetting(const FEVSettingDefinition& Setting)
{
	Settings.RemoveAll([&Setting](const FEVSettingDefinition& S) { return S.Key == Setting.Key; });
	Settings.Add(Setting);
	if (!bLoading)
	{
		FString Stored;
		SettingValues.Add(Setting.Key, GConfig && GConfig->GetString(ConfigSection, *Setting.Key.ToString(), Stored, GGameUserSettingsIni) ? Stored : Setting.DefaultValue);
	}
}

void UEVUISubsystem::RegisterSettingApplier(FName Key, TFunction<void(const FString&)> Apply)
{
	SettingAppliers.Add(Key, MoveTemp(Apply));
}

void UEVUISubsystem::RegisterSettingReader(FName Key, TFunction<FString()> Read)
{
	SettingReaders.Add(Key, MoveTemp(Read));
}

const FEVSettingDefinition* UEVUISubsystem::FindSetting(FName Key) const
{
	return Settings.FindByPredicate([Key](const FEVSettingDefinition& S) { return S.Key == Key; });
}

TArray<const FEVSettingDefinition*> UEVUISubsystem::GetSettingsInCategory(FName CategoryId) const
{
	TArray<const FEVSettingDefinition*> Out;
	for (const FEVSettingDefinition& S : Settings)
	{
		if (S.CategoryId == CategoryId) Out.Add(&S);
	}
	return Out;
}

FString UEVUISubsystem::GetSettingString(FName Key) const
{
	if (const TFunction<FString()>* Reader = SettingReaders.Find(Key))
	{
		if (*Reader) return (*Reader)();
	}
	if (const FString* V = SettingValues.Find(Key)) return *V;
	const FEVSettingDefinition* Def = FindSetting(Key);
	return Def ? Def->DefaultValue : FString();
}

float UEVUISubsystem::GetSettingFloat(FName Key) const
{
	return FCString::Atof(*GetSettingString(Key));
}

bool UEVUISubsystem::GetSettingBool(FName Key) const
{
	const FString V = GetSettingString(Key);
	return V == TEXT("1") || V.Equals(TEXT("true"), ESearchCase::IgnoreCase);
}

int32 UEVUISubsystem::GetSettingInt(FName Key) const
{
	return FCString::Atoi(*GetSettingString(Key));
}

void UEVUISubsystem::SetSetting(FName Key, const FString& Value)
{
	if (!FindSetting(Key))
	{
		UE_LOG(LogEthVeni, Warning, TEXT("EVUI: unknown setting '%s'"), *Key.ToString());
		return;
	}
	if (GetSettingString(Key) == Value) return;

	if (!SettingReaders.Contains(Key))
	{
		SettingValues.Add(Key, Value);
		if (GConfig) GConfig->SetString(ConfigSection, *Key.ToString(), *Value, GGameUserSettingsIni);
		SaveSettings();
	}
	ApplySetting(Key, Value);
	OnSettingChanged.Broadcast(Key, Value);
}

void UEVUISubsystem::ResetSetting(FName Key)
{
	if (const FEVSettingDefinition* Def = FindSetting(Key)) SetSetting(Key, Def->DefaultValue);
}

void UEVUISubsystem::ApplySetting(FName Key, const FString& Value)
{
	if (const TFunction<void(const FString&)>* Apply = SettingAppliers.Find(Key))
	{
		if (*Apply) (*Apply)(Value);
	}
}

void UEVUISubsystem::ApplyAllSettings()
{
	// Settings with a reader already live in their own system (GameUserSettings); only push ours.
	for (const FEVSettingDefinition& S : Settings)
	{
		if (!SettingReaders.Contains(S.Key)) ApplySetting(S.Key, GetSettingString(S.Key));
	}
}

void UEVUISubsystem::LoadSettings()
{
	for (const FEVSettingDefinition& S : Settings)
	{
		FString Stored;
		if (GConfig && GConfig->GetString(ConfigSection, *S.Key.ToString(), Stored, GGameUserSettingsIni))
		{
			SettingValues.Add(S.Key, Stored);
		}
		else
		{
			SettingValues.Add(S.Key, S.DefaultValue);
		}
	}
}

void UEVUISubsystem::SaveSettings()
{
	if (GConfig) GConfig->Flush(false, GGameUserSettingsIni);
}

void UEVUISubsystem::OpenOptions(FName CategoryId)
{
	if (!CategoryId.IsNone()) RequestedOptionsCategory = CategoryId;
	if (IsPanelOpen(OptionsPanel))
	{
		// Rebuild on the requested page.
		ClosePanelInternal(OptionsPanel, false);
	}
	OpenPanel(OptionsPanel);
}

// ====================================================================== key bindings

void UEVUISubsystem::RegisterKeyBinding(const FEVKeyBindingDefinition& Binding)
{
	BindingDefs.RemoveAll([&Binding](const FEVKeyBindingDefinition& B) { return B.ActionId == Binding.ActionId; });
	BindingDefs.Add(Binding);
	TArray<FEVKeyChord>& Slots = Bindings.FindOrAdd(Binding.ActionId);
	Slots.SetNum(2);
	Slots[0] = Binding.DefaultPrimary;
	Slots[1] = Binding.DefaultSecondary;
}

FEVKeyChord UEVUISubsystem::GetBinding(FName ActionId, int32 Slot) const
{
	const TArray<FEVKeyChord>* Slots = Bindings.Find(ActionId);
	return (Slots && Slots->IsValidIndex(Slot)) ? (*Slots)[Slot] : FEVKeyChord();
}

FName UEVUISubsystem::SetBinding(FName ActionId, int32 Slot, FEVKeyChord Chord)
{
	if (Slot < 0 || Slot > 1) return NAME_None;
	FName Displaced = NAME_None;
	if (Chord.IsValid())
	{
		for (TPair<FName, TArray<FEVKeyChord>>& Pair : Bindings)
		{
			for (int32 i = 0; i < Pair.Value.Num(); ++i)
			{
				if (Pair.Value[i] == Chord && !(Pair.Key == ActionId && i == Slot))
				{
					Pair.Value[i] = FEVKeyChord();
					Displaced = Pair.Key;
				}
			}
		}
	}
	TArray<FEVKeyChord>& Slots = Bindings.FindOrAdd(ActionId);
	Slots.SetNum(2);
	Slots[Slot] = Chord;
	SaveBindings();
	return Displaced;
}

void UEVUISubsystem::ClearBinding(FName ActionId, int32 Slot)
{
	SetBinding(ActionId, Slot, FEVKeyChord());
}

void UEVUISubsystem::ResetAllBindings()
{
	for (const FEVKeyBindingDefinition& B : BindingDefs)
	{
		TArray<FEVKeyChord>& Slots = Bindings.FindOrAdd(B.ActionId);
		Slots.SetNum(2);
		Slots[0] = B.DefaultPrimary;
		Slots[1] = B.DefaultSecondary;
	}
	SaveBindings();
}

FName UEVUISubsystem::FindActionForChord(const FEVKeyChord& Chord) const
{
	if (!Chord.IsValid()) return NAME_None;
	for (const TPair<FName, TArray<FEVKeyChord>>& Pair : Bindings)
	{
		if (Pair.Value.Contains(Chord)) return Pair.Key;
	}
	return NAME_None;
}

void UEVUISubsystem::BeginKeyCapture(FName ActionId, int32 Slot)
{
	CaptureAction = ActionId;
	CaptureSlot = FMath::Clamp(Slot, 0, 1);
}

void UEVUISubsystem::CancelKeyCapture()
{
	CaptureAction = NAME_None;
	CaptureSlot = -1;
}

void UEVUISubsystem::RegisterReservedKey(const FKey& Key, const FText& Reason)
{
	ReservedKeys.Add(Key, Reason);
}

void UEVUISubsystem::LoadBindings()
{
	if (!GConfig) return;
	for (const FEVKeyBindingDefinition& B : BindingDefs)
	{
		FString Stored;
		if (GConfig->GetString(ConfigSection, *(TEXT("Bind.") + B.ActionId.ToString()), Stored, GGameUserSettingsIni))
		{
			FString A, Bs;
			if (!Stored.Split(TEXT(";"), &A, &Bs)) A = Stored;
			TArray<FEVKeyChord>& Slots = Bindings.FindOrAdd(B.ActionId);
			Slots.SetNum(2);
			Slots[0] = FEVKeyChord::FromConfigString(A);
			Slots[1] = FEVKeyChord::FromConfigString(Bs);
		}
	}
}

void UEVUISubsystem::SaveBindings()
{
	if (!GConfig) return;
	for (const FEVKeyBindingDefinition& B : BindingDefs)
	{
		const FString Value = GetBinding(B.ActionId, 0).ToConfigString() + TEXT(";") + GetBinding(B.ActionId, 1).ToConfigString();
		GConfig->SetString(ConfigSection, *(TEXT("Bind.") + B.ActionId.ToString()), *Value, GGameUserSettingsIni);
	}
	SaveSettings();
}

// ====================================================================== input

void UEVUISubsystem::RegisterEscapeHandler(TFunction<bool()> Handler)
{
	EscapeHandlers.Add(MoveTemp(Handler));
}

bool UEVUISubsystem::HandleEscape()
{
	if (CaptureSlot >= 0) { CancelKeyCapture(); return true; }
	if (!bInterfaceVisible) { SetInterfaceVisible(true); return true; }
	if (IsAnyMenuOpen()) { CloseMenus(); return true; }
	if (IsPopupOpen()) { ClosePopup(); return true; }

	for (int32 i = OpenPanels.Num() - 1; i >= 0; --i)
	{
		const FEVPanelDefinition* Def = Panels.Find(OpenPanels[i]);
		if (Def && Def->bCloseOnEscape)
		{
			ClosePanel(OpenPanels[i]);
			return true;
		}
	}
	for (int32 i = EscapeHandlers.Num() - 1; i >= 0; --i)
	{
		if (EscapeHandlers[i] && EscapeHandlers[i]()) return true;
	}
	OpenPanel(GameMenuPanel);
	return true;
}

bool UEVUISubsystem::IsGameViewportFocused() const
{
	const ULocalPlayer* LP = GetLocalPlayer();
	const UGameViewportClient* VC = LP ? LP->ViewportClient.Get() : nullptr;
	if (!VC || !Root.IsValid()) return false;
	const TSharedPtr<SViewport> Viewport = VC->GetGameViewportWidget();
	return Viewport.IsValid() && Viewport->HasAnyUserFocusOrFocusedDescendants();
}

bool UEVUISubsystem::IsTextInputFocused() const
{
	const TSharedPtr<SWidget> Focused = FSlateApplication::Get().GetKeyboardFocusedWidget();
	return Focused.IsValid() && Focused->GetType().ToString().Contains(TEXT("EditableText"));
}

bool UEVUISubsystem::HandleKeyDown(const FKeyEvent& Event)
{
	if (!IsAttached() || !IsGameViewportFocused()) return false;
	const FKey Key = Event.GetKey();

	// Rebinding: the next real key goes to the action being edited.
	if (CaptureSlot >= 0)
	{
		if (Key == EKeys::Escape) { CancelKeyCapture(); return true; }
		if (IsModifierKey(Key) || Event.IsRepeat()) return true;
		const FEVKeyChord Chord(Key, Event.IsShiftDown(), Event.IsControlDown(), Event.IsAltDown());
		if (const FText* Reason = ReservedKeys.Find(Key))
		{
			if (!Chord.bCtrl && !Chord.bAlt && !Chord.bShift)
			{
				ShowMessage(FText::Format(LOCTEXT("Reserved", "{0} is reserved: {1}"), Chord.ToDisplayText(), *Reason), EVUIStyle::Bad);
				return true;
			}
		}
		const FName Action = CaptureAction;
		const int32 Slot = CaptureSlot;
		CancelKeyCapture();
		const FName Displaced = SetBinding(Action, Slot, Chord);
		if (!Displaced.IsNone())
		{
			const FEVKeyBindingDefinition* D = BindingDefs.FindByPredicate([Displaced](const FEVKeyBindingDefinition& B) { return B.ActionId == Displaced; });
			ShowMessage(FText::Format(LOCTEXT("Unbound", "{0} was unbound from \"{1}\""), Chord.ToDisplayText(), D ? D->Label : FText::FromName(Displaced)), FLinearColor(1.f, 0.8f, 0.4f));
		}
		return true;
	}

	if (Key == EKeys::Escape)
	{
		return Event.IsRepeat() ? true : HandleEscape();
	}
	if (IsTextInputFocused()) return false;

	const FEVKeyChord Chord(Key, Event.IsShiftDown(), Event.IsControlDown(), Event.IsAltDown());
	const FName Action = FindActionForChord(Chord);
	if (Action.IsNone()) return false;
	if (!Event.IsRepeat()) ExecuteAction(Action);
	return true;
}

// ====================================================================== icons

const FSlateBrush* UEVUISubsystem::GetIconBrush(FName IconName)
{
	if (IconName.IsNone() || MissingIcons.Contains(IconName)) return nullptr;
	if (const TSharedPtr<FSlateBrush>* Found = IconBrushes.Find(IconName)) return Found->Get();

	const FString N = IconName.ToString();
	UTexture2D* Tex = LoadObject<UTexture2D>(nullptr, *FString::Printf(TEXT("/Game/GoonZu/Icons/T_%s.T_%s"), *N, *N), nullptr, LOAD_NoWarn | LOAD_Quiet);
	if (!Tex)
	{
		MissingIcons.Add(IconName);
		return nullptr;
	}
	IconTextures.Add(IconName, Tex);
	TSharedPtr<FSlateBrush> Brush = MakeShared<FSlateBrush>();
	Brush->SetResourceObject(Tex);
	Brush->ImageSize = FVector2D(64.f, 64.f);
	Brush->DrawAs = ESlateBrushDrawType::Image;
	IconBrushes.Add(IconName, Brush);
	return Brush.Get();
}

#undef LOCTEXT_NAMESPACE
