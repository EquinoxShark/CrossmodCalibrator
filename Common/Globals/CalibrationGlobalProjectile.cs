using System.IO;
using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Config;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CrossmodCalibrator.Common.Globals;

public sealed class CalibrationGlobalProjectile : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	private string? sourceModName;
	private bool bossSource;

	public override void OnSpawn(Projectile projectile, IEntitySource source)
	{
		if (source is not EntitySource_Parent parent)
			return;

		if (parent.Entity is NPC npc && CalibrationRegistry.TryGetNpcSource(npc, out var sourceData))
		{
			sourceModName = sourceData.ModName;
			bossSource = sourceData.IsBoss;
			return;
		}

		if (parent.Entity is Projectile parentProjectile)
		{
			CalibrationGlobalProjectile parentData = parentProjectile.GetGlobalProjectile<CalibrationGlobalProjectile>();
			sourceModName = parentData.sourceModName;
			bossSource = parentData.bossSource;
		}
	}

	public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
	{
		if (CrossmodCalibratorConfig.Instance.ModSettings.TryGetValue(sourceModName ?? projectile.ModProjectile?.Mod.Name ?? CalibrationRegistry.VanillaModName, out ModCalibrationSettings? settings))
			modifiers.FinalDamage *= bossSource ? settings.BossDamageScale : settings.EnemyDamageScale;
	}

	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
	{
		if (sourceModName is not string modName)
		{
			bitWriter.WriteBit(false);
			return;
		}

		bitWriter.WriteBit(true);
		bitWriter.WriteBit(bossSource);
		binaryWriter.Write(modName);
	}

	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
	{
		if (!bitReader.ReadBit())
		{
			sourceModName = null;
			return;
		}

		bossSource = bitReader.ReadBit();
		sourceModName = binaryReader.ReadString();
	}
}
