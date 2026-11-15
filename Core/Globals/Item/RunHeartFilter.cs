using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class RunHeartFilter : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.Heart or ItemID.CandyApple or ItemID.CandyCane;

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (RunState.InRun && source is EntitySource_Loot { Entity: NPC })
                item.active = false;
        }
    }
}
