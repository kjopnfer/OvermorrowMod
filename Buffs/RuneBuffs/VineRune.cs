using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.RuneBuffs
{
	public class VineRune : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Imbuement: Thorns of the Jungle");
			Description.SetDefault("Your attacks launch vines to ensnare your enemy!");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<WardenRunePlayer>().RuneID = WardenRunePlayer.Runes.JungleRune;
			player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
		}
	}
}