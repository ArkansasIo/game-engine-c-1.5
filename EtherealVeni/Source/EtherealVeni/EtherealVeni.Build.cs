using UnrealBuildTool;

public class EtherealVeni : ModuleRules
{
	public EtherealVeni(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		// Lets sub-folders (EthVeni/, GZ/) include each other as "Folder/File.h".
		PublicIncludePaths.Add(ModuleDirectory);

		PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore" });

		// Slate / SlateCore / UMG: the MMO interface in UI/ (menus, panels, options, key bindings).
		PrivateDependencyModuleNames.AddRange(new string[] { "Json", "JsonUtilities", "Slate", "SlateCore", "UMG" });
	}
}
