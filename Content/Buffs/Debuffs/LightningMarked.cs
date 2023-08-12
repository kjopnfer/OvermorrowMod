using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class LightningMarked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Marked");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<OvermorrowGlobalNPC>().LightningMarked = true;
        }
    }
}