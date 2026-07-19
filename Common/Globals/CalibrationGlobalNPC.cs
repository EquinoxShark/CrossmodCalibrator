using System;
using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Config;
using Terraria;
using Terraria.ModLoader;

namespace CrossmodCalibrator.Common.Globals;

public sealed class CalibrationGlobalNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	private bool healthScaled;

	public override bool PreAI(NPC npc)
	{
		if (healthScaled)
			return true;
		healthScaled = true;

		if (CalibrationRegistry.TryGetNpcSource(npc, out var source) &&
			CrossmodCalibratorConfig.Instance.ModSettings.TryGetValue(source.ModName, out ModCalibrationSettings? settings))
		{
			float lifeFraction = npc.GetLifePercent();
			npc.lifeMax = (int)Math.Clamp(Math.Round(npc.lifeMax * (double)(source.IsBoss ? settings.BossHealthScale : settings.EnemyHealthScale), MidpointRounding.AwayFromZero), 1d, int.MaxValue);
			npc.life = (int)Math.Clamp(Math.Round(npc.lifeMax * (double)lifeFraction, MidpointRounding.AwayFromZero), 1d, npc.lifeMax);
		}

		return true;
	}

	public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
	{
		if (CalibrationRegistry.TryGetNpcSource(npc, out var source) &&
			CrossmodCalibratorConfig.Instance.ModSettings.TryGetValue(source.ModName, out ModCalibrationSettings? settings))
			modifiers.FinalDamage *= source.IsBoss ? settings.BossDamageScale : settings.EnemyDamageScale;
	}
}
