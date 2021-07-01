using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class RotatingDriplad : ModNPC
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/DripplerBoss/Driplad";
        private NPC parentNPC;
        private bool initializedRadius = false;
        private float radius;
        Vector2 ShootTarget;
        public bool Randomshotistrue = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplad");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 68;
            npc.height = 110;
            npc.damage = 25;
            npc.defense = 20;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
        }

        public override void AI()
        {
            if ((!Main.npc[(int)npc.ai[1]].boss) || !Main.bloodMoon)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }
            }
            else
            {
                parentNPC = Main.npc[(int)npc.ai[1]];
                npc.timeLeft = 20;
            }

            if (!initializedRadius)
            {
                radius = npc.ai[2];
                npc.ai[2] = 0;
                initializedRadius = true;
            }

            // AI[0] is the orbit position timer
            // AI[1] is used for locating Parent NPC
            // AI[2] is the phase
            // AI[3] is the timer


            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            npc.ai[3]++;

            NPC_OrbitPosition(npc, parentNPC.Center, radius, radius == /*1050*/525 ? 0.65f : 1.25f);

            switch (npc.ai[2])
            {
                case 0: // Do nothing
                    if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase3)
                    {
                        npc.ai[2] = 2;
                        npc.ai[3] = 0;
                    }
                    else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && Randomshotistrue == false)
                    {
                        npc.ai[2] = 1;
                        npc.ai[3] = 0;
                    }
                    else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && Randomshotistrue == true)
                    {
                        npc.ai[2] = 3;
                        npc.ai[3] = 0;
                    }

                    /*if (npc.ai[3] % 240 == 0)
                    {
                        npc.ai[2] = 2;
                        npc.ai[3] = 0;
                    }*/
                    break;
                case 1: // Shoot outwards
                    if (30 < npc.ai[3] && npc.ai[3] < 90 && npc.ai[3] % 10 == 0)
                    {
                        Vector2 ShootNormalizedVelocity = parentNPC.DirectionTo(npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, ShootNormalizedVelocity * 7, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 3f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (npc.ai[3] > 120)
                    {
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                    break;
                case 2: // Shoot towards center
                    if (30 < npc.ai[3] && npc.ai[3] < 90 && npc.ai[3] % 5 == 0)
                    {
                        int shootSpeed = 7;
                        Vector2 npcPosition = npc.Center;
                        Vector2 targetPosition = parentNPC.Center;
                        Vector2 direction = targetPosition - npcPosition;
                        direction.Normalize();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 3f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (npc.ai[3] > 90)
                    {
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                    break;
                case 3: //shoot  scatter shots at player
                    {
                        if (30 < npc.ai[3] && npc.ai[3] < 90 && npc.ai[3] % 10 == 0)
                        { 
                            ShootTarget = player.Center + Main.rand.NextVector2Circular(50, 50);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(ShootTarget) * 7, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (npc.ai[3] > 120)
                        {
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }
                    }
                    break;

            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)npc.ai[0];
            rad = deg * (Math.PI / 180);

            if (OvermorrowWorld.dripPhase3)
            {
                npc.ai[0] += 0.45f;
            }
            else
            {
                npc.ai[0] += 0.75f;
            }

            npc.position.X = position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2;
            npc.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2;
        }

        // Get the outer circle
        public Vector2 NPC_CalculateOuter(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * npc.ai[0];
            rad = deg * (Math.PI / 180);

            return new Vector2(position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2, position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)npc.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, 2.5f * (float)hitDirection, -2.5f);
            }

            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad1"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad2"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad3"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
        }

        public override void FindFrame(int frameHeight)
        {
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];

            npc.rotation = npc.velocity.X * 0.15f;
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y += num;
            }
            if (npc.frame.Y >= num * Main.npcFrameCount[npc.type])
            {
                npc.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/DripplerBoss/Driplad_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].boss)
            {
                NPC parentNPC = Main.npc[(int)npc.ai[1]];
                parentNPC.life -= 200;
            }
        }
    }
}