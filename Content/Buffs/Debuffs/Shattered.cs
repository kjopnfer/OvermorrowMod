using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
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
    }
}