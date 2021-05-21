using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class RuneBuff1 : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Imbuement: Blaze Binder");
			Description.SetDefault("Your attacks launch three chains instead of one!");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<WardenRunePlayer>().Rune1 = true;
			player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
		}
	}
}