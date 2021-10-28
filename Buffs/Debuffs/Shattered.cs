using Terraria;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.Buffs.Debuffs
{
    public class Shattered : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shattered");
            Description.SetDefault("Your gauntlets have been disabled!");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<WardenDamagePlayer>().Shattered = true;
        }
    }
}