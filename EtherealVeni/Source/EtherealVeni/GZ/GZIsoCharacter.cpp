#include "GZIsoCharacter.h"
#include "GZAudioSubsystem.h"
#include "GZRPGComponent.h"
#include "GZHUD.h"
#include "UI/EVUISubsystem.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/PlayerController.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"
#include "EngineUtils.h"
#include "TimerManager.h"
#include "Engine/World.h"
#include "Engine/GameInstance.h"

namespace
{
	void PlayGZSound(const UObject* WorldContext, FName CueId, float Volume = 1.f)
	{
		const UWorld* World = WorldContext ? WorldContext->GetWorld() : nullptr;
		UGameInstance* GameInstance = World ? World->GetGameInstance() : nullptr;
		UGZAudioSubsystem* Audio = GameInstance ? GameInstance->GetSubsystem<UGZAudioSubsystem>() : nullptr;
		if (Audio)
		{
			Audio->PlaySoundEffectById(CueId, Volume);
		}
	}

	FName MusicCueForZone(const FString& Zone, bool bTown)
	{
		const FString LowerZone = Zone.ToLower();
		if (LowerZone.Contains(TEXT("frostmere"))) return bTown ? TEXT("frostmere_town") : TEXT("frostmere_wilds");
		if (LowerZone.Contains(TEXT("emberfall"))) return bTown ? TEXT("emberfall_market") : TEXT("emberfall_fields");
		if (LowerZone.Contains(TEXT("greenhollow"))) return bTown ? TEXT("greenhollow_village") : TEXT("greenhollow_canopy");
		if (LowerZone.Contains(TEXT("sunreach"))) return TEXT("sunreach_dunes");
		if (LowerZone.Contains(TEXT("stormwatch"))) return TEXT("stormwatch_coast");
		if (LowerZone.Contains(TEXT("moonfen"))) return TEXT("moonfen_mist");
		if (LowerZone.Contains(TEXT("skyreach"))) return TEXT("skyreach_heights");
		if (LowerZone.Contains(TEXT("deepdelve"))) return TEXT("deepdelve_echoes");
		if (LowerZone.Contains(TEXT("thornvale"))) return TEXT("thornvale_frontier");
		if (LowerZone.Contains(TEXT("starfall"))) return TEXT("starfall_shore");
		return bTown ? FName(TEXT("capital_day")) : FName(TEXT("world_awakening"));
	}
}

AGZIsoCharacter::AGZIsoCharacter()
{
	PrimaryActorTick.bCanEverTick = true;

	GetCapsuleComponent()->InitCapsuleSize(40.f, 96.f);

	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;

	UCharacterMovementComponent* Move = GetCharacterMovement();
	Move->bOrientRotationToMovement = true;
	Move->RotationRate = FRotator(0.f, 720.f, 0.f);
	Move->MaxWalkSpeed = 650.f;
	Move->bConstrainToPlane = false;

	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->SetUsingAbsoluteRotation(true);
	CameraBoom->TargetArmLength = 2200.f;
	CameraBoom->SetRelativeRotation(FRotator(-50.f, 45.f, 0.f));
	CameraBoom->bDoCollisionTest = false;
	CameraBoom->bEnableCameraLag = true;
	CameraBoom->CameraLagSpeed = 8.f;

	IsoCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("IsoCamera"));
	IsoCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);
	IsoCamera->bUsePawnControlRotation = false;
	IsoCamera->SetFieldOfView(45.f);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> Cylinder(TEXT("/Engine/BasicShapes/Cylinder.Cylinder"));
	static ConstructorHelpers::FObjectFinder<UStaticMesh> Sphere(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
	static ConstructorHelpers::FObjectFinder<UStaticMesh> Cone(TEXT("/Engine/BasicShapes/Cone.Cone"));

	BodyMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Body"));
	BodyMesh->SetupAttachment(RootComponent);
	BodyMesh->SetRelativeLocation(FVector(0.f, 0.f, -30.f));
	BodyMesh->SetRelativeScale3D(FVector(0.7f, 0.7f, 1.3f));
	BodyMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	if (Cylinder.Succeeded()) BodyMesh->SetStaticMesh(Cylinder.Object);

	HeadMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Head"));
	HeadMesh->SetupAttachment(RootComponent);
	HeadMesh->SetRelativeLocation(FVector(0.f, 0.f, 62.f));
	HeadMesh->SetRelativeScale3D(FVector(0.62f));
	HeadMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	if (Sphere.Succeeded()) HeadMesh->SetStaticMesh(Sphere.Object);

	HatMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Hat"));
	HatMesh->SetupAttachment(RootComponent);
	HatMesh->SetRelativeLocation(FVector(0.f, 0.f, 100.f));
	HatMesh->SetRelativeScale3D(FVector(1.1f, 1.1f, 0.35f));
	HatMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	if (Cone.Succeeded()) HatMesh->SetStaticMesh(Cone.Object);

	RPG = CreateDefaultSubobject<UGZRPGComponent>(TEXT("RPG"));
}

