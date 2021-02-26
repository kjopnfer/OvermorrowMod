using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class LoomingDrippler : ModNPC
    {
        private NPC parentNPC;
        private int speed;
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
            npc.lifeMax = 110;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0.4f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.2f);
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

            if(npc.ai[3] == 0)
            {
                speed = Main.rand.Next(14, 21);
                npc.ai[3]++;
            }

            switch (npc.ai[2])
            {
                case 0: // Follow player
                    Vector2 moveTo = player.Center;
                    var move = moveTo - npc.Center;
                    var speed = 10;

                    float length = move.Length();
                    if (length > speed)
                    {
                        move *= speed / length;
                    }
                    var turnResistance = 45;
                    move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                    length = move.Length();
                    if (length > 10)
                    {
                        move *= speed / length;
                    }
                    npc.velocity.X = move.X;
                    npc.velocity.Y = move.Y * .98f;

                    if (npc.ai[0] % Main.rand.Next(300, 900) == 0)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 1;
                    }
                    break;
                case 1: // Shoot at player
                    npc.velocity = Vector2.Zero;
                    if (npc.ai[0] % 30 == 0)
                    {
                        int shootSpeed = Main.rand.Next(6, 10);
                        Vector2 position = npc.Center;
                        Vector2 targetPosition = Main.player[npc.target].Center;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();

                        if(parentNPC.life <= parentNPC.lifeMax * 0.39f)
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
                                for (int i = 0; i < numberProjectiles; i++)
                                {
                                    Vector2 perturbedSpeed = new Vector2(delta.X, delta.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .3f;
                                    // * 3f increases speed
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X * 3f, perturbedSpeed.Y * 3f, ModContent.ProjectileType<BloodyBall>(), npc.damage, 2f, Main.myPlayer, 0f, 0f);
                                }
                            }
                            else
                            {
                                Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }
                        else
                        {
                            Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<BloodyBall>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (npc.ai[0] == 90)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
            }

            if (npc.ai[0] == 900)
            {
                npc.ai[0] = 0;
            }
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
            Texture2D texture = mod.GetTexture("NPCs/Bosses/DripplerBoss/LoomingDrippler_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y - 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].boss)
            {
                NPC parentNPC = Main.npc[(int)npc.ai[1]];
                parentNPC.life -= 100;
            }
        }
    }
}