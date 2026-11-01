#include "UI/EVUIDefaultContent.h"
#include "UI/EVUISubsystem.h"
#include "UI/EVUIStyle.h"
#include "UI/SEVOptions.h"
#include "UI/SEVPanels.h"
#include "UI/SEVRoot.h"
#include "GZ/GZHUD.h"
#include "GZ/GZIsoCharacter.h"
#include "GZ/GZRPGComponent.h"
#include "GZ/GZAudioSubsystem.h"

#include "AudioDevice.h"
#include "Engine/Engine.h"
#include "Engine/GameInstance.h"
#include "Engine/World.h"
#include "GameFramework/GameUserSettings.h"
#include "GameFramework/PlayerController.h"
#include "HAL/IConsoleManager.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Misc/App.h"
#include "Misc/ConfigCacheIni.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "Widgets/SBoxPanel.h"
#include "Widgets/Input/SButton.h"
#include "Widgets/Input/SMultiLineEditableTextBox.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Layout/SScrollBox.h"
#include "Widgets/Text/STextBlock.h"

#define LOCTEXT_NAMESPACE "EVUIDefaults"

namespace
{
	using FUI = UEVUISubsystem;

	AGZHUD* HUDOf(const FUI* UI)
	{
		APlayerController* PC = UI ? UI->GetPlayerController() : nullptr;
		return PC ? Cast<AGZHUD>(PC->GetHUD()) : nullptr;
	}

	AGZIsoCharacter* PlayerOf(const FUI* UI)
	{
		APlayerController* PC = UI ? UI->GetPlayerController() : nullptr;
		return PC ? Cast<AGZIsoCharacter>(PC->GetPawn()) : nullptr;
	}

	UGZRPGComponent* RPGOf(const FUI* UI)
	{
		AGZIsoCharacter* P = PlayerOf(UI);
		return P ? P->GetRPG() : nullptr;
	}

	UGZAudioSubsystem* AudioOf(const FUI* UI)
	{
		APlayerController* PC = UI ? UI->GetPlayerController() : nullptr;
		UGameInstance* GI = PC ? PC->GetGameInstance() : nullptr;
		return GI ? GI->GetSubsystem<UGZAudioSubsystem>() : nullptr;
	}

	UGameUserSettings* GameSettings()
	{
		return GEngine ? GEngine->GetGameUserSettings() : nullptr;
	}

	FName OptionsAction(FName CategoryId)
	{
		return FName(*(TEXT("ui.options.") + CategoryId.ToString()));
	}

	// ------------------------------------------------------------------ game windows (drawn by AGZHUD)

	struct FGameWindow
	{
		const TCHAR* Action;
		EGZWindow Window;
		FText Label;
		FText Tooltip;
		const TCHAR* Icon;
		FText Glyph;
		FKey Key;
		const TCHAR* ContextMenu;
	};

	TArray<FGameWindow> GameWindows()
	{
		return {
			{TEXT("window.character"), EGZWindow::Character, LOCTEXT("WinChar", "Character Info"), LOCTEXT("WinCharTip", "Attributes, equipment and bonus points."), TEXT("portrait_player"), FText(), EKeys::C, TEXT("unit.self")},
			{TEXT("window.inventory"), EGZWindow::Inventory, LOCTEXT("WinInv", "Inventory"), LOCTEXT("WinInvTip", "Use items, equip gear and assign quick slots."), TEXT("bag"), FText(), EKeys::I, nullptr},
			{TEXT("window.skills"), EGZWindow::Skills, LOCTEXT("WinSkills", "Skills"), LOCTEXT("WinSkillsTip", "Gathering, crafting and combat skill levels."), TEXT("skill_sword"), FText(), EKeys::K, nullptr},
			{TEXT("window.manufacture"), EGZWindow::Manufacture, LOCTEXT("WinMake", "Manufacture"), LOCTEXT("WinMakeTip", "Turn materials into goods."), TEXT("tool_anvil"), FText(), EKeys::U, nullptr},
			{TEXT("window.market"), EGZWindow::Market, LOCTEXT("WinMarket", "Market"), LOCTEXT("WinMarketTip", "Buy and sell goods."), TEXT("tool_coins"), FText(), EKeys::B, nullptr},
			{TEXT("window.quests"), EGZWindow::Quests, LOCTEXT("WinQuests", "Quest Log"), LOCTEXT("WinQuestsTip", "Active and completed quests."), TEXT("scroll"), FText(), EKeys::L, nullptr},
			{TEXT("window.worldmap"), EGZWindow::WorldMap, LOCTEXT("WinMap", "World Map"), LOCTEXT("WinMapTip", "Towns, regions and points of interest."), TEXT("worldmap"), FText(), EKeys::M, TEXT("minimap")},
			{TEXT("window.town"), EGZWindow::Town, LOCTEXT("WinTown", "Town Hall"), LOCTEXT("WinTownTip", "Town shares, treasury and elections."), TEXT("bld_town_hall"), FText(), EKeys::N, nullptr},
			{TEXT("window.community"), EGZWindow::Community, LOCTEXT("WinGuild", "Community & Guilds"), LOCTEXT("WinGuildTip", "Guilds and townsfolk."), TEXT("npc_guild_master"), FText(), EKeys::J, nullptr},
			{TEXT("window.help"), EGZWindow::Help, LOCTEXT("WinHelp", "How to Play"), LOCTEXT("WinHelpTip", "Controls reference."), nullptr, INVTEXT("?"), EKeys::H, nullptr},
		};
	}

	// ------------------------------------------------------------------ setting helpers

	FEVSettingDefinition MakeSetting(FName Key, FName Category, const FText& Section, const FText& Label, const FText& Tooltip)
	{
		FEVSettingDefinition S;
		S.Key = Key;
		S.CategoryId = Category;
		S.Section = Section;
		S.Label = Label;
		S.Tooltip = Tooltip;
		return S;
	}

	void AddCheck(FUI& UI, FName Key, FName Category, const FText& Section, const FText& Label, const FText& Tooltip, bool bDefault)
	{
		FEVSettingDefinition S = MakeSetting(Key, Category, Section, Label, Tooltip);
		S.Type = EEVSettingType::Checkbox;
		S.DefaultValue = bDefault ? TEXT("1") : TEXT("0");
		UI.RegisterSetting(S);
	}

	void AddSlider(FUI& UI, FName Key, FName Category, const FText& Section, const FText& Label, const FText& Tooltip, float Min, float Max, float Step, bool bPercent, float Default)
	{
		FEVSettingDefinition S = MakeSetting(Key, Category, Section, Label, Tooltip);
		S.Type = EEVSettingType::Slider;
		S.Min = Min;
		S.Max = Max;
		S.Step = Step;
		S.bPercent = bPercent;
		S.DefaultValue = FString::SanitizeFloat(Default);
		UI.RegisterSetting(S);
	}

	void AddChoice(FUI& UI, FName Key, FName Category, const FText& Section, const FText& Label, const FText& Tooltip, const TArray<FText>& Choices, int32 Default)
	{
		FEVSettingDefinition S = MakeSetting(Key, Category, Section, Label, Tooltip);
		S.Type = EEVSettingType::Choice;
		S.Choices = Choices;
		S.DefaultValue = FString::FromInt(Default);
		UI.RegisterSetting(S);
	}

	TArray<FText> QualityLevels()
	{
		return {LOCTEXT("QLow", "Low"), LOCTEXT("QMed", "Medium"), LOCTEXT("QHigh", "High"), LOCTEXT("QEpic", "Epic"), LOCTEXT("QCine", "Cinematic")};
	}

	void ApplyVolume(FUI* UI)
	{
		APlayerController* PC = UI->GetPlayerController();
		UWorld* World = PC ? PC->GetWorld() : nullptr;
		if (!World) return;
		FAudioDeviceHandle Device = World->GetAudioDevice();
		if (Device.IsValid())
		{
			const float Master = UI->GetSettingBool(TEXT("audio.enable")) ? FMath::Clamp(UI->GetSettingFloat(TEXT("audio.master")), 0.f, 1.f) : 0.f;
			Device->SetTransientPrimaryVolume(Master);
		}
	}

