using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class FeatherArrow : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Featherfall Arrow");
        }

		public override void AI() 
		{
            projectile.velocity.Y += 0.2f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            if(projectile.velocity.Y > 6)
            {
                projectile.velocity.Y = 6f;
            }

        }



        public override void Kill(int timeLeft)
        {
            if (Main.rand.Next(5) == 3)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<FeatherArrowAmmo>());
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
            return true;
        }
    }
}