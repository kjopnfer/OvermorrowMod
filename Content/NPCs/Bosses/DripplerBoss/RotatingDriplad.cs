using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Projectiles.Boss;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class RotatingDriplad : ModNPC
    {
        public bool run;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/DripplerBoss/Driplad";
        private NPC parentNPC;
        private bool initializedRadius = false;
        private float radius;
        Vector2 ShootTarget;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplad");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 68;
            npc.height = 110;
            npc.damage = 37; //25;
            npc.defense = 0;
            npc.lifeMax = 150; //100; //250;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * /*0.5f*/ 1 * bossLifeScale);
        }

        public override void AI()
        {
            if (run)
            {
                npc.velocity = Vector2.UnitY * -7.5f;
            }
            else
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

                NPC_OrbitPosition(npc, parentNPC.Center, radius, radius == /*1050*/525 ? /*0.65*/ /*0.33f*/ /*0.65f*/ /*0.325f*/ ((OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && npc.ai[3] > 400 && !OvermorrowWorld.dripPhase3) ? 0.4875f * 2 : 0.4875f) : /*1.25f*/ /*0.75f*/ /*0.5f*/ /*0.75f*/ 1.25f);
                switch (npc.ai[2])
                {

                    case 0: // Do nothing
                        if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase3 && npc.ai[3] > 1600)
                        {
                            ((DripplerBoss)Main.npc[(int)npc.ai[1]].modNPC).turnonattackindicator = true;
                            npc.ai[2] = 2;
                            npc.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && npc.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            ((DripplerBoss)Main.npc[(int)npc.ai[1]].modNPC).turnonattackindicator = true;
                            npc.ai[2] = 1;
                            npc.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 1 && npc.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            npc.ai[2] = 3;
                            npc.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 2 && npc.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            npc.ai[2] = 4;
                            npc.ai[3] = 0;
                        }

                        /*if (npc.ai[3] % 240 == 0)
                        {
                            npc.ai[2] = 2;
                            npc.ai[3] = 0;
                        }*/
                        break;
                    case 1: // Shoot outwards

                        if (/*30 <*/ 0 < npc.ai[3] && npc.ai[3] < 480/*90*/ && npc.ai[3] % 10 == 0)
                        {
                            //Vector2 ShootNormalizedVelocity = parentNPC.DirectionTo(npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, parentNPC.DirectionTo(npc.Center) * 7, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (npc.ai[3] > /*120*/ 480)
                        {
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }
                        break;
                    case 2: // Shoot towards center

                        if (0 < npc.ai[3] && npc.ai[3] < 480 && npc.ai[3] % 5 == 0 && ((0 < npc.ai[3] && npc.ai[3] < 60) || (120 < npc.ai[3] && npc.ai[3] < 180) || (240 < npc.ai[3] && npc.ai[3] < 300) || (360 < npc.ai[3] && npc.ai[3] < 420)))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, /*direction * shootSpeed*/ parentNPC.DirectionFrom(npc.Center) * 7, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (npc.ai[3] > 480)
                        {
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }
                        break;
                    case 3: //shoot scatter shots at player
                        {
                            if (0 < npc.ai[3] && npc.ai[3] < 480 && npc.ai[3] % 40 == 0)
                            {
                                ShootTarget = player.Center + Main.rand.NextVector2Circular(50, 50);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(ShootTarget) * 5, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                }
                            }

                            if (npc.ai[3] > 480)
                            {
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                        break;
                    case 4: // volley
                        {
                            if (0 < npc.ai[3] && npc.ai[3] < 480 && npc.ai[3] % /*10*/ 30 == 0)
                            {
                                ShootTarget = new Vector2((((npc.Center.X - player.Center.X) * 0.75f) * -1) + npc.Center.X, npc.Center.Y - 150);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(ShootTarget) * 7.5f, ModContent.ProjectileType<BloodyBallGravity>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0/*1*/);
                                }
                            }

                            if (npc.ai[3] > /*120*/ 480)
                            {
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                        break;

                }
                npc.ai[3]++;
            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)npc.ai[0];
            rad = deg * (Math.PI / 180);

            if (OvermorrowWorld.dripPhase3)
            {
                if ((OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && npc.ai[3] > 400 && !OvermorrowWorld.dripPhase3))
                {
                    npc.ai[0] += 0.3375f * 2;
                }
                else
                {
                    npc.ai[0] += 0.3375f; //0.225f;
                }
            }
            else
            {
                npc.ai[0] += 0.75f;
            }

            npc.position.X = position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2;
            npc.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2;
        }
        //// Get the outer circle
        //public Vector2 NPC_CalculateOuter(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        //{
        //    double rad;
        //    double deg = speed * npc.ai[0];
        //    rad = deg * (Math.PI / 180);

        //    return new Vector2(position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2, position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2);
        //}
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)npc.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 2.5f * (float)hitDirection, -2.5f);
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
            Texture2D texture = mod.GetTexture("Content/NPCs/Bosses/DripplerBoss/Driplad_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].boss)
            {
                NPC parentNPC = Main.npc[(int)npc.ai[1]];
                if (Main.expertMode)
                {
                    parentNPC.life -= 400; //200;
                }
                else
                {
                    parentNPC.life -= 200; //100;
                }
            }
        }
    }
}