	// ------------------------------------------------------------------ registration steps

	void RegisterActions(FUI& UI)
	{
		FUI* Self = &UI;

		for (const FGameWindow& W : GameWindows())
		{
			const EGZWindow Window = W.Window;
			UI.RegisterAction(W.Action,
				[Self, Window]() { if (AGZHUD* H = HUDOf(Self)) H->ToggleWindow(Window); },
				[Self, Window]() { AGZHUD* H = HUDOf(Self); return H && H->IsOpen(Window); });
		}

		UI.RegisterAction(TEXT("game.resume"), [Self]() { Self->ClosePanel(TEXT("game_menu")); });
		UI.RegisterAction(TEXT("ui.options"), [Self]() { Self->OpenOptions(NAME_None); }, [Self]() { return Self->IsPanelOpen(TEXT("options")); });
		UI.RegisterAction(TEXT("ui.toggle_interface"), [Self]() { Self->SetInterfaceVisible(!Self->IsInterfaceVisible()); }, [Self]() { return !Self->IsInterfaceVisible(); });
		UI.RegisterAction(TEXT("ui.close_all"), [Self]()
		{
			Self->CloseAllPanels();
			if (AGZHUD* H = HUDOf(Self)) H->CloseAllWindows();
		});
		UI.RegisterAction(TEXT("ui.reset_positions"), [Self]()
		{
			Self->ResetPanelPositions();
			Self->ShowMessage(LOCTEXT("PosReset", "Window positions reset"), EVUIStyle::Good);
		});
		UI.RegisterAction(TEXT("ui.screenshot"), [Self]()
		{
			if (APlayerController* PC = Self->GetPlayerController())
			{
				PC->ConsoleCommand(TEXT("shot"));
				Self->ShowMessage(LOCTEXT("ShotSaved", "Screenshot saved to Saved/Screenshots"), EVUIStyle::Good);
			}
		});
		UI.RegisterAction(TEXT("game.logout"), [Self]()
		{
			Self->ShowConfirmationNative(LOCTEXT("LogoutTitle", "Log Out"),
				LOCTEXT("LogoutMsg", "Leave the world and return to the start? Progress that has not been saved will be lost."),
				[Self]()
				{
					APlayerController* PC = Self->GetPlayerController();
					if (!PC) return;
					FString Map;
					if (GConfig && GConfig->GetString(TEXT("/Script/EtherealVeni.EVUI"), TEXT("LogoutMap"), Map, GGameIni) && !Map.IsEmpty())
					{
						UGameplayStatics::OpenLevel(PC, FName(*Map));
					}
					else
					{
						PC->ConsoleCommand(TEXT("RestartLevel"));
					}
				}, LOCTEXT("LogoutBtn", "Log Out"));
		});
		UI.RegisterAction(TEXT("game.exit"), [Self]()
		{
			Self->ShowConfirmationNative(LOCTEXT("ExitTitle", "Exit Game"), LOCTEXT("ExitMsg", "Are you sure you want to exit the game?"),
				[Self]()
				{
					APlayerController* PC = Self->GetPlayerController();
					UKismetSystemLibrary::QuitGame(PC, PC, EQuitPreference::Quit, false);
				}, LOCTEXT("ExitBtn", "Exit Game"));
		});

		// Target
		auto HasTarget = [Self]()
		{
			AGZIsoCharacter* P = PlayerOf(Self);
			const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
			return T && T->Actor.IsValid() && !T->bDead;
		};
		UI.RegisterAction(TEXT("target.attack"), [Self]() { if (AGZIsoCharacter* P = PlayerOf(Self)) P->AttackTarget(); }, nullptr, HasTarget);
		UI.RegisterAction(TEXT("target.clear"), [Self]() { if (AGZIsoCharacter* P = PlayerOf(Self)) P->ClearTarget(); }, nullptr, HasTarget);
		UI.RegisterAction(TEXT("target.inspect"), [Self]()
		{
			AGZIsoCharacter* P = PlayerOf(Self);
			UGZRPGComponent* R = RPGOf(Self);
			const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
			if (T && R)
			{
				R->AddChat(TEXT("System"), FString::Printf(TEXT("%s - Level %d - HP %d / %d"), *T->Name, T->Level,
					FMath::RoundToInt(FMath::Max(0.f, T->HP)), FMath::RoundToInt(T->MaxHP)), FLinearColor(1.f, 0.8f, 0.6f));
			}
		}, nullptr, HasTarget);
		UI.RegisterAction(TEXT("target.menu"), [Self]()
		{
			AGZIsoCharacter* P = PlayerOf(Self);
			const FGZWorldObject* T = P ? P->GetTarget() : nullptr;
			if (T) Self->OpenContextMenu(TEXT("unit.target"), T->Name);
		}, nullptr, HasTarget);
		for (int32 i = 0; i <= AGZHUD::NumTargetMarkers; ++i)
		{
			UI.RegisterAction(FName(*FString::Printf(TEXT("target.marker.%d"), i)),
				[Self, i]() { if (AGZHUD* H = HUDOf(Self)) H->SetTargetMarker(i); },
				[Self, i]() { AGZHUD* H = HUDOf(Self); return H && H->GetTargetMarker() == i; },
				HasTarget);
		}

		// Chat
		for (int32 i = 0; i < AGZHUD::NumChatFilters; ++i)
		{
			UI.RegisterAction(FName(*FString::Printf(TEXT("chat.filter.%d"), i)),
				[Self, i]() { if (AGZHUD* H = HUDOf(Self)) H->SetChatFilter(i); },
				[Self, i]() { AGZHUD* H = HUDOf(Self); return H && H->GetChatFilter() == i; });
		}
		UI.RegisterAction(TEXT("chat.clear"), [Self]() { if (UGZRPGComponent* R = RPGOf(Self)) R->Chat.Reset(); });
		UI.RegisterAction(TEXT("chat.hide"), [Self]() { Self->SetSetting(TEXT("interface.show_chat"), TEXT("0")); });

		// Escape closes the HUD's own windows before the Game Menu opens.
		UI.RegisterEscapeHandler([Self]()
		{
			AGZHUD* H = HUDOf(Self);
			return H && H->CloseTopmostWindow();
		});
	}

