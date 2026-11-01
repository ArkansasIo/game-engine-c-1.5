#include "GZAudioSubsystem.h"

#include "Components/AudioComponent.h"
#include "Engine/GameInstance.h"
#include "Engine/World.h"
#include "Kismet/GameplayStatics.h"
#include "Sound/SoundBase.h"
#include "Sound/SoundWave.h"

namespace
{
	struct FGZAudioAsset
	{
		FName Id;
		FString ObjectPath;
	};

	const TArray<FGZAudioAsset>& MusicAssets()
	{
		static const TArray<FGZAudioAsset> Assets = {
			{TEXT("frostmere_wilds"), TEXT("/Game/GoonZu/Audio/Music/M_01_Frostmere_Wilds.M_01_Frostmere_Wilds")},
			{TEXT("frostmere_town"), TEXT("/Game/GoonZu/Audio/Music/M_02_Frostmere_Town.M_02_Frostmere_Town")},
			{TEXT("emberfall_fields"), TEXT("/Game/GoonZu/Audio/Music/M_03_Emberfall_Fields.M_03_Emberfall_Fields")},
			{TEXT("emberfall_market"), TEXT("/Game/GoonZu/Audio/Music/M_04_Emberfall_Market.M_04_Emberfall_Market")},
			{TEXT("greenhollow_canopy"), TEXT("/Game/GoonZu/Audio/Music/M_05_Greenhollow_Canopy.M_05_Greenhollow_Canopy")},
			{TEXT("greenhollow_village"), TEXT("/Game/GoonZu/Audio/Music/M_06_Greenhollow_Village.M_06_Greenhollow_Village")},
			{TEXT("sunreach_dunes"), TEXT("/Game/GoonZu/Audio/Music/M_07_Sunreach_Dunes.M_07_Sunreach_Dunes")},
			{TEXT("stormwatch_coast"), TEXT("/Game/GoonZu/Audio/Music/M_08_Stormwatch_Coast.M_08_Stormwatch_Coast")},
			{TEXT("moonfen_mist"), TEXT("/Game/GoonZu/Audio/Music/M_09_Moonfen_Mist.M_09_Moonfen_Mist")},
			{TEXT("skyreach_heights"), TEXT("/Game/GoonZu/Audio/Music/M_10_Skyreach_Heights.M_10_Skyreach_Heights")},
			{TEXT("deepdelve_echoes"), TEXT("/Game/GoonZu/Audio/Music/M_11_Deepdelve_Echoes.M_11_Deepdelve_Echoes")},
			{TEXT("thornvale_frontier"), TEXT("/Game/GoonZu/Audio/Music/M_12_Thornvale_Frontier.M_12_Thornvale_Frontier")},
			{TEXT("starfall_shore"), TEXT("/Game/GoonZu/Audio/Music/M_13_Starfall_Shore.M_13_Starfall_Shore")},
			{TEXT("capital_day"), TEXT("/Game/GoonZu/Audio/Music/M_14_Capital_Day.M_14_Capital_Day")},
			{TEXT("capital_night"), TEXT("/Game/GoonZu/Audio/Music/M_15_Capital_Night.M_15_Capital_Night")},
			{TEXT("traveler_inn"), TEXT("/Game/GoonZu/Audio/Music/M_16_Traveler_Inn.M_16_Traveler_Inn")},
			{TEXT("harbor_lanterns"), TEXT("/Game/GoonZu/Audio/Music/M_17_Harbor_Lanterns.M_17_Harbor_Lanterns")},
			{TEXT("guild_hall"), TEXT("/Game/GoonZu/Audio/Music/M_18_Guild_Hall.M_18_Guild_Hall")},
			{TEXT("hollow_cistern"), TEXT("/Game/GoonZu/Audio/Music/M_19_Hollow_Cistern.M_19_Hollow_Cistern")},
			{TEXT("cinder_foundry"), TEXT("/Game/GoonZu/Audio/Music/M_20_Cinder_Foundry.M_20_Cinder_Foundry")},
			{TEXT("gloam_sanctum"), TEXT("/Game/GoonZu/Audio/Music/M_21_Gloam_Sanctum.M_21_Gloam_Sanctum")},
			{TEXT("sunken_archive"), TEXT("/Game/GoonZu/Audio/Music/M_22_Sunken_Archive.M_22_Sunken_Archive")},
			{TEXT("thorn_bastion"), TEXT("/Game/GoonZu/Audio/Music/M_23_Thorn_Bastion.M_23_Thorn_Bastion")},
			{TEXT("frost_monastery"), TEXT("/Game/GoonZu/Audio/Music/M_24_Frost_Monastery.M_24_Frost_Monastery")},
			{TEXT("star_vault"), TEXT("/Game/GoonZu/Audio/Music/M_25_Star_Vault.M_25_Star_Vault")},
			{TEXT("ash_citadel"), TEXT("/Game/GoonZu/Audio/Music/M_26_Ash_Citadel.M_26_Ash_Citadel")},
			{TEXT("raid_approach"), TEXT("/Game/GoonZu/Audio/Music/M_27_Raid_Approach.M_27_Raid_Approach")},
			{TEXT("raid_battle"), TEXT("/Game/GoonZu/Audio/Music/M_28_Raid_Battle.M_28_Raid_Battle")},
			{TEXT("raid_victory"), TEXT("/Game/GoonZu/Audio/Music/M_29_Raid_Victory.M_29_Raid_Victory")},
			{TEXT("trial_focus"), TEXT("/Game/GoonZu/Audio/Music/M_30_Trial_Focus.M_30_Trial_Focus")},
			{TEXT("trial_triumph"), TEXT("/Game/GoonZu/Audio/Music/M_31_Trial_Triumph.M_31_Trial_Triumph")},
			{TEXT("world_awakening"), TEXT("/Game/GoonZu/Audio/Music/M_32_World_Awakening.M_32_World_Awakening")}
		};
		return Assets;
	}