void AGZIsoCharacter::BeginPlay()
{
	Super::BeginPlay();
	SpawnPoint = GetActorLocation();

	auto Tint = [](UStaticMeshComponent* C, const FLinearColor& Color)
	{
		if (UMaterialInstanceDynamic* M = C->CreateDynamicMaterialInstance(0))
		{
			M->SetVectorParameterValue(TEXT("Color"), Color);
		}
	};
	Tint(BodyMesh, FLinearColor(0.12f, 0.32f, 0.85f));
	Tint(HeadMesh, FLinearColor(0.95f, 0.78f, 0.62f));
	Tint(HatMesh, FLinearColor(0.08f, 0.08f, 0.1f));

	ScanWorld();
	ApplyRetro(bRetro);
}

void AGZIsoCharacter::ApplyRetro(bool bOn)
{
	// 8-bit look: render the 3D world at low resolution and upscale with nearest-neighbour filtering.
	APlayerController* PC = Cast<APlayerController>(GetController());
	if (!PC) return;
	if (bOn)
	{
		PC->ConsoleCommand(TEXT("r.AntiAliasingMethod 0"));
		PC->ConsoleCommand(TEXT("r.ScreenPercentage 33"));
		PC->ConsoleCommand(TEXT("r.Upscale.Quality 0"));
	}
	else
	{
		PC->ConsoleCommand(TEXT("r.ScreenPercentage 100"));
		PC->ConsoleCommand(TEXT("r.Upscale.Quality 3"));
		PC->ConsoleCommand(TEXT("r.AntiAliasingMethod 4"));
	}
	if (RPG)
	{
		RPG->AddChat(TEXT("System"), bOn ? TEXT("8-bit render mode ON (press P to toggle).") : TEXT("8-bit render mode OFF (press P to toggle)."), FLinearColor(0.7f, 0.9f, 1.f));
	}
}

void AGZIsoCharacter::EndPlay(const EEndPlayReason::Type Reason)
{
	// Leave the editor viewport the way we found it.
	if (bRetro) ApplyRetro(false);
	Super::EndPlay(Reason);
}

float AGZIsoCharacter::GetCameraYaw() const
{
	return CameraBoom ? CameraBoom->GetComponentRotation().Yaw : 45.f;
}

void AGZIsoCharacter::ScanWorld()
{
	WorldObjects.Reset();
	Labels.Reset();
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		AActor* A = *It;
		if (A->Tags.Num() == 1 && A->Tags[0] == FName(TEXT("GZLabel")))
		{
			Labels.Add(A);
			continue;
		}
		if (A->Tags.Num() < 2) continue;
		const FString Kind = A->Tags[0].ToString();
		FGZWorldObject O;
		O.Actor = A;
		O.Name = A->Tags[1].ToString();
		O.Arg1 = A->Tags.Num() > 2 ? A->Tags[2].ToString() : FString();
		O.Arg2 = A->Tags.Num() > 3 ? A->Tags[3].ToString() : FString();
		if (Kind == TEXT("GZZone"))
		{
			O.Kind = TEXT("zone");
			O.Radius = FCString::Atof(*O.Arg1);
		}
		else if (Kind == TEXT("GZNPC"))
		{
			O.Kind = TEXT("npc");
		}
		else if (Kind == TEXT("GZMonster"))
		{
			O.Kind = TEXT("monster");
			O.Level = FMath::Max(1, FCString::Atoi(*O.Arg1));
			O.MaxHP = 30.f + O.Level * 28.f;
			if (O.Name.Contains(TEXT("Drake"))) O.MaxHP *= 5.f;
			O.HP = O.MaxHP;
		}
		else if (Kind == TEXT("GZResource"))
		{
			O.Kind = TEXT("resource");
		}
		else
		{
			continue;
		}
		WorldObjects.Add(O);
	}
	if (RPG)
	{
		RPG->AddChat(TEXT("System"), FString::Printf(TEXT("World scan: %d zones/NPCs/monsters/resources found."), WorldObjects.Num()), FLinearColor(0.6f, 0.6f, 0.6f));
	}
}

