using System.Collections.Generic;
using CrossmodCalibrator.Common.Config;
using Terraria;

namespace CrossmodCalibrator.Common.Compatibility;

internal static class CalibrationRegistry
{
	internal const string VanillaModName = "Vanilla Terraria";

	internal static readonly Dictionary<int, string> WeaponOwnerByType = new();
	internal static readonly Dictionary<int, (string ModName, bool IsBoss)> NpcSourceByType = new();
	internal static readonly Dictionary<string, CalibrationContent> ContentByMod = new(System.StringComparer.Ordinal);

	internal static void RebuildContentByMod()
	{
		foreach (string modName in new List<string>(ContentByMod.Keys))
			ContentByMod[modName] = CalibrationContent.None;

		ContentByMod[VanillaModName] = CalibrationContent.All;

		foreach (string modName in WeaponOwnerByType.Values)
			ContentByMod[modName] = ContentByMod.GetValueOrDefault(modName) | CalibrationContent.Weapon;

		foreach ((string modName, bool isBoss) in NpcSourceByType.Values)
			ContentByMod[modName] = ContentByMod.GetValueOrDefault(modName) | (isBoss ? CalibrationContent.Boss : CalibrationContent.Enemy);
	}

	internal static bool TryGetNpcSource(NPC npc, out (string ModName, bool IsBoss) source)
	{
		if (npc.realLife >= 0 && NpcSourceByType.TryGetValue(Main.npc[npc.realLife].type, out source))
			return true;

		return NpcSourceByType.TryGetValue(npc.type, out source);
	}
}
