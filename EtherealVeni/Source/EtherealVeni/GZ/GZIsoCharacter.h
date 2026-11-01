#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "GZIsoCharacter.generated.h"

class USpringArmComponent;
class UCameraComponent;
class UStaticMeshComponent;
class UGZRPGComponent;

/** A world object the player can interact with (built by the level builder and found via actor tags). */
struct FGZWorldObject
{
	TWeakObjectPtr<AActor> Actor;
	FString Kind;     // zone | npc | monster | resource
	FString Name;
	FString Arg1;     // zone: radius | npc: role | monster: level | resource: item
	FString Arg2;     // zone: town/wild | npc: town | resource: skill
	float Radius = 0.f;
	float HP = 0.f;
	float MaxHP = 0.f;
	int32 Level = 1;
	bool bDead = false;
};

/** Isometric GoonZu-style player: WASD or click-to-move, gathering, combat, NPC interaction. */
UCLASS()
class ETHEREALVENI_API AGZIsoCharacter : public ACharacter
{
	GENERATED_BODY()

public:
	AGZIsoCharacter();
	virtual void Tick(float DeltaSeconds) override;

	UGZRPGComponent* GetRPG() const { return RPG; }
	const TArray<FGZWorldObject>& GetWorldObjects() const { return WorldObjects; }
	float GetCameraYaw() const;
	const FGZWorldObject* GetTarget() const;
	const FGZWorldObject* GetInteractable() const;

	/** Same as pressing Space. Used by the target frame dropdown. */
	void AttackTarget();
	/** Drops the target and pauses auto-targeting until the player picks one (Tab / click). */
	void ClearTarget();

protected:
	virtual void BeginPlay() override;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<USpringArmComponent> CameraBoom;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<UCameraComponent> IsoCamera;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<UStaticMeshComponent> BodyMesh;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<UStaticMeshComponent> HeadMesh;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<UStaticMeshComponent> HatMesh;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "GoonZu")
	TObjectPtr<UGZRPGComponent> RPG;

	UPROPERTY(EditAnywhere, Category = "GoonZu")
	float MinZoom = 900.f;

	UPROPERTY(EditAnywhere, Category = "GoonZu")
	float MaxZoom = 5000.f;

private:
	void ScanWorld();
	void UpdateZone();
	void HandleHotkeys(class APlayerController* PC);
	void TryAttack();
	void TryGather();
	void TryInteract();
	void Die();
	void ApplyRetro(bool bOn);
	virtual void EndPlay(const EEndPlayReason::Type Reason) override;
	void SetObjectHidden(AActor* Actor, bool bHide);
	FVector GetRespawnPoint() const;

	TArray<FGZWorldObject> WorldObjects;
	TArray<TWeakObjectPtr<AActor>> Labels;
	bool bRetro = true;
	FVector ClickTarget = FVector::ZeroVector;
	bool bHasClickTarget = false;
	float TargetYaw = 45.f;
	int32 TargetIndex = INDEX_NONE;
	bool bSuppressAutoTarget = false;
	int32 InteractIndex = INDEX_NONE;
	FString LastZone;
	FVector SpawnPoint = FVector::ZeroVector;
};
