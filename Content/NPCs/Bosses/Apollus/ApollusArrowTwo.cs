using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ApollusArrowTwo : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "MarbleBook/MarbleArrow";

        private Vector2 storeVelocity;
        private float storeRotation = 25;
        bool part2 = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollus Light Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
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


            projectile.ai[0]++;

            if (projectile.ai[0] < 15 && part2 == false)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (projectile.ai[0] == 5 && part2 == false)
            {
                storeVelocity = projectile.velocity;
                storeRotation = projectile.rotation;
            }

            projectile.rotation = storeRotation;

            if (projectile.ai[0] > 15 && part2 == false)
            {
                projectile.velocity = Vector2.Zero;
            }

            if (projectile.velocity == Vector2.Zero && part2 == false)
            {
                projectile.ai[0] = 0;
                part2 = true;
            }

            if (projectile.ai[0] > 90 + (projectile.ai[1] * -4) && part2 == true)
            {
                projectile.velocity = storeVelocity;
            }

        }
    }
}
