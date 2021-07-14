using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.RuneBuffs
{
	public class FungiRune : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Imbuement: Mycelium Chains");
			Description.SetDefault("Your attacks launch three chains instead of one!");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<WardenRunePlayer>().RuneID = WardenRunePlayer.Runes.MushroomRune;
			player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
		}
	}
}