	void RegisterOptions(FUI& UI)
	{
		FUI* Self = &UI;
		auto Cat = [&UI](FName Id, FName Parent, const FText& Label, const FText& Desc, FName Page = NAME_None)
		{
			FEVOptionsCategory C;
			C.Id = Id;
			C.ParentId = Parent;
			C.Label = Label;
			C.Description = Desc;
			C.Page = Page;
			UI.RegisterOptionsCategory(C);
		};

		Cat(TEXT("gameplay"), NAME_None, LOCTEXT("CatGameplay", "Gameplay"), LOCTEXT("CatGameplayD", "How the game plays and how the interface behaves."));
		Cat(TEXT("gameplay.controls"), TEXT("gameplay"), LOCTEXT("CatControls", "Controls"), LOCTEXT("CatControlsD", "Movement, targeting and camera."));
		Cat(TEXT("gameplay.interface"), TEXT("gameplay"), LOCTEXT("CatInterface", "Interface"), LOCTEXT("CatInterfaceD", "Scale, visible frames, minimap and tooltips."));
		Cat(TEXT("gameplay.combat"), TEXT("gameplay"), LOCTEXT("CatCombat", "Combat"), LOCTEXT("CatCombatD", "Combat feedback."));
		Cat(TEXT("gameplay.social"), TEXT("gameplay"), LOCTEXT("CatSocial", "Social"), LOCTEXT("CatSocialD", "Status and chat."));
		Cat(TEXT("gameplay.accessibility"), TEXT("gameplay"), LOCTEXT("CatAccess", "Accessibility"), LOCTEXT("CatAccessD", "Readability of the HUD."));
		Cat(TEXT("graphics"), NAME_None, LOCTEXT("CatGraphics", "Graphics"), LOCTEXT("CatGraphicsD", "Display mode and rendering quality."));
		Cat(TEXT("graphics.display"), TEXT("graphics"), LOCTEXT("CatDisplay", "Display"), LOCTEXT("CatDisplayD", "Window mode, resolution, frame rate and brightness."));
		Cat(TEXT("graphics.quality"), TEXT("graphics"), LOCTEXT("CatQuality", "Quality"), LOCTEXT("CatQualityD", "A preset, or each rendering feature individually."));
		Cat(TEXT("audio"), NAME_None, LOCTEXT("CatAudio", "Audio"), LOCTEXT("CatAudioD", "Volume levels."));
		Cat(TEXT("audio.volume"), TEXT("audio"), LOCTEXT("CatVolume", "Volume"), LOCTEXT("CatVolumeD", "Master, music and sound effects."));
		Cat(TEXT("keybindings"), NAME_None, LOCTEXT("CatKeys", "Key Bindings"), LOCTEXT("CatKeysD", "Rebind interface commands."), TEXT("KeyBindings"));

		// One "open options on this page" action per category, used by menus.
		for (const FEVOptionsCategory& C : UI.GetOptionsCategories())
		{
			const FName Id = C.Id;
			UI.RegisterAction(OptionsAction(Id), [Self, Id]() { Self->OpenOptions(Id); });
		}

		const FText NoSection;

		// ---- Controls
		const FName Controls(TEXT("gameplay.controls"));
		AddCheck(UI, TEXT("controls.click_to_move"), Controls, NoSection, LOCTEXT("ClickMove", "Click to Move"), LOCTEXT("ClickMoveT", "Left-click the ground to walk there."), true);
		AddCheck(UI, TEXT("controls.auto_target"), Controls, NoSection, LOCTEXT("AutoTarget", "Auto-Target Nearest Enemy"), LOCTEXT("AutoTargetT", "Select the closest monster when you have no target."), true);
		AddChoice(UI, TEXT("controls.camera_step"), Controls, NoSection, LOCTEXT("CamStep", "Camera Rotation Step (Q / E)"), LOCTEXT("CamStepT", "How far the camera turns per key press."),
			{INVTEXT("15°"), INVTEXT("30°"), INVTEXT("45°"), INVTEXT("90°")}, 2);

		// ---- Interface
		const FName Interface(TEXT("gameplay.interface"));
		const FText Display = LOCTEXT("SecDisplay", "Display");
		AddSlider(UI, TEXT("ui.scale"), Interface, Display, LOCTEXT("UIScale", "Menu & Window Scale"), LOCTEXT("UIScaleT", "Size of menus, windows and dialogs."), 0.7f, 1.5f, 0.05f, true, 1.f);
		AddSlider(UI, TEXT("interface.hud_scale"), Interface, Display, LOCTEXT("HUDScale", "HUD Scale"), LOCTEXT("HUDScaleT", "Size of the status, chat, quick bar and minimap frames."), 0.75f, 1.5f, 0.05f, true, 1.f);
		AddCheck(UI, TEXT("interface.show_micromenu"), Interface, Display, LOCTEXT("ShowMicro", "Show Menu Bar"), LOCTEXT("ShowMicroT", "The button row in the bottom-right corner."), true);
		AddCheck(UI, TEXT("interface.show_clock"), Interface, Display, LOCTEXT("ShowClock", "Show Zone & Clock Banner"), FText(), true);
		AddCheck(UI, TEXT("interface.show_target"), Interface, Display, LOCTEXT("ShowTarget", "Show Target Frame"), FText(), true);
		AddCheck(UI, TEXT("interface.show_chat"), Interface, Display, LOCTEXT("ShowChat", "Show Chat Window"), FText(), true);
		const FText Minimap = LOCTEXT("SecMinimap", "Minimap");
		AddCheck(UI, TEXT("interface.show_minimap"), Interface, Minimap, LOCTEXT("ShowMinimap", "Show Minimap"), FText(), true);
		AddCheck(UI, TEXT("minimap.track_npcs"), Interface, Minimap, LOCTEXT("TrackNpc", "Track Townsfolk & Merchants"), FText(), true);
		AddCheck(UI, TEXT("minimap.track_monsters"), Interface, Minimap, LOCTEXT("TrackMon", "Track Monsters"), FText(), true);
		AddCheck(UI, TEXT("minimap.track_resources"), Interface, Minimap, LOCTEXT("TrackRes", "Track Gathering Nodes"), FText(), true);
		AddChoice(UI, TEXT("minimap.zoom"), Interface, Minimap, LOCTEXT("MapZoom", "Minimap Zoom"), FText(), {LOCTEXT("ZNear", "Near"), LOCTEXT("ZNormal", "Normal"), LOCTEXT("ZFar", "Far")}, 1);
		const FText Tips = LOCTEXT("SecTips", "Tooltips");
		AddCheck(UI, TEXT("interface.tooltips"), Interface, Tips, LOCTEXT("ShowTips", "Show Tooltips"), LOCTEXT("ShowTipsT", "Item and button tooltips."), true);

		// ---- Combat
		const FName Combat(TEXT("gameplay.combat"));
		AddCheck(UI, TEXT("combat.floating_text"), Combat, NoSection, LOCTEXT("FloatText", "Floating Combat Text"), LOCTEXT("FloatTextT", "Damage, healing and loot numbers above characters."), true);
		AddCheck(UI, TEXT("combat.world_marker"), Combat, NoSection, LOCTEXT("WorldMarker", "Show Marker Above Target"), FText(), true);

		// ---- Social
		const FName Social(TEXT("gameplay.social"));
		AddChoice(UI, TEXT("social.status"), Social, NoSection, LOCTEXT("Status", "Status"), LOCTEXT("StatusT", "Shown next to your name."),
			{LOCTEXT("StOnline", "Online"), LOCTEXT("StAway", "Away"), LOCTEXT("StBusy", "Busy")}, 0);
		AddCheck(UI, TEXT("social.system_chat"), Social, NoSection, LOCTEXT("SysChat", "Show System Messages in \"All\""), FText(), true);

		// ---- Accessibility
		const FName Access(TEXT("gameplay.accessibility"));
		AddCheck(UI, TEXT("access.text_shadow"), Access, NoSection, LOCTEXT("TextShadow", "HUD Text Shadows"), LOCTEXT("TextShadowT", "Dark outline behind HUD text."), true);
		AddSlider(UI, TEXT("access.panel_opacity"), Access, NoSection, LOCTEXT("PanelOpacity", "HUD Panel Opacity"), FText(), 0.3f, 1.f, 0.05f, true, 1.f);

		// ---- Display (GameUserSettings is the source of truth)
		const FName DisplayCat(TEXT("graphics.display"));
		AddChoice(UI, TEXT("gfx.window_mode"), DisplayCat, NoSection, LOCTEXT("WinMode", "Display Mode"), FText(),
			{LOCTEXT("WMFull", "Fullscreen"), LOCTEXT("WMWinFull", "Windowed (Fullscreen)"), LOCTEXT("WMWin", "Windowed")}, 1);
		UI.RegisterSettingReader(TEXT("gfx.window_mode"), []()
		{
			UGameUserSettings* GS = GameSettings();
			return FString::FromInt(GS ? (int32)GS->GetFullscreenMode() : 1);
		});
		UI.RegisterSettingApplier(TEXT("gfx.window_mode"), [](const FString& V)
		{
			if (UGameUserSettings* GS = GameSettings())
			{
				GS->SetFullscreenMode((EWindowMode::Type)FMath::Clamp(FCString::Atoi(*V), 0, 2));
				GS->ApplySettings(false);
			}
		});

		TSharedRef<TArray<FIntPoint>> Resolutions = MakeShared<TArray<FIntPoint>>();
		UKismetSystemLibrary::GetSupportedFullscreenResolutions(*Resolutions);
		if (UGameUserSettings* GS = GameSettings())
		{
			Resolutions->AddUnique(GS->GetScreenResolution());
		}
		Resolutions->Sort([](const FIntPoint& A, const FIntPoint& B) { return A.X * A.Y > B.X * B.Y; });
		TArray<FText> ResLabels;
		for (const FIntPoint& R : *Resolutions) ResLabels.Add(FText::FromString(FString::Printf(TEXT("%d x %d"), R.X, R.Y)));
		AddChoice(UI, TEXT("gfx.resolution"), DisplayCat, NoSection, LOCTEXT("Resolution", "Resolution"), FText(), ResLabels, 0);
		UI.RegisterSettingReader(TEXT("gfx.resolution"), [Resolutions]()
		{
			UGameUserSettings* GS = GameSettings();
			return FString::FromInt(GS ? FMath::Max(0, Resolutions->IndexOfByKey(GS->GetScreenResolution())) : 0);
		});
		UI.RegisterSettingApplier(TEXT("gfx.resolution"), [Resolutions](const FString& V)
		{
			UGameUserSettings* GS = GameSettings();
			const int32 Index = FCString::Atoi(*V);
			if (GS && Resolutions->IsValidIndex(Index))
			{
				GS->SetScreenResolution((*Resolutions)[Index]);
				GS->ApplySettings(false);
			}
		});

		AddCheck(UI, TEXT("gfx.vsync"), DisplayCat, NoSection, LOCTEXT("VSync", "Vertical Sync"), LOCTEXT("VSyncT", "Prevents tearing; caps the frame rate to the monitor."), false);
		UI.RegisterSettingReader(TEXT("gfx.vsync"), []() { UGameUserSettings* GS = GameSettings(); return FString(GS && GS->IsVSyncEnabled() ? TEXT("1") : TEXT("0")); });
		UI.RegisterSettingApplier(TEXT("gfx.vsync"), [](const FString& V)
		{
			if (UGameUserSettings* GS = GameSettings()) { GS->SetVSyncEnabled(V == TEXT("1")); GS->ApplySettings(false); }
		});

		static const float FrameLimits[] = {0.f, 30.f, 60.f, 120.f, 144.f, 240.f};
		AddChoice(UI, TEXT("gfx.frame_limit"), DisplayCat, NoSection, LOCTEXT("FpsCap", "Max Frame Rate"), FText(),
			{LOCTEXT("Unlimited", "Unlimited"), INVTEXT("30"), INVTEXT("60"), INVTEXT("120"), INVTEXT("144"), INVTEXT("240")}, 0);
		UI.RegisterSettingReader(TEXT("gfx.frame_limit"), []()
		{
			UGameUserSettings* GS = GameSettings();
			const float L = GS ? GS->GetFrameRateLimit() : 0.f;
			int32 Best = 0;
			for (int32 i = 0; i < UE_ARRAY_COUNT(FrameLimits); ++i)
			{
				if (FMath::Abs(FrameLimits[i] - L) < FMath::Abs(FrameLimits[Best] - L)) Best = i;
			}
			return FString::FromInt(Best);
		});
		UI.RegisterSettingApplier(TEXT("gfx.frame_limit"), [](const FString& V)
		{
			const int32 I = FMath::Clamp(FCString::Atoi(*V), 0, (int32)UE_ARRAY_COUNT(FrameLimits) - 1);
			if (UGameUserSettings* GS = GameSettings()) { GS->SetFrameRateLimit(FrameLimits[I]); GS->ApplySettings(false); }
		});

		AddSlider(UI, TEXT("gfx.render_scale"), DisplayCat, NoSection, LOCTEXT("RenderScale", "Render Scale"), LOCTEXT("RenderScaleT", "Internal resolution; lower is faster."), 0.5f, 1.f, 0.05f, true, 1.f);
		UI.RegisterSettingApplier(TEXT("gfx.render_scale"), [](const FString& V)
		{
			if (IConsoleVariable* CVar = IConsoleManager::Get().FindConsoleVariable(TEXT("r.ScreenPercentage")))
			{
				CVar->Set(FMath::Clamp(FCString::Atof(*V), 0.5f, 1.f) * 100.f, ECVF_SetByGameSetting);
			}
		});
		AddSlider(UI, TEXT("gfx.gamma"), DisplayCat, NoSection, LOCTEXT("Gamma", "Brightness (Gamma)"), FText(), 1.6f, 2.8f, 0.05f, false, 2.2f);
		UI.RegisterSettingApplier(TEXT("gfx.gamma"), [](const FString& V)
		{
			if (GEngine) GEngine->DisplayGamma = FMath::Clamp(FCString::Atof(*V), 1.f, 3.f);
		});

		// ---- Quality
		const FName QualityCat(TEXT("graphics.quality"));
		TArray<FText> Presets = QualityLevels();
		Presets.Add(LOCTEXT("Custom", "Custom"));
		AddChoice(UI, TEXT("gfx.preset"), QualityCat, NoSection, LOCTEXT("Preset", "Overall Quality"), LOCTEXT("PresetT", "Sets every option below at once."), Presets, 3);
		UI.RegisterSettingReader(TEXT("gfx.preset"), []()
		{
			UGameUserSettings* GS = GameSettings();
			const int32 L = GS ? GS->GetOverallScalabilityLevel() : 3;
			return FString::FromInt(L < 0 ? 5 : L);
		});
		UI.RegisterSettingApplier(TEXT("gfx.preset"), [](const FString& V)
		{
			const int32 L = FCString::Atoi(*V);
			UGameUserSettings* GS = GameSettings();
			if (GS && L >= 0 && L <= 4) { GS->SetOverallScalabilityLevel(L); GS->ApplyNonResolutionSettings(); GS->SaveSettings(); }
		});

		struct FQuality
		{
			const TCHAR* Key;
			FText Label;
			int32 (UGameUserSettings::*Get)() const;
			void (UGameUserSettings::*Set)(int32);
		};
		const FText Features = LOCTEXT("SecFeatures", "Features");
		const FQuality Qualities[] = {
			{TEXT("gfx.view_distance"), LOCTEXT("QView", "View Distance"), &UGameUserSettings::GetViewDistanceQuality, &UGameUserSettings::SetViewDistanceQuality},
			{TEXT("gfx.shadows"), LOCTEXT("QShadow", "Shadows"), &UGameUserSettings::GetShadowQuality, &UGameUserSettings::SetShadowQuality},
			{TEXT("gfx.gi"), LOCTEXT("QGI", "Global Illumination"), &UGameUserSettings::GetGlobalIlluminationQuality, &UGameUserSettings::SetGlobalIlluminationQuality},
			{TEXT("gfx.reflections"), LOCTEXT("QRefl", "Reflections"), &UGameUserSettings::GetReflectionQuality, &UGameUserSettings::SetReflectionQuality},
			{TEXT("gfx.antialiasing"), LOCTEXT("QAA", "Anti-Aliasing"), &UGameUserSettings::GetAntiAliasingQuality, &UGameUserSettings::SetAntiAliasingQuality},
			{TEXT("gfx.textures"), LOCTEXT("QTex", "Textures"), &UGameUserSettings::GetTextureQuality, &UGameUserSettings::SetTextureQuality},
			{TEXT("gfx.effects"), LOCTEXT("QFx", "Effects"), &UGameUserSettings::GetVisualEffectQuality, &UGameUserSettings::SetVisualEffectQuality},
			{TEXT("gfx.post"), LOCTEXT("QPost", "Post Processing"), &UGameUserSettings::GetPostProcessingQuality, &UGameUserSettings::SetPostProcessingQuality},
			{TEXT("gfx.foliage"), LOCTEXT("QFoliage", "Foliage"), &UGameUserSettings::GetFoliageQuality, &UGameUserSettings::SetFoliageQuality},
			{TEXT("gfx.shading"), LOCTEXT("QShading", "Shading"), &UGameUserSettings::GetShadingQuality, &UGameUserSettings::SetShadingQuality},
		};
		for (const FQuality& Q : Qualities)
		{
			AddChoice(UI, Q.Key, QualityCat, Features, Q.Label, FText(), QualityLevels(), 3);
			const auto Get = Q.Get;
			const auto Set = Q.Set;
			UI.RegisterSettingReader(Q.Key, [Get]()
			{
				UGameUserSettings* GS = GameSettings();
				return FString::FromInt(GS ? FMath::Clamp((GS->*Get)(), 0, 4) : 3);
			});
			UI.RegisterSettingApplier(Q.Key, [Set](const FString& V)
			{
				if (UGameUserSettings* GS = GameSettings())
				{
					(GS->*Set)(FMath::Clamp(FCString::Atoi(*V), 0, 4));
					GS->ApplyNonResolutionSettings();
					GS->SaveSettings();
				}
			});
		}

		// ---- Audio
		const FName Volume(TEXT("audio.volume"));
		AddCheck(UI, TEXT("audio.enable"), Volume, NoSection, LOCTEXT("EnableSound", "Enable Sound"), FText(), true);
		AddSlider(UI, TEXT("audio.master"), Volume, NoSection, LOCTEXT("Master", "Master Volume"), FText(), 0.f, 1.f, 0.05f, true, 1.f);
		AddSlider(UI, TEXT("audio.music"), Volume, NoSection, LOCTEXT("Music", "Music"), FText(), 0.f, 1.f, 0.05f, true, 0.65f);
		AddSlider(UI, TEXT("audio.effects"), Volume, NoSection, LOCTEXT("Effects", "Sound Effects"), FText(), 0.f, 1.f, 0.05f, true, 0.8f);
		AddCheck(UI, TEXT("audio.background"), Volume, NoSection, LOCTEXT("BgSound", "Sound in Background"), LOCTEXT("BgSoundT", "Keep playing sound while the game window is not focused."), true);

		UI.RegisterSettingApplier(TEXT("audio.enable"), [Self](const FString&) { ApplyVolume(Self); });
		UI.RegisterSettingApplier(TEXT("audio.master"), [Self](const FString&) { ApplyVolume(Self); });
		UI.RegisterSettingApplier(TEXT("audio.music"), [Self](const FString& V) { if (UGZAudioSubsystem* A = AudioOf(Self)) A->SetMusicVolume(FCString::Atof(*V)); });
		UI.RegisterSettingApplier(TEXT("audio.effects"), [Self](const FString& V) { if (UGZAudioSubsystem* A = AudioOf(Self)) A->SetSoundEffectVolume(FCString::Atof(*V)); });
		UI.RegisterSettingApplier(TEXT("audio.background"), [](const FString& V) { FApp::SetUnfocusedVolumeMultiplier(V == TEXT("1") ? 1.f : 0.f); });
	}

