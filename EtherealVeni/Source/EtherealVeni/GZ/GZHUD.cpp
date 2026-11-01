#include "GZHUD.h"
#include "GZAudioSubsystem.h"
#include "GZIsoCharacter.h"
#include "GZRPGComponent.h"
#include "Engine/Canvas.h"
#include "Engine/Engine.h"
#include "Engine/Font.h"
#include "Engine/Texture2D.h"
#include "GameFramework/PlayerController.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Engine/World.h"
#include "Engine/GameInstance.h"
#include "UI/EVUISubsystem.h"

namespace GZUI
{
	const FLinearColor PanelBg(0.035f, 0.045f, 0.075f, 0.90f);
	const FLinearColor PanelBg2(0.07f, 0.08f, 0.12f, 0.92f);
	const FLinearColor TitleBg(0.20f, 0.12f, 0.05f, 0.97f);
	const FLinearColor Gold(0.86f, 0.68f, 0.32f, 1.f);
	const FLinearColor GoldDim(0.45f, 0.36f, 0.18f, 1.f);
	const FLinearColor TextDim(0.70f, 0.72f, 0.78f, 1.f);
	const FLinearColor Good(0.45f, 0.95f, 0.5f, 1.f);
	const FLinearColor Bad(1.f, 0.45f, 0.4f, 1.f);
	const FLinearColor Btn(0.16f, 0.13f, 0.09f, 0.95f);
	const FLinearColor BtnHover(0.32f, 0.24f, 0.12f, 0.98f);
	const FLinearColor BtnActive(0.48f, 0.34f, 0.12f, 1.f);
}
using namespace GZUI;

namespace
{
	const TArray<FGZMenuEntry>& GetGZMenuRegistry()
	{
		static const TArray<FGZMenuEntry> Entries = []()
		{
			TArray<FGZMenuEntry> Result;
			auto Add = [&Result](FName Id, FName ParentId, const TCHAR* Label, const TCHAR* Description, EGZWindow Window, EGZMenuAction Action)
			{
				FGZMenuEntry Entry;
				Entry.Id = Id;
				Entry.ParentId = ParentId;
				Entry.Label = FText::FromString(Label);
				Entry.Description = FText::FromString(Description);
				Entry.Window = Window;
				Entry.Action = Action;
				Result.Add(MoveTemp(Entry));
			};

			Add(TEXT("player"), NAME_None, TEXT("Character"), TEXT("Character progression and personal items."), EGZWindow::None, EGZMenuAction::ExpandGroup);
			Add(TEXT("world"), NAME_None, TEXT("World"), TEXT("Explore the world, track objectives, and visit towns."), EGZWindow::None, EGZMenuAction::ExpandGroup);
			Add(TEXT("trade"), NAME_None, TEXT("Crafting & Trade"), TEXT("Gather, manufacture, and trade goods."), EGZWindow::None, EGZMenuAction::ExpandGroup);
			Add(TEXT("social"), NAME_None, TEXT("Social"), TEXT("Community and guild information."), EGZWindow::None, EGZMenuAction::ExpandGroup);
			Add(TEXT("support"), NAME_None, TEXT("Help"), TEXT("Controls and gameplay reference."), EGZWindow::None, EGZMenuAction::ExpandGroup);
			Add(TEXT("system"), NAME_None, TEXT("System"), TEXT("Resume play or leave the current session."), EGZWindow::None, EGZMenuAction::ExpandGroup);

			Add(TEXT("character"), TEXT("player"), TEXT("Character  [C]"), TEXT("View character details and attributes."), EGZWindow::Character, EGZMenuAction::OpenWindow);
			Add(TEXT("inventory"), TEXT("player"), TEXT("Inventory  [I]"), TEXT("Use items, equip gear, and assign quick slots."), EGZWindow::Inventory, EGZMenuAction::OpenWindow);
			Add(TEXT("skills"), TEXT("player"), TEXT("Skills  [K]"), TEXT("Review available skills and progression."), EGZWindow::Skills, EGZMenuAction::OpenWindow);

			Add(TEXT("map"), TEXT("world"), TEXT("World Map  [M]"), TEXT("View the world map and discovered locations."), EGZWindow::WorldMap, EGZMenuAction::OpenWindow);
			Add(TEXT("quests"), TEXT("world"), TEXT("Quest Log  [L]"), TEXT("Review active and completed quests."), EGZWindow::Quests, EGZMenuAction::OpenWindow);
			Add(TEXT("town"), TEXT("world"), TEXT("Town Hall  [N]"), TEXT("Visit town services, shares, and elections."), EGZWindow::Town, EGZMenuAction::OpenWindow);

			Add(TEXT("manufacture"), TEXT("trade"), TEXT("Manufacture  [U]"), TEXT("Select a recipe and craft items from materials."), EGZWindow::Manufacture, EGZMenuAction::OpenWindow);
			Add(TEXT("market"), TEXT("trade"), TEXT("Market  [B]"), TEXT("Buy and sell goods with available currency."), EGZWindow::Market, EGZMenuAction::OpenWindow);

			Add(TEXT("community"), TEXT("social"), TEXT("Community & Guilds  [J]"), TEXT("Review community and guild information."), EGZWindow::Community, EGZMenuAction::OpenWindow);
			Add(TEXT("help"), TEXT("support"), TEXT("How to Play  [H]"), TEXT("Movement, combat, interaction, and interface controls."), EGZWindow::Help, EGZMenuAction::OpenWindow);

			Add(TEXT("resume"), TEXT("system"), TEXT("Resume Game"), TEXT("Close this menu and return to play."), EGZWindow::None, EGZMenuAction::Resume);
			Add(TEXT("close_all"), TEXT("system"), TEXT("Close All Windows"), TEXT("Close every open game window."), EGZWindow::None, EGZMenuAction::CloseAll);
			Add(TEXT("quit"), TEXT("system"), TEXT("Quit Game"), TEXT("Exit the current game."), EGZWindow::None, EGZMenuAction::Quit);
			return Result;
		}();
		return Entries;
	}
}

AGZHUD::AGZHUD()
{
}

UGZRPGComponent* AGZHUD::RPG() const
{
	const AGZIsoCharacter* P = Player();
	return P ? P->GetRPG() : nullptr;
}

AGZIsoCharacter* AGZHUD::Player() const
{
	return PlayerOwner ? Cast<AGZIsoCharacter>(PlayerOwner->GetPawn()) : nullptr;
}

// ------------------------------------------------------------------ windows

FString AGZHUD::WindowTitle(EGZWindow W) const
{
	switch (W)
	{
	case EGZWindow::Character: return TEXT("Character  [C]");
	case EGZWindow::Inventory: return TEXT("Inventory  [I]");
	case EGZWindow::Skills: return TEXT("Skills  [K]");
	case EGZWindow::Manufacture: return TEXT("Manufacture / Game Info  [U]");
	case EGZWindow::Market: return TEXT("Market  [B]");
	case EGZWindow::Town: return TEXT("Town Hall & Election  [N]");
	case EGZWindow::Quests: return TEXT("Quest Log  [L]");
	case EGZWindow::WorldMap: return TEXT("World Map  [M]");
	case EGZWindow::Community: return TEXT("Community & Guilds  [J]");
	case EGZWindow::Help: return TEXT("How to Play  [H]");
	case EGZWindow::GameMenu: return TEXT("Game Menu");
	default: return TEXT("");
	}
}

void AGZHUD::ToggleWindow(EGZWindow W)
{
	if (IsOpen(W)) CloseWindow(W);
	else OpenWindow(W);
}

void AGZHUD::OpenWindow(EGZWindow W)
{
	if (W == EGZWindow::None || W == EGZWindow::Count) return;
	const bool bWasOpen = IsOpen(W);
	OpenWindows.Remove(W);
	OpenWindows.Add(W);   // last = on top
	if (!bWasOpen)
	{
		OnWindowStateChanged.Broadcast(W, true);
		if (UGameInstance* GameInstance = GetGameInstance())
		{
			if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
			{
				Audio->PlaySoundEffectById(TEXT("ui_menu_open"), 0.65f);
			}
		}
	}
}

void AGZHUD::CloseWindow(EGZWindow W)
{
	if (OpenWindows.Remove(W) > 0)
	{
		OnWindowStateChanged.Broadcast(W, false);
		if (UGameInstance* GameInstance = GetGameInstance())
		{
			if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
			{
				Audio->PlaySoundEffectById(TEXT("ui_menu_close"), 0.65f);
			}
		}
	}
}

TArray<FGZMenuEntry> AGZHUD::GetMenuEntries() const
{
	return GetGZMenuRegistry();
}

bool AGZHUD::SetExpandedMenuGroup(FName GroupId)
{
	if (!GroupId.IsNone() && !GetGZMenuRegistry().ContainsByPredicate([GroupId](const FGZMenuEntry& Entry)
	{
		return Entry.Id == GroupId && Entry.Action == EGZMenuAction::ExpandGroup;
	}))
	{
		UE_LOG(LogTemp, Warning, TEXT("Unknown game menu group: %s"), *GroupId.ToString());
		return false;
	}

	if (ExpandedMenuGroup != GroupId)
	{
		ExpandedMenuGroup = GroupId;
		OnMenuGroupChanged.Broadcast(GroupId);
	}
	return true;
}

bool AGZHUD::ActivateMenuEntryById(FName EntryId)
{
	const FGZMenuEntry* Entry = GetGZMenuRegistry().FindByPredicate([EntryId](const FGZMenuEntry& Candidate)
	{
		return Candidate.Id == EntryId && !Candidate.ParentId.IsNone();
	});
	if (!Entry)
	{
		UE_LOG(LogTemp, Warning, TEXT("Unknown game menu entry: %s"), *EntryId.ToString());
		return false;
	}

	ActivateMenuEntry(*Entry);
	return true;
}

void AGZHUD::ActivateMenuEntry(const FGZMenuEntry& Entry)
{
	switch (Entry.Action)
	{
	case EGZMenuAction::ExpandGroup:
		SetExpandedMenuGroup(ExpandedMenuGroup == Entry.Id ? NAME_None : Entry.Id);
		break;
	case EGZMenuAction::OpenWindow:
		CloseWindow(EGZWindow::GameMenu);
		OpenWindow(Entry.Window);
		break;
	case EGZMenuAction::Resume:
		CloseWindow(EGZWindow::GameMenu);
		break;
	case EGZMenuAction::CloseAll:
		{
			const TArray<EGZWindow> WindowsToClose = OpenWindows;
			OpenWindows.Reset();
			for (EGZWindow Window : WindowsToClose)
			{
				OnWindowStateChanged.Broadcast(Window, false);
			}
		}
		break;
	case EGZMenuAction::Quit:
		UKismetSystemLibrary::QuitGame(GetWorld(), PlayerOwner, EQuitPreference::Quit, false);
		break;
	default:
		UE_LOG(LogTemp, Warning, TEXT("Unsupported game menu action for entry: %s"), *Entry.Id.ToString());
		break;
	}
}

