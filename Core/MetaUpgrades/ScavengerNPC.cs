using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.MetaUpgrades
{
    public class ScavengerNPC : GlobalNPC
    {
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!RunState.InRun || npc.friendly || npc.value <= 0f) return;

            int percent = Main.LocalPlayer.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.Scavenger);
            npc.value *= 1f + percent / 100f;
        }
    }
}
