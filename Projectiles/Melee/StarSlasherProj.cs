using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;


namespace OvermorrowMod.Projectiles.Melee
{
    public class StarSlasherProj : ModProjectile
    {

        int randhigh = 0;
        int randdust = Main.rand.Next(6);

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 67;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.light = 1f;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300; //The amount of time the projectile is alive for
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Slasher");
        }



        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Color alpha = Color.Pink;
            Color alpha2 = Color.Blue;
            randdust = Main.rand.Next(6);
            if(randdust < 3)
            {
                {
                    Dust.NewDust(projectile.Center, 0, 0, 184, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, alpha, 1f);
                }
            }
            else
            {
                {
                    Dust.NewDust(projectile.Center, 0, 0, 184, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, alpha2, 1f);
                }
            }

            randhigh++;
            if(randhigh == 1)
            {
                projectile.position.Y = projectile.position.Y - projectile.height + 13;
            }



            projectile.scale = 1f;
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 140f;
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







        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
            {
                vector *= 20f / magnitude;
            }
        }
    }
}