void AGZHUD::AddFloatingText(const FVector& WorldLocation, const FString& Text, const FLinearColor& Color)
{
	if (!bFloatingText) return;
	FFloating F;
	F.World = WorldLocation;
	F.Text = Text;
	F.Color = Color;
	Floating.Add(F);
}

// ------------------------------------------------------------------ MMO interface integration

void AGZHUD::BeginPlay()
{
	Super::BeginPlay();
	// The Slate interface (menus, sub-menus, Options, key bindings) lives in UEVUISubsystem.
	if (UEVUISubsystem* UI = UEVUISubsystem::Get(PlayerOwner))
	{
		UI->AttachToViewport();
	}
}

void AGZHUD::ReadInterfaceSettings()
{
	UEVUISubsystem* UI = UEVUISubsystem::Get(PlayerOwner);
	bSlateUI = UI && UI->IsAttached();
	if (!UI) return;

	auto B = [UI](const TCHAR* Key, bool bDefault) { return UI->FindSetting(Key) ? UI->GetSettingBool(Key) : bDefault; };
	auto F = [UI](const TCHAR* Key, float Default) { return UI->FindSetting(Key) ? UI->GetSettingFloat(Key) : Default; };
	bShowClock = B(TEXT("interface.show_clock"), true);
	bShowMinimap = B(TEXT("interface.show_minimap"), true);
	bShowTarget = B(TEXT("interface.show_target"), true);
	bShowChat = B(TEXT("interface.show_chat"), true);
	bTooltips = B(TEXT("interface.tooltips"), true);
	bFloatingText = B(TEXT("combat.floating_text"), true);
	bWorldMarker = B(TEXT("combat.world_marker"), true);
	bTrackNpcs = B(TEXT("minimap.track_npcs"), true);
	bTrackMonsters = B(TEXT("minimap.track_monsters"), true);
	bTrackResources = B(TEXT("minimap.track_resources"), true);
	bSystemChat = B(TEXT("social.system_chat"), true);
	bTextShadow = B(TEXT("access.text_shadow"), true);
	HudScale = FMath::Clamp(F(TEXT("interface.hud_scale"), 1.f), 0.5f, 2.f);
	PanelOpacity = FMath::Clamp(F(TEXT("access.panel_opacity"), 1.f), 0.1f, 1.f);
	PlayerStatus = UI->FindSetting(TEXT("social.status")) ? UI->GetSettingInt(TEXT("social.status")) : 0;
	static const float ZoomRadius[] = {3500.f, 6000.f, 10000.f};
	MinimapRadius = ZoomRadius[FMath::Clamp(UI->FindSetting(TEXT("minimap.zoom")) ? UI->GetSettingInt(TEXT("minimap.zoom")) : 1, 0, 2)];
}

void AGZHUD::HandleContextClicks()
{
	if (!bSlateUI || !PlayerOwner->WasInputKeyJustPressed(EKeys::RightMouseButton)) return;
	UEVUISubsystem* UI = UEVUISubsystem::Get(PlayerOwner);
	UGZRPGComponent* R = RPG();
	if (!UI || !R) return;

	// Rects are from the previous frame, matching what the player clicked on.
	auto Over = [this](const FBox2D& Box) { return Box.bIsValid && Box.IsInside(Mouse); };
	if (Over(TargetRect))
	{
		const AGZIsoCharacter* P = Player();
		const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
		if (T) UI->OpenContextMenu(TEXT("unit.target"), T->Name);
	}
	else if (Over(StatusRect))
	{
		UI->OpenContextMenu(TEXT("unit.self"), R->CharacterName);
	}
	else if (Over(MinimapRect))
	{
		UI->OpenContextMenu(TEXT("minimap"), FString());
	}
	else if (Over(ChatRect))
	{
		UI->OpenContextMenu(TEXT("chat"), FString());
	}
}

bool AGZHUD::CloseTopmostWindow()
{
	if (OpenWindows.Num() == 0) return false;
	CloseWindow(OpenWindows.Last());
	return true;
}

void AGZHUD::CloseAllWindows()
{
	while (CloseTopmostWindow()) {}
}

FString AGZHUD::GetChatFilterName(int32 Filter)
{
	static const TCHAR* Names[NumChatFilters] = {TEXT("All"), TEXT("System"), TEXT("Combat"), TEXT("NPC"), TEXT("Quest"), TEXT("Town")};
	return Names[FMath::Clamp(Filter, 0, NumChatFilters - 1)];
}

FText AGZHUD::GetMarkerName(int32 Marker)
{
	static const FText Names[NumTargetMarkers + 1] = {
		NSLOCTEXT("GZHUD", "MarkNone", "None"), NSLOCTEXT("GZHUD", "MarkStar", "Star"), NSLOCTEXT("GZHUD", "MarkCircle", "Circle"),
		NSLOCTEXT("GZHUD", "MarkDiamond", "Diamond"), NSLOCTEXT("GZHUD", "MarkTriangle", "Triangle"), NSLOCTEXT("GZHUD", "MarkMoon", "Moon"),
		NSLOCTEXT("GZHUD", "MarkSquare", "Square"), NSLOCTEXT("GZHUD", "MarkCross", "Cross"), NSLOCTEXT("GZHUD", "MarkSkull", "Skull")};
	return Names[FMath::Clamp(Marker, 0, NumTargetMarkers)];
}

FLinearColor AGZHUD::GetMarkerColor(int32 Marker)
{
	static const FLinearColor Colors[NumTargetMarkers + 1] = {
		FLinearColor::White, FLinearColor(1.f, 0.92f, 0.2f), FLinearColor(1.f, 0.55f, 0.1f), FLinearColor(0.85f, 0.35f, 1.f),
		FLinearColor(0.35f, 1.f, 0.35f), FLinearColor(0.8f, 0.85f, 0.95f), FLinearColor(0.3f, 0.6f, 1.f), FLinearColor(1.f, 0.3f, 0.25f),
		FLinearColor(0.95f, 0.95f, 0.95f)};
	return Colors[FMath::Clamp(Marker, 0, NumTargetMarkers)];
}

int32 AGZHUD::MarkerFor(const AActor* Actor) const
{
	if (!Actor) return 0;
	for (const TPair<TWeakObjectPtr<AActor>, int32>& Pair : TargetMarkers)
	{
		if (Pair.Key.Get() == Actor) return Pair.Value;
	}
	return 0;
}

void AGZHUD::SetTargetMarker(int32 Marker)
{
	const AGZIsoCharacter* P = Player();
	const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
	if (!T || !T->Actor.IsValid()) return;
	Marker = FMath::Clamp(Marker, 0, NumTargetMarkers);
	// A marker belongs to one target at a time.
	for (auto It = TargetMarkers.CreateIterator(); It; ++It)
	{
		if (!It.Key().IsValid() || It.Value() == Marker || It.Key().Get() == T->Actor.Get()) It.RemoveCurrent();
	}
	if (Marker > 0) TargetMarkers.Add(T->Actor, Marker);
}

int32 AGZHUD::GetTargetMarker() const
{
	const AGZIsoCharacter* P = Player();
	const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
	return T ? MarkerFor(T->Actor.Get()) : 0;
}

// ------------------------------------------------------------------ primitives

UTexture2D* AGZHUD::Tex(const FString& Name)
{
	if (TObjectPtr<UTexture2D>* Found = TextureCache.Find(Name))
	{
		return Found->Get();
	}
	if (MissingTextures.Contains(Name)) return nullptr;
	const FString Path = FString::Printf(TEXT("/Game/GoonZu/Icons/T_%s.T_%s"), *Name, *Name);
	UTexture2D* T = LoadObject<UTexture2D>(nullptr, *Path, nullptr, LOAD_NoWarn | LOAD_Quiet);
	if (T)
	{
		TextureCache.Add(Name, T);
	}
	else
	{
		MissingTextures.Add(Name);
	}
	return T;
}

void AGZHUD::Frame(float X, float Y, float W, float H, const FLinearColor& C, float T)
{
	DrawRect(C, X, Y, W, T);
	DrawRect(C, X, Y + H - T, W, T);
	DrawRect(C, X, Y, T, H);
	DrawRect(C, X + W - T, Y, T, H);
}

void AGZHUD::Panel(float X, float Y, float W, float H, const FLinearColor& Bg, bool bGoldFrame)
{
	DrawRect(FLinearColor(Bg.R, Bg.G, Bg.B, Bg.A * PanelOpacity), X, Y, W, H);
	if (bGoldFrame)
	{
		Frame(X, Y, W, H, GoldDim, FMath::Max(1.f, S));
		Frame(X + 3 * S, Y + 3 * S, W - 6 * S, H - 6 * S, FLinearColor(0.f, 0.f, 0.f, 0.5f), 1.f);
	}
	UIRects.Add(FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H)));
}

void AGZHUD::Txt(const FString& T, float X, float Y, const FLinearColor& C, float Size, bool bShadow)
{
	UFont* Font = GEngine ? GEngine->GetMediumFont() : nullptr;
	const float Scale = S * Size;
	if (bShadow && bTextShadow)
	{
		DrawText(T, FLinearColor(0.f, 0.f, 0.f, 0.85f), X + 1.f, Y + 1.f, Font, Scale);
	}
	DrawText(T, C, X, Y, Font, Scale);
}

float AGZHUD::TxtW(const FString& T, float Size)
{
	float W = 0.f, H = 0.f;
	GetTextSize(T, W, H, GEngine ? GEngine->GetMediumFont() : nullptr, S * Size);
	return W;
}

void AGZHUD::TxtC(const FString& T, float CX, float Y, const FLinearColor& C, float Size)
{
	Txt(T, CX - TxtW(T, Size) * 0.5f, Y, C, Size);
}

void AGZHUD::Hit(const FString& Id, float X, float Y, float W, float H)
{
	FButton B;
	B.Box = FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H));
	B.Id = Id;
	B.Layer = CurrentLayer;
	Buttons.Add(B);
}

