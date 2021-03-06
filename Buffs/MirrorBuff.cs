using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class MirrorBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Twisted Reflection");
			Description.SetDefault("Damage taken from enemies is reduced by 50%\nEnemies are then dealt their damage" +
				"\n(Will not deal fatal damage to enemies)");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().mirrorBuff = true;
		}
	}
}