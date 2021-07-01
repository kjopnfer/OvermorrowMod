using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Accessory;


namespace OvermorrowMod.Projectiles.Accessory
{
    public class BlackHole : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        Vector2 Rot;
        private float CircleArr = 1;
        public override bool CanDamage() => false;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 70;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {



            if (Main.player[projectile.owner].HasBuff(ModContent.BuffType<SpiderWebBuff>()))
            {
                projectile.timeLeft = 2;
            }

                projectile.spriteDirection = Main.player[projectile.owner].direction;

                if (Main.player[projectile.owner].direction == 1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + 30;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - projectile.height / 2;
                }
                if (Main.player[projectile.owner].direction == -1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 - 30;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - projectile.height / 2;
                }


            float distanceFromTarget = 500f;
            Vector2 targetCenter = projectile.Center;
            bool foundTarget = false;
            projectile.velocity.X = 0;
            projectile.velocity.Y = 0;
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Projectile suck = Main.projectile[i];




                        float between2 = Vector2.Distance(suck.Center, projectile.Center);



                        if(suck.Center.Y > projectile.position.Y && suck.Center.Y < projectile.position.Y + projectile.height && suck.Center.X > projectile.position.X && suck.Center.X < projectile.position.X + projectile.width && !suck.friendly)
                        {
                            suck.velocity = suck.velocity * -1f;
                            suck.friendly = true;
                            suck.hostile = false;
                            suck.damage *= 2;
                        }
                }
        }
    }
}
