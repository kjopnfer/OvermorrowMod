using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class DiceArrow : ModProjectile
    {
        int BOUNCE = 6;
        int Splittimer = 0;
        bool rando = false;
        int Random = Main.rand.Next(1, 7);

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dice Arrow");
            Main.projFrames[base.projectile.type] = 6;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            if (projectile.ai[0] != 10)
            {
                if (!rando)
                {
                    Random = Main.rand.Next(1, 7);
                    rando = true;
                }
            }
            else
            {
                if (!rando)
                {
                    Random = Main.rand.Next(1, 6);
                    rando = true;
                }
            }


            if (Random == 1)
            {
                projectile.frame = 0;
                projectile.penetrate = 5;
            }

            if (Random == 2)
            {
                projectile.frame = 1;
            }

            if (Random == 3)
            {
                projectile.frame = 2;
                projectile.penetrate = -1;
            }

            if (Random == 4)
            {
                projectile.frame = 3;
                Vector2 move = Vector2.Zero;
                float distance = 200f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                    {
                        Vector2 newMove = Main.npc[k].Center - projectile.Center;
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
                    projectile.velocity = (10 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }

            if (Random == 5)
            {
                projectile.frame = 4;
            }


            if (Random == 6)
            {
                projectile.frame = 5;
                Splittimer++;
                if (Splittimer == 30)
                {
                    int splitproj1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DiceArrow>(), projectile.damage - 5, 3f, projectile.owner, 0f);
                    Main.projectile[splitproj1].ai[0] = 10;
                    Vector2 newpoint1 = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(25f));
                    Vector2 newpoint2 = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-25f));
                    int splitproj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint1.X, newpoint1.Y, ModContent.ProjectileType<DiceArrow>(), projectile.damage - 5, 3f, projectile.owner, 0f);
                    int splitproj3 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X, newpoint2.Y, ModContent.ProjectileType<DiceArrow>(), projectile.damage - 5, 3f, projectile.owner, 0f);
                    Main.projectile[splitproj2].ai[0] = 10;
                    Main.projectile[splitproj3].ai[0] = 10;
                    projectile.Kill();
                }
            }



            if (Random != 4)
            {
                projectile.velocity.Y += 0.076f;
            }



        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Random == 2)
            {
                int explode = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ProjectileID.RocketSnowmanI, projectile.damage, 3f, projectile.owner, 0f);
                Main.projectile[explode].timeLeft = 0;
                Main.projectile[explode].friendly = true;
                Main.projectile[explode].hostile = false;
            }
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Random == 3)
            {
                target.immune[projectile.owner] = 3;
            }


            if (Random == 1 && BOUNCE > 1)
            {
                BOUNCE--;
                projectile.velocity.X = -projectile.velocity.X * 1.5f;
                projectile.velocity.Y = -projectile.velocity.Y * 1.5f;
            }

            if (Random == 5)
            {
                target.AddBuff(24, 280);
                target.AddBuff(20, 280);
                target.AddBuff(44, 280);
                target.AddBuff(153, 280);
                target.AddBuff(39, 280);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
            if (Random == 1 && BOUNCE > 1)
            {
                BOUNCE -= 1;
                projectile.velocity.X = -projectile.velocity.X * 1.1f;
                projectile.velocity.Y = -projectile.velocity.Y * 1.1f;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}