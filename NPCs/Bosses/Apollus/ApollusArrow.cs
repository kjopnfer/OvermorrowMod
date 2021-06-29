using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ApollusArrow : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Magic/MarbleArrow";

        private Vector2 storeVelocity;
        private float storeRotation = 25;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollus Light Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.penetrate = 30;
            projectile.timeLeft = 600;
            projectile.light = 0.75f;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust dust = Dust.NewDustPerfect(projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);

            if (projectile.ai[1] == 0)
            {
                projectile.ai[0]++;

                if (projectile.ai[0] < 15)
                {
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if (projectile.ai[0] == 5)
                {
                    storeVelocity = projectile.velocity;
                    storeRotation = projectile.rotation;
                }

                projectile.rotation = storeRotation;

                if (projectile.ai[0] > 15 && projectile.velocity != Vector2.Zero)
                {
                    projectile.velocity = new Vector2(MathHelper.Lerp(projectile.velocity.X, 0, 0.25f), MathHelper.Lerp(projectile.velocity.Y, 0, 0.25f));
                    if(projectile.velocity.X < 0.01 && projectile.velocity.Y < 0.01)
                    {
                        projectile.velocity = Vector2.Zero;
                    }
                }

                if (projectile.velocity == Vector2.Zero)
                {
                    projectile.ai[0] = 0;
                    projectile.ai[1] = 1;
                }
            }
            else
            {
                projectile.ai[0]++;
                projectile.rotation = storeRotation;

                if(projectile.ai[0] > 90)
                {
                    projectile.velocity = new Vector2(MathHelper.Lerp(0, storeVelocity.X * 2, 0.4f), MathHelper.Lerp(0, storeVelocity.Y * 2, 0.4f));
                }
            }
        }
    }
}
