using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
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
        public override void OnKill(NPC npc)
        {
            Container.ResetProjectiles();

            base.OnKill(npc);
        }
    }
}