bool AGZHUD::Button(const FString& Id, float X, float Y, float W, float H, const FString& Label, bool bEnabled, bool bActive, float Size)
{
	const bool bHover = bEnabled && Mouse.X >= X && Mouse.X <= X + W && Mouse.Y >= Y && Mouse.Y <= Y + H;
	DrawRect(bActive ? BtnActive : (bHover ? BtnHover : Btn), X, Y, W, H);
	Frame(X, Y, W, H, bEnabled ? Gold * FLinearColor(1, 1, 1, bHover ? 1.f : 0.6f) : FLinearColor(0.3f, 0.3f, 0.3f, 0.6f), 1.f);
	TxtC(Label, X + W * 0.5f, Y + (H - 16.f * S * Size) * 0.5f - 1.f, bEnabled ? FLinearColor(1.f, 0.95f, 0.85f) : FLinearColor(0.5f, 0.5f, 0.5f), Size);
	if (bEnabled)
	{
		Hit(Id, X, Y, W, H);
	}
	return bHover;
}

void AGZHUD::Bar(float X, float Y, float W, float H, float Frac, const FLinearColor& C, const FString& Label)
{
	DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.7f), X, Y, W, H);
	DrawRect(C * 0.55f, X + 1, Y + 1, (W - 2) * FMath::Clamp(Frac, 0.f, 1.f), H - 2);
	DrawRect(C, X + 1, Y + 1, (W - 2) * FMath::Clamp(Frac, 0.f, 1.f), (H - 2) * 0.45f);
	Frame(X, Y, W, H, FLinearColor(0.f, 0.f, 0.f, 0.9f), 1.f);
	if (!Label.IsEmpty())
	{
		TxtC(Label, X + W * 0.5f, Y + (H - 15.f * S * 0.8f) * 0.5f - 1.f, FLinearColor::White, 0.8f);
	}
}

void AGZHUD::Icon(const FString& IconId, float X, float Y, float Size, const FLinearColor& Fallback, const FString& FallbackText)
{
	if (UTexture2D* T = Tex(IconId))
	{
		DrawTexture(T, X, Y, Size, Size, 0.f, 0.f, 1.f, 1.f, FLinearColor::White, BLEND_Translucent);
	}
	else
	{
		DrawRect(Fallback, X + 2, Y + 2, Size - 4, Size - 4);
		TxtC(FallbackText, X + Size * 0.5f, Y + Size * 0.5f - 8.f * S, FLinearColor::White, 0.85f);
	}
}

void AGZHUD::ItemSlot(const FString& Item, float X, float Y, float Size, int32 Count, const FString& ButtonId)
{
	DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.55f), X, Y, Size, Size);
	const bool bHover = Mouse.X >= X && Mouse.X <= X + Size && Mouse.Y >= Y && Mouse.Y <= Y + Size;
	Frame(X, Y, Size, Size, bHover ? Gold : GoldDim, 1.f);
	if (!Item.IsEmpty() && RPG())
	{
		const FGZItemDef& D = RPG()->GetItem(Item);
		Icon(Item, X + 2, Y + 2, Size - 4, D.Color, D.Name.Left(2).ToUpper());
		if (Count > 1 || Count == 0)
		{
			const FString C = FString::FromInt(Count);
			Txt(C, X + Size - TxtW(C, 0.75f) - 3, Y + Size - 15 * S, Count == 0 ? Bad : FLinearColor::White, 0.75f);
		}
		if (bHover && bTooltips)
		{
			// Tooltip
			FString Tip = D.Name;
			if (D.Attack > 0) Tip += FString::Printf(TEXT("  ATK +%d"), D.Attack);
			if (D.Defense > 0) Tip += FString::Printf(TEXT("  DEF +%d"), D.Defense);
			if (D.Heal > 0) Tip += FString::Printf(TEXT("  HP +%.0f"), D.Heal);
			if (D.Mana > 0) Tip += FString::Printf(TEXT("  MP +%.0f"), D.Mana);
			Tip += FString::Printf(TEXT("  (%s, base %d %s)"), *D.Type, D.BasePrice, *RPG()->Currency);
			const float TW = TxtW(Tip, 0.8f) + 12 * S;
			DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.92f), Mouse.X + 14 * S, Mouse.Y + 14 * S, TW, 22 * S);
			Frame(Mouse.X + 14 * S, Mouse.Y + 14 * S, TW, 22 * S, Gold, 1.f);
			Txt(Tip, Mouse.X + 20 * S, Mouse.Y + 17 * S, FLinearColor(1.f, 0.95f, 0.8f), 0.8f);
		}
	}
	if (!ButtonId.IsEmpty())
	{
		Hit(ButtonId, X, Y, Size, Size);
	}
}

bool AGZHUD::BeginWindow(EGZWindow W, const FString& Title, float W0, float H0, float& OutX, float& OutY)
{
	const float Wd = W0 * S, Ht = H0 * S;
	const uint8 Key = (uint8)W;
	if (!WindowPos.Contains(Key))
	{
		FVector2D P;
		switch (W)
		{
		case EGZWindow::Character: case EGZWindow::Skills: case EGZWindow::Quests: case EGZWindow::Town:
			P = FVector2D(20 * S, 150 * S); break;
		case EGZWindow::Inventory: case EGZWindow::Market:
			P = FVector2D(Canvas->ClipX - Wd - 20 * S, 300 * S); break;
		default:
			P = FVector2D((Canvas->ClipX - Wd) * 0.5f, (Canvas->ClipY - Ht) * 0.42f); break;
		}
		WindowPos.Add(Key, P);
	}
	FVector2D& Pos = WindowPos[Key];
	Pos.X = FMath::Clamp(Pos.X, 0.f, FMath::Max(0.f, Canvas->ClipX - Wd));
	Pos.Y = FMath::Clamp(Pos.Y, 0.f, FMath::Max(0.f, Canvas->ClipY - Ht));
	OutX = Pos.X;
	OutY = Pos.Y;

	CurrentWindow = W;
	Panel(OutX, OutY, Wd, Ht, PanelBg);
	// title bar
	DrawRect(TitleBg, OutX + 2, OutY + 2, Wd - 4, 26 * S);
	DrawRect(Gold, OutX + 2, OutY + 28 * S, Wd - 4, 1.f);
	Txt(Title, OutX + 12 * S, OutY + 6 * S, Gold, 0.95f);
	Hit(FString::Printf(TEXT("Drag_%d"), Key), OutX, OutY, Wd - 30 * S, 28 * S);
	Button(FString::Printf(TEXT("Close_%d"), Key), OutX + Wd - 27 * S, OutY + 4 * S, 22 * S, 20 * S, TEXT("X"));
	// background blocker (lowest priority within this layer)
	FButton Block;
	Block.Box = FBox2D(FVector2D(OutX, OutY), FVector2D(OutX + Wd, OutY + Ht));
	Block.Id = FString::Printf(TEXT("Block_%d"), Key);
	Block.Layer = CurrentLayer;
	Buttons.Insert(Block, 0);
	OutY += 36 * S;
	OutX += 12 * S;
	return true;
}

// ------------------------------------------------------------------ main

void AGZHUD::DrawHUD()
{
	Super::DrawHUD();
	if (!Canvas || !PlayerOwner) return;
	UGZRPGComponent* R = RPG();
	if (!R) return;

	ReadInterfaceSettings();
	if (bSlateUI)
	{
		UEVUISubsystem* UI = UEVUISubsystem::Get(PlayerOwner);
		if (UI && !UI->IsInterfaceVisible())
		{
			// Interface hidden (Alt+Z): draw nothing and let every click reach the world.
			PrevButtons.Reset();
			PrevUIRects.Reset();
			bMouseOverUI = false;
			return;
		}
	}

	const float Now = GetWorld()->GetTimeSeconds();
	const float Dt = FMath::Clamp(Now - LastTime, 0.f, 0.1f);
	LastTime = Now;
	S = FMath::Clamp(Canvas->ClipY / 1080.f, 0.75f, 1.6f) * HudScale;

	float MX = 0.f, MY = 0.f;
	PlayerOwner->GetMousePosition(MX, MY);
	Mouse = FVector2D(MX, MY);

	HandleContextClicks();
	StatusRect = MinimapRect = TargetRect = ChatRect = FBox2D(ForceInit);

	// Use last frame's layout for hover/click so input matches what the player saw.
	bMouseOverUI = false;
	for (const FBox2D& B : PrevUIRects)
	{
		if (B.IsInside(Mouse)) { bMouseOverUI = true; break; }
	}

	const bool bDown = PlayerOwner->IsInputKeyDown(EKeys::LeftMouseButton);
	if (PlayerOwner->WasInputKeyJustPressed(EKeys::LeftMouseButton))
	{
		const FButton* Best = nullptr;
		for (const FButton& B : PrevButtons)
		{
			// Later entries are drawn on top; within a layer, buttons come after the window blocker.
			if (B.Box.IsInside(Mouse) && (!Best || B.Layer >= Best->Layer))
			{
				Best = &B;
			}
		}
		if (Best)
		{
			HandleClick(Best->Id);
		}
	}
	if (!bDown) DragWindow = -1;
	if (DragWindow >= 0 && WindowPos.Contains((uint8)DragWindow))
	{
		WindowPos[(uint8)DragWindow] = Mouse - DragOffset;
	}

	// Mouse wheel scroll for lists.
	if (bMouseOverUI)
	{
		const int32 Wheel = (PlayerOwner->WasInputKeyJustPressed(EKeys::MouseScrollDown) ? 1 : 0) - (PlayerOwner->WasInputKeyJustPressed(EKeys::MouseScrollUp) ? 1 : 0);
		if (Wheel != 0)
		{
			if (IsOpen(EGZWindow::Manufacture)) RecipeScroll = FMath::Max(0, RecipeScroll + Wheel);
			if (IsOpen(EGZWindow::Market)) MarketScroll = FMath::Max(0, MarketScroll + Wheel);
		}
	}

	Buttons.Reset();
	UIRects.Reset();
	CurrentLayer = 0;

	DrawFloating(Dt);
	DrawStatus();
	if (bShowClock) DrawClock();
	if (bShowMinimap) DrawMinimap();
	if (bShowTarget) DrawTargetFrame();
	if (bShowChat) DrawChat();
	DrawQuickBar();
	if (!bSlateUI) DrawMenuBar();   // the Slate micro menu replaces it
	DrawPrompt();

	for (int32 i = 0; i < OpenWindows.Num(); ++i)
	{
		CurrentLayer = 10 + i;
		switch (OpenWindows[i])
		{
		case EGZWindow::Character: DrawCharacter(); break;
		case EGZWindow::Inventory: DrawInventory(); break;
		case EGZWindow::Skills: DrawSkills(); break;
		case EGZWindow::Manufacture: DrawManufacture(); break;
		case EGZWindow::Market: DrawMarket(); break;
		case EGZWindow::Town: DrawTown(); break;
		case EGZWindow::Quests: DrawQuests(); break;
		case EGZWindow::WorldMap: DrawWorldMap(); break;
		case EGZWindow::Community: DrawCommunity(); break;
		case EGZWindow::Help: DrawHelp(); break;
		case EGZWindow::GameMenu: DrawGameMenu(); break;
		default: break;
		}
	}

	PrevButtons = Buttons;
	PrevUIRects = UIRects;
}