const FGZWorldObject* AGZIsoCharacter::GetTarget() const
{
	return WorldObjects.IsValidIndex(TargetIndex) ? &WorldObjects[TargetIndex] : nullptr;
}

const FGZWorldObject* AGZIsoCharacter::GetInteractable() const
{
	return WorldObjects.IsValidIndex(InteractIndex) ? &WorldObjects[InteractIndex] : nullptr;
}

void AGZIsoCharacter::UpdateZone()
{
	const FVector Me = GetActorLocation();
	FString Best = TEXT("Wilderness");
	bool bTown = false;
	float BestD = TNumericLimits<float>::Max();
	float BestMonster = 1600.f;
	float BestInteract = 380.f;
	int32 NewTarget = INDEX_NONE;
	int32 NewInteract = INDEX_NONE;
	for (int32 i = 0; i < WorldObjects.Num(); ++i)
	{
		const FGZWorldObject& O = WorldObjects[i];
		if (!O.Actor.IsValid()) continue;
		const float D = FVector::Dist2D(O.Actor->GetActorLocation(), Me);
		if (O.Kind == TEXT("zone"))
		{
			// Towns win over wilderness zones when overlapping.
			const bool bIsTown = O.Arg2 == TEXT("town");
			if (D < O.Radius && (D < BestD || (bIsTown && !bTown)))
			{
				if (!bTown || bIsTown)
				{
					BestD = D;
					Best = O.Name;
					bTown = bIsTown;
				}
			}
		}
		else if (O.Kind == TEXT("monster") && !O.bDead && D < BestMonster)
		{
			BestMonster = D;
			NewTarget = i;
		}
		else if ((O.Kind == TEXT("npc") || O.Kind == TEXT("resource")) && D < BestInteract)
		{
			BestInteract = D;
			NewInteract = i;
		}
	}
	// Keep a manually clicked target while it is alive and reasonably close.
	if (!(WorldObjects.IsValidIndex(TargetIndex) && !WorldObjects[TargetIndex].bDead && WorldObjects[TargetIndex].Actor.IsValid()
		&& FVector::Dist2D(WorldObjects[TargetIndex].Actor->GetActorLocation(), Me) < 2500.f))
	{
		// Options > Gameplay > Controls > Auto-Target; "Clear Target" pauses it until the next manual pick.
		const bool bAutoTarget = UEVUISubsystem::GetBoolSettingFor(Cast<APlayerController>(GetController()), TEXT("controls.auto_target"), true);
		TargetIndex = (bAutoTarget && !bSuppressAutoTarget) ? NewTarget : INDEX_NONE;
	}
	InteractIndex = NewInteract;

	if (RPG)
	{
		RPG->CurrentZone = Best;
		RPG->bInTown = bTown;
		if (Best != LastZone)
		{
			LastZone = Best;
			RPG->AddChat(TEXT("System"), FString::Printf(TEXT("Entering %s%s"), *Best, bTown ? TEXT(" (town: market, manufacture and town hall available)") : TEXT("")), FLinearColor(0.85f, 0.85f, 0.6f));
			RPG->OnVisitZone(Best);
			if (UGameInstance* GameInstance = GetGameInstance())
			{
				if (UGZAudioSubsystem* Audio = GameInstance->GetSubsystem<UGZAudioSubsystem>())
				{
					Audio->PlayMusicById(MusicCueForZone(Best, bTown), 2.5f);
				}
			}
		}
	}
}

