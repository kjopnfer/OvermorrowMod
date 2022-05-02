using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.GameContent;
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
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 110;
            NPC.damage = 37; //25;
            NPC.defense = 0;
            NPC.lifeMax = 150; //100; //250;
            NPC.HitSound = SoundID.NPCHit19;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * /*0.5f*/ 1 * bossLifeScale);
        }

        public override void AI()
        {
            if (run)
            {
                NPC.velocity = Vector2.UnitY * -7.5f;
            }
            else
            {
                if ((!Main.npc[(int)NPC.ai[1]].boss) || !Main.bloodMoon)
                {
                    NPC.TargetClosest(false);
                    NPC.direction = 1;
                    NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                    if (NPC.timeLeft > 20)
                    {
                        NPC.timeLeft = 20;
                        return;
                    }
                }
                else
                {
                    parentNPC = Main.npc[(int)NPC.ai[1]];
                    NPC.timeLeft = 20;
                }

                if (!initializedRadius)
                {
                    radius = NPC.ai[2];
                    NPC.ai[2] = 0;
                    initializedRadius = true;
                }

                // AI[0] is the orbit position timer
                // AI[1] is used for locating Parent NPC
                // AI[2] is the phase
                // AI[3] is the timer


                Player player = Main.player[NPC.target];
                NPC.TargetClosest(true);

                NPC_OrbitPosition(NPC, parentNPC.Center, radius, radius == /*1050*/525 ? /*0.65*/ /*0.33f*/ /*0.65f*/ /*0.325f*/ ((OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && NPC.ai[3] > 400 && !OvermorrowWorld.dripPhase3) ? 0.4875f * 2 : 0.4875f) : /*1.25f*/ /*0.75f*/ /*0.5f*/ /*0.75f*/ 1.25f);
                switch (NPC.ai[2])
                {

                    case 0: // Do nothing
                        if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase3 && NPC.ai[3] > 1600)
                        {
                            ((DripplerBoss)Main.npc[(int)NPC.ai[1]].ModNPC).turnonattackindicator = true;
                            NPC.ai[2] = 2;
                            NPC.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && NPC.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            ((DripplerBoss)Main.npc[(int)NPC.ai[1]].ModNPC).turnonattackindicator = true;
                            NPC.ai[2] = 1;
                            NPC.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 1 && NPC.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            NPC.ai[2] = 3;
                            NPC.ai[3] = 0;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 2 && NPC.ai[3] > 400 && !OvermorrowWorld.dripPhase3)
                        {
                            NPC.ai[2] = 4;
                            NPC.ai[3] = 0;
                        }

                        /*if (npc.ai[3] % 240 == 0)
                        {
                            npc.ai[2] = 2;
                            npc.ai[3] = 0;
                        }*/
                        break;
                    case 1: // Shoot outwards

                        if (/*30 <*/ 0 < NPC.ai[3] && NPC.ai[3] < 480/*90*/ && NPC.ai[3] % 10 == 0)
                        {
                            //Vector2 ShootNormalizedVelocity = parentNPC.DirectionTo(npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, parentNPC.DirectionTo(NPC.Center) * 7, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (NPC.ai[3] > /*120*/ 480)
                        {
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                        }
                        break;
                    case 2: // Shoot towards center

                        if (0 < NPC.ai[3] && NPC.ai[3] < 480 && NPC.ai[3] % 5 == 0 && ((0 < NPC.ai[3] && NPC.ai[3] < 60) || (120 < NPC.ai[3] && NPC.ai[3] < 180) || (240 < NPC.ai[3] && NPC.ai[3] < 300) || (360 < NPC.ai[3] && NPC.ai[3] < 420)))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, /*direction * shootSpeed*/ parentNPC.DirectionFrom(NPC.Center) * 7, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (NPC.ai[3] > 480)
                        {
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                        }
                        break;
                    case 3: //shoot scatter shots at player
                        {
                            if (0 < NPC.ai[3] && NPC.ai[3] < 480 && NPC.ai[3] % 40 == 0)
                            {
                                ShootTarget = player.Center + Main.rand.NextVector2Circular(50, 50);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(ShootTarget) * 5, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                }
                            }

                            if (NPC.ai[3] > 480)
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                            }
                        }
                        break;
                    case 4: // volley
                        {
                            if (0 < NPC.ai[3] && NPC.ai[3] < 480 && NPC.ai[3] % /*10*/ 30 == 0)
                            {
                                ShootTarget = new Vector2((((NPC.Center.X - player.Center.X) * 0.75f) * -1) + NPC.Center.X, NPC.Center.Y - 150);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(ShootTarget) * 7.5f, ModContent.ProjectileType<BloodyBallGravity>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0/*1*/);
                                }
                            }

                            if (NPC.ai[3] > /*120*/ 480)
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                            }
                        }
                        break;

                }
                NPC.ai[3]++;
            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)NPC.ai[0];
            rad = deg * (Math.PI / 180);

            if (OvermorrowWorld.dripPhase3)
            {
                if ((OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && OvermorrowWorld.RotatingDripladAttackCounter == 0 && NPC.ai[3] > 400 && !OvermorrowWorld.dripPhase3))
                {
                    NPC.ai[0] += 0.3375f * 2;
                }
                else
                {
                    NPC.ai[0] += 0.3375f; //0.225f;
                }
            }
            else
            {
                NPC.ai[0] += 0.75f;
            }

            NPC.position.X = position.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;
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
            if (NPC.life > 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)NPC.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * (float)hitDirection, -2.5f);
            }
            var source = NPC.GetSource_OnHurt(null);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Driplad1").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Driplad2").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Driplad3").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
        }

        public override void FindFrame(int frameHeight)
        {
            int num = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];

            NPC.rotation = NPC.velocity.X * 0.15f;
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += num;
            }
            if (NPC.frame.Y >= num * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/DripplerBoss/Driplad_Glowmask").Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - screenPos.X, NPC.Center.Y - screenPos.Y + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void OnKill()
        {
            if (Main.npc[(int)NPC.ai[1]].active && Main.npc[(int)NPC.ai[1]].boss)
            {
                NPC parentNPC = Main.npc[(int)NPC.ai[1]];
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