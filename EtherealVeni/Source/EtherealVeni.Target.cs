using UnrealBuildTool;
using System.Collections.Generic;

public class EtherealVeniTarget : TargetRules
{
	public EtherealVeniTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Game;
		DefaultBuildSettings = BuildSettingsVersion.V5;
		IncludeOrderVersion = EngineIncludeOrderVersion.Unreal5_8;
		ExtraModuleNames.Add("EtherealVeni");
	}
}
