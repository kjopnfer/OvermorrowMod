using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class AbsorbEnergy : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
      
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 270;
        }

        public override void AI()
        {
            // Get the ID of the Parent NPC that was passed in via AI[1]
            NPC parent = Main.npc[(int)projectile.ai[1]];

            Vector2 move = Vector2.Zero;
            Vector2 newMove = parent.Center - projectile.Center;
            float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
            move = newMove;
            float launchSpeed = Main.expertMode ? 60: 75f;
            projectile.velocity = (move) / launchSpeed;


            /*for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }*/

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if(projectile.Center == parent.Center)
            {
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            // Get the ID of the Parent NPC that was passed in via AI[1]
            NPC parent = Main.npc[(int)projectile.ai[1]];
            if(parent.life < parent.lifeMax)
            {
                parent.life += 5;
            }

            base.Kill(timeLeft);
        }
    }
}