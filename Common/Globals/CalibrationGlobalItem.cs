using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Config;
using Terraria;
using Terraria.ModLoader;

namespace CrossmodCalibrator.Common.Globals;

public sealed class CalibrationGlobalItem : GlobalItem
{
	public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
	{
		if (CalibrationRegistry.WeaponOwnerByType.TryGetValue(item.type, out string? modName) &&
			CrossmodCalibratorConfig.Instance.ModSettings.TryGetValue(modName, out ModCalibrationSettings? settings))
			damage *= settings.WeaponDamageScale;
	}
}
