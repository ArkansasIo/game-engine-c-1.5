#include "EtherealVeniEditor.h"
#include "Misc/MessageDialog.h"

#define LOCTEXT_NAMESPACE "FEtherealVeniEditorModule"

void FEtherealVeniEditorModule::StartupModule()
{
	// Menu registrations can go here later (asset importers, editor tools, etc.)
}

void FEtherealVeniEditorModule::ShutdownModule()
{
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FEtherealVeniEditorModule, EtherealVeniEditor)
