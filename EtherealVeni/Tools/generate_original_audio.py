#!/usr/bin/env python3
"""Render original synthesized fantasy-RPG music loops and gameplay sound effects."""

import argparse
import json
import math
import random
import struct
import wave
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SAMPLE_RATE = 22050

MUSIC = [
    ("frostmere_wilds", "Frostmere Wilds", "regional", 72, "dorian"),
    ("frostmere_town", "Frostmere Town", "town", 84, "major"),
    ("emberfall_fields", "Emberfall Fields", "regional", 78, "minor"),
    ("emberfall_market", "Emberfall Market", "town", 96, "major"),
    ("greenhollow_canopy", "Greenhollow Canopy", "regional", 74, "lydian"),
    ("greenhollow_village", "Greenhollow Village", "town", 88, "major"),
    ("sunreach_dunes", "Sunreach Dunes", "regional", 76, "phrygian"),
    ("stormwatch_coast", "Stormwatch Coast", "regional", 82, "mixolydian"),
    ("moonfen_mist", "Moonfen Mist", "regional", 68, "dorian"),
    ("skyreach_heights", "Skyreach Heights", "regional", 80, "lydian"),
    ("deepdelve_echoes", "Deepdelve Echoes", "regional", 70, "minor"),
    ("thornvale_frontier", "Thornvale Frontier", "regional", 86, "mixolydian"),
    ("starfall_shore", "Starfall Shore", "regional", 72, "major"),
    ("capital_day", "Capital by Day", "town", 92, "major"),
    ("capital_night", "Capital at Night", "town", 70, "dorian"),
    ("traveler_inn", "Traveler's Inn", "town", 88, "major"),
    ("harbor_lanterns", "Harbor Lanterns", "town", 78, "mixolydian"),
    ("guild_hall", "Guild Hall", "town", 82, "dorian"),
    ("hollow_cistern", "Hollow Cistern", "dungeon", 66, "minor"),
    ("cinder_foundry", "Cinder Foundry", "dungeon", 78, "phrygian"),
    ("gloam_sanctum", "Gloam Sanctum", "dungeon", 68, "dorian"),
    ("sunken_archive", "Sunken Archive", "dungeon", 72, "minor"),
    ("thorn_bastion", "Thorn Bastion", "dungeon", 84, "phrygian"),
    ("frost_monastery", "Frost Monastery", "dungeon", 70, "dorian"),
    ("star_vault", "Star Vault", "dungeon", 76, "lydian"),
    ("ash_citadel", "Ash Citadel", "dungeon", 82, "minor"),
    ("raid_approach", "Raid: The Long Approach", "raid", 88, "dorian"),
    ("raid_battle", "Raid: The Turning Battle", "raid", 104, "minor"),
    ("raid_victory", "Raid: A Shared Victory", "raid", 96, "major"),
    ("trial_focus", "Trial: Quiet Focus", "trial", 80, "dorian"),
    ("trial_triumph", "Trial: Earned Triumph", "trial", 108, "major"),
    ("world_awakening", "World Awakening", "world", 76, "lydian"),
]

SFX = [
    ("ui_menu_open", "UI Menu Open", "bright"),
    ("ui_menu_close", "UI Menu Close", "soft"),
    ("ui_select", "UI Select", "bright"),
    ("ui_error", "UI Error", "low"),
    ("quest_accepted", "Quest Accepted", "bright"),
    ("quest_progress", "Quest Progress", "soft"),
    ("quest_complete", "Quest Complete", "triumph"),
    ("loot_common", "Loot Common", "bright"),
    ("loot_rare", "Loot Rare", "sparkle"),
    ("loot_epic", "Loot Epic", "triumph"),
    ("loot_legendary", "Loot Legendary", "grand"),
    ("weapon_swing", "Weapon Swing", "swish"),
    ("weapon_hit", "Weapon Hit", "impact"),
    ("shield_block", "Shield Block", "metal"),
    ("critical_hit", "Critical Hit", "impact"),
    ("bow_release", "Bow Release", "swish"),
    ("spell_cast", "Spell Cast", "sparkle"),
    ("spell_impact", "Spell Impact", "impact"),
    ("gather_resource", "Gather Resource", "metal"),
    ("craft_item", "Craft Item", "metal"),
    ("level_up", "Level Up", "triumph"),
    ("dungeon_enter", "Dungeon Enter", "low"),
    ("encounter_start", "Encounter Start", "grand"),
    ("boss_warning", "Boss Warning", "low"),
    ("encounter_complete", "Encounter Complete", "triumph"),
    ("trial_rank", "Trial Rank", "sparkle"),
    ("footstep_stone", "Footstep Stone", "step"),
    ("footstep_grass", "Footstep Grass", "step"),
    ("door_stone", "Stone Door", "impact"),
    ("coin_reward", "Coin Reward", "bright"),
]