void AGZIsoCharacter::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);
	APlayerController* PC = Cast<APlayerController>(GetController());
	if (!PC || !RPG) return;
	AGZHUD* HUD = Cast<AGZHUD>(PC->GetHUD());

	UpdateZone();

	// Camera rotation (Q/E) and zoom.
	static const float CameraSteps[] = {15.f, 30.f, 45.f, 90.f};
	const UEVUISubsystem* UI = UEVUISubsystem::Get(PC);
	const float Step = CameraSteps[FMath::Clamp(UI && UI->FindSetting(TEXT("controls.camera_step")) ? UI->GetSettingInt(TEXT("controls.camera_step")) : 2, 0, 3)];
	if (PC->WasInputKeyJustPressed(EKeys::Q)) TargetYaw -= Step;
	if (PC->WasInputKeyJustPressed(EKeys::E) && !GetInteractable()) TargetYaw += Step;
	const FRotator BoomRot = CameraBoom->GetComponentRotation();
	const float NewYaw = FMath::FixedTurn(BoomRot.Yaw, TargetYaw, 240.f * DeltaSeconds);
	CameraBoom->SetWorldRotation(FRotator(-50.f, NewYaw, 0.f));

	const bool bOverUI = HUD && HUD->IsMouseOverUI();
	if (!bOverUI)
	{
		if (PC->WasInputKeyJustPressed(EKeys::MouseScrollUp))
			CameraBoom->TargetArmLength = FMath::Clamp(CameraBoom->TargetArmLength - 250.f, MinZoom, MaxZoom);
		if (PC->WasInputKeyJustPressed(EKeys::MouseScrollDown))
			CameraBoom->TargetArmLength = FMath::Clamp(CameraBoom->TargetArmLength + 250.f, MinZoom, MaxZoom);
	}

	// Keyboard movement relative to the isometric camera.
	const FVector Fwd = FRotator(0.f, NewYaw, 0.f).Vector();
	const FVector Right = FRotator(0.f, NewYaw + 90.f, 0.f).Vector();
	const float F = (PC->IsInputKeyDown(EKeys::W) || PC->IsInputKeyDown(EKeys::Up) ? 1.f : 0.f) - (PC->IsInputKeyDown(EKeys::S) || PC->IsInputKeyDown(EKeys::Down) ? 1.f : 0.f);
	const float R = (PC->IsInputKeyDown(EKeys::D) || PC->IsInputKeyDown(EKeys::Right) ? 1.f : 0.f) - (PC->IsInputKeyDown(EKeys::A) || PC->IsInputKeyDown(EKeys::Left) ? 1.f : 0.f);
	if (F != 0.f || R != 0.f)
	{
		bHasClickTarget = false;
		AddMovementInput(Fwd, F);
		AddMovementInput(Right, R);
	}

	// Click to move / click a monster to target it.
	if (PC->WasInputKeyJustPressed(EKeys::LeftMouseButton) && !bOverUI
		&& UEVUISubsystem::GetBoolSettingFor(PC, TEXT("controls.click_to_move"), true))
	{
		FHitResult Hit;
		if (PC->GetHitResultUnderCursor(ECC_Visibility, false, Hit) && Hit.bBlockingHit)
		{
			ClickTarget = Hit.ImpactPoint;
			bHasClickTarget = true;
			if (AActor* HitActor = Hit.GetActor())
			{
				AActor* Root = HitActor->GetAttachParentActor() ? HitActor->GetAttachParentActor() : HitActor;
				for (int32 i = 0; i < WorldObjects.Num(); ++i)
				{
					if (WorldObjects[i].Actor.Get() == Root && WorldObjects[i].Kind == TEXT("monster") && !WorldObjects[i].bDead)
					{
						TargetIndex = i;
						bSuppressAutoTarget = false;
					}
				}
			}
		}
	}
	if (bHasClickTarget)
	{
		FVector D = ClickTarget - GetActorLocation();
		D.Z = 0.f;
		if (D.SizeSquared() < 70.f * 70.f)
		{
			bHasClickTarget = false;
		}
		else
		{
			AddMovementInput(D.GetSafeNormal(), 1.f);
		}
	}

	HandleHotkeys(PC);

	if (PC->WasInputKeyJustPressed(EKeys::P))
	{
		bRetro = !bRetro;
		ApplyRetro(bRetro);
	}

	// Billboard world labels toward the camera.
	const FVector CamLoc = IsoCamera->GetComponentLocation();
	const FVector Me = GetActorLocation();
	for (const TWeakObjectPtr<AActor>& L : Labels)
	{
		AActor* A = L.Get();
		if (!A || A->IsHidden()) continue;
		const FVector Loc = A->GetActorLocation();
		if (FVector::DistSquared2D(Loc, Me) > 9000.f * 9000.f) continue;
		A->SetActorRotation((CamLoc - Loc).Rotation());
	}

	if (RPG->HP <= 0.f)
	{
		Die();
	}
}

