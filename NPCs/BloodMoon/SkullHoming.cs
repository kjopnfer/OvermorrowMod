using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.NPCs.BloodMoon
{
    public class SkullHoming : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Skull");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 160;
            projectile.light = 0.35f;
            projectile.penetrate = 1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.velocity.Y += 0.30f;





            projectile.rotation = projectile.velocity.X * 0.13f;
            Player player = Main.player[projectile.owner];


            if(Vector2.Distance(player.Center, projectile.Center) > 500)
            {
                if(projectile.velocity.Y > 0.9f)
                {
                    projectile.velocity.Y = 0.9f;
                }
            }
            else
            {
                if(projectile.velocity.Y > 2f)
                {
                    projectile.velocity.Y = 2f;
                }
            }


            if(projectile.Center.Y > player.Center.Y + 17 && Vector2.Distance(player.Center, projectile.Center) < 30)
            {
                projectile.Kill();
            }


            if(projectile.velocity.Y > 0)
            {
                if (projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref projectile.velocity);
                    projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 5000f;
                bool target = false;
                if (!player.dead)
                {
                    Vector2 newMove = player.Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    projectile.velocity.X = (10 * projectile.velocity.X + move.X) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }
        }


  

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 7.7f)
            {
                vector *= 7.7f / magnitude;
            }
        }


        public override void Kill(int timeLeft)
        {
            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.NPCHit, (int)position.X, (int)position.Y, 2);
            int radius = 2;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Dust.NewDust(position, 5, 5, DustID.HeatRay, 0.0f, 0.0f, 120, new Color(), 1f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
