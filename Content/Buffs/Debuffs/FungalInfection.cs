using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    // Ethereal Flames is an example of a buff that causes constant loss of life.
    // See ExamplePlayer.UpdateBadLifeRegen and ExampleGlobalNPC.UpdateLifeRegen for more information.
    public class FungalInfection : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bleeding");
            Description.SetDefault("Losing life");
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