void AGZIsoCharacter::HandleHotkeys(APlayerController* PC)
{
	AGZHUD* HUD = Cast<AGZHUD>(PC->GetHUD());
	if (PC->WasInputKeyJustPressed(EKeys::SpaceBar)) TryAttack();
	if (PC->WasInputKeyJustPressed(EKeys::G)) TryGather();
	if (PC->WasInputKeyJustPressed(EKeys::E) || PC->WasInputKeyJustPressed(EKeys::T)) TryInteract();
	if (PC->WasInputKeyJustPressed(EKeys::Tab))
	{
		// Cycle through nearby monsters.
		const FVector Me = GetActorLocation();
		int32 Next = INDEX_NONE;
		for (int32 Step = 1; Step <= WorldObjects.Num(); ++Step)
		{
			const int32 i = (FMath::Max(TargetIndex, 0) + Step) % WorldObjects.Num();
			const FGZWorldObject& O = WorldObjects[i];
			if (O.Kind == TEXT("monster") && !O.bDead && O.Actor.IsValid() && FVector::Dist2D(O.Actor->GetActorLocation(), Me) < 2500.f)
			{
				Next = i;
				break;
			}
		}
		if (Next != INDEX_NONE)
		{
			TargetIndex = Next;
			bSuppressAutoTarget = false;
		}
	}

	const FKey Quick[] = {EKeys::F1, EKeys::F2, EKeys::F3, EKeys::F4, EKeys::F5};
	for (int32 i = 0; i < 5; ++i)
	{
		if (PC->WasInputKeyJustPressed(Quick[i])) RPG->UseQuickSlot(i);
	}

	// With the MMO interface attached, window hotkeys and Escape are rebindable key bindings
	// handled by UEVUISubsystem (Options > Key Bindings). This block is the fallback without it.
	const UEVUISubsystem* UI = UEVUISubsystem::Get(PC);
	if (!HUD || (UI && UI->IsAttached())) return;
	if (PC->WasInputKeyJustPressed(EKeys::C)) HUD->ToggleWindow(EGZWindow::Character);
	if (PC->WasInputKeyJustPressed(EKeys::I)) HUD->ToggleWindow(EGZWindow::Inventory);
	if (PC->WasInputKeyJustPressed(EKeys::K)) HUD->ToggleWindow(EGZWindow::Skills);
	if (PC->WasInputKeyJustPressed(EKeys::U)) HUD->ToggleWindow(EGZWindow::Manufacture);
	if (PC->WasInputKeyJustPressed(EKeys::B)) HUD->ToggleWindow(EGZWindow::Market);
	if (PC->WasInputKeyJustPressed(EKeys::N)) HUD->ToggleWindow(EGZWindow::Town);
	if (PC->WasInputKeyJustPressed(EKeys::L)) HUD->ToggleWindow(EGZWindow::Quests);
	if (PC->WasInputKeyJustPressed(EKeys::M)) HUD->ToggleWindow(EGZWindow::WorldMap);
	if (PC->WasInputKeyJustPressed(EKeys::J)) HUD->ToggleWindow(EGZWindow::Community);
	if (PC->WasInputKeyJustPressed(EKeys::H)) HUD->ToggleWindow(EGZWindow::Help);
	if (PC->WasInputKeyJustPressed(EKeys::F10) || PC->WasInputKeyJustPressed(EKeys::Escape)) HUD->ToggleWindow(EGZWindow::GameMenu);
}

