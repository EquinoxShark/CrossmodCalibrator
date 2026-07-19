using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Systems;
using Terraria.ModLoader;

namespace CrossmodCalibrator;

public sealed class CrossmodCalibrator : Mod
{
	public override object Call(params object[] args)
	{
		int contentType = (int)args[1];
		string? ownerModName = args.Length > 2 ? (string)args[2] : null;

		switch ((string)args[0])
		{
			case "RegisterWeapon":
				ownerModName ??= ItemLoader.GetItem(contentType)!.Mod.Name;
				CalibrationRegistry.WeaponOwnerByType[contentType] = ownerModName;
				break;
			case "RegisterBoss":
				ownerModName ??= NPCLoader.GetNPC(contentType)!.Mod.Name;
				CalibrationRegistry.NpcSourceByType[contentType] = (ownerModName, true);
				break;
			case "RegisterEnemy":
				ownerModName ??= NPCLoader.GetNPC(contentType)!.Mod.Name;
				CalibrationRegistry.NpcSourceByType[contentType] = (ownerModName, false);
				break;
			default:
				throw new System.ArgumentException($"Unknown Crossmod Calibrator call '{args[0]}'.");
		}

		CalibrationRegistry.RebuildContentByMod();
		CalibrationSystem.SyncConfig();
		return true;
	}

	public override void Unload()
	{
		CalibrationRegistry.WeaponOwnerByType.Clear();
		CalibrationRegistry.NpcSourceByType.Clear();
		CalibrationRegistry.ContentByMod.Clear();
	}
}
