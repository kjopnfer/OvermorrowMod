using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class WormHeadHostile : ModProjectile
    {
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 42;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ranged = true;
            projectile.netImportant = true;
            projectile.penetrate = 5;
            projectile.light = 1.4f;
            projectile.timeLeft = 110;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {

            projectile.velocity.Y = projectile.velocity.Y + 0.06f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            timer++;
            if (timer == 3)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("WormBodyHostile"), projectile.damage - 10, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }
    }
}