void AGZHUD::HandleClick(const FString& Id)
{
	UGZRPGComponent* R = RPG();
	if (!R) return;
	if (UGameInstance* GameInstance = GetGameInstance())
	{
		if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
		{
			Audio->PlaySoundEffectById(TEXT("ui_select"));
		}
	}
	FString Prefix, Arg;
	if (!Id.Split(TEXT("_"), &Prefix, &Arg))
	{
		Prefix = Id;
	}
	const int32 N = FCString::Atoi(*Arg);

	if (Prefix == TEXT("Close")) { CloseWindow((EGZWindow)N); return; }
	if (Prefix == TEXT("Block")) { OpenWindow((EGZWindow)N); return; }
	if (Prefix == TEXT("Drag"))
	{
		OpenWindow((EGZWindow)N);
		DragWindow = N;
		DragOffset = Mouse - WindowPos.FindRef((uint8)N);
		return;
	}
	if (Prefix == TEXT("Open")) { ToggleWindow((EGZWindow)N); return; }
	if (Prefix == TEXT("Quick")) { R->UseQuickSlot(N); return; }
	if (Prefix == TEXT("Inv"))
	{
		if (R->InventoryOrder.IsValidIndex(N))
		{
			const FString Item = R->InventoryOrder[N];
			if (InventoryMode == 1)
			{
				R->QuickSlots[AssignSlot] = Item;
				R->AddChat(TEXT("System"), FString::Printf(TEXT("%s assigned to F%d."), *R->GetItem(Item).Name, AssignSlot + 1), TextDim);
				InventoryMode = 0;
			}
			else
			{
				R->UseItem(Item);
			}
		}
		return;
	}
	if (Prefix == TEXT("InvMode")) { InventoryMode = InventoryMode == 1 ? 0 : 1; return; }
	if (Prefix == TEXT("Slot")) { AssignSlot = FMath::Clamp(N, 0, 4); InventoryMode = 1; return; }
	if (Prefix == TEXT("Attr")) { R->RaiseAttribute(N); return; }
	if (Prefix == TEXT("Recipe")) { SelectedRecipe = N; return; }
	if (Prefix == TEXT("Craft")) { R->Craft(SelectedRecipe); return; }
	if (Prefix == TEXT("Craft5")) { for (int32 i = 0; i < 5; ++i) { if (!R->Recipes.IsValidIndex(SelectedRecipe) || !R->HasMaterials(R->Recipes[SelectedRecipe])) break; R->Craft(SelectedRecipe); } return; }
	if (Prefix == TEXT("Buy")) { if (R->MarketGoods.IsValidIndex(N)) R->Buy(R->MarketGoods[N]); return; }
	if (Prefix == TEXT("Sell")) { if (R->MarketGoods.IsValidIndex(N)) R->Sell(R->MarketGoods[N]); return; }
	if (Prefix == TEXT("SellInv")) { if (R->InventoryOrder.IsValidIndex(N)) R->Sell(R->InventoryOrder[N]); return; }
	if (Prefix == TEXT("Share")) { R->BuyShare(); return; }
	if (Prefix == TEXT("Vote")) { R->Vote(N); return; }
	if (Prefix == TEXT("Chat")) { ChatFilter = N; return; }
	if (Prefix == TEXT("Scroll")) { RecipeScroll = FMath::Max(0, RecipeScroll + (N == 0 ? -1 : 1)); return; }
	if (Prefix == TEXT("MScroll")) { MarketScroll = FMath::Max(0, MarketScroll + (N == 0 ? -1 : 1)); return; }
	if (Prefix == TEXT("MenuGroup")) { const FName GroupId(*Arg); SetExpandedMenuGroup(ExpandedMenuGroup == GroupId ? NAME_None : GroupId); return; }
	if (Prefix == TEXT("MenuEntry"))
	{
		ActivateMenuEntryById(FName(*Arg));
		return;
	}
	if (Prefix == TEXT("Menu"))
	{
		if (N == 0) CloseWindow(EGZWindow::GameMenu);
		if (N == 1) { CloseWindow(EGZWindow::GameMenu); OpenWindow(EGZWindow::Help); }
		if (N == 2) UKismetSystemLibrary::QuitGame(GetWorld(), PlayerOwner, EQuitPreference::Quit, false);
		if (N == 3) { OpenWindows.Reset(); }
		return;
	}
	if (Prefix == TEXT("Act"))
	{
		// Action buttons on the quick bar simulate the keys.
		if (N == 0) R->AddChat(TEXT("System"), TEXT("Press Space near a monster to attack (Tab cycles targets)."), TextDim);
		if (N == 1) R->AddChat(TEXT("System"), TEXT("Press G beside a resource node to gather."), TextDim);
		if (N == 2) R->AddChat(TEXT("System"), TEXT("Press E beside an NPC to talk."), TextDim);
		return;
	}
}

// ------------------------------------------------------------------ fixed HUD

void AGZHUD::DrawStatus()
{
	UGZRPGComponent* R = RPG();
	const float X = 12 * S, Y = 12 * S, W = 360 * S, H = 118 * S;
	Panel(X, Y, W, H, PanelBg);
	StatusRect = FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H));
	// Portrait
	DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.6f), X + 8 * S, Y + 8 * S, 100 * S, 100 * S);
	Icon(TEXT("portrait_player"), X + 8 * S, Y + 8 * S, 100 * S, FLinearColor(0.2f, 0.3f, 0.6f), TEXT("You"));
	Frame(X + 8 * S, Y + 8 * S, 100 * S, 100 * S, Gold, 1.f);
	DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.8f), X + 8 * S, Y + 88 * S, 34 * S, 20 * S);
	Txt(FString::Printf(TEXT("%d"), R->Level), X + 13 * S, Y + 90 * S, Gold, 0.9f);

	const float BX = X + 118 * S, BW = W - 128 * S;
	Txt(FString::Printf(TEXT("%s"), *R->CharacterName), BX, Y + 8 * S, FLinearColor::White, 1.0f);
	if (PlayerStatus == 1 || PlayerStatus == 2)
	{
		Txt(PlayerStatus == 1 ? TEXT("<Away>") : TEXT("<Busy>"), BX + TxtW(R->CharacterName, 1.f) + 8 * S, Y + 11 * S,
			PlayerStatus == 1 ? FLinearColor(1.f, 0.85f, 0.3f) : Bad, 0.75f);
	}
	const FString LvTxt = FString::Printf(TEXT("Lv.%d"), R->Level);
	Txt(LvTxt, BX + BW - TxtW(LvTxt, 0.85f), Y + 10 * S, Gold, 0.85f);
	Bar(BX, Y + 34 * S, BW, 18 * S, R->HP / R->MaxHP, FLinearColor(0.85f, 0.15f, 0.15f), FString::Printf(TEXT("HP %d / %d"), FMath::RoundToInt(R->HP), FMath::RoundToInt(R->MaxHP)));
	Bar(BX, Y + 56 * S, BW, 18 * S, R->MP / R->MaxMP, FLinearColor(0.2f, 0.4f, 0.95f), FString::Printf(TEXT("MP %d / %d"), FMath::RoundToInt(R->MP), FMath::RoundToInt(R->MaxMP)));
	Bar(BX, Y + 78 * S, BW, 12 * S, R->Exp / R->ExpToNext, FLinearColor(0.95f, 0.8f, 0.2f), TEXT(""));
	Txt(FString::Printf(TEXT("EXP %.1f%%"), 100.f * R->Exp / R->ExpToNext), BX, Y + 94 * S, TextDim, 0.75f);
	const FString Money = FString::Printf(TEXT("%lld %s"), R->Luma, *R->Currency);
	Txt(Money, BX + BW - TxtW(Money, 0.8f), Y + 94 * S, FLinearColor(1.f, 0.85f, 0.35f), 0.8f);
}

void AGZHUD::DrawClock()
{
	UGZRPGComponent* R = RPG();
	// 1 real second = 1 game minute, starting 08:00.
	const int32 Minutes = (int32)(GetWorld()->GetTimeSeconds()) + 8 * 60;
	const FString T = FString::Printf(TEXT("%s  |  %s  |  %02d:%02d"), *R->WorldName, *R->CurrentZone, (Minutes / 60) % 24, Minutes % 60);
	const float W = TxtW(T, 0.9f) + 30 * S;
	const float X = (Canvas->ClipX - W) * 0.5f, Y = 8 * S;
	Panel(X, Y, W, 26 * S, PanelBg, true);
	TxtC(T, Canvas->ClipX * 0.5f, Y + 5 * S, R->bInTown ? FLinearColor(1.f, 0.9f, 0.6f) : FLinearColor(0.85f, 0.95f, 0.85f), 0.9f);
}

