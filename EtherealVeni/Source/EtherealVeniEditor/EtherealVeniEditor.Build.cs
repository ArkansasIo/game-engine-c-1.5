// Copyright (c) Ethereal Veni project. All rights reserved.

using UnrealBuildTool;

public class EtherealVeniEditor : ModuleRules
{
	public EtherealVeniEditor(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[] { "Core", "EtherealVeni" });
		PrivateDependencyModuleNames.AddRange(new string[] { "CoreUObject", "Engine", "EditorFramework", "UnrealEd", "ToolMenus" });
	}
}
