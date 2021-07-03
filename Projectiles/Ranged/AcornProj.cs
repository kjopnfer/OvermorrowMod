using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class AcornProj : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 500;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
        }

		public override void AI() 
		{
            projectile.velocity.X *= 0.97f;
            projectile.velocity.Y += 0.3f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item64, projectile.position);
            return true;
        }
    }
}