	void RegisterBindings(FUI& UI)
	{
		auto Bind = [&UI](FName Action, const FText& Label, const FText& Group, FEVKeyChord Primary, FEVKeyChord Secondary = FEVKeyChord())
		{
			FEVKeyBindingDefinition B;
			B.ActionId = Action;
			B.Label = Label;
			B.Group = Group;
			B.DefaultPrimary = Primary;
			B.DefaultSecondary = Secondary;
			UI.RegisterKeyBinding(B);
		};

		const FText Panels = LOCTEXT("GrpPanels", "Interface Panels");
		for (const FGameWindow& W : GameWindows()) Bind(W.Action, W.Label, Panels, FEVKeyChord(W.Key));
		Bind(TEXT("panel.support"), LOCTEXT("BindSupport", "Help & Support"), Panels, FEVKeyChord(EKeys::H, true));
		Bind(TEXT("panel.game_menu"), LOCTEXT("BindGameMenu", "Game Menu"), Panels, FEVKeyChord(EKeys::F10));
		Bind(TEXT("ui.options"), LOCTEXT("BindOptions", "Options"), Panels, FEVKeyChord(EKeys::O, false, true));
		Bind(TEXT("ui.options.keybindings"), LOCTEXT("BindKeys", "Key Bindings"), Panels, FEVKeyChord(EKeys::K, false, true));

		const FText Interface = LOCTEXT("GrpInterface", "Interface");
		Bind(TEXT("ui.toggle_interface"), LOCTEXT("BindHideUI", "Hide / Show Interface"), Interface, FEVKeyChord(EKeys::Z, false, false, true));
		Bind(TEXT("ui.close_all"), LOCTEXT("BindCloseAll", "Close All Windows"), Interface, FEVKeyChord());
		Bind(TEXT("ui.reset_positions"), LOCTEXT("BindResetPos", "Reset Window Positions"), Interface, FEVKeyChord());
		Bind(TEXT("ui.screenshot"), LOCTEXT("BindShot", "Take Screenshot"), Interface, FEVKeyChord(EKeys::S, true, true));

		const FText Targeting = LOCTEXT("GrpTarget", "Targeting");
		Bind(TEXT("target.menu"), LOCTEXT("BindTargetMenu", "Open Target Menu"), Targeting, FEVKeyChord(EKeys::F10, true));
		Bind(TEXT("target.inspect"), LOCTEXT("BindInspect", "Inspect Target"), Targeting, FEVKeyChord(EKeys::I, true));
		Bind(TEXT("target.clear"), LOCTEXT("BindClear", "Clear Target"), Targeting, FEVKeyChord());

		const FText Chat = LOCTEXT("GrpChat", "Chat");
		Bind(TEXT("chat.clear"), LOCTEXT("BindChatClear", "Clear Chat Log"), Chat, FEVKeyChord());

		// Keys the character reads directly every frame; rebinding a command onto them would fight gameplay.
		const FText Move = LOCTEXT("RsvMove", "movement / camera");
		const FText Act = LOCTEXT("RsvAct", "combat and interaction");
		const FText Quick = LOCTEXT("RsvQuick", "quick slots");
		for (const FKey& K : {EKeys::W, EKeys::A, EKeys::S, EKeys::D, EKeys::Up, EKeys::Down, EKeys::Left, EKeys::Right, EKeys::Q, EKeys::E}) UI.RegisterReservedKey(K, Move);
		for (const FKey& K : {EKeys::SpaceBar, EKeys::Tab, EKeys::G, EKeys::T, EKeys::P}) UI.RegisterReservedKey(K, Act);
		for (const FKey& K : {EKeys::F1, EKeys::F2, EKeys::F3, EKeys::F4, EKeys::F5}) UI.RegisterReservedKey(K, Quick);
	}

