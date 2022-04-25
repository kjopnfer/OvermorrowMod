using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.WormStaff
{
    public class WormT1 : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "WormStaff/WormT1";

        private int Wtimer = 0;
        private bool didHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Wtimer++;
            if (Wtimer == 1)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<WormT2>(), 10, 0f, Main.myPlayer, Projectile.whoAmI, Main.myPlayer);
            }

            if (!didHit)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
                if (Projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref Projectile.velocity);
                    Projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                    {
                        Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity += (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
            if (Projectile.velocity.X > 11)
            {
                Projectile.velocity.X = 11;
            }
            if (Projectile.velocity.X < -11)
            {
                Projectile.velocity.X = -11;
            }

            if (Projectile.velocity.Y > 11)
            {
                Projectile.velocity.Y = 11;
            }
            if (Projectile.velocity.Y < -11)
            {
                Projectile.velocity.Y = -11;
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 0.06f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            didHit = true;
            Projectile.tileCollide = false;
        }
    }

    public class WormT2 : ModProjectile
    {
        private int timer = 0;
        public override string Texture => AssetDirectory.Magic + "WormStaff/WormT3";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }



        public override void AI()
        {

            timer++;
            if (timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<WormT3>(), 10, 0f, Main.myPlayer, Projectile.whoAmI, Main.myPlayer);
            }


            Projectile Projectile2 = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile2.active && Projectile2.type == ModContent.ProjectileType<WormT1>())
            {
                // set rotation to the parent segment
                Projectile.rotation = Projectile.DirectionTo(Projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = Projectile2.DirectionTo(Projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                Projectile.Center = Projectile2.Center + new Vector2(dir.X * Projectile2.height, dir.Y * Projectile2.width);
            }
            else
            {
                // kil
                Projectile.Kill();
            }
        }
    }

    public class WormT3 : ModProjectile
    {
        private int timer = 0;
        public override string Texture => AssetDirectory.Magic + "WormStaff/WormT3";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }



        public override void AI()
        {

            timer++;
            if (timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<WormT4>(), 10, 0f, Main.myPlayer, Projectile.whoAmI, Main.myPlayer);
            }

            Projectile Projectile2 = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile2.active && Projectile2.type == ModContent.ProjectileType<WormT2>())
            {
                // set rotation to the parent segment
                Projectile.rotation = Projectile.DirectionTo(Projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = Projectile2.DirectionTo(Projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                Projectile.Center = Projectile2.Center + new Vector2(dir.X * Projectile2.height, dir.Y * Projectile2.width);
            }
            else
            {
                // kil
                Projectile.Kill();
            }
        }
    }
    public class WormT4 : ModProjectile
    {
        private int timer = 0;
        public override string Texture => AssetDirectory.Magic + "WormStaff/WormT3";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }



        public override void AI()
        {

            if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) > 2000)
            {
                Projectile.Kill();
            }

            timer++;
            if (timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<WormT5>(), 10, 0f, Main.myPlayer, Projectile.whoAmI, Main.myPlayer);
            }
            Projectile Projectile2 = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile2.active && Projectile2.type == ModContent.ProjectileType<WormT3>())
            {
                // set rotation to the parent segment
                Projectile.rotation = Projectile.DirectionTo(Projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = Projectile2.DirectionTo(Projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                Projectile.Center = Projectile2.Center + new Vector2(dir.X * Projectile2.height, dir.Y * Projectile2.width);
            }
            else
            {
                // kil
                Projectile.Kill();
            }
        }
    }

    public class WormT5 : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "WormStaff/WormT5";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }



        public override void AI()
        {
            Projectile Projectile2 = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile2.active && Projectile2.type == ModContent.ProjectileType<WormT4>())
            {
                // set rotation to the parent segment
                Projectile.rotation = Projectile.DirectionTo(Projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = Projectile2.DirectionTo(Projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                Projectile.Center = Projectile2.Center + new Vector2(dir.X * Projectile2.height, dir.Y * Projectile2.width);
            }
            else
            {
                // kil
                Projectile.Kill();
            }
        }
    }
}