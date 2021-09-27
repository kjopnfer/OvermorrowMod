using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.RuneBuffs
{
    public class FungiRune : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Imbuement: Fungal Vine");
            Description.SetDefault("Your Artifacts will become empowered on use!");
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