	void RegisterMenus(FUI& UI)
	{
		using FI = FEVMenuItem;
		auto Menu = [&UI](FName Id, const FText& Title, TArray<FEVMenuItem> Items, float MinWidth = 200.f)
		{
			FEVMenuDefinition M;
			M.MenuId = Id;
			M.Title = Title;
			M.Items = MoveTemp(Items);
			M.MinWidth = MinWidth;
			UI.RegisterMenu(M);
		};
		auto Colored = [](FEVMenuItem Item, const FLinearColor& C) { Item.TextColor = C; return Item; };
		const FLinearColor Danger(1.f, 0.55f, 0.45f, 1.f);

		// ---- Escape menu (big buttons)
		Menu(TEXT("game_menu"), FText(), {
			FI::MakeAction(TEXT("options"), LOCTEXT("MOptions", "Options"), TEXT("ui.options"), LOCTEXT("MOptionsT", "Gameplay, graphics, audio and key bindings.")),
			FI::MakeSubmenu(TEXT("interface"), LOCTEXT("MInterface", "Interface"), TEXT("system.interface"), LOCTEXT("MInterfaceT", "Quick interface toggles.")),
			FI::MakeAction(TEXT("keybindings"), LOCTEXT("MKeys", "Key Bindings"), OptionsAction(TEXT("keybindings"))),
			FI::MakeAction(TEXT("support"), LOCTEXT("MSupport", "Help & Support"), TEXT("panel.support")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("logout"), LOCTEXT("MLogout", "Log Out"), TEXT("game.logout")),
			Colored(FI::MakeAction(TEXT("exit"), LOCTEXT("MExit", "Exit Game"), TEXT("game.exit")), Danger),
			FI::MakeSeparator(),
			Colored(FI::MakeAction(TEXT("resume"), LOCTEXT("MResume", "Return to Game"), TEXT("game.resume")), EVUIStyle::Good),
		});

		// ---- System dropdown (right-click the Game Menu button)
		Menu(TEXT("system"), LOCTEXT("SystemTitle", "System"), {
			FI::MakeSubmenu(TEXT("options"), LOCTEXT("SOptions", "Options"), TEXT("system.options")),
			FI::MakeSubmenu(TEXT("interface"), LOCTEXT("SInterface", "Interface"), TEXT("system.interface")),
			FI::MakeAction(TEXT("keybindings"), LOCTEXT("SKeys", "Key Bindings"), OptionsAction(TEXT("keybindings"))),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("close_all"), LOCTEXT("SCloseAll", "Close All Windows"), TEXT("ui.close_all")),
			FI::MakeAction(TEXT("reset_positions"), LOCTEXT("SResetPos", "Reset Window Positions"), TEXT("ui.reset_positions")),
			FI::MakeAction(TEXT("hide_ui"), LOCTEXT("SHideUI", "Hide Interface"), TEXT("ui.toggle_interface")),
			FI::MakeAction(TEXT("screenshot"), LOCTEXT("SShot", "Take Screenshot"), TEXT("ui.screenshot")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("support"), LOCTEXT("SSupport", "Help & Support"), TEXT("panel.support")),
			FI::MakeAction(TEXT("logout"), LOCTEXT("SLogout", "Log Out"), TEXT("game.logout")),
			Colored(FI::MakeAction(TEXT("exit"), LOCTEXT("SExit", "Exit Game"), TEXT("game.exit")), Danger),
		});

