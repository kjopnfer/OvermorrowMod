using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace OvermorrowMod.WardenClass
{
    public class OrbitPlayer : ModPlayer
    {
        public OrbitEntityContainer Container { get; } = new OrbitEntityContainer();

        public override void OnEnterWorld(Player player)
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