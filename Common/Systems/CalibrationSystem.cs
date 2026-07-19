using CrossmodCalibrator.Common.Compatibility;
using CrossmodCalibrator.Common.Config;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CrossmodCalibrator.Common.Systems;

public sealed class CalibrationSystem : ModSystem
{
	public override void PostSetupContent()
	{
		for (int itemType = 1; itemType < ItemID.Count; itemType++)
		{
			Item item = ContentSamples.ItemsByType[itemType];
			if (IsWeaponCandidate(item))
				CalibrationRegistry.WeaponOwnerByType.TryAdd(itemType, CalibrationRegistry.VanillaModName);
		}

		for (int npcType = 1; npcType < NPCID.Count; npcType++)
		{
			NPC npc = ContentSamples.NpcsByNetId[npcType];

			if (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npcType])
				CalibrationRegistry.NpcSourceByType.TryAdd(npcType, (CalibrationRegistry.VanillaModName, true));
			else if (!npc.friendly && !npc.townNPC && !npc.dontTakeDamage && !npc.dontCountMe && npc.lifeMax > 1 && !NPCID.Sets.CountsAsCritter[npcType])
				CalibrationRegistry.NpcSourceByType.TryAdd(npcType, (CalibrationRegistry.VanillaModName, false));
		}
		foreach (Mod mod in ModLoader.Mods)
		{
			if (mod.Name is "ModLoader" or "CrossmodCalibrator")
				continue;

			CalibrationRegistry.ContentByMod.TryAdd(mod.Name, CalibrationContent.None);

			foreach (ModItem modItem in mod.GetContent<ModItem>())
			{
				Item item = ContentSamples.ItemsByType[modItem.Type];
				if (IsWeaponCandidate(item))
					CalibrationRegistry.WeaponOwnerByType.TryAdd(modItem.Type, mod.Name);
			}

			foreach (ModNPC modNpc in mod.GetContent<ModNPC>())
			{
				NPC npc = ContentSamples.NpcsByNetId[modNpc.Type];

				if (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[modNpc.Type])
					CalibrationRegistry.NpcSourceByType.TryAdd(modNpc.Type, (mod.Name, true));
				else if (!npc.friendly && !npc.townNPC && !npc.dontTakeDamage && !npc.dontCountMe && npc.lifeMax > 1 && !NPCID.Sets.CountsAsCritter[modNpc.Type])
					CalibrationRegistry.NpcSourceByType.TryAdd(modNpc.Type, (mod.Name, false));
			}
		}

		CalibrationRegistry.RebuildContentByMod();
		SyncConfig();
	}

	internal static void SyncConfig()
	{
		CrossmodCalibratorConfig active = CrossmodCalibratorConfig.Instance;
		CrossmodCalibratorConfig? pending = null;

		foreach ((string modName, CalibrationContent content) in CalibrationRegistry.ContentByMod)
		{
			if ((pending ?? active).ModSettings.TryGetValue(modName, out ModCalibrationSettings? settings))
			{
				if (settings.Content == content)
					continue;
			}
			else if (content == CalibrationContent.None)
				continue;

			pending ??= (CrossmodCalibratorConfig)ConfigManager.GeneratePopulatedClone(active);
			if (!pending.ModSettings.TryGetValue(modName, out settings))
				pending.ModSettings.Add(modName, settings = new ModCalibrationSettings());
			settings.Content = content;
		}

		if (pending is null)
			return;

		if (Main.netMode != NetmodeID.MultiplayerClient)
			active.SaveChanges(pending, status: null, silent: true, broadcast: false);
		else
			active.ModSettings = pending.ModSettings;
	}

	private static bool IsWeaponCandidate(Item item)
	{
		return item.useStyle != ItemUseStyleID.None && (item.damage > 0 || item.knockBack > 0 || item.useAmmo > 0 || item.shoot > ProjectileID.None || item.DamageType != DamageClass.Default);
	}
}