		// Options > <category> > <page>, generated from the Options tree so they never drift apart.
		TArray<FEVMenuItem> OptionItems;
		for (const FEVOptionsCategory& Top : UI.GetOptionsCategories())
		{
			if (!Top.ParentId.IsNone()) continue;
			TArray<FEVMenuItem> Children;
			for (const FEVOptionsCategory& Child : UI.GetOptionsCategories())
			{
				if (Child.ParentId == Top.Id) Children.Add(FI::MakeAction(Child.Id, Child.Label, OptionsAction(Child.Id), Child.Description));
			}
			if (Children.Num() > 0)
			{
				const FName SubId(*(TEXT("system.options.") + Top.Id.ToString()));
				Menu(SubId, FText(), MoveTemp(Children), 170.f);
				OptionItems.Add(FI::MakeSubmenu(Top.Id, Top.Label, SubId, Top.Description));
			}
			else
			{
				OptionItems.Add(FI::MakeAction(Top.Id, Top.Label, OptionsAction(Top.Id), Top.Description));
			}
		}
		Menu(TEXT("system.options"), FText(), MoveTemp(OptionItems), 170.f);

		// Interface quick toggles
		Menu(TEXT("system.interface"), FText(), {
			FI::MakeHeader(LOCTEXT("IShow", "Show")),
			FI::MakeToggle(TEXT("micromenu"), LOCTEXT("IMicro", "Menu Bar"), TEXT("interface.show_micromenu")),
			FI::MakeToggle(TEXT("clock"), LOCTEXT("IClock", "Zone & Clock Banner"), TEXT("interface.show_clock")),
			FI::MakeToggle(TEXT("minimap"), LOCTEXT("IMinimap", "Minimap"), TEXT("interface.show_minimap")),
			FI::MakeToggle(TEXT("target"), LOCTEXT("ITarget", "Target Frame"), TEXT("interface.show_target")),
			FI::MakeToggle(TEXT("chat"), LOCTEXT("IChat", "Chat Window"), TEXT("interface.show_chat")),
			FI::MakeToggle(TEXT("floating"), LOCTEXT("IFloat", "Floating Combat Text"), TEXT("combat.floating_text")),
			FI::MakeToggle(TEXT("tooltips"), LOCTEXT("ITips", "Tooltips"), TEXT("interface.tooltips")),
			FI::MakeSeparator(),
			FI::MakeSubmenu(TEXT("ui_scale"), LOCTEXT("IUIScale", "Menu Scale"), TEXT("system.interface.ui_scale")),
			FI::MakeSubmenu(TEXT("hud_scale"), LOCTEXT("IHUDScale", "HUD Scale"), TEXT("system.interface.hud_scale")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("more"), LOCTEXT("IMore", "More Interface Options..."), OptionsAction(TEXT("gameplay.interface"))),
		}, 220.f);

		auto ScaleMenu = [&](FName Id, FName Setting)
		{
			TArray<FEVMenuItem> Items;
			for (const float V : {0.8f, 0.9f, 1.0f, 1.1f, 1.25f, 1.4f})
			{
				Items.Add(FI::MakeRadio(FName(*FString::Printf(TEXT("s%d"), FMath::RoundToInt(V * 100.f))),
					FText::FromString(FString::Printf(TEXT("%d%%"), FMath::RoundToInt(V * 100.f))), Setting, FString::SanitizeFloat(V)));
			}
			Menu(Id, FText(), MoveTemp(Items), 120.f);
		};
		ScaleMenu(TEXT("system.interface.ui_scale"), TEXT("ui.scale"));
		ScaleMenu(TEXT("system.interface.hud_scale"), TEXT("interface.hud_scale"));

		// ---- Player portrait
		Menu(TEXT("unit.self"), INVTEXT("{Context}"), {
			FI::MakeAction(TEXT("character"), LOCTEXT("UCharacter", "Character Info"), TEXT("window.character")),
			FI::MakeAction(TEXT("inventory"), LOCTEXT("UInventory", "Inventory"), TEXT("window.inventory")),
			FI::MakeAction(TEXT("skills"), LOCTEXT("USkills", "Skills"), TEXT("window.skills")),
			FI::MakeSeparator(),
			FI::MakeSubmenu(TEXT("status"), LOCTEXT("UStatus", "Status"), TEXT("unit.self.status")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("cancel"), LOCTEXT("UCancel", "Cancel"), NAME_None),
		});
		Menu(TEXT("unit.self.status"), FText(), {
			Colored(FI::MakeRadio(TEXT("online"), LOCTEXT("StOn", "Online"), TEXT("social.status"), TEXT("0")), EVUIStyle::Good),
			Colored(FI::MakeRadio(TEXT("away"), LOCTEXT("StAw", "Away"), TEXT("social.status"), TEXT("1")), FLinearColor(1.f, 0.85f, 0.3f, 1.f)),
			Colored(FI::MakeRadio(TEXT("busy"), LOCTEXT("StBu", "Busy"), TEXT("social.status"), TEXT("2")), Danger),
		}, 130.f);

		// ---- Target frame
		Menu(TEXT("unit.target"), INVTEXT("{Context}"), {
			FI::MakeAction(TEXT("attack"), LOCTEXT("TAttack", "Attack"), TEXT("target.attack")),
			FI::MakeAction(TEXT("inspect"), LOCTEXT("TInspect", "Inspect"), TEXT("target.inspect"), LOCTEXT("TInspectT", "Print level and health to chat.")),
			FI::MakeSubmenu(TEXT("marker"), LOCTEXT("TMarker", "Target Marker"), TEXT("unit.target.marker")),
			FI::MakeAction(TEXT("clear"), LOCTEXT("TClear", "Clear Target"), TEXT("target.clear")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("cancel"), LOCTEXT("TCancel", "Cancel"), NAME_None),
		});
		TArray<FEVMenuItem> Markers;
		for (int32 i = 1; i <= AGZHUD::NumTargetMarkers; ++i)
		{
			FEVMenuItem M;
			M.Id = FName(*FString::Printf(TEXT("m%d"), i));
			M.Kind = EEVMenuItemKind::Radio;
			M.Label = AGZHUD::GetMarkerName(i);
			M.ActionId = FName(*FString::Printf(TEXT("target.marker.%d"), i));
			M.TextColor = AGZHUD::GetMarkerColor(i);
			Markers.Add(M);
		}
		Markers.Add(FI::MakeSeparator());
		{
			FEVMenuItem None;
			None.Id = TEXT("none");
			None.Kind = EEVMenuItemKind::Radio;
			None.Label = LOCTEXT("MarkNone", "None");
			None.ActionId = TEXT("target.marker.0");
			Markers.Add(None);
		}
		Menu(TEXT("unit.target.marker"), FText(), MoveTemp(Markers), 140.f);

