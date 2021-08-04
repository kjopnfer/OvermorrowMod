using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class GoldWind : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Golden Wind");
			Description.SetDefault("Greatly increased movement speed");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().goldWind = true;
		}
	}
}