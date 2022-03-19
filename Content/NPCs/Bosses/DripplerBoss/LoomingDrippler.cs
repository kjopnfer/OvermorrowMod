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
    public class LoomingDrippler : ModNPC
    {
        public bool run;
        private NPC parentNPC;
        private int moveSpeed;
        private int randIncrementer;
        private bool secondTeleport = false;
        private int randSwitch = 300;
        private int storedDamage;
        private Vector2 origin;
        int attackCounter;
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
            npc.defense = 0;
            npc.lifeMax = 75;//200;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0.4f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * /*0.5f*/ 1 * bossLifeScale);
            npc.defense = 0;
            npc.knockBackResist = 0f;
        }

        public override void AI()
        {

            if (run)
            {
                npc.velocity = Vector2.UnitY * -7.5f;
            }
            else
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
                    moveSpeed = Main.rand.Next(5, 7);
                    npc.ai[3]++;
                }

                // AI[0] is the counter
                // AI[1] is the parent NPC
                // AI[2] is the phase
                // AI[3] is the randomly assigned movespeed

                switch (npc.ai[2])
                {
                    case -1: // Hide NPC
                        if (OvermorrowWorld.DripladShoot)
                        {
                            if (npc.alpha < 255)
                            {
                                npc.alpha += 3;
                            }

                            npc.velocity = Vector2.Zero;
                            npc.damage = 0;
                        }
                        else
                        {
                            if (npc.alpha > 0)
                            {
                                npc.alpha -= 3;
                            }

                            if (npc.alpha <= 0)
                            {
                                npc.damage = storedDamage;
                                npc.ai[2] = 0;
                                npc.ai[0] = 0;
                            }
                        }
                        break;
                    case 0: // Follow player
                        if (OvermorrowWorld.DripplerCircle)
                        {
                            npc.ai[2] = 2;
                            npc.ai[0] = 0;
                            break;
                        }

                        if (OvermorrowWorld.DripladShoot)
                        {
                            storedDamage = npc.damage;
                            npc.ai[2] = -1;
                            npc.ai[0] = 0;
                            break;
                        }

                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;

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
                                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X * 3f, perturbedSpeed.Y * 3f, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 2f, Main.myPlayer, 0f, 0f);
                                        }
                                    }
                                }
                                else
                                {
                                    Main.PlaySound(SoundID.Item17, (int)npc.Center.X, (int)npc.Center.Y);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*4*/ 3, 3f, Main.myPlayer, 0, 0);
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
                                    ((DripplerBoss)Main.npc[(int)npc.ai[1]].modNPC).turnonattackindicator = true;
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
                        if (OvermorrowWorld.DripladShoot)
                        {
                            storedDamage = npc.damage;
                            npc.ai[2] = -1;
                            npc.ai[0] = 0;
                            break;
                        }

                        if (npc.ai[0] == 1)
                        {
                            randIncrementer = Main.rand.Next(2, 5);
                            Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-6, 6) * 100, player.Center.Y + Main.rand.Next(-6, 6) * 100);
                            while (Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].active())
                            {
                                randPos = new Vector2(player.Center.X + Main.rand.Next(-6, 6) * 100, player.Center.Y + Main.rand.Next(-6, 6) * 100);
                            }
                            npc.position = randPos;
                            npc.netUpdate = true;
                        }
                        if (OvermorrowWorld.loomingdripplerdeadcount <= 6)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        if (npc.alpha > 0)
                        {
                            npc.alpha -= randIncrementer;
                        }

                        if (npc.alpha <= 0 && OvermorrowWorld.loomingdripplerdeadcount <= 6)
                        {
                            int shootSpeed = Main.rand.Next(6, 10);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.rand.Next(2) == 0)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                }
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
                        else if (npc.alpha <= 0 && OvermorrowWorld.loomingdripplerdeadcount > 6)
                        {
                            Vector2 targetPosition = Main.player[npc.target].Center;
                            if (++attackCounter == 30)
                            {
                                Vector2 origin = npc.Center;
                                float radius = 45;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.HeartCrystal, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                                npc.velocity = 10f * npc.DirectionTo(new Vector2(Main.rand.NextFloat(targetPosition.X - 25, targetPosition.X + 25), Main.rand.NextFloat(targetPosition.Y - 25, targetPosition.Y + 25)));
                            }
                            else if (attackCounter > 60 && attackCounter < 120)
                            {
                                npc.velocity = new Vector2(MathHelper.Lerp(npc.velocity.X, 0, 0.025f), MathHelper.Lerp(npc.velocity.Y, 0, 0.025f));
                            }

                            if (attackCounter > 120)
                            {
                                if (!secondTeleport)
                                {
                                    secondTeleport = true;
                                    npc.ai[2] = 2;
                                    npc.ai[0] = 0;
                                    attackCounter = 0;
                                    npc.velocity = Vector2.Zero;
                                }
                                else
                                {
                                    secondTeleport = false;
                                    randSwitch = Main.rand.Next(300, 700);
                                    npc.ai[2] = 0;
                                    npc.ai[0] = 0;
                                    attackCounter = 0;
                                    npc.velocity = Vector2.Zero;
                                }
                            }
                        }
                        break;
                }
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
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 2.5f * (float)hitDirection, -2.5f);
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
                Texture2D texture = mod.GetTexture("Content/NPCs/Bosses/DripplerBoss/LoomingDrippler_Glowmask");
                spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y - 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.loomingdripplerdeadcount += 1;
            if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].boss)
            {
                if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].boss)
                {
                    NPC parentNPC = Main.npc[(int)npc.ai[1]];
                    if (Main.expertMode)
                    {
                        parentNPC.life -= 200;//400;
                    }
                    else
                    {
                        parentNPC.life -= 100;//200;
                    }
                }
            }
        }
    }
}