void AGZHUD::DrawMapContents(float X, float Y, float W, float H, float WorldRadius, bool bLabels)
{
	AGZIsoCharacter* P = Player();
	if (!P) return;
	const FVector Me = P->GetActorLocation();
	// The world map is north-up (+X up, +Y right) to match its painted texture; the minimap follows the camera.
	const float Yaw = bLabels ? 0.f : FMath::DegreesToRadians(P->GetCameraYaw());
	const float CosY = FMath::Cos(Yaw), SinY = FMath::Sin(Yaw);
	const FVector2D C(X + W * 0.5f, Y + H * 0.5f);
	const float Scale = (FMath::Min(W, H) * 0.5f) / WorldRadius;
	// For the big map centre on the world origin, for the minimap centre on the player.
	const FVector Center = bLabels ? FVector::ZeroVector : Me;

	auto ToMap = [&](const FVector& L, FVector2D& Out) -> bool
	{
		const float DX = L.X - Center.X, DY = L.Y - Center.Y;
		const float Fwd = DX * CosY + DY * SinY;
		const float Rt = -DX * SinY + DY * CosY;
		Out = FVector2D(C.X + Rt * Scale, C.Y - Fwd * Scale);
		return Out.X >= X + 3 && Out.X <= X + W - 3 && Out.Y >= Y + 3 && Out.Y <= Y + H - 3;
	};

	DrawRect(FLinearColor(0.12f, 0.2f, 0.12f, 0.9f), X, Y, W, H);
	if (bLabels)
	{
		if (UTexture2D* MapTex = Tex(TEXT("worldmap")))
		{
			// Texture covers world -20000..20000 cm on both axes.
			const float Half = 20000.f * Scale;
			DrawTexture(MapTex, C.X - Half, C.Y - Half, 2 * Half, 2 * Half, 0.f, 0.f, 1.f, 1.f, FLinearColor::White, BLEND_Opaque);
		}
	}
	for (const FGZWorldObject& O : P->GetWorldObjects())
	{
		if (!O.Actor.IsValid()) continue;
		FVector2D M;
		if (O.Kind == TEXT("zone"))
		{
			if (!ToMap(O.Actor->GetActorLocation(), M)) continue;
			const bool bTown = O.Arg2 == TEXT("town");
			const float R = FMath::Clamp(O.Radius * Scale, 3.f, W);
			// Zone disc approximated by a square tint.
			if (bLabels || bTown)
			{
				DrawRect(bTown ? FLinearColor(0.85f, 0.7f, 0.35f, 0.35f) : FLinearColor(0.4f, 0.55f, 0.4f, 0.18f), M.X - R * 0.7f, M.Y - R * 0.7f, R * 1.4f, R * 1.4f);
			}
			if (bLabels || bTown)
			{
				TxtC(O.Name, M.X, M.Y - 8 * S, bTown ? FLinearColor(1.f, 0.9f, 0.6f) : FLinearColor(0.8f, 0.9f, 0.8f), bTown ? 0.85f : 0.7f);
			}
		}
		else if (ToMap(O.Actor->GetActorLocation(), M))
		{
			// Minimap tracking filters (right-click the minimap).
			if (!bLabels && ((O.Kind == TEXT("npc") && !bTrackNpcs) || (O.Kind == TEXT("monster") && !bTrackMonsters) || (O.Kind == TEXT("resource") && !bTrackResources)))
			{
				continue;
			}
			FLinearColor Col = FLinearColor::White;
			float Sz = 4.f * S;
			if (O.Kind == TEXT("npc")) Col = FLinearColor(1.f, 0.9f, 0.2f);
			else if (O.Kind == TEXT("monster")) { if (O.bDead) continue; Col = FLinearColor(0.95f, 0.25f, 0.2f); Sz = O.Name.Contains(TEXT("Drake")) ? 9.f * S : 3.f * S; }
			else if (O.Kind == TEXT("resource")) Col = FLinearColor(0.3f, 0.95f, 0.5f);
			DrawRect(Col, M.X - Sz * 0.5f, M.Y - Sz * 0.5f, Sz, Sz);
		}
	}
	// Player arrow
	FVector2D PM;
	ToMap(Me, PM);
	DrawRect(FLinearColor::White, PM.X - 4 * S, PM.Y - 4 * S, 8 * S, 8 * S);
	DrawRect(FLinearColor(0.1f, 0.4f, 1.f), PM.X - 2.5f * S, PM.Y - 2.5f * S, 5 * S, 5 * S);
	Frame(X, Y, W, H, Gold, 1.f);
}

void AGZHUD::DrawMinimap()
{
	UGZRPGComponent* R = RPG();
	AGZIsoCharacter* P = Player();
	const float W = 230 * S, H = 230 * S;
	const float X = Canvas->ClipX - W - 12 * S, Y = 12 * S;
	Panel(X - 6 * S, Y - 4 * S, W + 12 * S, H + 54 * S, PanelBg);
	MinimapRect = FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H));
	DrawMapContents(X, Y, W, H, MinimapRadius, false);
	if (P)
	{
		const FVector L = P->GetActorLocation();
		Txt(FString::Printf(TEXT("%s"), *R->CurrentZone), X + 2 * S, Y + H + 4 * S, R->bInTown ? FLinearColor(1.f, 0.9f, 0.6f) : FLinearColor::White, 0.85f);
		const FString Coords = FString::Printf(TEXT("(%d, %d)"), FMath::RoundToInt(L.X / 100.f), FMath::RoundToInt(L.Y / 100.f));
		Txt(Coords, X + W - TxtW(Coords, 0.75f), Y + H + 5 * S, TextDim, 0.75f);
	}
	Button(FString::Printf(TEXT("Open_%d"), (int32)EGZWindow::WorldMap), X + 2 * S, Y + H + 25 * S, 72 * S, 18 * S, TEXT("Map [M]"), true, IsOpen(EGZWindow::WorldMap), 0.7f);
	Button(FString::Printf(TEXT("Open_%d"), (int32)EGZWindow::Help), X + 80 * S, Y + H + 25 * S, 72 * S, 18 * S, TEXT("Help [H]"), true, IsOpen(EGZWindow::Help), 0.7f);
	Button(FString::Printf(TEXT("Open_%d"), (int32)EGZWindow::Quests), X + 158 * S, Y + H + 25 * S, 72 * S, 18 * S, TEXT("Quest [L]"), true, IsOpen(EGZWindow::Quests), 0.7f);
}

static FString MonsterIconId(const FString& Name)
{
	FString N = Name.ToLower();
	N.ReplaceInline(TEXT(" "), TEXT("_"));
	return TEXT("mon_") + N;
}

void AGZHUD::DrawTargetFrame()
{
	AGZIsoCharacter* P = Player();
	const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
	if (!T || !T->Actor.IsValid()) return;
	const float W = 300 * S, H = 64 * S;
	const float X = (Canvas->ClipX - W) * 0.5f, Y = 42 * S;
	Panel(X, Y, W, H, PanelBg);
	TargetRect = FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H));
	Icon(MonsterIconId(T->Name), X + 6 * S, Y + 6 * S, 52 * S, FLinearColor(0.5f, 0.2f, 0.2f), T->Name.Left(2));
	Frame(X + 6 * S, Y + 6 * S, 52 * S, 52 * S, Bad, 1.f);
	Txt(FString::Printf(TEXT("%s  Lv.%d"), *T->Name, T->Level), X + 66 * S, Y + 8 * S, FLinearColor(1.f, 0.75f, 0.6f), 0.9f);
	Bar(X + 66 * S, Y + 32 * S, W - 76 * S, 18 * S, T->HP / FMath::Max(1.f, T->MaxHP), FLinearColor(0.85f, 0.15f, 0.15f), FString::Printf(TEXT("%d / %d"), FMath::RoundToInt(FMath::Max(0.f, T->HP)), FMath::RoundToInt(T->MaxHP)));

	const int32 Marker = MarkerFor(T->Actor.Get());
	if (Marker > 0)
	{
		const FString MarkerName = GetMarkerName(Marker).ToString();
		Txt(MarkerName, X + W - TxtW(MarkerName, 0.75f) - 8 * S, Y + 10 * S, GetMarkerColor(Marker), 0.75f);
	}

	// Marker over the target in the world.
	if (!bWorldMarker) return;
	FVector Screen = Project(T->Actor->GetActorLocation() + FVector(0, 0, 220));
	if (Screen.Z > 0.f)
	{
		const FLinearColor Outer = Marker > 0 ? GetMarkerColor(Marker) : Bad;
		DrawRect(Outer, Screen.X - 6 * S, Screen.Y - 6 * S, 12 * S, 12 * S);
		DrawRect(FLinearColor(1.f, 0.9f, 0.3f), Screen.X - 3 * S, Screen.Y - 3 * S, 6 * S, 6 * S);
		if (Marker > 0)
		{
			TxtC(GetMarkerName(Marker).ToString(), Screen.X, Screen.Y - 26 * S, Outer, 0.8f);
		}
	}
}

void AGZHUD::DrawChat()
{
	UGZRPGComponent* R = RPG();
	const float W = 520 * S, H = 220 * S;
	const float X = 12 * S, Y = Canvas->ClipY - H - 12 * S;
	Panel(X, Y, W, H, FLinearColor(0.02f, 0.03f, 0.05f, 0.72f));
	ChatRect = FBox2D(FVector2D(X, Y), FVector2D(X + W, Y + H));
	static const TCHAR* Tabs[] = {TEXT("All"), TEXT("System"), TEXT("Combat"), TEXT("NPC"), TEXT("Quest"), TEXT("Town")};
	for (int32 i = 0; i < 6; ++i)
	{
		Button(FString::Printf(TEXT("Chat_%d"), i), X + 6 * S + i * 70 * S, Y + 5 * S, 66 * S, 20 * S, Tabs[i], true, ChatFilter == i, 0.75f);
	}
	TArray<const FGZChatLine*> Lines;
	for (int32 i = R->Chat.Num() - 1; i >= 0 && Lines.Num() < 10; --i)
	{
		const FGZChatLine& L = R->Chat[i];
		if ((ChatFilter == 0 && (bSystemChat || L.Channel != TEXT("System"))) || (ChatFilter > 0 && L.Channel == Tabs[ChatFilter]))
		{
			Lines.Add(&L);
		}
	}
	float LY = Y + H - 24 * S;
	for (const FGZChatLine* L : Lines)
	{
		FString T = FString::Printf(TEXT("[%s] %s"), *L->Channel, *L->Text);
		// Crude wrap/clip to the panel width.
		if (TxtW(T, 0.78f) > W - 16 * S)
		{
			while (T.Len() > 4 && TxtW(T + TEXT("..."), 0.78f) > W - 16 * S)
			{
				T = T.LeftChop(3);
			}
			T += TEXT("...");
		}
		Txt(T, X + 8 * S, LY, L->Color, 0.78f);
		LY -= 18.5f * S;
	}
}