		// ---- Minimap
		Menu(TEXT("minimap"), LOCTEXT("MinimapTitle", "Minimap"), {
			FI::MakeHeader(LOCTEXT("Tracking", "Tracking")),
			FI::MakeToggle(TEXT("npcs"), LOCTEXT("MTNpc", "Townsfolk & Merchants"), TEXT("minimap.track_npcs")),
			FI::MakeToggle(TEXT("monsters"), LOCTEXT("MTMon", "Monsters"), TEXT("minimap.track_monsters")),
			FI::MakeToggle(TEXT("resources"), LOCTEXT("MTRes", "Gathering Nodes"), TEXT("minimap.track_resources")),
			FI::MakeSeparator(),
			FI::MakeSubmenu(TEXT("zoom"), LOCTEXT("MZoom", "Zoom"), TEXT("minimap.zoom")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("worldmap"), LOCTEXT("MWorld", "Open World Map"), TEXT("window.worldmap")),
		});
		Menu(TEXT("minimap.zoom"), FText(), {
			FI::MakeRadio(TEXT("near"), LOCTEXT("ZN", "Near"), TEXT("minimap.zoom"), TEXT("0")),
			FI::MakeRadio(TEXT("normal"), LOCTEXT("ZM", "Normal"), TEXT("minimap.zoom"), TEXT("1")),
			FI::MakeRadio(TEXT("far"), LOCTEXT("ZF", "Far"), TEXT("minimap.zoom"), TEXT("2")),
		}, 120.f);

