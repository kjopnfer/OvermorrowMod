using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Runs
{
    public class RunKillCounter : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!RunState.InRun || !npc.CanBeChasedBy(ignoreDontTakeDamage: true) || NPCID.Sets.CountsAsCritter[npc.type]) return;

            Main.LocalPlayer.GetModPlayer<RunRecordPlayer>().RecordKill(npc);
        }
    }
}
