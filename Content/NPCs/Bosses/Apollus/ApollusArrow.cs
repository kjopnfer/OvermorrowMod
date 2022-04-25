using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ApollusArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "MarbleBook/MarbleArrow";

        private Vector2 storeVelocity;
        private float storeRotation = 25;
        float wait;
        bool gofast = false;

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
            if (Projectile.ai[0] == -1)
            {
                if (Projectile.ai[1] != 0)
                {
                    gofast = true;
                    wait = Projectile.ai[1];
                    Projectile.ai[1] = 0;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0);

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] < 15)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if (Projectile.ai[0] == 5)
                {
                    storeVelocity = Projectile.velocity;
                    storeRotation = Projectile.rotation;
                }

                Projectile.rotation = storeRotation;

                if (Projectile.ai[0] > 15 && Projectile.velocity != Vector2.Zero)
                {
                    Projectile.velocity = new Vector2(MathHelper.Lerp(Projectile.velocity.X, 0, 0.25f), MathHelper.Lerp(Projectile.velocity.Y, 0, 0.25f));
                    if (Projectile.velocity.X < 0.01 && Projectile.velocity.Y < 0.01)
                    {
                        Projectile.velocity = Vector2.Zero;
                    }
                }

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 1;
                }
            }
            else
            {
                Projectile.ai[0]++;
                Projectile.rotation = storeRotation;

                if (Projectile.ai[0] > 90 + (wait * -2))
                {
                    Projectile.velocity = new Vector2(MathHelper.Lerp(0, storeVelocity.X * 2, (gofast ? 1.5f : 0.4f)), MathHelper.Lerp(0, storeVelocity.Y * 2, (gofast ? 1.5f : 0.4f)));
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }
    }
}
