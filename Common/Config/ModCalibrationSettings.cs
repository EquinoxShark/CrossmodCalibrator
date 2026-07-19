using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Terraria;

namespace CrossmodCalibrator.Common.Config;

[System.Flags]
internal enum CalibrationContent
{
	None = 0,
	Weapon = 1,
	Boss = 2,
	Enemy = 4,
	All = Weapon | Boss | Enemy
}

public sealed class ModCalibrationSettings
{
	private string weaponDamageMultiplier = "1";
	private string bossHealthMultiplier = "1";
	private string bossDamageMultiplier = "1";
	private string enemyHealthMultiplier = "1";
	private string enemyDamageMultiplier = "1";
	private float weaponDamageScale = 1f;
	private float bossHealthScale = 1f;
	private float bossDamageScale = 1f;
	private float enemyHealthScale = 1f;
	private float enemyDamageScale = 1f;

	internal float WeaponDamageScale => weaponDamageScale;
	internal float BossHealthScale => bossHealthScale;
	internal float BossDamageScale => bossDamageScale;
	internal float EnemyHealthScale => enemyHealthScale;
	internal float EnemyDamageScale => enemyDamageScale;

	[JsonProperty]
	internal CalibrationContent Content { get; set; }

	[DefaultValue("1")]
	public string WeaponDamageMultiplier
	{
		get => weaponDamageMultiplier;
		set => SetMultiplier(ref weaponDamageMultiplier, ref weaponDamageScale, value);
	}

	[DefaultValue("1")]
	public string BossDamageMultiplier
	{
		get => bossDamageMultiplier;
		set => SetMultiplier(ref bossDamageMultiplier, ref bossDamageScale, value);
	}

	[DefaultValue("1")]
	public string BossHealthMultiplier
	{
		get => bossHealthMultiplier;
		set => SetMultiplier(ref bossHealthMultiplier, ref bossHealthScale, value);
	}

	[DefaultValue("1")]
	public string EnemyDamageMultiplier
	{
		get => enemyDamageMultiplier;
		set => SetMultiplier(ref enemyDamageMultiplier, ref enemyDamageScale, value);
	}

	[DefaultValue("1")]
	public string EnemyHealthMultiplier
	{
		get => enemyHealthMultiplier;
		set => SetMultiplier(ref enemyHealthMultiplier, ref enemyHealthScale, value);
	}

	private static void SetMultiplier(ref string text, ref float cached, string value)
	{
		if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) || !float.IsFinite(parsed))
			return;

		cached = Utils.Clamp(parsed, 0.0001f, 10000f);
		text = cached.ToString("G9", CultureInfo.InvariantCulture);
	}
}
