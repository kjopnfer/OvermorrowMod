using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class FungalInfection : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Infection");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
    }
}