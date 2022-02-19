using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.NPCs.Hostile
{
    public class Spider_Fire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulfire");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.penetrate = 1;
            projectile.alpha = 125;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.5f);

            projectile.ai[0]++;
            if (projectile.ai[0] >= 120)
            {
                Vector2 move = Vector2.Zero;
                float distance = 2000f;
                bool target = false;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    if (Main.player[k].active && !Main.player[k].dead)
                    {
                        Vector2 newMove = Main.player[k].Center - projectile.Center;
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
                    projectile.velocity = (9 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }

            projectile.localAI[0]++;
            if (projectile.localAI[0] > 3f && Main.rand.Next(3) == 0)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, DustID.Frost, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.5f);
            }

            // Loop through the 4 animation frames, spending 5 ticks on each.
            if (++projectile.frameCounter >= 5)
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
    }
}