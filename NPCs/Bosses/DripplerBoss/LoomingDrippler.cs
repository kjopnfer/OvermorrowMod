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
    public class LoomingDrippler : ModNPC
    {
        private NPC parentNPC;
        private int moveSpeed;
        private int randIncrementer;
        private bool secondTeleport = false;
        private int randSwitch = 300;
        private Vector2 origin;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Looming Drippler");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 28;
            npc.height = 30;
            npc.damage = 28;
            npc.defense = 17;
            npc.lifeMax = 240;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0.4f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 19;
            npc.knockBackResist = 0f;
        }

        public override void AI()
        {
            // Vanilla code is garbage that breaks itself, seriously what is even wrong with the X velocity of this crap
            // You can only count on your own code nowadays
            // This is the stupidest NPC I've ever made and I've made bosses, it LITERALLY WON'T MOVE

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
            }

            Player player = Main.player[npc.target];

            npc.TargetClosest(true);
            npc.ai[0]++;

            if (npc.ai[3] == 0)
            {
                moveSpeed = Main.rand.Next(7, 14);
                npc.ai[3]++;
            }

            // AI[0] is the counter
            // AI[1] is the parent NPC
            // AI[2] is the phase
            // AI[3] is the randomly assigned movespeed

            switch (npc.ai[2])
            {
                case 0: // Follow player
                    if (OvermorrowWorld.DripplerCircle)
                    {
                        npc.ai[2] = 2;
                        npc.ai[0] = 0;
                        break;
                    }

                    Vector2 moveTo = player.Center;
                    var move = moveTo - npc.Center;
                    //var speed = 10;

                    float length = move.Length();
                    if (length > moveSpeed)
                    {
                        move *= moveSpeed / length;
                    }
                    var turnResistance = 45;
                    move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                    length = move.Length();
                    if (length > 10)
                    {
                        move *= moveSpeed / length;
                    }
                    npc.velocity.X = move.X;
                    npc.velocity.Y = move.Y * .98f;

                    if (npc.ai[0] % randSwitch == 0)
                    {
                        npc.ai[2] = 2;
                        npc.ai[0] = 0;
                    }
                    break;
                case 1: // Shoot at player
                    if (OvermorrowWorld.DripplerCircle)
                    {
                        npc.ai[2] = 2;
                        npc.ai[0] = 0;
                        break;
                    }

                    npc.velocity = Vector2.Zero;
                    if (npc.ai[0] % 30 == 0)
                    {
                        int shootSpeed = Main.rand.Next(6, 10);
                        Vector2 npcPosition = npc.Center;
                        Vector2 targetPosition = Main.player[npc.target].Center;
                        Vector2 direction = targetPosition - npcPosition;
                        direction.Normalize();

                        if (parentNPC.life <= parentNPC.lifeMax * 0.39f)
                        {
                            int chooseShoot = Main.rand.Next(2);
                            if (chooseShoot == 0)
                            {
                                float numberProjectiles = 3;
                                float rotation = MathHelper.ToRadians(45);
                                Vector2 delta = player.Center - npc.Center;
                                float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                                if (magnitude > 0)
                                {
                                    delta *= 5f / magnitude;
                                }
                                else
                                {
                                    delta = new Vector2(0f, 5f);
                                }

                                Main.PlaySound(SoundID.Item17, (int)npc.Center.X, (int)npc.Center.Y);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < numberProjectiles; i++)
                                    {
                                        Vector2 perturbedSpeed = new Vector2(delta.X, delta.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .3f;
                                        // * 3f increases speed
                                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X * 3f, perturbedSpeed.Y * 3f, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    }
                                }
                            }
                            else
                            {
                                Main.PlaySound(SoundID.Item17, (int)npc.Center.X, (int)npc.Center.Y);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 3f, Main.myPlayer, 0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 4, 3f, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (npc.ai[0] == 90)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case 2: // Teleportation buffer
                    if (npc.ai[2] == 2)
                    {
                        int countDripplers = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<LoomingDrippler>())
                            {
                                countDripplers++;
                            }
                        }

                        npc.velocity = Vector2.Zero;
                        
                        if (npc.alpha < 255)
                        {
                            if (npc.alpha > 255)
                            {
                                npc.alpha = 255;
                            }
                            else
                            {
                                npc.alpha += 3;
                            }
                        }

                        if (npc.alpha >= 255)
                        {
                            if (OvermorrowWorld.DripplerCircle)
                            {
                                npc.ai[2] = 3;
                                npc.ai[0] = 0;
                                npc.ai[3] = Main.rand.Next(0, 360); // rotation counter
                                origin = player.Center;
                            }
                            else
                            {
                                npc.ai[2] = 4;
                                npc.ai[0] = 0;
                            }
                        }
                    }
                    break;
                case 3: // Rotate around the player
                    NPC_OrbitPosition(npc, origin, 300, 1f);

                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 3;
                    }

                    if (npc.ai[0] >= 395)
                    {
                        npc.alpha += 3;
                    }

                    if (npc.ai[0] == 480)
                    {
                        OvermorrowWorld.DripplerCircle = false;
                        npc.alpha = 0;
                        npc.ai[2] = 2; // switch
                        npc.ai[0] = 0;
                        npc.ai[3] = 0;
                    }
                    break;
                case 4: // Teleport to a random position
                    if (npc.ai[0] == 1)
                    {
                        randIncrementer = Main.rand.Next(2, 5);
                        Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-6, 6) * 100, player.Center.Y + Main.rand.Next(-6, 6) * 100);
                        npc.position = randPos;
                        npc.netUpdate = true;
                    }

                    npc.velocity = Vector2.Zero;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= randIncrementer;
                    }

                    if (npc.alpha <= 0)
                    {
                        int shootSpeed = Main.rand.Next(6, 10);
                        Vector2 npcPosition = npc.Center;
                        Vector2 targetPosition = Main.player[npc.target].Center;
                        Vector2 direction = targetPosition - npcPosition;
                        direction.Normalize();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 3, 3f, Main.myPlayer, 0, 0);
                        }

                        if (!secondTeleport)
                        {
                            secondTeleport = true;
                            npc.ai[2] = 2;
                            npc.ai[0] = 0;
                        }
                        else
                        {
                            secondTeleport = false;
                            randSwitch = Main.rand.Next(300, 700);
                            npc.ai[2] = 0;
                            npc.ai[0] = 0;
                        }
                    }

                    break;
            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)npc.ai[3];
            rad = deg * (Math.PI / 180);
            npc.ai[3] += 2f;

            npc.position.X = position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2;
            npc.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2;
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

            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Looming1"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Looming2"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Looming3"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Looming" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Looming" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
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
            if (npc.alpha == 0)
            {
                Texture2D texture = mod.GetTexture("NPCs/Bosses/DripplerBoss/LoomingDrippler_Glowmask");
                spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y - 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
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