	const TArray<FGZAudioAsset>& SoundEffectAssets()
	{
		static const TArray<FGZAudioAsset> Assets = {
			{TEXT("ui_menu_open"), TEXT("/Game/GoonZu/Audio/SFX/S_01_Ui_Menu_Open.S_01_Ui_Menu_Open")},
			{TEXT("ui_menu_close"), TEXT("/Game/GoonZu/Audio/SFX/S_02_Ui_Menu_Close.S_02_Ui_Menu_Close")},
			{TEXT("ui_select"), TEXT("/Game/GoonZu/Audio/SFX/S_03_Ui_Select.S_03_Ui_Select")},
			{TEXT("ui_error"), TEXT("/Game/GoonZu/Audio/SFX/S_04_Ui_Error.S_04_Ui_Error")},
			{TEXT("quest_accepted"), TEXT("/Game/GoonZu/Audio/SFX/S_05_Quest_Accepted.S_05_Quest_Accepted")},
			{TEXT("quest_progress"), TEXT("/Game/GoonZu/Audio/SFX/S_06_Quest_Progress.S_06_Quest_Progress")},
			{TEXT("quest_complete"), TEXT("/Game/GoonZu/Audio/SFX/S_07_Quest_Complete.S_07_Quest_Complete")},
			{TEXT("loot_common"), TEXT("/Game/GoonZu/Audio/SFX/S_08_Loot_Common.S_08_Loot_Common")},
			{TEXT("loot_rare"), TEXT("/Game/GoonZu/Audio/SFX/S_09_Loot_Rare.S_09_Loot_Rare")},
			{TEXT("loot_epic"), TEXT("/Game/GoonZu/Audio/SFX/S_10_Loot_Epic.S_10_Loot_Epic")},
			{TEXT("loot_legendary"), TEXT("/Game/GoonZu/Audio/SFX/S_11_Loot_Legendary.S_11_Loot_Legendary")},
			{TEXT("weapon_swing"), TEXT("/Game/GoonZu/Audio/SFX/S_12_Weapon_Swing.S_12_Weapon_Swing")},
			{TEXT("weapon_hit"), TEXT("/Game/GoonZu/Audio/SFX/S_13_Weapon_Hit.S_13_Weapon_Hit")},
			{TEXT("shield_block"), TEXT("/Game/GoonZu/Audio/SFX/S_14_Shield_Block.S_14_Shield_Block")},
			{TEXT("critical_hit"), TEXT("/Game/GoonZu/Audio/SFX/S_15_Critical_Hit.S_15_Critical_Hit")},
			{TEXT("bow_release"), TEXT("/Game/GoonZu/Audio/SFX/S_16_Bow_Release.S_16_Bow_Release")},
			{TEXT("spell_cast"), TEXT("/Game/GoonZu/Audio/SFX/S_17_Spell_Cast.S_17_Spell_Cast")},
			{TEXT("spell_impact"), TEXT("/Game/GoonZu/Audio/SFX/S_18_Spell_Impact.S_18_Spell_Impact")},
			{TEXT("gather_resource"), TEXT("/Game/GoonZu/Audio/SFX/S_19_Gather_Resource.S_19_Gather_Resource")},
			{TEXT("craft_item"), TEXT("/Game/GoonZu/Audio/SFX/S_20_Craft_Item.S_20_Craft_Item")},
			{TEXT("level_up"), TEXT("/Game/GoonZu/Audio/SFX/S_21_Level_Up.S_21_Level_Up")},
			{TEXT("dungeon_enter"), TEXT("/Game/GoonZu/Audio/SFX/S_22_Dungeon_Enter.S_22_Dungeon_Enter")},
			{TEXT("encounter_start"), TEXT("/Game/GoonZu/Audio/SFX/S_23_Encounter_Start.S_23_Encounter_Start")},
			{TEXT("boss_warning"), TEXT("/Game/GoonZu/Audio/SFX/S_24_Boss_Warning.S_24_Boss_Warning")},
			{TEXT("encounter_complete"), TEXT("/Game/GoonZu/Audio/SFX/S_25_Encounter_Complete.S_25_Encounter_Complete")},
			{TEXT("trial_rank"), TEXT("/Game/GoonZu/Audio/SFX/S_26_Trial_Rank.S_26_Trial_Rank")},
			{TEXT("footstep_stone"), TEXT("/Game/GoonZu/Audio/SFX/S_27_Footstep_Stone.S_27_Footstep_Stone")},
			{TEXT("footstep_grass"), TEXT("/Game/GoonZu/Audio/SFX/S_28_Footstep_Grass.S_28_Footstep_Grass")},
			{TEXT("door_stone"), TEXT("/Game/GoonZu/Audio/SFX/S_29_Door_Stone.S_29_Door_Stone")},
			{TEXT("coin_reward"), TEXT("/Game/GoonZu/Audio/SFX/S_30_Coin_Reward.S_30_Coin_Reward")}
		};
		return Assets;
	}
}

