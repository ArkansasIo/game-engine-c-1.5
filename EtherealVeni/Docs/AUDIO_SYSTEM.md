# Original UE5 Audio Pack

The UE5 project includes an original procedural audio prototype: 32 looping music cues and 30 gameplay sound effects. The arrangement and sound design use broad fantasy-MMORPG conventions, but the melodies, synthesis, titles, and assets are original; no WoW audio or third-party samples are included.

## Generate and import

From the project root, render the WAV files and manifest with Python 3:

```powershell
python EtherealVeni/Tools/generate_original_audio.py
```

Or use the Python runtime bundled with UE 5.8:

```powershell
& "C:\Program Files\Epic Games\UE_5.8\Engine\Binaries\ThirdParty\Python3\Win64\python.exe" `
  "EtherealVeni\Tools\generate_original_audio.py"
```

This writes stereo 22.05 kHz PCM WAV files and `audio_catalog.json` below `EtherealVeni/Content/GoonZu/Audio/`. Open the project in Unreal Editor and allow the files to import. The code references the imported assets at `/Game/GoonZu/Audio/Music/` and `/Game/GoonZu/Audio/SFX/`.

The cues are synthesized placeholder compositions rather than recorded orchestral performances. Replace or re-mix the imported WAVs in Unreal while preserving the filenames to keep the runtime cue IDs stable.

## Runtime API

`UGZAudioSubsystem` is a `UGameInstanceSubsystem`, accessible from C++ or Blueprint through the Game Instance:

- `PlayMusicById(CueId, FadeSeconds)` crossfades to a looping music track.
- `StopMusic(FadeSeconds)` fades out the current track.
- `PlaySoundEffectById(CueId, VolumeMultiplier)` plays a one-shot effect.
- `SetMusicVolume(Volume)` and `SetSoundEffectVolume(Volume)` adjust independent levels from 0 to 1.
- `OnMusicCueChanged` reports the current cue ID.

Example IDs include `frostmere_wilds`, `raid_battle`, `ui_select`, `quest_complete`, `weapon_hit`, and `loot_legendary`. Zone changes choose a regional cue automatically. Combat, gathering, crafting, quest completion, level-up, and HUD selection events play matching effects. Audio calls log a warning and return `false` if a cue ID or imported asset is missing.

## Catalog

Music themes cover regional exploration, towns, dungeons, raids, trials, and a world theme. Sound effects cover UI, quests, rarity-tier loot, weapons, spells, gathering/crafting, progression, instance encounters, footsteps, doors, and currency. See `Content/GoonZu/Audio/audio_catalog.json` for generated IDs and filenames.