		// ---- Chat
		TArray<FEVMenuItem> Channels;
		for (int32 i = 0; i < AGZHUD::NumChatFilters; ++i)
		{
			FEVMenuItem C;
			C.Id = FName(*FString::Printf(TEXT("ch%d"), i));
			C.Kind = EEVMenuItemKind::Radio;
			C.Label = FText::FromString(AGZHUD::GetChatFilterName(i));
			C.ActionId = FName(*FString::Printf(TEXT("chat.filter.%d"), i));
			Channels.Add(C);
		}
		Menu(TEXT("chat.channels"), FText(), MoveTemp(Channels), 130.f);
		Menu(TEXT("chat"), LOCTEXT("ChatTitle", "Chat"), {
			FI::MakeSubmenu(TEXT("channels"), LOCTEXT("CShow", "Show Channel"), TEXT("chat.channels")),
			FI::MakeToggle(TEXT("system"), LOCTEXT("CSys", "System Messages in \"All\""), TEXT("social.system_chat")),
			FI::MakeSeparator(),
			FI::MakeAction(TEXT("clear"), LOCTEXT("CClear", "Clear Chat Log"), TEXT("chat.clear")),
			FI::MakeAction(TEXT("hide"), LOCTEXT("CHide", "Hide Chat Window"), TEXT("chat.hide"), LOCTEXT("CHideT", "Bring it back from Game Menu > Interface.")),
		});
	}

	void RegisterMicroMenu(FUI& UI)
	{
		for (const FGameWindow& W : GameWindows())
		{
			FEVMicroButton B;
			B.Id = W.Action;
			B.Label = W.Label;
			B.Tooltip = W.Tooltip;
			B.IconName = W.Icon ? FName(W.Icon) : NAME_None;
			B.Glyph = W.Glyph;
			B.ActionId = W.Action;
			B.ContextMenuId = W.ContextMenu ? FName(W.ContextMenu) : NAME_None;
			UI.RegisterMicroButton(B);
		}

		FEVMicroButton Support;
		Support.Id = TEXT("support");
		Support.Label = LOCTEXT("MBSupport", "Help & Support");
		Support.Tooltip = LOCTEXT("MBSupportT", "Guides, key reference and bug reports.");
		Support.IconName = TEXT("npc_quest_giver");
		Support.ActionId = TEXT("panel.support");
		UI.RegisterMicroButton(Support);

		FEVMicroButton Game;
		Game.Id = TEXT("game_menu");
		Game.Label = LOCTEXT("MBGame", "Game Menu");
		Game.Tooltip = LOCTEXT("MBGameT", "Options, key bindings, log out and exit.");
		Game.IconName = TEXT("crystal_gem");
		Game.Glyph = INVTEXT("Esc");
		Game.ActionId = TEXT("panel.game_menu");
		Game.ContextMenuId = TEXT("system");
		UI.RegisterMicroButton(Game);
	}

	// ------------------------------------------------------------------ Help & Support pages

	TSharedRef<SWidget> MakeGuidePage()
	{
		struct FSection { FText Title; FText Body; };
		const FSection Sections[] = {
			{LOCTEXT("GMove", "Getting Around"), LOCTEXT("GMoveB", "Walk with W A S D or the arrow keys, or left-click the ground (Options > Gameplay > Controls). Q and E rotate the camera, the mouse wheel zooms.")},
			{LOCTEXT("GCombat", "Combat & Targeting"), LOCTEXT("GCombatB", "Tab cycles through nearby monsters and Space attacks your target. Right-click the target frame (or press Shift+F10) for Attack, Inspect, Target Marker and Clear Target.")},
			{LOCTEXT("GWork", "Gathering, Crafting & Trade"), LOCTEXT("GWorkB", "Press G beside a resource node to gather and E or T to talk to townsfolk. Manufacture (U) turns materials into goods, the Market (B) buys and sells them. F1-F5 use your quick slots.")},
			{LOCTEXT("GUI", "The Interface"), LOCTEXT("GUIB", "Esc closes the top window or dropdown; with nothing open it shows the Game Menu. Every button in the bottom-right menu bar has a key binding, and right-clicking your portrait, the target frame, the minimap, the chat window or the menu bar's Character, World Map and Game Menu buttons opens a dropdown. Drag a window by its title bar. Alt+Z hides the whole interface.")},
		};
		TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
		for (const FSection& S : Sections)
		{
			Box->AddSlot().AutoHeight().Padding(0.f, 4.f, 0.f, 0.f)[EVUIWidgets::MakeSectionHeader(S.Title)];
			Box->AddSlot().AutoHeight().Padding(0.f, 0.f, 0.f, 8.f)[EVUIWidgets::MakeParagraph(S.Body)];
		}
		return SNew(SScrollBox).ScrollBarStyle(&EVUIStyle::ScrollBar()) + SScrollBox::Slot().Padding(0.f, 0.f, 10.f, 0.f)[Box];
	}

	TSharedRef<SWidget> MakeKeyReferencePage(FUI* UI)
	{
		TSharedRef<SVerticalBox> Box = SNew(SVerticalBox);
		FString LastGroup = TEXT("\x01");
		for (const FEVKeyBindingDefinition& B : UI->GetKeyBindings())
		{
			if (B.Group.ToString() != LastGroup)
			{
				LastGroup = B.Group.ToString();
				Box->AddSlot().AutoHeight().Padding(0.f, 8.f, 0.f, 0.f)[EVUIWidgets::MakeSectionHeader(B.Group)];
			}
			const FEVKeyChord A = UI->GetBinding(B.ActionId, 0);
			const FEVKeyChord C = UI->GetBinding(B.ActionId, 1);
			FText Keys = A.ToDisplayText();
			if (C.IsValid()) Keys = FText::Format(INVTEXT("{0}  /  {1}"), Keys, C.ToDisplayText());
			Box->AddSlot().AutoHeight().Padding(0.f, 1.f)
			[
				SNew(SHorizontalBox)
				+ SHorizontalBox::Slot().FillWidth(1.f)[SNew(STextBlock).Text(B.Label).Font(EVUIStyle::Font(10)).ColorAndOpacity(EVUIStyle::Text)]
				+ SHorizontalBox::Slot().AutoWidth()[SNew(STextBlock).Text(Keys).Font(EVUIStyle::Font(10, true)).ColorAndOpacity(A.IsValid() ? EVUIStyle::Accent : EVUIStyle::TextDisabled)]
			];
		}
		Box->AddSlot().AutoHeight().Padding(0.f, 12.f, 0.f, 0.f)
		[
			SNew(SBox).HAlign(HAlign_Left)
			[
				SNew(SButton)
				.ButtonStyle(&EVUIStyle::Button())
				.IsFocusable(false)
				.OnClicked_Lambda([UI]() { UI->OpenOptions(TEXT("keybindings")); return FReply::Handled(); })
				[
					SNew(STextBlock).Text(LOCTEXT("ChangeKeys", "Change Key Bindings...")).Font(EVUIStyle::Font(11, true))
				]
			]
		];
		return SNew(SScrollBox).ScrollBarStyle(&EVUIStyle::ScrollBar()) + SScrollBox::Slot().Padding(0.f, 0.f, 10.f, 0.f)[Box];
	}

	TSharedRef<SWidget> MakeBugReportPage(FUI* UI)
	{
		struct FState
		{
			int32 Category = 0;
			TSharedPtr<SMultiLineEditableTextBox> Text;
			TSharedPtr<SButton> CategoryButton;
		};
		TSharedRef<FState> State = MakeShared<FState>();
		const TArray<FText> Categories = {LOCTEXT("BugGameplay", "Gameplay"), LOCTEXT("BugUI", "Interface"), LOCTEXT("BugGfx", "Graphics"),
			LOCTEXT("BugAudio", "Audio"), LOCTEXT("BugQuest", "Quests"), LOCTEXT("BugOther", "Other")};

		TSharedRef<SVerticalBox> Box = SNew(SVerticalBox)
			+ SVerticalBox::Slot().AutoHeight()[EVUIWidgets::MakeSectionHeader(LOCTEXT("ReportTitle", "Report a Bug"))]
			+ SVerticalBox::Slot().AutoHeight().Padding(0.f, 0.f, 0.f, 8.f)
			[
				EVUIWidgets::MakeParagraph(LOCTEXT("ReportHelp", "Describe what happened and what you expected. Your zone and position are attached automatically. Reports are saved to Saved/BugReports."), true)
			]
			+ SVerticalBox::Slot().AutoHeight().Padding(0.f, 0.f, 0.f, 6.f)
			[
				SNew(SHorizontalBox)
				+ SHorizontalBox::Slot().AutoWidth().VAlign(VAlign_Center).Padding(0.f, 0.f, 10.f, 0.f)
				[
					SNew(STextBlock).Text(LOCTEXT("Category", "Category")).Font(EVUIStyle::Font(11)).ColorAndOpacity(EVUIStyle::Text)
				]
				+ SHorizontalBox::Slot().AutoWidth()
				[
					SNew(SBox).MinDesiredWidth(160.f)
					[
						SAssignNew(State->CategoryButton, SButton)
						.ButtonStyle(&EVUIStyle::Button())
						.IsFocusable(false)
						.OnClicked_Lambda([UI, State, Categories]()
						{
							const FGeometry& Geo = State->CategoryButton->GetTickSpaceGeometry();
							if (UI->GetRoot().IsValid())
							{
								const FVector2D Below = UI->GetRoot()->AbsoluteToLayer(Geo.LocalToAbsolute(FVector2D(0.f, Geo.GetLocalSize().Y)));
								UI->OpenChoiceMenu(Categories, State->Category, [State](int32 Picked) { State->Category = Picked; }, Below + FVector2D(0.f, 2.f), Geo.GetLocalSize().X);
							}
							return FReply::Handled();
						})
						[
							SNew(STextBlock).Font(EVUIStyle::Font(10)).Text_Lambda([State, Categories]() { return Categories[State->Category]; })
						]
					]
				]
			]
			+ SVerticalBox::Slot().FillHeight(1.f)
			[
				SAssignNew(State->Text, SMultiLineEditableTextBox)
				.Style(&EVUIStyle::TextBox())
				.HintText(LOCTEXT("ReportHint", "What happened?"))
				.AutoWrapText(true)
			]
			+ SVerticalBox::Slot().AutoHeight().HAlign(HAlign_Right).Padding(0.f, 8.f, 0.f, 0.f)
			[
				SNew(SButton)
				.ButtonStyle(&EVUIStyle::Button())
				.IsFocusable(false)
				.OnClicked_Lambda([UI, State, Categories]()
				{
					const FString Body = State->Text->GetText().ToString().TrimStartAndEnd();
					if (Body.IsEmpty())
					{
						UI->ShowMessage(LOCTEXT("ReportEmpty", "Please describe the problem first."), EVUIStyle::Bad);
						return FReply::Handled();
					}
					FString Where = TEXT("unknown");
					if (AGZIsoCharacter* P = PlayerOf(UI))
					{
						const FVector L = P->GetActorLocation();
						Where = FString::Printf(TEXT("%s (%.0f, %.0f, %.0f)"), P->GetRPG() ? *P->GetRPG()->CurrentZone : TEXT("?"), L.X, L.Y, L.Z);
					}
					const FDateTime Now = FDateTime::Now();
					const FString Report = FString::Printf(TEXT("Category: %s\nTime: %s\nLocation: %s\n\n%s\n"),
						*Categories[State->Category].ToString(), *Now.ToString(), *Where, *Body);
					const FString File = FPaths::ProjectSavedDir() / TEXT("BugReports") / FString::Printf(TEXT("Report_%s.txt"), *Now.ToString());
					if (FFileHelper::SaveStringToFile(Report, *File))
					{
						State->Text->SetText(FText::GetEmpty());
						UI->ShowMessage(LOCTEXT("ReportSaved", "Thank you - your report was saved."), EVUIStyle::Good);
					}
					else
					{
						UI->ShowMessage(LOCTEXT("ReportFailed", "Could not save the report."), EVUIStyle::Bad);
					}
					return FReply::Handled();
				})
				[
					SNew(STextBlock).Text(LOCTEXT("Submit", "Submit Report")).Font(EVUIStyle::Font(11, true))
				]
			];
		return Box;
	}

	void RegisterPanels(FUI& UI)
	{
		FUI* Self = &UI;

		FEVPanelDefinition GameMenu;
		GameMenu.PanelId = TEXT("game_menu");
		GameMenu.Title = LOCTEXT("PGameMenu", "Game Menu");
		GameMenu.Area = EEVPanelArea::Center;
		GameMenu.Size = FVector2D(290.f, 0.f);
		UI.RegisterPanel(GameMenu);
		UI.RegisterPanelContent(GameMenu.PanelId, NAME_None, [Self]() { return SNew(SEVGameMenu, Self, TEXT("game_menu")); });

		FEVPanelDefinition Options;
		Options.PanelId = TEXT("options");
		Options.Title = LOCTEXT("POptions", "Options");
		Options.Area = EEVPanelArea::Center;
		Options.Size = FVector2D(900.f, 600.f);
		Options.bReturnToPrevious = true;
		UI.RegisterPanel(Options);
		UI.RegisterPanelContent(Options.PanelId, NAME_None, [Self]() { return SNew(SEVOptions, Self); });

		FEVPanelDefinition Support;
		Support.PanelId = TEXT("support");
		Support.Title = LOCTEXT("PSupport", "Help & Support");
		Support.Area = EEVPanelArea::Center;
		Support.Size = FVector2D(640.f, 500.f);
		Support.bReturnToPrevious = true;
		FEVPanelTab Guide; Guide.TabId = TEXT("guide"); Guide.Label = LOCTEXT("TGuide", "Getting Started");
		FEVPanelTab Keys; Keys.TabId = TEXT("keys"); Keys.Label = LOCTEXT("TKeys", "Key Reference");
		FEVPanelTab Report; Report.TabId = TEXT("report"); Report.Label = LOCTEXT("TReport", "Report a Bug");
		Support.Tabs = {Guide, Keys, Report};
		UI.RegisterPanel(Support);
		UI.RegisterPanelContent(Support.PanelId, TEXT("guide"), []() { return MakeGuidePage(); });
		UI.RegisterPanelContent(Support.PanelId, TEXT("keys"), [Self]() { return MakeKeyReferencePage(Self); });
		UI.RegisterPanelContent(Support.PanelId, TEXT("report"), [Self]() { return MakeBugReportPage(Self); });
	}
}

void EVUIDefaultContent::Register(UEVUISubsystem& UI)
{
	RegisterPanels(UI);      // panels first: they create the panel.* actions
	RegisterActions(UI);
	RegisterOptions(UI);     // categories feed the Options sub-menus
	RegisterBindings(UI);
	RegisterMenus(UI);
	RegisterMicroMenu(UI);
}

#undef LOCTEXT_NAMESPACE
