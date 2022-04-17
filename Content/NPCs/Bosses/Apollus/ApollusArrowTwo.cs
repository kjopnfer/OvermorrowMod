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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 600;
            Projectile.light = 0.75f;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0);


            Projectile.ai[0]++;

            if (Projectile.ai[0] < 15 && part2 == false)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Projectile.ai[0] == 5 && part2 == false)
            {
                storeVelocity = Projectile.velocity;
                storeRotation = Projectile.rotation;
            }

            Projectile.rotation = storeRotation;

            if (Projectile.ai[0] > 15 && part2 == false)
            {
                Projectile.velocity = Vector2.Zero;
            }

            if (Projectile.velocity == Vector2.Zero && part2 == false)
            {
                Projectile.ai[0] = 0;
                part2 = true;
            }

            if (Projectile.ai[0] > 90 + (Projectile.ai[1] * -4) && part2 == true)
            {
                Projectile.velocity = storeVelocity;
            }

        }
    }
}
