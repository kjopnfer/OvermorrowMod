using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class LightningCloud : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Divine Cloud");
			Description.SetDefault("The Divine Cloud will fight for you");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldCloud>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}

			player.GetModPlayer<OvermorrowModPlayer>().goldWind = true;
		}
	}
}