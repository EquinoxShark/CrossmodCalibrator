using System.Collections.Generic;
using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Config.UI;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace CrossmodCalibrator.Common.Config;

public sealed class CrossmodCalibratorConfig : ModConfig
{
	public static CrossmodCalibratorConfig Instance { get; private set; } = null!;

	public override ConfigScope Mode => ConfigScope.ServerSide;

	[CustomModConfigItem(typeof(CalibrationDictionaryElement))]
	public Dictionary<string, ModCalibrationSettings> ModSettings { get; set; } = new();

	public CrossmodCalibratorConfig()
	{
		foreach ((string modName, CalibrationContent content) in CalibrationRegistry.ContentByMod)
			if (content != CalibrationContent.None)
				ModSettings[modName] = new ModCalibrationSettings { Content = content };
	}

	public override void OnLoaded() => Instance = this;

	public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
	{
		if (Main.countsAsHostForGameplay[whoAmI])
			return true;

		message = NetworkText.FromKey("Mods.CrossmodCalibrator.Configs.CrossmodCalibratorConfig.OnlyHost");
		return false;
	}

	public override ModConfig Clone()
	{
		var clone = (CrossmodCalibratorConfig)MemberwiseClone();
		clone.ModSettings = new Dictionary<string, ModCalibrationSettings>(ModSettings.Count);

		foreach ((string modName, ModCalibrationSettings settings) in ModSettings)
			clone.ModSettings[modName] = new ModCalibrationSettings
			{
				Content = settings.Content,
				WeaponDamageMultiplier = settings.WeaponDamageMultiplier,
				BossHealthMultiplier = settings.BossHealthMultiplier,
				BossDamageMultiplier = settings.BossDamageMultiplier,
				EnemyHealthMultiplier = settings.EnemyHealthMultiplier,
				EnemyDamageMultiplier = settings.EnemyDamageMultiplier
			};

		return clone;
	}
}
