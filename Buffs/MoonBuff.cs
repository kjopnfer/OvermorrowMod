using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class MoonBuff : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Light of the Moon");
			Description.SetDefault("Your damage is being increased");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().moonBuff = true;
		}
	}
}