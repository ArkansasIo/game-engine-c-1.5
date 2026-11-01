#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "GameFramework/PlayerController.h"
#include "GZGameMode.generated.h"

/** Player controller with a visible mouse cursor for click-to-move and the windowed UI. */
UCLASS()
class ETHEREALVENI_API AGZPlayerController : public APlayerController
{
	GENERATED_BODY()

public:
	AGZPlayerController();

protected:
	virtual void BeginPlay() override;
};

/** Game mode for the Luminara (GoonZu-inspired) world. */
UCLASS()
class ETHEREALVENI_API AGZGameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	AGZGameMode();
};