void AGZIsoCharacter::AttackTarget()
{
	TryAttack();
}

void AGZIsoCharacter::ClearTarget()
{
	TargetIndex = INDEX_NONE;
	bSuppressAutoTarget = true;
}

void AGZIsoCharacter::TryAttack()
{
	if (!WorldObjects.IsValidIndex(TargetIndex) || RPG->AttackCooldown > 0.f) return;
	FGZWorldObject& O = WorldObjects[TargetIndex];
	if (O.bDead || !O.Actor.IsValid()) return;
	const FVector Loc = O.Actor->GetActorLocation();
	if (FVector::Dist2D(Loc, GetActorLocation()) > 420.f)
	{
		// Walk toward the target first, like a click-attack.
		ClickTarget = Loc;
		bHasClickTarget = true;
		return;
	}
	bHasClickTarget = false;
	SetActorRotation(FRotator(0.f, (Loc - GetActorLocation()).Rotation().Yaw, 0.f));
	RPG->AttackCooldown = FMath::Max(0.35f, 0.9f - RPG->Attr(TEXT("Agility")) * 0.02f);
	RPG->CombatTimer = 6.f;

	const bool bCrit = FMath::FRand() < 0.05f + RPG->Attr(TEXT("Luck")) * 0.01f;
	float Dmg = RPG->GetAttack() * FMath::FRandRange(0.85f, 1.15f) * (bCrit ? 2.f : 1.f);
	Dmg = FMath::Max(1.f, Dmg - O.Level);
	O.HP -= Dmg;
	const bool bUsingBow = RPG->EquippedWeapon.Contains(TEXT("bow"));
	RPG->GiveSkillXp(bUsingBow ? TEXT("bow") : TEXT("sword"), 6.f);
	PlayGZSound(this, bUsingBow ? TEXT("bow_release") : TEXT("weapon_swing"), 0.7f);
	PlayGZSound(this, bCrit ? TEXT("critical_hit") : TEXT("weapon_hit"), bCrit ? 1.f : 0.65f);

	APlayerController* PC = Cast<APlayerController>(GetController());
	AGZHUD* HUD = PC ? Cast<AGZHUD>(PC->GetHUD()) : nullptr;
	if (HUD) HUD->AddFloatingText(Loc + FVector(0, 0, 160), FString::Printf(TEXT("%s%d"), bCrit ? TEXT("CRITICAL ") : TEXT(""), FMath::RoundToInt(Dmg)), bCrit ? FLinearColor(1.f, 0.8f, 0.1f) : FLinearColor::White);

	if (O.HP <= 0.f)
	{
		O.bDead = true;
		SetObjectHidden(O.Actor.Get(), true);
		const int32 ExpGain = 12 + O.Level * 14;
		RPG->GiveExp(ExpGain);
		RPG->OnKill(O.Name);
		PlayGZSound(this, O.Name.Contains(TEXT("Drake")) ? TEXT("loot_rare") : TEXT("loot_common"));

		// Loot tables by creature type (GoonZu-style: monsters drop production materials, equipment comes from players).
		TArray<TPair<FString, int32>> Loot;
		const FString N = O.Name.ToLower();
		if (N.Contains(TEXT("slime"))) Loot.Add(TPair<FString, int32>(TEXT("clear_water"), 2));
		if (N.Contains(TEXT("goblin"))) Loot.Add(TPair<FString, int32>(TEXT("iron_ore"), 2));
		if (N.Contains(TEXT("wolf")) || N.Contains(TEXT("boar"))) Loot.Add(TPair<FString, int32>(TEXT("leather_strip"), 2));
		if (N.Contains(TEXT("boar"))) Loot.Add(TPair<FString, int32>(TEXT("grain"), 1));
		if (N.Contains(TEXT("spider"))) Loot.Add(TPair<FString, int32>(TEXT("red_herb"), 2));
		if (N.Contains(TEXT("skeleton")) || N.Contains(TEXT("orc"))) Loot.Add(TPair<FString, int32>(TEXT("iron_ingot"), 1));
		if (N.Contains(TEXT("elemental")) || N.Contains(TEXT("guardian")) || N.Contains(TEXT("drake"))) Loot.Add(TPair<FString, int32>(TEXT("crystal_gem"), N.Contains(TEXT("drake")) ? 5 : 1));
		const int32 LumaDrop = FMath::RandRange(3, 8) * O.Level * (N.Contains(TEXT("bandit")) ? 4 : 1) * (N.Contains(TEXT("drake")) ? 20 : 1);
		RPG->GiveLuma(LumaDrop);
		FString LootText;
		for (const auto& L : Loot)
		{
			RPG->AddItem(L.Key, L.Value);
			LootText += FString::Printf(TEXT(", %s x%d"), *UGZRPGComponent::Pretty(L.Key), L.Value);
		}
		RPG->AddChat(TEXT("Combat"), FString::Printf(TEXT("Defeated %s (Lv.%d): +%d EXP, +%d Luma%s"), *O.Name, O.Level, ExpGain, LumaDrop, *LootText), FLinearColor(1.f, 0.7f, 0.4f));

		// Respawn later.
		const int32 Idx = TargetIndex;
		FTimerHandle Handle;
		GetWorldTimerManager().SetTimer(Handle, FTimerDelegate::CreateWeakLambda(this, [this, Idx]()
		{
			if (WorldObjects.IsValidIndex(Idx))
			{
				WorldObjects[Idx].bDead = false;
				WorldObjects[Idx].HP = WorldObjects[Idx].MaxHP;
				SetObjectHidden(WorldObjects[Idx].Actor.Get(), false);
			}
		}), O.Name.Contains(TEXT("Drake")) ? 120.f : 25.f, false);
		TargetIndex = INDEX_NONE;
		return;
	}

	// Counter-attack.
	const float Incoming = FMath::Max(1.f, O.Level * 4.f + FMath::FRandRange(0.f, O.Level * 2.f) - RPG->GetDefense() * 0.6f) * (O.Name.Contains(TEXT("Drake")) ? 2.5f : 1.f);
	if (FMath::FRand() > 0.05f + RPG->Attr(TEXT("Agility")) * 0.01f)
	{
		RPG->HP -= Incoming;
		if (HUD) HUD->AddFloatingText(GetActorLocation() + FVector(0, 0, 150), FString::Printf(TEXT("-%d"), FMath::RoundToInt(Incoming)), FLinearColor(1.f, 0.25f, 0.25f));
	}
	else if (HUD)
	{
		HUD->AddFloatingText(GetActorLocation() + FVector(0, 0, 150), TEXT("Dodge"), FLinearColor(0.6f, 0.9f, 1.f));
	}
}

