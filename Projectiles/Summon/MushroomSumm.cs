using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MushroomSumm : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool CanDamage() => false;

        private int eyetimer = 0;
        private int timer = 0;
        private int PosCheck = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom");
            Main.projFrames[base.projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.sentry = true;
            projectile.width = 52;
            projectile.height = 54;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 8000;
        }
        
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            player.UpdateMaxTurrets();

            projectile.velocity.Y += 0.5f;
            PosCheck++;

            float distanceFromTarget = 300f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Main.player[projectile.owner].Center);
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if ((closest && inRange) || !foundTarget)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                timer++;
                if (timer == 150)
                {
                    int Random5 = Main.rand.Next(-2, 3);        
                    int Random4 = Main.rand.Next(-2, 3);
                    int Random3 = Main.rand.Next(-2, 3);                    
                    Random2 = Main.rand.Next(-2, 3);
                    Random = Main.rand.Next(-2, 3);
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = projectile.Center + new Vector2(0, -300f);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint1 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));
                    Vector2 newpoint3 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));
                    Vector2 newpoint4 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));
                    Vector2 newpoint5 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));

                    float speed = 10f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint1.X * speed, newpoint1.Y * speed + Random, ModContent.ProjectileType<SummSpore>(), projectile.damage, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed + Random2 , ModContent.ProjectileType<SummSpore>(), projectile.damage, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint3.X * speed, newpoint3.Y * speed + Random3, ModContent.ProjectileType<SummSpore>(), projectile.damage, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint4.X * speed, newpoint4.Y * speed + Random4, ModContent.ProjectileType<SummSpore>(), projectile.damage, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint5.X * speed, newpoint5.Y * speed + Random5 , ModContent.ProjectileType<SummSpore>(), projectile.damage, 1f, projectile.owner, 0f);

                    timer = 0;
                }
            }

            if(timer < 130)
            {
                eyetimer = 0;
                if (++projectile.frameCounter >= 8)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 4)
                    {
                        projectile.frame = 0;
                    }
                }
            }
            else
            {
                eyetimer++;
                if(eyetimer < 11)
                {
                    projectile.frame = 4;
                }
                if(eyetimer > 10)
                {
                    projectile.frame = 5;
                }
                if(eyetimer == 20)
                {
                    eyetimer = 0;
                }
            }


        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                projectile.velocity.Y = 0;
            }
            return false;
        }
    }
}

