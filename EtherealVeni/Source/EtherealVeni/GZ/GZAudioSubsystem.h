#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "GZAudioSubsystem.generated.h"

class UAudioComponent;
class USoundBase;

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FGZMusicCueChanged, FName, CueId);

UCLASS()
class ETHEREALVENI_API UGZAudioSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Deinitialize() override;

	UFUNCTION(BlueprintCallable, Category = "Audio|Music")
	bool PlayMusicById(FName CueId, float FadeSeconds = 1.5f);

	UFUNCTION(BlueprintCallable, Category = "Audio|Music")
	void StopMusic(float FadeSeconds = 1.5f);

	UFUNCTION(BlueprintPure, Category = "Audio|Music")
	FName GetCurrentMusicCue() const { return CurrentMusicCue; }

	UFUNCTION(BlueprintCallable, Category = "Audio|Effects")
	bool PlaySoundEffectById(FName CueId, float VolumeMultiplier = 1.0f);

	UFUNCTION(BlueprintCallable, Category = "Audio|Settings")
	void SetMusicVolume(float Volume);

	UFUNCTION(BlueprintCallable, Category = "Audio|Settings")
	void SetSoundEffectVolume(float Volume);

	UFUNCTION(BlueprintPure, Category = "Audio|Settings")
	float GetMusicVolume() const { return MusicVolume; }

	UFUNCTION(BlueprintPure, Category = "Audio|Settings")
	float GetSoundEffectVolume() const { return SoundEffectVolume; }

	UPROPERTY(BlueprintAssignable, Category = "Audio|Music")
	FGZMusicCueChanged OnMusicCueChanged;

private:
	USoundBase* LoadCue(FName CueId, bool bMusic);

	UPROPERTY(Transient)
	TObjectPtr<UAudioComponent> MusicComponent;

	UPROPERTY(Transient)
	TMap<FName, TObjectPtr<USoundBase>> LoadedCues;

	FName CurrentMusicCue = NAME_None;
	float MusicVolume = 0.65f;
	float SoundEffectVolume = 0.8f;
};
