using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class PlasmaKnife : ModProjectile
    {
        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private bool OtherCheck = false;
        private int flametimer = 0;
        Vector2 endPoint;
        bool foundTarget;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        Vector2 newMove;
        Vector2 Rot;
        Vector2 SavedMove;
        float CircleArr = 0;
        float CircleArr2 = 0;
        int NPCheight;
        int NPCwidth;
        Vector2 tar;
        Vector2 tarvelo;
        int TimerOf = 0;
        bool closest;
        bool inRange;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Boomerang");
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 26;
            projectile.timeLeft = 500;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {




            float distanceFromTarget = 110f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;
            if(!ComingBack)
            {
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.velocity.Y += 0.07f;
            }
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, projectile.Center);
                         closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                         inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y;

                            NPCheight = npc.height;
                            NPCwidth = npc.width;

                            tar = npc.Center;
                            tarvelo = npc.velocity;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            if (foundTarget && Main.player[projectile.owner].channel)
            {
                float betweentar = Vector2.Distance(tar, projectile.Center);
                if(betweentar < 110 || ComingBack)
                {
                projectile.tileCollide = false;
                projectile.timeLeft = 100;
                TimerOf++;
                projectile.velocity.X = 0;
                projectile.velocity.Y = 0;
                if(TimerOf < 4)
                {
                    CircleArr = (projectile.Center - tar).ToRotation();
                }
                if(TimerOf > 2)
                {
                    projectile.rotation = (tar - projectile.Center).ToRotation();
                    ComingBack = true;
                    projectile.position.X = 90 * (float)Math.Cos(CircleArr) + tar.X - projectile.width / 2;
                    projectile.position.Y = 90 * (float)Math.Sin(CircleArr) + tar.Y - projectile.height / 2;
                    CircleArr += (float)((2 * Math.PI) / (Math.PI * 2 * 200 / 10)); // 200 is the speed, god only knows what dividing by 10 does
                }
                }
            }
            else if(foundTarget && ComingBack)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = tar;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 10f;
            }
        }
    }
}