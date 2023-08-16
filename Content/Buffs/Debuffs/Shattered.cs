using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class Shattered : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shattered");
            // Description.SetDefault("Your gauntlets have been disabled!");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}