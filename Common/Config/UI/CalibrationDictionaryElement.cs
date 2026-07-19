using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrossmodCalibrator.Common.Compatibility;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace CrossmodCalibrator.Common.Config.UI;

public sealed class CalibrationDictionaryElement : ConfigElement
{
	public override void OnBind()
	{
		base.OnBind();

		int groupTop = 0;

		foreach (var (_, settings, displayName) in ((Dictionary<string, ModCalibrationSettings>)GetObject())
			.Where(pair => pair.Value.Content != CalibrationContent.None)
			.Select(pair => (pair.Key, pair.Value, ModLoader.TryGetMod(pair.Key, out Mod? mod) ? mod.DisplayName : pair.Key))
			.OrderBy(group => group.Key == CalibrationRegistry.VanillaModName ? "" : group.Item3))
		{
			CalibrationContent content = settings.Content;

			var group = new UIPanel();
			group.SetPadding(0);
			group.Top.Set(groupTop, 0f);
			group.Left.Set(8, 0f);
			group.Width.Set(-16, 1f);
			Append(group);

			var heading = new UIText(displayName, 0.8f, false);
			heading.Top.Set(10, 0f);
			heading.Left.Set(10, 0f);
			group.Append(heading);

			int top = 34;
			foreach (PropertyFieldWrapper member in ConfigManager.GetFieldsAndProperties(settings).Where(member => member.Name switch
			{
				nameof(ModCalibrationSettings.WeaponDamageMultiplier) => (content & CalibrationContent.Weapon) != 0,
				nameof(ModCalibrationSettings.BossDamageMultiplier) or nameof(ModCalibrationSettings.BossHealthMultiplier) => (content & CalibrationContent.Boss) != 0,
				nameof(ModCalibrationSettings.EnemyDamageMultiplier) or nameof(ModCalibrationSettings.EnemyHealthMultiplier) => (content & CalibrationContent.Enemy) != 0,
				_ => false
			}))
			{
				var field = ConfigManager.WrapIt(group, ref top, member, settings, 0);
				field.Item1.Width.Pixels -= 10;
				field.Item2.Append(new ModTooltipElement(Language.GetTextValue($"Mods.CrossmodCalibrator.Configs.ModCalibrationSettings.{member.Name}.Tooltip", displayName)));
			}

			group.Height.Set(top + 4, 0f);
			groupTop += top + 12;
		}

		MaxHeight.Set(groupTop, 0f);
		Height.Set(groupTop, 0f);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
	}

	private sealed class ModTooltipElement : UIElement
	{
		private static readonly PropertyInfo TooltipProperty = typeof(ConfigElement).Assembly
			.GetType("Terraria.ModLoader.Config.UI.UIModConfig")!
			.GetProperty("Tooltip", BindingFlags.Public | BindingFlags.Static)!;

		private readonly string tooltip;

		public ModTooltipElement(string tooltip)
		{
			this.tooltip = tooltip;
			IgnoresMouseInteraction = true;
			Width.Set(0, 1f);
			Height.Set(0, 1f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (Parent!.IsMouseHovering)
				TooltipProperty.SetValue(null, tooltip);
		}
	}
}
