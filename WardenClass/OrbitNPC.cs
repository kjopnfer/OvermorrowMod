using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass
{
    public class OrbitNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public OrbitEntityContainer Container { get; } = new OrbitEntityContainer();

        public override void PostAI(NPC npc)
        {
            Container.OnUpdate();

            base.PostAI(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            Container.ResetProjectiles();

            base.NPCLoot(npc);
        }
    }
}