SCALES = {
    "major": [0, 2, 4, 5, 7, 9, 11],
    "minor": [0, 2, 3, 5, 7, 8, 10],
    "dorian": [0, 2, 3, 5, 7, 9, 10],
    "phrygian": [0, 1, 3, 5, 7, 8, 10],
    "lydian": [0, 2, 4, 6, 7, 9, 11],
    "mixolydian": [0, 2, 4, 5, 7, 9, 10],
}


def midi_hz(note):
    return 440.0 * (2.0 ** ((note - 69) / 12.0))


def note_wave(frequency, time, voice):
    phase = 2.0 * math.pi * frequency * time
    if voice == "pad":
        return 0.72 * math.sin(phase) + 0.18 * math.sin(phase * 2.003) + 0.10 * math.sin(phase * 0.5)
    if voice == "bass":
        return 0.82 * math.sin(phase) + 0.18 * math.sin(phase * 2.0)
    return 0.62 * math.sin(phase) + 0.23 * math.sin(phase * 2.0) + 0.15 * math.sin(phase * 3.01)


def add_note(buffer, start, duration, note, amplitude, voice, pan=0.5, seed=0):
    first = max(0, int(start * SAMPLE_RATE))
    last = min(len(buffer), int((start + duration) * SAMPLE_RATE))
    frequency = midi_hz(note)
    attack = min(0.7 if voice == "pad" else 0.008, duration * 0.35)
    release = min(0.9 if voice == "pad" else 0.12, duration * 0.4)
    left_gain = math.cos(pan * math.pi * 0.5)
    right_gain = math.sin(pan * math.pi * 0.5)
    tremolo_rate = 0.18 + (seed % 7) * 0.035
    for frame in range(first, last):
        time = frame / SAMPLE_RATE - start
        envelope = min(1.0, time / max(attack, 0.001), (duration - time) / max(release, 0.001))
        if voice == "pluck":
            envelope *= math.exp(-time * 2.3)
        if voice == "pad":
            envelope *= 0.92 + 0.08 * math.sin(2.0 * math.pi * tremolo_rate * time + seed)
        sample = amplitude * envelope * note_wave(frequency, time, voice)
        buffer[frame][0] += sample * left_gain
        buffer[frame][1] += sample * right_gain


def render_music(index, cue):
    cue_id, _, group, tempo, scale_name = cue
    randomizer = random.Random(12000 + index * 97)
    bar_seconds = 240.0 / tempo
    duration = bar_seconds * 4
    frame_count = int(duration * SAMPLE_RATE)
    buffer = [[0.0, 0.0] for _ in range(frame_count)]
    scale = SCALES[scale_name]
    root = 42 + (index * 3) % 12
    progression = [0, 5, 3, 4] if scale_name in ("major", "lydian", "mixolydian") else [0, 3, 5, 4]
    if group == "raid":
        amplitude, density = 0.13, 1.0
    elif group == "dungeon":
        amplitude, density = 0.10, 0.65
    elif group == "trial":
        amplitude, density = 0.12, 0.8
    else:
        amplitude, density = 0.115, 0.75

    for bar in range(4):
        start = bar * bar_seconds
        degree = progression[bar]
        chord_notes = [root + scale[degree % 7], root + scale[(degree + 2) % 7] + 12, root + scale[(degree + 4) % 7] + 12]
        if group in ("dungeon", "raid"):
            chord_notes = [note - 12 if position == 1 else note for position, note in enumerate(chord_notes)]
        for voice_index, note in enumerate(chord_notes):
            add_note(buffer, start, bar_seconds, note, amplitude * (0.44 if voice_index else 0.58),
                     "pad", 0.28 + voice_index * 0.22, index + bar + voice_index)
        add_note(buffer, start, bar_seconds, root - 12 + scale[degree % 7], amplitude * 0.52, "bass", 0.5, index)
        beat = bar_seconds / 4
        for beat_index in range(4):
            if randomizer.random() > density:
                continue
            degree_index = randomizer.choice([0, 1, 2, 4, 5, 6])
            note = root + 12 + scale[(degree + degree_index) % 7]
            if randomizer.random() < 0.25:
                note += 12
            add_note(buffer, start + beat_index * beat, beat * randomizer.choice([0.72, 0.95]),
                     note, amplitude * randomizer.uniform(0.12, 0.25), "pluck",
                     randomizer.uniform(0.18, 0.82), index + beat_index)

    # A quiet, deterministic delay adds depth without external samples.
    delay = int(0.19 * SAMPLE_RATE)
    for frame in range(delay, frame_count):
        buffer[frame][0] += buffer[frame - delay][1] * 0.12
        buffer[frame][1] += buffer[frame - delay][0] * 0.12
    return encode_pcm(buffer)


