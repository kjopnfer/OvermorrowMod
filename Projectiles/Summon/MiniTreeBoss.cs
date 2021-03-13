using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MiniTreeBoss : ModProjectile
    {
        private int randDelay;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini-Iorich");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 84;
            projectile.height = 84;
            projectile.tileCollide = true;
            //projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.sentry = true;
            projectile.minion = true;
            projectile.timeLeft = 7200; // 2 minutes

            //drawOriginOffsetY = -80;
            drawOffsetX = 0;
            drawOriginOffsetX = 0;
            drawOriginOffsetY = 0;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.66f, 0f);

            Player player = Main.player[projectile.owner];
            player.UpdateMaxTurrets();

            if(projectile.localAI[0] == 0f)
            {
                projectile.localAI[1] = 1f;
                projectile.localAI[0] = 1f;
                //projectile.ai[0] = 120f;
                int num1109 = 80;
                for (int num1108 = 0; num1108 < num1109; num1108++)
                {
                        int num1107 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 107);
                        Dust dust81 = Main.dust[num1107];
                        dust81.velocity *= 2f;
                        Main.dust[num1107].noGravity = true;
                        dust81 = Main.dust[num1107];
                        dust81.scale *= 1.15f;
                }    
            }
            projectile.velocity.X = 0f;
            projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            if(projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            // Starting search distance
            float distanceFromTarget = 700f; // range of 700f
            Vector2 targetCenter = projectile.position;
            NPC targetNPC = null;
            bool foundTarget = false;


            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, projectile.Center); // distance between the npc and minion
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between; // targetcenter = npc center
                        bool inRange = between < distanceFromTarget; // if the distance between npc and minion is less than 700
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 2000f;
                        if (((closest && inRange) || !foundTarget) && closeThroughWall)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            targetNPC = npc;
                            foundTarget = true;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                Vector2 delta = targetCenter - projectile.Center;
                float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                if (magnitude > 0)
                {
                    delta *= 12f / magnitude;
                }
                else
                {
                    delta = new Vector2(0f, 5f);
                }

                if (projectile.ai[0] == 1)
                {
                    randDelay = Main.rand.Next(200, 300);
                }

                //bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);
                projectile.ai[0]++;

                if (projectile.ai[0] > 300)
                {
                    projectile.ai[0] = 0;
                }

                if (projectile.ai[0] % 240 == 0) // prevent from instantly shooting when spawned
                {
                    Vector2 npcPos = new Vector2(targetNPC.position.X / 16, targetNPC.position.Y / 16);
                    Tile tile2 = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                    while (!tile2.active() || tile2.type == TileID.Trees)
                    {
                        npcPos.Y += 1;
                        tile2 = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(npcPos * 16, new Vector2(0, -6), ModContent.ProjectileType<ThornHeadFriendly>(), projectile.damage / 2, 0f, projectile.owner);
                    }
                } 
            }


            // Loop through the 4 animation frames, spending 12 ticks on each.
            if (++projectile.frameCounter >= 12)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            base.PostDraw(spriteBatch, lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}