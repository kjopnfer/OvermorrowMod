using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class FungalInfection : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fungal Infection");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<OvermorrowGlobalNPC>().FungiInfection = true;
        }
    }
}