void AGZHUD::DrawQuickBar()
{
	UGZRPGComponent* R = RPG();
	const float Slot = 54 * S, Gap = 6 * S;
	const float W = 5 * Slot + 4 * Gap + 3 * (70 * S) + 40 * S;
	const float X = (Canvas->ClipX - W) * 0.5f, Y = Canvas->ClipY - Slot - 24 * S;
	Panel(X - 8 * S, Y - 8 * S, W + 16 * S, Slot + 28 * S, PanelBg);
	for (int32 i = 0; i < 5; ++i)
	{
		const float SX = X + i * (Slot + Gap);
		const FString Item = R->QuickSlots.IsValidIndex(i) ? R->QuickSlots[i] : FString();
		ItemSlot(Item, SX, Y, Slot, Item.IsEmpty() ? 1 : R->Count(Item), FString::Printf(TEXT("Quick_%d"), i));
		Txt(FString::Printf(TEXT("F%d"), i + 1), SX + 3 * S, Y + 2 * S, Gold, 0.7f);
		Button(FString::Printf(TEXT("Slot_%d"), i), SX, Y + Slot + 2 * S, Slot, 12 * S, TEXT(""), true, InventoryMode == 1 && AssignSlot == i, 0.5f);
	}
	const float AX = X + 5 * (Slot + Gap) + 30 * S;
	static const TCHAR* Acts[] = {TEXT("Attack\n[Space]"), TEXT("Gather\n[G]"), TEXT("Talk\n[E]")};
	static const TCHAR* ActIcons[] = {TEXT("skill_sword"), TEXT("skill_mining"), TEXT("npc_quest_giver")};
	for (int32 i = 0; i < 3; ++i)
	{
		const float BX = AX + i * 70 * S;
		DrawRect(Btn, BX, Y, 64 * S, Slot);
		Icon(ActIcons[i], BX + 14 * S, Y + 2 * S, 36 * S, FLinearColor(0.3f, 0.3f, 0.3f), TEXT(""));
		FString A, B;
		FString(Acts[i]).Split(TEXT("\n"), &A, &B);
		TxtC(A, BX + 32 * S, Y + 36 * S, FLinearColor::White, 0.65f);
		Frame(BX, Y, 64 * S, Slot, GoldDim, 1.f);
		Hit(FString::Printf(TEXT("Act_%d"), i), BX, Y, 64 * S, Slot);
	}
	if (InventoryMode == 1)
	{
		TxtC(FString::Printf(TEXT("Click an item in the Inventory to put it in F%d"), AssignSlot + 1), Canvas->ClipX * 0.5f, Y - 30 * S, Gold, 0.85f);
	}
}

void AGZHUD::DrawMenuBar()
{
	struct FEntry { EGZWindow W; const TCHAR* Label; const TCHAR* IconId; };
	static const FEntry Entries[] = {
		{EGZWindow::Character, TEXT("Char C"), TEXT("portrait_player")},
		{EGZWindow::Inventory, TEXT("Inven I"), TEXT("item_bag")},
		{EGZWindow::Skills, TEXT("Skill K"), TEXT("skill_blacksmithing")},
		{EGZWindow::Manufacture, TEXT("Make U"), TEXT("skill_alchemy")},
		{EGZWindow::Market, TEXT("Market B"), TEXT("skill_trading")},
		{EGZWindow::Town, TEXT("Town N"), TEXT("bld_town_hall")},
		{EGZWindow::Community, TEXT("Guild J"), TEXT("npc_guild_master")},
		{EGZWindow::GameMenu, TEXT("Menu F10"), TEXT("item_scroll")},
	};
	const int32 Num = UE_ARRAY_COUNT(Entries);
	const float BW = 62 * S, BH = 62 * S, Gap = 4 * S;
	const float W = Num * BW + (Num - 1) * Gap;
	const float X = Canvas->ClipX - W - 18 * S, Y = Canvas->ClipY - BH - 18 * S;
	Panel(X - 6 * S, Y - 6 * S, W + 12 * S, BH + 12 * S, PanelBg);
	for (int32 i = 0; i < Num; ++i)
	{
		const float BX = X + i * (BW + Gap);
		const bool bOpen = IsOpen(Entries[i].W);
		const bool bHover = Mouse.X >= BX && Mouse.X <= BX + BW && Mouse.Y >= Y && Mouse.Y <= Y + BH;
		DrawRect(bOpen ? BtnActive : (bHover ? BtnHover : Btn), BX, Y, BW, BH);
		Icon(Entries[i].IconId, BX + 13 * S, Y + 3 * S, 36 * S, FLinearColor(0.3f, 0.3f, 0.3f), TEXT(""));
		TxtC(Entries[i].Label, BX + BW * 0.5f, Y + 42 * S, FLinearColor(1.f, 0.93f, 0.8f), 0.65f);
		Frame(BX, Y, BW, BH, bHover || bOpen ? Gold : GoldDim, 1.f);
		Hit(FString::Printf(TEXT("Open_%d"), (int32)Entries[i].W), BX, Y, BW, BH);
	}
}

void AGZHUD::DrawPrompt()
{
	AGZIsoCharacter* P = Player();
	const FGZWorldObject* O = P ? P->GetInteractable() : nullptr;
	if (!O || !O->Actor.IsValid()) return;
	FString T;
	if (O->Kind == TEXT("npc")) T = FString::Printf(TEXT("[E] Talk to %s"), *O->Name);
	else T = FString::Printf(TEXT("[G] Gather %s  (%s)"), *UGZRPGComponent::Pretty(O->Arg1), *UGZRPGComponent::Pretty(O->Arg2));
	const FVector Screen = Project(O->Actor->GetActorLocation() + FVector(0, 0, 260));
	if (Screen.Z <= 0.f) return;
	const float W = TxtW(T, 0.9f) + 20 * S;
	DrawRect(FLinearColor(0.f, 0.f, 0.f, 0.75f), Screen.X - W * 0.5f, Screen.Y - 12 * S, W, 24 * S);
	Frame(Screen.X - W * 0.5f, Screen.Y - 12 * S, W, 24 * S, Gold, 1.f);
	TxtC(T, Screen.X, Screen.Y - 9 * S, FLinearColor(1.f, 0.95f, 0.7f), 0.9f);
}

void AGZHUD::DrawFloating(float Dt)
{
	for (int32 i = Floating.Num() - 1; i >= 0; --i)
	{
		FFloating& F = Floating[i];
		F.Age += Dt;
		if (F.Age > 1.4f)
		{
			Floating.RemoveAt(i);
			continue;
		}
		const FVector Screen = Project(F.World + FVector(0, 0, F.Age * 120.f));
		if (Screen.Z <= 0.f) continue;
		FLinearColor C = F.Color;
		C.A = FMath::Clamp(1.4f - F.Age, 0.f, 1.f);
		TxtC(F.Text, Screen.X, Screen.Y, C, 1.25f);
	}
}

// ------------------------------------------------------------------ windows

void AGZHUD::DrawCharacter()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Character, WindowTitle(EGZWindow::Character), 380, 500, X, Y);
	Icon(TEXT("portrait_player"), X, Y, 120 * S, FLinearColor(0.2f, 0.3f, 0.6f), TEXT("You"));
	Frame(X, Y, 120 * S, 120 * S, Gold, 1.f);
	Txt(R->CharacterName, X + 132 * S, Y, FLinearColor::White, 1.1f);
	Txt(FString::Printf(TEXT("Level %d   EXP %.0f / %.0f"), R->Level, R->Exp, R->ExpToNext), X + 132 * S, Y + 26 * S, TextDim, 0.8f);
	Txt(FString::Printf(TEXT("Attack  %d"), R->GetAttack()), X + 132 * S, Y + 50 * S, FLinearColor(1.f, 0.7f, 0.5f), 0.85f);
	Txt(FString::Printf(TEXT("Defense %d"), R->GetDefense()), X + 132 * S, Y + 70 * S, FLinearColor(0.6f, 0.8f, 1.f), 0.85f);
	Txt(FString::Printf(TEXT("HP %d   MP %d"), FMath::RoundToInt(R->MaxHP), FMath::RoundToInt(R->MaxMP)), X + 132 * S, Y + 90 * S, TextDim, 0.85f);

	float AY = Y + 134 * S;
	Txt(FString::Printf(TEXT("Attributes   (bonus points: %d)"), R->BonusPoints), X, AY, Gold, 0.9f);
	AY += 24 * S;
	for (int32 i = 0; i < R->AttributeNames.Num(); ++i)
	{
		DrawRect(PanelBg2, X, AY, 350 * S, 24 * S);
		Txt(R->AttributeNames[i], X + 8 * S, AY + 4 * S, FLinearColor::White, 0.85f);
		Txt(FString::FromInt(R->AttributeValues[i]), X + 200 * S, AY + 4 * S, FLinearColor(1.f, 0.9f, 0.6f), 0.85f);
		Button(FString::Printf(TEXT("Attr_%d"), i), X + 300 * S, AY + 2 * S, 40 * S, 20 * S, TEXT("+"), R->BonusPoints > 0, false, 0.85f);
		AY += 27 * S;
	}
	AY += 8 * S;
	Txt(TEXT("Equipment"), X, AY, Gold, 0.9f);
	AY += 24 * S;
	ItemSlot(R->EquippedWeapon, X, AY, 56 * S, 1, TEXT(""));
	Txt(R->EquippedWeapon.IsEmpty() ? TEXT("Weapon: (none)") : FString::Printf(TEXT("Weapon: %s"), *R->GetItem(R->EquippedWeapon).Name), X + 64 * S, AY + 18 * S, FLinearColor::White, 0.8f);
	ItemSlot(R->EquippedArmor, X + 180 * S, AY, 56 * S, 1, TEXT(""));
	Txt(R->EquippedArmor.IsEmpty() ? TEXT("Armor: (none)") : FString::Printf(TEXT("Armor: %s"), *R->GetItem(R->EquippedArmor).Name), X + 244 * S, AY + 18 * S, FLinearColor::White, 0.75f);
}

void AGZHUD::DrawInventory()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Inventory, WindowTitle(EGZWindow::Inventory), 384, 470, X, Y);
	const int32 Cols = 6;
	const float Slot = 56 * S, Gap = 4 * S;
	for (int32 i = 0; i < 36; ++i)
	{
		const float SX = X + (i % Cols) * (Slot + Gap);
		const float SY = Y + (i / Cols) * (Slot + Gap);
		if (R->InventoryOrder.IsValidIndex(i))
		{
			const FString& Item = R->InventoryOrder[i];
			ItemSlot(Item, SX, SY, Slot, R->Count(Item), FString::Printf(TEXT("Inv_%d"), i));
			if (Item == R->EquippedWeapon || Item == R->EquippedArmor)
			{
				Txt(TEXT("E"), SX + 4 * S, SY + 2 * S, Good, 0.75f);
			}
		}
		else
		{
			ItemSlot(TEXT(""), SX, SY, Slot, 0, TEXT(""));
		}
	}
	const float BY = Y + 6 * (Slot + Gap) + 4 * S;
	Txt(FString::Printf(TEXT("%lld %s"), R->Luma, *R->Currency), X, BY + 4 * S, FLinearColor(1.f, 0.85f, 0.35f), 0.95f);
	Button(TEXT("InvMode_0"), X + 200 * S, BY, 156 * S, 24 * S, InventoryMode == 1 ? FString::Printf(TEXT("Assigning to F%d..."), AssignSlot + 1) : TEXT("Assign to quick slot"), true, InventoryMode == 1, 0.75f);
	Txt(TEXT("Click: use / equip.  Hover for details."), X, BY + 30 * S, TextDim, 0.7f);
}

