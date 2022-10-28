using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class Watched : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Watched");
            Description.SetDefault("'It sees you'");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}