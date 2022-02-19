using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class Skull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull");
            Main.projFrames[projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
            projectile.light = 0.5f;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (base.projectile.alpha > 0)
            {
                base.projectile.alpha -= 25;
                if (base.projectile.alpha < 0)
                {
                    base.projectile.alpha = 0;
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
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

            base.projectile.localAI[0] += 1f;
            if (base.projectile.localAI[0] > 4f)
            {
                int num = Dust.NewDust(new Vector2(base.projectile.position.X, base.projectile.position.Y), base.projectile.width, base.projectile.height, 37, 0f, 0f, 100); //Could use a better dust
                Main.dust[num].noGravity = true;
            }

            if (++projectile.frameCounter >= 9)
            {
                projectile.frameCounter = 0;
                {
                    projectile.frame = 0;
                }
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 15f)
            {
                vector *= 15f / magnitude;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Blue;
        }
    }
}