void AGZHUD::DrawSkills()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Skills, WindowTitle(EGZWindow::Skills), 430, 520, X, Y);
	Txt(TEXT("Skills grow by use. Past the basic cap they train slower."), X, Y, TextDim, 0.75f);
	float SY = Y + 24 * S;
	FString LastCat;
	for (const FGZSkillState& Sk : R->Skills)
	{
		if (Sk.Category != LastCat)
		{
			LastCat = Sk.Category;
			Txt(UGZRPGComponent::Pretty(Sk.Category), X, SY, Gold, 0.85f);
			SY += 20 * S;
		}
		DrawRect(PanelBg2, X, SY, 400 * S, 34 * S);
		Icon(TEXT("skill_") + Sk.Id, X + 2 * S, SY + 1 * S, 32 * S, FLinearColor(0.3f, 0.3f, 0.4f), Sk.Name.Left(2));
		Txt(Sk.Name, X + 40 * S, SY + 2 * S, FLinearColor::White, 0.82f);
		Txt(FString::Printf(TEXT("Lv %d  (basic %d / max %d)"), Sk.Level, Sk.BasicCap, Sk.Cap), X + 40 * S, SY + 18 * S, TextDim, 0.68f);
		Bar(X + 230 * S, SY + 9 * S, 160 * S, 16 * S, Sk.Xp / 100.f, Sk.Level >= Sk.BasicCap ? FLinearColor(0.7f, 0.4f, 1.f) : FLinearColor(0.3f, 0.8f, 0.4f), FString::Printf(TEXT("%.0f%%"), Sk.Xp));
		SY += 37 * S;
	}
}

void AGZHUD::DrawManufacture()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Manufacture, WindowTitle(EGZWindow::Manufacture), 680, 500, X, Y);
	const int32 Visible = 11;
	RecipeScroll = FMath::Clamp(RecipeScroll, 0, FMath::Max(0, R->Recipes.Num() - Visible));
	float LY = Y;
	for (int32 i = RecipeScroll; i < FMath::Min(R->Recipes.Num(), RecipeScroll + Visible); ++i)
	{
		const FGZRecipe& Rc = R->Recipes[i];
		const bool bOk = R->HasMaterials(Rc);
		const FString Label = FString::Printf(TEXT("%s%s"), Rc.bProcessing ? TEXT("[Process] ") : TEXT(""), *UGZRPGComponent::Pretty(Rc.Output));
		const bool bSel = i == SelectedRecipe;
		DrawRect(bSel ? BtnActive : PanelBg2, X, LY, 250 * S, 34 * S);
		Icon(Rc.Output, X + 2 * S, LY + 1 * S, 32 * S, R->GetItem(Rc.Output).Color, R->GetItem(Rc.Output).Name.Left(2));
		Txt(Label, X + 40 * S, LY + 8 * S, bOk ? Good : FLinearColor::White, 0.75f);
		Hit(FString::Printf(TEXT("Recipe_%d"), i), X, LY, 250 * S, 34 * S);
		LY += 36 * S;
	}
	Button(TEXT("Scroll_0"), X, LY + 2 * S, 120 * S, 20 * S, TEXT("Up"), RecipeScroll > 0, false, 0.7f);
	Button(TEXT("Scroll_1"), X + 130 * S, LY + 2 * S, 120 * S, 20 * S, TEXT("Down"), RecipeScroll + Visible < R->Recipes.Num(), false, 0.7f);

	if (!R->Recipes.IsValidIndex(SelectedRecipe)) return;
	const FGZRecipe& Rc = R->Recipes[SelectedRecipe];
	const float DX = X + 266 * S;
	float DY = Y;
	Icon(Rc.Output, DX, DY, 96 * S, R->GetItem(Rc.Output).Color, TEXT(""));
	Frame(DX, DY, 96 * S, 96 * S, Gold, 1.f);
	Txt(UGZRPGComponent::Pretty(Rc.Output), DX + 108 * S, DY, FLinearColor::White, 1.05f);
	const FGZItemDef& Out = R->GetItem(Rc.Output);
	Txt(FString::Printf(TEXT("Type: %s   Base price: %d %s"), *Out.Type, Out.BasePrice, *R->Currency), DX + 108 * S, DY + 26 * S, TextDim, 0.75f);
	Txt(FString::Printf(TEXT("Skill: %s Lv %d  (difficulty %d)"), *UGZRPGComponent::Pretty(Rc.Skill), R->SkillLevel(Rc.Skill), Rc.Difficulty), DX + 108 * S, DY + 46 * S, TextDim, 0.75f);
	Txt(FString::Printf(TEXT("Success chance: %.0f%%"), R->CraftChance(Rc) * 100.f), DX + 108 * S, DY + 66 * S, R->CraftChance(Rc) > 0.6f ? Good : FLinearColor(1.f, 0.8f, 0.4f), 0.85f);
	DY += 110 * S;
	Txt(TEXT("Materials"), DX, DY, Gold, 0.9f);
	DY += 22 * S;
	for (const auto& M : Rc.Materials)
	{
		const int32 Have = R->Count(M.Key);
		ItemSlot(M.Key, DX, DY, 40 * S, Have, TEXT(""));
		Txt(FString::Printf(TEXT("%s   %d / %d"), *UGZRPGComponent::Pretty(M.Key), Have, M.Value), DX + 48 * S, DY + 11 * S, Have >= M.Value ? Good : Bad, 0.82f);
		DY += 44 * S;
	}
	DY += 6 * S;
	const bool bCan = R->HasMaterials(Rc);
	Button(TEXT("Craft_0"), DX, DY, 180 * S, 34 * S, Rc.bProcessing ? TEXT("Process") : TEXT("Manufacture"), bCan, false, 0.95f);
	Button(TEXT("Craft5_0"), DX + 190 * S, DY, 120 * S, 34 * S, TEXT("x5"), bCan, false, 0.95f);
	Txt(TEXT("Gather -> Process -> Manufacture -> Trade"), DX, DY + 44 * S, TextDim, 0.72f);
}

void AGZHUD::DrawMarket()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Market, WindowTitle(EGZWindow::Market), 580, 560, X, Y);
	const FGZTownInfo* T = R->FindTown(R->CurrentZone);
	if (!T)
	{
		Txt(TEXT("There is no market in the wilderness."), X, Y + 10 * S, Bad, 0.95f);
		Txt(TEXT("Travel to Emberfall, Frostmere or Greenhollow to trade."), X, Y + 34 * S, TextDim, 0.85f);
		return;
	}
	Txt(FString::Printf(TEXT("%s Market   -   sales tax %.0f%%   -   %lld %s"), *T->Name, T->SalesTax * 100.f, R->Luma, *R->Currency), X, Y, Gold, 0.85f);
	Txt(R->PriceFormula, X, Y + 20 * S, TextDim, 0.62f);
	float LY = Y + 44 * S;
	Txt(TEXT("Item"), X + 44 * S, LY, TextDim, 0.75f);
	Txt(TEXT("Buy"), X + 250 * S, LY, TextDim, 0.75f);
	Txt(TEXT("Sell"), X + 330 * S, LY, TextDim, 0.75f);
	Txt(TEXT("Own"), X + 405 * S, LY, TextDim, 0.75f);
	LY += 20 * S;
	const int32 Visible = 10;
	MarketScroll = FMath::Clamp(MarketScroll, 0, FMath::Max(0, R->MarketGoods.Num() - Visible));
	for (int32 i = MarketScroll; i < FMath::Min(R->MarketGoods.Num(), MarketScroll + Visible); ++i)
	{
		const FString& G = R->MarketGoods[i];
		DrawRect(PanelBg2, X, LY, 550 * S, 38 * S);
		ItemSlot(G, X + 2 * S, LY + 2 * S, 34 * S, 1, TEXT(""));
		Txt(R->GetItem(G).Name, X + 44 * S, LY + 10 * S, FLinearColor::White, 0.8f);
		const int32 BuyP = R->GetBuyPrice(G), SellP = R->GetSellPrice(G);
		const int32 Base = R->GetItem(G).BasePrice;
		Txt(FString::FromInt(BuyP), X + 250 * S, LY + 10 * S, BuyP > Base * 1.1f ? Bad : Good, 0.8f);
		Txt(FString::FromInt(SellP), X + 330 * S, LY + 10 * S, FLinearColor(1.f, 0.85f, 0.35f), 0.8f);
		Txt(FString::FromInt(R->Count(G)), X + 405 * S, LY + 10 * S, FLinearColor::White, 0.8f);
		Button(FString::Printf(TEXT("Buy_%d"), i), X + 440 * S, LY + 7 * S, 50 * S, 24 * S, TEXT("Buy"), R->Luma >= BuyP, false, 0.75f);
		Button(FString::Printf(TEXT("Sell_%d"), i), X + 494 * S, LY + 7 * S, 50 * S, 24 * S, TEXT("Sell"), R->Count(G) > 0, false, 0.75f);
		LY += 40 * S;
	}
	Button(TEXT("MScroll_0"), X, LY + 4 * S, 120 * S, 20 * S, TEXT("Up"), MarketScroll > 0, false, 0.7f);
	Button(TEXT("MScroll_1"), X + 130 * S, LY + 4 * S, 120 * S, 20 * S, TEXT("Down"), MarketScroll + Visible < R->MarketGoods.Num(), false, 0.7f);
	Txt(TEXT("Buying raises demand, selling raises supply. Tax goes to the town treasury."), X, LY + 30 * S, TextDim, 0.68f);
}