def render_sfx(index, cue):
    cue_id, _, style = cue
    duration = 1.4 if style in ("grand", "triumph") else 0.85
    if style == "step":
        duration = 0.22
    frame_count = int(duration * SAMPLE_RATE)
    buffer = [[0.0, 0.0] for _ in range(frame_count)]
    randomizer = random.Random(88000 + index * 37)
    base = 220 + (index * 41) % 500
    for voice_index in range(3 if style in ("grand", "triumph", "sparkle") else 1):
        frequency = base * (1.0 + voice_index * (0.25 if style == "sparkle" else 0.5))
        phase = randomizer.random()
        for frame in range(frame_count):
            time = frame / SAMPLE_RATE
            decay = math.exp(-time * (2.5 if style in ("grand", "triumph") else 8.0))
            attack = min(1.0, time * 90)
            if style == "swish":
                current_frequency = frequency * (1.0 + (frame / max(1, frame_count)) * 0.8)
                sample = math.sin(2 * math.pi * current_frequency * time + phase) * decay * 0.12
                sample += randomizer.uniform(-1.0, 1.0) * decay * 0.035
            elif style in ("impact", "metal", "step"):
                noise = randomizer.uniform(-1.0, 1.0) * math.exp(-time * 38)
                sample = (math.sin(2 * math.pi * frequency * time) * decay * 0.28 + noise * 0.16)
            else:
                sample = math.sin(2 * math.pi * frequency * time + phase) * decay * 0.28
            sample *= attack
            buffer[frame][0] += sample * (0.65 if voice_index % 2 else 1.0)
            buffer[frame][1] += sample * (1.0 if voice_index % 2 else 0.65)
    return encode_pcm(buffer)


def encode_pcm(buffer):
    peak = max((abs(value) for frame in buffer for value in frame), default=0.0)
    gain = min(0.88 / peak, 1.0) if peak else 1.0
    packed = bytearray()
    for left, right in buffer:
        packed.extend(struct.pack("<hh",
                                  max(-32767, min(32767, int(left * gain * 32767))),
                                  max(-32767, min(32767, int(right * gain * 32767)))))
    return packed


def write_wave(path, pcm):
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "wb") as file:
        file.setnchannels(2)
        file.setsampwidth(2)
        file.setframerate(SAMPLE_RATE)
        file.writeframes(pcm)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, default=ROOT / "Content" / "GoonZu" / "Audio")
    args = parser.parse_args()
    output = args.output.resolve()
    music_manifest = []
    sfx_manifest = []

    for index, cue in enumerate(MUSIC, 1):
        cue_id, title, group, tempo, scale = cue
        filename = f"M_{index:02d}_{cue_id.title().replace('_', '_')}.wav"
        write_wave(output / "Music" / filename, render_music(index, cue))
        music_manifest.append({
            "id": cue_id, "title": title, "category": group, "tempo_bpm": tempo,
            "scale": scale, "file": f"Music/{filename}", "loop": True,
            "note": "Original synthesized cue; no third-party samples or borrowed melodies.",
        })

    for index, cue in enumerate(SFX, 1):
        cue_id, title, _ = cue
        filename = f"S_{index:02d}_{cue_id.title().replace('_', '_')}.wav"
        write_wave(output / "SFX" / filename, render_sfx(index, cue))
        sfx_manifest.append({
            "id": cue_id, "title": title, "file": f"SFX/{filename}",
            "note": "Original synthesized effect; no third-party samples.",
        })

    manifest = {
        "version": 1,
        "music_tracks": music_manifest,
        "sound_effects": sfx_manifest,
        "license": "Original procedural prototype assets generated for Ethereal Veni.",
    }
    manifest_path = output / "audio_catalog.json"
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"Rendered {len(music_manifest)} music loops and {len(sfx_manifest)} SFX to {output}")
    print(f"Manifest: {manifest_path}")


if __name__ == "__main__":
    main()
