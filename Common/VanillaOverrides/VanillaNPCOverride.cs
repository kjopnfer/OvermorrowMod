using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides
{
    public class VanillaNPCOverride : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        protected Entity target = null;

        public void FaceTarget(NPC npc)
        {
            if (target.Center.X < npc.Center.X) npc.direction = -1;
            else npc.direction = 1;
        }

        public void SetTarget(Entity target)
        {
            if (target != null && target.active) this.target = target;
        }
    }
}