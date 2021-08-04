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
			Main.debuff[Type] = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().lightningCloud = true;
		}
	}
}