void UGZAudioSubsystem::Deinitialize()
{
	if (MusicComponent)
	{
		MusicComponent->Stop();
		MusicComponent = nullptr;
	}
	LoadedCues.Reset();
	CurrentMusicCue = NAME_None;
	Super::Deinitialize();
}

USoundBase* UGZAudioSubsystem::LoadCue(FName CueId, bool bMusic)
{
	const FName CacheKey(*(FString(bMusic ? TEXT("music_") : TEXT("sfx_")) + CueId.ToString()));
	if (TObjectPtr<USoundBase>* Cached = LoadedCues.Find(CacheKey))
	{
		return Cached->Get();
	}

	const TArray<FGZAudioAsset>& Assets = bMusic ? MusicAssets() : SoundEffectAssets();
	const FGZAudioAsset* Asset = Assets.FindByPredicate([CueId](const FGZAudioAsset& Candidate)
	{
		return Candidate.Id == CueId;
	});
	if (!Asset)
	{
		UE_LOG(LogTemp, Warning, TEXT("Unknown %s cue ID: %s"), bMusic ? TEXT("music") : TEXT("sound effect"), *CueId.ToString());
		return nullptr;
	}

	USoundBase* Sound = Cast<USoundBase>(StaticLoadObject(USoundBase::StaticClass(), nullptr, *Asset->ObjectPath));
	if (!Sound)
	{
		UE_LOG(LogTemp, Warning, TEXT("Audio asset for cue '%s' was not found at '%s'. Import the generated WAV files in Content/GoonZu/Audio."),
			*CueId.ToString(), *Asset->ObjectPath);
		return nullptr;
	}

	LoadedCues.Add(CacheKey, Sound);
	return Sound;
}