void AGZIsoCharacter::TryGather()
{
	const FGZWorldObject* O = GetInteractable();
	if (!O || O->Kind != TEXT("resource"))
	{
		RPG->AddChat(TEXT("System"), TEXT("Nothing to gather here. Find an ore vein, oak grove, herb patch, farm field or fishing spot."), FLinearColor(0.8f, 0.8f, 0.8f));
		return;
	}
	if (RPG->GatherCooldown > 0.f) return;
	RPG->GatherCooldown = 1.2f;
	const FString Item = O->Arg1;
	const FString Skill = O->Arg2;
	const float Chance = FMath::Clamp(0.7f + RPG->SkillLevel(Skill) * 0.02f + RPG->Attr(TEXT("Luck")) * 0.005f, 0.f, 0.98f);
	RPG->GiveSkillXp(Skill, 9.f);
	APlayerController* PC = Cast<APlayerController>(GetController());
	AGZHUD* HUD = PC ? Cast<AGZHUD>(PC->GetHUD()) : nullptr;
	if (FMath::FRand() < Chance)
	{
		const int32 N = 1 + (FMath::FRand() < RPG->SkillLevel(Skill) * 0.03f ? 1 : 0);
		RPG->AddItem(Item, N);
		RPG->GiveExp(2.f);
		PlayGZSound(this, TEXT("gather_resource"), 0.75f);
		if (HUD) HUD->AddFloatingText(GetActorLocation() + FVector(0, 0, 150), FString::Printf(TEXT("+%d %s"), N, *UGZRPGComponent::Pretty(Item)), FLinearColor(0.5f, 1.f, 0.5f));
	}
	else if (HUD)
	{
		HUD->AddFloatingText(GetActorLocation() + FVector(0, 0, 150), TEXT("Failed"), FLinearColor(0.8f, 0.8f, 0.8f));
	}
}

