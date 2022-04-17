using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BoltStream
{
    public class BoltStreamBolt : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        private bool canAccelerate = false;
        private Vector2 storeVelocity;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);

            if (Projectile.ai[0] == 0) // Store the Projectile's velocity when it was fired from the weapon
            {
                storeVelocity = Projectile.velocity;
            }

            if (Projectile.ai[0] >= 5 && Projectile.ai[0] <= 25) // Make the Projectile stops momentarily for 40 ticks
            {
                Projectile.velocity = new Vector2(0, 0);
                if (Projectile.ai[0] == 25) // Allow the Projectile to accelerate
                {
                    canAccelerate = true;
                }
            }

            if (Projectile.ai[0] <= 25) // Let's the 3rd Projectile not get stuck in the ground
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (canAccelerate)
            {
                if (Projectile.ai[0] == 25)
                {
                    Projectile.velocity = storeVelocity;
                }
                else
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 3f)
                    {
                        for (int num1202 = 0; num1202 < 4; num1202++)
                        {
                            Vector2 vector304 = Projectile.position;
                            vector304 -= Projectile.velocity * ((float)num1202 * 0.25f);
                            Projectile.alpha = 255;
                            int num1200 = Dust.NewDust(vector304, 1, 1, DustID.UnusedWhiteBluePurple);
                            Main.dust[num1200].position = vector304;
                            Dust expr_140F1_cp_0 = Main.dust[num1200];
                            expr_140F1_cp_0.position.X = expr_140F1_cp_0.position.X + (float)(Projectile.width / 2);
                            Dust expr_14115_cp_0 = Main.dust[num1200];
                            expr_14115_cp_0.position.Y = expr_14115_cp_0.position.Y + (float)(Projectile.height / 2);
                            Main.dust[num1200].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                            Dust dust81 = Main.dust[num1200];
                            dust81.velocity *= 0.2f;
                        }
                    }

                    if (Projectile.ai[0] % 4 == 0)
                    {
                        Projectile.velocity *= 1.47f;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, 1, 1, DustID.UnusedWhiteBluePurple);
            }
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
        }
    }
}