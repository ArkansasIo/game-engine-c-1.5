#include "EthVeni/EthVeni.h"
#include "Modules/ModuleManager.h"
#include "Misc/MessageDialog.h"

DEFINE_LOG_CATEGORY(LogEthVeni);

#define LOCTEXT_NAMESPACE "FEtherealVeniModule"

void FEtherealVeniModule::StartupModule()
{
	UE_LOG(LogEthVeni, Log, TEXT("Ethereal Veni / Ethereo Pandora - EtherealVeni module started"));
}

void FEtherealVeniModule::ShutdownModule()
{
	UE_LOG(LogEthVeni, Log, TEXT("EtherealVeni module shut down"));
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_PRIMARY_GAME_MODULE(FEtherealVeniModule, EtherealVeni, "EtherealVeni");