void AGZHUD::DrawTown()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Town, WindowTitle(EGZWindow::Town), 540, 520, X, Y);
	const FGZTownInfo* T = R->FindTown(R->CurrentZone);
	if (!T)
	{
		Txt(TEXT("Visit a town hall to see its government."), X, Y + 10 * S, Bad, 0.95f);
		float LY = Y + 44 * S;
		Txt(TEXT("Towns of Luminara"), X, LY, Gold, 0.9f);
		LY += 24 * S;
		for (const FGZTownInfo& Tw : R->Towns)
		{
			Txt(FString::Printf(TEXT("%s  (%s)  tax %.0f%%  treasury %lld"), *Tw.Name, *Tw.Biome, Tw.SalesTax * 100.f, Tw.Treasury), X, LY, FLinearColor::White, 0.8f);
			LY += 22 * S;
		}
		return;
	}
	Icon(TEXT("bld_town_hall"), X, Y, 110 * S, FLinearColor(0.4f, 0.3f, 0.2f), TEXT("Hall"));
	Frame(X, Y, 110 * S, 110 * S, Gold, 1.f);
	Txt(T->Name, X + 122 * S, Y, FLinearColor::White, 1.2f);
	Txt(FString::Printf(TEXT("Biome: %s"), *UGZRPGComponent::Pretty(T->Biome)), X + 122 * S, Y + 30 * S, TextDim, 0.8f);
	Txt(FString::Printf(TEXT("Treasury: %lld %s"), T->Treasury, *R->Currency), X + 122 * S, Y + 50 * S, FLinearColor(1.f, 0.85f, 0.35f), 0.85f);
	Txt(FString::Printf(TEXT("Sales tax: %.0f%%"), T->SalesTax * 100.f), X + 122 * S, Y + 70 * S, TextDim, 0.8f);
	Txt(FString::Printf(TEXT("Town Head: %s"), *T->Head), X + 122 * S, Y + 90 * S, Gold, 0.85f);

	float LY = Y + 124 * S;
	Txt(FString::Printf(TEXT("Your shares: %d"), T->PlayerShares), X, LY, FLinearColor::White, 0.9f);
	Button(TEXT("Share_0"), X + 200 * S, LY - 4 * S, 200 * S, 26 * S, TEXT("Buy share (500 Luma)"), R->Luma >= 500, false, 0.8f);
	LY += 36 * S;
	Txt(TEXT("Town Head Election  (shareholders vote)"), X, LY, Gold, 0.9f);
	LY += 24 * S;
	for (int32 i = 0; i < T->Candidates.Num(); ++i)
	{
		DrawRect(PanelBg2, X, LY, 500 * S, 30 * S);
		Icon(FString::Printf(TEXT("npc_candidate_%d"), i % 3), X + 2 * S, LY + 1 * S, 28 * S, FLinearColor(0.3f, 0.3f, 0.5f), TEXT(""));
		Txt(T->Candidates[i], X + 38 * S, LY + 6 * S, FLinearColor::White, 0.82f);
		Button(FString::Printf(TEXT("Vote_%d"), i), X + 400 * S, LY + 3 * S, 90 * S, 24 * S, T->VotedCandidate == i ? TEXT("Voted") : TEXT("Vote"), T->PlayerShares > 0, T->VotedCandidate == i, 0.75f);
		LY += 33 * S;
	}
	LY += 10 * S;
	Txt(TEXT("Offices"), X, LY, Gold, 0.9f);
	LY += 22 * S;
	for (const FString& O : R->OfficeLines)
	{
		Txt(FString::Printf(TEXT("- %s"), *O), X, LY, TextDim, 0.78f);
		LY += 20 * S;
	}
}

void AGZHUD::DrawQuests()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Quests, WindowTitle(EGZWindow::Quests), 500, 520, X, Y);
	float LY = Y;
	for (const FGZQuest& Q : R->Quests)
	{
		DrawRect(PanelBg2, X, LY, 470 * S, 54 * S);
		Txt(Q.Title, X + 8 * S, LY + 4 * S, Q.bDone ? Good : FLinearColor(1.f, 0.9f, 0.6f), 0.85f);
		Txt(Q.Desc, X + 8 * S, LY + 22 * S, TextDim, 0.68f);
		const FString P = Q.bDone ? TEXT("Complete") : FString::Printf(TEXT("%d / %d"), Q.Progress, Q.Goal);
		Txt(P, X + 462 * S - TxtW(P, 0.75f), LY + 4 * S, Q.bDone ? Good : FLinearColor::White, 0.75f);
		Txt(FString::Printf(TEXT("Reward: %d Luma, %d EXP"), Q.RewardLuma, Q.RewardExp), X + 8 * S, LY + 37 * S, FLinearColor(1.f, 0.85f, 0.35f), 0.62f);
		LY += 57 * S;
	}
}

void AGZHUD::DrawWorldMap()
{
	float X, Y;
	BeginWindow(EGZWindow::WorldMap, WindowTitle(EGZWindow::WorldMap), 760, 660, X, Y);
	DrawMapContents(X, Y, 736 * S, 580 * S, 21000.f, true);
	Txt(TEXT("Gold: towns   Yellow: NPCs   Red: monsters   Green: resources"), X + 6 * S, Y + 584 * S, TextDim, 0.72f);
}

void AGZHUD::DrawCommunity()
{
	UGZRPGComponent* R = RPG();
	float X, Y;
	BeginWindow(EGZWindow::Community, WindowTitle(EGZWindow::Community), 440, 430, X, Y);
	Txt(TEXT("Guilds"), X, Y, Gold, 0.9f);
	static const TCHAR* Guilds[][3] = {
		{TEXT("Ember Wardens"), TEXT("Lv 7"), TEXT("Hunting / PvE")},
		{TEXT("Frostmere Forge Union"), TEXT("Lv 5"), TEXT("Blacksmithing")},
		{TEXT("Greenhollow Traders"), TEXT("Lv 6"), TEXT("Trading / Farming")},
	};
	float LY = Y + 24 * S;
	for (auto& G : Guilds)
	{
		DrawRect(PanelBg2, X, LY, 410 * S, 30 * S);
		Txt(G[0], X + 8 * S, LY + 6 * S, FLinearColor::White, 0.8f);
		Txt(G[1], X + 230 * S, LY + 6 * S, Gold, 0.75f);
		Txt(G[2], X + 290 * S, LY + 6 * S, TextDim, 0.7f);
		LY += 33 * S;
	}
	LY += 10 * S;
	Txt(TEXT("Season Hall"), X, LY, Gold, 0.9f);
	LY += 24 * S;
	Txt(FString::Printf(TEXT("Your rank: Lv.%d %s  -  quests done: %d / %d"), R->Level, *R->CharacterName,
		R->Quests.FilterByPredicate([](const FGZQuest& Q) { return Q.bDone; }).Num(), R->Quests.Num()), X, LY, FLinearColor::White, 0.8f);
	LY += 30 * S;
	Txt(TEXT("Server Ruler (elected office)"), X, LY, Gold, 0.9f);
	LY += 24 * S;
	Txt(TEXT("Current ruler: Lord Hwan of Emberfall"), X, LY, FLinearColor::White, 0.8f);
	LY += 20 * S;
	Txt(TEXT("Elections run every term; town heads may stand."), X, LY, TextDim, 0.72f);
}

void AGZHUD::DrawHelp()
{
	float X, Y;
	BeginWindow(EGZWindow::Help, WindowTitle(EGZWindow::Help), 500, 470, X, Y);
	static const TCHAR* Lines[] = {
		TEXT("Move: WASD / arrows, or left-click the ground"),
		TEXT("Camera: Q / E rotate, mouse wheel zoom"),
		TEXT("Attack: Space (Tab cycles targets, click a monster)"),
		TEXT("Gather: G beside ore, oak, herbs, fields, fish"),
		TEXT("Talk: E beside an NPC (opens their service)"),
		TEXT("Quick items: F1 - F5    8-bit render mode: P"),
		TEXT(""),
		TEXT("C Character   I Inventory   K Skills"),
		TEXT("U Manufacture / Game Info   B Market"),
		TEXT("N Town & Election   L Quests   M World Map"),
		TEXT("J Community   H Help   F10 Menu"),
		TEXT(""),
		TEXT("Loop: gather -> process -> manufacture -> trade."),
		TEXT("Monsters drop materials; equipment is player-made."),
		TEXT("Buy town shares to vote for the Town Head."),
	};
	float LY = Y;
	for (const TCHAR* L : Lines)
	{
		Txt(L, X, LY, FLinearColor::White, 0.82f);
		LY += 24 * S;
	}
}

void AGZHUD::DrawGameMenu()
{
	float X, Y;
	BeginWindow(EGZWindow::GameMenu, WindowTitle(EGZWindow::GameMenu), 470, 480, X, Y);

	const TArray<FGZMenuEntry>& Entries = GetGZMenuRegistry();
	TArray<const FGZMenuEntry*> Groups;
	for (const FGZMenuEntry& Entry : Entries)
	{
		if (Entry.ParentId.IsNone() && Entry.Action == EGZMenuAction::ExpandGroup)
		{
			Groups.Add(&Entry);
		}
	}

	const float GroupWidth = 210 * S;
	const float GroupHeight = 31 * S;
	for (int32 Index = 0; Index < Groups.Num(); ++Index)
	{
		const FGZMenuEntry& Group = *Groups[Index];
		const int32 Column = Index % 2;
		const int32 Row = Index / 2;
		const float BX = X + Column * (GroupWidth + 8 * S);
		const float BY = Y + 4 * S + Row * (GroupHeight + 4 * S);
		Button(FString::Printf(TEXT("MenuGroup_%s"), *Group.Id.ToString()), BX, BY, GroupWidth, GroupHeight,
			Group.Label.ToString(), true, ExpandedMenuGroup == Group.Id, 0.82f);
	}

	const float SectionY = Y + 112 * S;
	const float SectionHeight = 300 * S;
	Panel(X, SectionY, 436 * S, SectionHeight, PanelBg2);
	const FGZMenuEntry* ActiveGroup = Entries.FindByPredicate([this](const FGZMenuEntry& Entry)
	{
		return Entry.Id == ExpandedMenuGroup && Entry.Action == EGZMenuAction::ExpandGroup;
	});

	if (!ActiveGroup)
	{
		Txt(TEXT("Choose a section above to browse its menus."), X + 14 * S, SectionY + 16 * S, TextDim, 0.82f);
		return;
	}

	Txt(ActiveGroup->Label.ToString(), X + 14 * S, SectionY + 10 * S, Gold, 0.9f);
	Txt(ActiveGroup->Description.ToString(), X + 14 * S, SectionY + 34 * S, TextDim, 0.72f);

	int32 ItemIndex = 0;
	for (const FGZMenuEntry& Entry : Entries)
	{
		if (Entry.ParentId != ActiveGroup->Id) continue;

		const float ItemY = SectionY + (68 + ItemIndex * 63) * S;
		Button(FString::Printf(TEXT("MenuEntry_%s"), *Entry.Id.ToString()), X + 12 * S, ItemY, 410 * S, 34 * S,
			Entry.Label.ToString(), true, false, 0.82f);
		Txt(Entry.Description.ToString(), X + 18 * S, ItemY + 37 * S, TextDim, 0.66f);
		++ItemIndex;
	}
}
