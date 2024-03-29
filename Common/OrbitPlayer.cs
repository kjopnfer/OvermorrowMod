using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OrbitPlayer : ModPlayer
    {
        public OrbitEntityContainer Container { get; } = new OrbitEntityContainer();

        public override void OnEnterWorld()
        {
            Container.ResetProjectiles();
        }

        public override void UpdateDead()
        {
            Container.ResetProjectiles();
        }

        // This is where we make our central timer that the orbiting projectile uses.
        public override void PostUpdate()
        {
            Container.OnUpdate();
        }
    }
}