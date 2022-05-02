using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BarnabyStaff
{
    public class ManaWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        private float storeDirection;
        private float trigCounter = 0;
        private float period = 60;
        private float amplitude = 20;
        private float previousR = 0;

        private bool initProperties = true;
        private Projectile childProjectile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Storm");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 410;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // This runs once when the Projectile is created
            if (initProperties)
            {
                storeDirection = Projectile.velocity.ToRotation();
                if (Main.netMode != NetmodeID.Server && Projectile.owner == Main.myPlayer)
                {
                    // This spawns the child Projectile that travels in the opposite direction
                    if (Projectile.ai[0] == 0)
                    {
                        childProjectile = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1, Projectile.whoAmI)];
                    }
                    else // This is the check that the child Projectile enters in
                    {
                        childProjectile = Main.projectile[(int)Projectile.ai[1]];
                    }
                }
                initProperties = false;
            }

            trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (Projectile.ai[0] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            Projectile.position += instaVel;
            previousR = r;
            Projectile.rotation = (Projectile.velocity + instaVel).ToRotation() + (float)Math.PI / 27;


            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.FireworkFountain_Blue, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + Projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (Projectile.Center + Projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            float radius = 30;
            Projectile.ai[1] += Projectile.ai[0] == 0 ? 16 : -16;

            // Honestly after like 2 hours, I have no idea how to convert the movement of the dust to the movement of the Projectiles
            // So what this does is it just spams Projectiles, over and over and over again that act as the hitbox for the dust.
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = Projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + Projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(dustPos, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (i == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), dustPos, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), Projectile.damage / 2, i, Projectile.owner, Projectile.whoAmI, 1);
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = Projectile.Center - new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + Projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(dustPos, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (i == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), dustPos, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), Projectile.damage / 2, i, Projectile.owner, Projectile.whoAmI, 1);
                    }
                }
            }
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
        }
    }
}