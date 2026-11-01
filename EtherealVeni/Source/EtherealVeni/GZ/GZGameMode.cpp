#include "GZGameMode.h"
#include "GZIsoCharacter.h"
#include "GZHUD.h"

AGZPlayerController::AGZPlayerController()
{
	bShowMouseCursor = true;
	bEnableClickEvents = false;
	DefaultMouseCursor = EMouseCursor::Default;
}

void AGZPlayerController::BeginPlay()
{
	Super::BeginPlay();
	FInputModeGameAndUI Mode;
	Mode.SetHideCursorDuringCapture(false);
	Mode.SetLockMouseToViewportBehavior(EMouseLockMode::DoNotLock);
	SetInputMode(Mode);
}

AGZGameMode::AGZGameMode()
{
	DefaultPawnClass = AGZIsoCharacter::StaticClass();
	PlayerControllerClass = AGZPlayerController::StaticClass();
	HUDClass = AGZHUD::StaticClass();
}
