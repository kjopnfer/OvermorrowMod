using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class DemonEye : ModProjectile
    {
        private bool canDive = false;
        private int diveTimer = 0;
        private int projectileMovement = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mRand = Main.rand.Next(-100, 101);
        float NPCtargetHeight = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bound Crimera");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 22;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 600; 
        }

        public override void AI()
        {
            projectile.ai[0]++;

            projectile.rotation = projectile.velocity.ToRotation();

            if (projectile.ai[0] > 60) // Chase after enemies after 1 second
            {
                projectile.tileCollide = true;

                if (projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref projectile.velocity);
                    projectile.localAI[0] = 1f;
                }

                Vector2 move = Vector2.Zero;
                float distance = 1200f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].CanBeChasedBy())
                    {
                        Vector2 newMove = Main.npc[k].Center - projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                            NPCtargetX = Main.npc[k].Center.X;
                            NPCtargetY = Main.npc[k].Center.Y;
                            NPCtargetHeight = Main.npc[k].height / 2;
                        }
                    }
                }

                if (target)
                {
                    projectile.tileCollide = false;
                    if (!canDive)
                    {
                        diveTimer = 0;
                        projectile.ai[1]++;
                        projectileMovement--;
                        if (projectile.ai[1] == 20)
                        {
                            canDive = true;
                        }

                        if (projectileMovement == 50)
                        {
                            mRand = Main.rand.Next(-100, 100);
                        }

                        if (NPCtargetX + mRand > projectile.Center.X)
                        {
                            projectile.velocity.X += 1.1f;
                        }

                        if (NPCtargetX + mRand < projectile.Center.X)
                        {
                            projectile.velocity.X -= 1.1f;
                        }

                        if (NPCtargetY - 50 - NPCtargetHeight > projectile.Center.Y)
                        {
                            projectile.velocity.Y += 1.1f;
                        }
                        if (NPCtargetY - 50 - NPCtargetHeight < projectile.Center.Y)
                        {
                            projectile.velocity.Y -= 1.1f;
                        }
                    }

                    if (canDive)
                    {
                        diveTimer++;
                        if (diveTimer < 6)
                        {
                            Vector2 position = projectile.Center;
                            Vector2 targetPosition = new Vector2(NPCtargetX, NPCtargetY);
                            Vector2 direction = targetPosition - position;
                            direction.Normalize();
                            projectile.velocity = direction * 11;

                        }

                        if (diveTimer == 15)
                        {
                            projectile.ai[1] = 0;
                            canDive = false;
                            projectileMovement = 51;
                        }
                    }
                }
            }

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }

            // Loop through the 10 animation frames, spending 3 ticks on each.
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/Artifact/DemonEye");

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {

                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Red) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, 235, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            Main.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}