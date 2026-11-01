using UnrealBuildTool;
using System.Collections.Generic;

public class EtherealVeniEditorTarget : TargetRules
{
	public EtherealVeniEditorTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Editor;
		DefaultBuildSettings = BuildSettingsVersion.V7;
		IncludeOrderVersion = EngineIncludeOrderVersion.Unreal5_8;
		bOverrideBuildEnvironment = true;
		ExtraModuleNames.AddRange(new string[] { "EtherealVeni", "EtherealVeniEditor" });
	}
}
