using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class TreeBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shade of the Forest");
			Description.SetDefault("Your health regen is being increased");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().treeBuff = true;
		}
	}
}