bool UGZAudioSubsystem::PlayMusicById(FName CueId, float FadeSeconds)
{
	if (CueId == CurrentMusicCue && MusicComponent && MusicComponent->IsPlaying())
	{
		return true;
	}

	USoundBase* Sound = LoadCue(CueId, true);
	if (!Sound) return false;

	if (USoundWave* Wave = Cast<USoundWave>(Sound))
	{
		Wave->bLooping = true;
	}

	UGameInstance* GameInstance = GetGameInstance();
	UWorld* World = GameInstance ? GameInstance->GetWorld() : nullptr;
	if (!World)
	{
		UE_LOG(LogTemp, Warning, TEXT("Cannot play music cue '%s' because the game instance has no active world."), *CueId.ToString());
		return false;
	}

	if (MusicComponent)
	{
		MusicComponent->FadeOut(FMath::Max(0.f, FadeSeconds), 0.f);
	}

	MusicComponent = UGameplayStatics::CreateSound2D(World, Sound, 0.f, 1.f, 0.f, nullptr, true, true);
	if (!MusicComponent)
	{
		UE_LOG(LogTemp, Warning, TEXT("Could not create audio component for music cue '%s'."), *CueId.ToString());
		CurrentMusicCue = NAME_None;
		return false;
	}

	MusicComponent->FadeIn(FMath::Max(0.f, FadeSeconds), MusicVolume, 0.f);
	CurrentMusicCue = CueId;
	OnMusicCueChanged.Broadcast(CueId);
	return true;
}

void UGZAudioSubsystem::StopMusic(float FadeSeconds)
{
	if (MusicComponent)
	{
		MusicComponent->FadeOut(FMath::Max(0.f, FadeSeconds), 0.f);
		MusicComponent = nullptr;
	}
	if (!CurrentMusicCue.IsNone())
	{
		CurrentMusicCue = NAME_None;
		OnMusicCueChanged.Broadcast(NAME_None);
	}
}

bool UGZAudioSubsystem::PlaySoundEffectById(FName CueId, float VolumeMultiplier)
{
	USoundBase* Sound = LoadCue(CueId, false);
	if (!Sound) return false;

	UGameInstance* GameInstance = GetGameInstance();
	UWorld* World = GameInstance ? GameInstance->GetWorld() : nullptr;
	if (!World)
	{
		UE_LOG(LogTemp, Warning, TEXT("Cannot play sound effect cue '%s' because the game instance has no active world."), *CueId.ToString());
		return false;
	}

	UAudioComponent* Component = UGameplayStatics::SpawnSound2D(
		World, Sound, SoundEffectVolume * FMath::Max(0.f, VolumeMultiplier), 1.f, 0.f, nullptr, false, true);
	if (!Component)
	{
		UE_LOG(LogTemp, Warning, TEXT("Could not play sound effect cue '%s'."), *CueId.ToString());
		return false;
	}
	return true;
}

void UGZAudioSubsystem::SetMusicVolume(float Volume)
{
	MusicVolume = FMath::Clamp(Volume, 0.f, 1.f);
	if (MusicComponent)
	{
		MusicComponent->SetVolumeMultiplier(MusicVolume);
	}
}

void UGZAudioSubsystem::SetSoundEffectVolume(float Volume)
{
	SoundEffectVolume = FMath::Clamp(Volume, 0.f, 1.f);
}
