using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class ExplosionBuff : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Unstable Fire");
			Description.SetDefault("Releases bursts of flame whenever damaged");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().explosionBuff = true;
		}
	}
}