void AGZIsoCharacter::TryInteract()
{
	const FGZWorldObject* O = GetInteractable();
	if (!O) return;
	if (O->Kind == TEXT("resource"))
	{
		TryGather();
		return;
	}
	APlayerController* PC = Cast<APlayerController>(GetController());
	AGZHUD* HUD = PC ? Cast<AGZHUD>(PC->GetHUD()) : nullptr;
	const FString NpcRole = O->Arg1;
	FString Line = TEXT("Good day, traveler.");
	EGZWindow Open = EGZWindow::None;
	if (NpcRole == TEXT("merchant") || NpcRole == TEXT("banker")) { Line = TEXT("Prices follow supply and demand here. Buy low, sell high!"); Open = EGZWindow::Market; }
	else if (NpcRole == TEXT("blacksmith")) { Line = TEXT("Bring me ingots and handles and I'll teach you to forge."); Open = EGZWindow::Manufacture; }
	else if (NpcRole == TEXT("alchemist")) { Line = TEXT("Herbs plus clear water make a fine tonic."); Open = EGZWindow::Manufacture; }
	else if (NpcRole == TEXT("mayor")) { Line = TEXT("Shareholders choose this town's head. Care to invest?"); Open = EGZWindow::Town; }
	else if (NpcRole == TEXT("quest_giver")) { Line = TEXT("I have work for capable hands."); Open = EGZWindow::Quests; }
	else if (NpcRole == TEXT("guild_master")) { Line = TEXT("Guilds share storage and glory. Join one!"); Open = EGZWindow::Community; }
	else if (NpcRole == TEXT("miner")) Line = TEXT("Iron veins glitter in the northern hills near Frostmere. Press G beside one.");
	else if (NpcRole == TEXT("fisher")) Line = TEXT("The lake by Greenhollow is full of fish.");
	else if (NpcRole == TEXT("farmer")) Line = TEXT("Grain from Emberfall's fields feeds the whole realm.");
	else if (NpcRole == TEXT("guard")) Line = TEXT("Beware: an Ember Drake nests in the eastern lair.");
	else if (NpcRole == TEXT("stable_master")) Line = TEXT("Mounts are coming soon to Luminara.");
	else if (NpcRole == TEXT("pet_trainer")) Line = TEXT("Pets will fight beside you one day.");
	RPG->AddChat(TEXT("NPC"), FString::Printf(TEXT("%s: %s"), *O->Name, *Line), FLinearColor(1.f, 1.f, 0.7f));
	if (HUD && Open != EGZWindow::None) HUD->OpenWindow(Open);
}

void AGZIsoCharacter::SetObjectHidden(AActor* Actor, bool bHide)
{
	if (!Actor) return;
	TArray<AActor*> Group;
	Actor->GetAttachedActors(Group, true, true);
	Group.Add(Actor);
	for (AActor* A : Group)
	{
		A->SetActorHiddenInGame(bHide);
		A->SetActorEnableCollision(!bHide);
	}
}

FVector AGZIsoCharacter::GetRespawnPoint() const
{
	return SpawnPoint;
}

void AGZIsoCharacter::Die()
{
	const int64 Lost = RPG->Luma / 10;
	RPG->Luma -= Lost;
	RPG->HP = RPG->MaxHP;
	RPG->MP = RPG->MaxMP;
	RPG->CombatTimer = 0.f;
	bHasClickTarget = false;
	TargetIndex = INDEX_NONE;
	SetActorLocation(GetRespawnPoint(), false, nullptr, ETeleportType::TeleportPhysics);
	RPG->AddChat(TEXT("System"), FString::Printf(TEXT("You were defeated and revived in town. Lost %lld Luma."), Lost), FLinearColor(1.f, 0.35f, 0.35f));
}
