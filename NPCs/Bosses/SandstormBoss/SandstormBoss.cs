using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Microsoft.Xna.Framework.Graphics;
using System;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Items.BossBags;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    [AutoloadBossHead]
    public class SandstormBoss : ModNPC
    {
        private bool doDustAttack = false;
        private Vector2 rotationCenter;
        private int enrageTimer;
        private bool enraged;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dharuud, the Sandstorm");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 136;
            npc.height = 112;
            npc.aiStyle = -1;
            npc.damage = 24;
            npc.defense = 12;
            npc.lifeMax = 5200;
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SandstormBoss");
            bossBag = ModContent.ItemType<SandstormBag>();
        }

        public static void SandstormStuff()
        {
            Sandstorm.IntendedSeverity = !Sandstorm.Happening ? /*(Main.rand.Next(3) != 0 ? Main.rand.NextFloat() * 0.3f : 0.0f)*/ 0.4f : 0.4f /*+ Main.rand.NextFloat()*/;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            NetMessage.SendData(7, -1, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

            // Handles Despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (Sandstorm.Happening)
                {
                    Sandstorm.Happening = false;
                    Sandstorm.TimeLeft = 18000;
                    SandstormStuff();
                }

                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }

            }

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = 2000;
            }

            if (!Sandstorm.Happening)
            {
                Sandstorm.Happening = true;
                Sandstorm.TimeLeft = (int)(3600.0 * (8.0 + (double)Main.rand.NextFloat() * 16.0));
                SandstormStuff();
            }

            if (!player.ZoneDesert)
            {
                if(enrageTimer == 120)
                {
                    enraged = true;
                }
                else
                {
                    enraged = false;
                    enrageTimer++;
                }
            }
            else
            {
                enraged = false;
                enrageTimer = 0;
            }

            if (npc.ai[0] == 3)
            {
                npc.ai[1] += 10f;
            }
            else
            {
                npc.ai[1]++;
            }

            if (npc.ai[0] == 2 || npc.ai[0] == 3)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }

            switch (npc.ai[0])
            {
                case 0: // General case: Move towards player
                    if (!player.ZoneDesert && enraged)
                    {
                        npc.ai[0] = 1.5f;
                        npc.ai[1] = 0;
                        npc.ai[2] = 900;
                        break;
                    }

                    Vector2 moveTo = player.Center;
                    var move = moveTo - npc.Center;
                    var speed = 5;

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

                    /*
                    if(npc.ai[1] == 10) EZ CODE TESTER FOR ATTACKS
                    {
                        npc.ai[0] = 3;
                        npc.ai[1] = 0;
                    }*/
                    if (npc.ai[1] == 360)
                    {
                        if (doDustAttack)
                        {
                            npc.ai[0] = 1.5f;
                            npc.ai[1] = 0;
                            npc.ai[2] = 900;
                            doDustAttack = false;
                        }
                        else
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Spawn sand from off screen
                    if (!player.ZoneDesert && enraged)
                    {
                        npc.ai[0] = 1.5f;
                        npc.ai[1] = 0;
                        npc.ai[2] = 900;
                        break;
                    }

                    npc.velocity = Vector2.Zero;

                    if (npc.ai[1] % 60 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.rand.Next(3, 6); i++)
                            {
                                npc.netUpdate = true;
                                Projectile.NewProjectile(new Vector2(player.Center.X + Main.rand.Next(1200, 1500), npc.Center.Y + Main.rand.Next(-360, 360)), new Vector2(Main.rand.Next(-11, -6), 0), ModContent.ProjectileType<SandBall>(), npc.damage / (Main.expertMode ? 4 : 2), 0f, Main.myPlayer);
                            }

                            if(npc.life <= npc.lifeMax * 0.5f)
                            {
                                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                                {
                                    npc.netUpdate = true;
                                    Projectile.NewProjectile(new Vector2(player.Center.X - Main.rand.Next(1200, 1500), npc.Center.Y + Main.rand.Next(-360, 360)), new Vector2(Main.rand.Next(6, 11), 0), ModContent.ProjectileType<SandBall>(), npc.damage / (Main.expertMode ? 4 : 2), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    if (npc.ai[1] == 600)
                    {
                        if (!doDustAttack)
                        {
                            doDustAttack = true;
                        }

                        npc.ai[0] = 0f;
                        npc.ai[1] = 0;
                    }
                    break;
                case 1.5f: // Do dust swirl animation
                    npc.velocity = Vector2.Zero;

                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustPos = npc.Center + new Vector2(npc.ai[2], 0).RotatedBy(MathHelper.ToRadians(i * 20 + npc.ai[1]));
                        Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 32, 0f, 0f, 0, default, 2.04f)];
                        dust.noGravity = true;
                    }

                    npc.ai[2] -= 15;

                    if (npc.ai[2] == 0)
                    {
                        npc.ai[0] = 2;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case 2: // Turn into dust cloud
                    npc.hide = true;

                    if (npc.ai[2] == (Main.expertMode ? 50 : 66))
                    {
                        npc.velocity = Vector2.Zero;
                    }

                    npc.width = 68;
                    npc.height = 56;

                    if (npc.ai[2] <= 0)
                    {
                        float chargeSpeed = !player.ZoneDesert ? 30 : 22;
                        move = player.Center - npc.Center;
                        float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                        move *= chargeSpeed / magnitude;
                        npc.velocity = move;
                        npc.netUpdate = true;
                        if (!player.ZoneDesert)
                        {
                            npc.ai[2] = 40;
                        }
                        else
                        {
                            npc.ai[2] = 80; // charging delay
                        }
                    }
                    npc.ai[2] -= 1f;

                    if (npc.ai[1] == 600)
                    {
                        if (player.ZoneDesert)
                        {
                            npc.width = 136;
                            npc.height = 112;

                            npc.velocity = Vector2.Zero;
                            npc.hide = false;

                            if (Main.expertMode)
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                            }
                            else
                            {
                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                            }
                        }
                        else
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Spin in a circle

                    // Get center to spin around
                    if (npc.ai[1] == 10)
                    {
                        rotationCenter = new Vector2(npc.Center.X + 50, npc.Center.Y);
                    }

                    double deg = (double)npc.ai[1];
                    double rad = deg * (Math.PI / 180); //Convert degrees to radians
                    double dist = 50; //Distance away from the player

                    npc.position.X = rotationCenter.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
                    npc.position.Y = rotationCenter.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;

                    if (npc.ai[1] % 50 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(Math.PI) * 6, ModContent.ProjectileType<SandBall2>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    // Move the NPC around the center
                    if (npc.ai[1] == 2400)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                    }
                    break;
            }

            if (npc.ai[0] != 2 && npc.ai[0] != 3)
            {
                if (Main.rand.NextFloat() < 0.5526316f)
                {

                    for (int num1202 = 0; num1202 < 4; num1202++)
                    {
                        Dust.NewDust(npc.Center - new Vector2(npc.width / 4, -35), npc.width / 3, npc.height, 32, 0, 2.63f, default, default, 1.45f);
                    }
                }
            }
            else
            {
                for (int num1101 = 0; num1101 < 6; num1101++)
                {
                    int num1110 = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y), npc.width, npc.height, 32, npc.velocity.X, npc.velocity.Y, 50, default(Color), 3f);
                    Main.dust[num1110].position = (Main.dust[num1110].position + npc.Center) / 2f;
                    Main.dust[num1110].noGravity = true;
                    Dust dust81 = Main.dust[num1110];
                    dust81.velocity *= 0.5f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 6) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] != 2 && npc.ai[0] != 3)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                Texture2D texture2D16 = mod.GetTexture("NPCs/Bosses/SandstormBoss/SandstormBoss");


                // this gets the npc's frame
                Vector2 vector47 = drawOrigin;
                Color color55 = Color.White; // This is just white lol
                float amount10 = 0f; // I think this controls amount of color
                int num178 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num177 = 6; // ok i think this controls the number of afterimage frames
                float num176 = 1f - (float)Math.Cos((npc.ai[1] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num176 /= 3f;
                float scaleFactor10 = 7f; // Change scale factor of the pulsing effect and how far it draws outwards

                Color color47 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                color55 = Color.LightGoldenrodYellow;
                amount10 = 1f;

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num177; num164++)
                {
                    // these assign the color of the pulsing
                    Color color45 = color47;
                    color45 = Color.Lerp(color45, color55, amount10);
                    color45 = ((ModNPC)this).npc.GetAlpha(color45);
                    color45 *= 1f - num176; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector45 = ((Entity)((ModNPC)this).npc).Center + Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + ((ModNPC)this).npc.rotation) * scaleFactor10 * num176 - Main.screenPosition;
                    vector45 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).npc.type]) * ((ModNPC)this).npc.scale / 2f;
                    vector45 += vector47 * ((ModNPC)this).npc.scale + new Vector2(0f, 4f + ((ModNPC)this).npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture2D16, vector45 /*- new Vector2(0, 290 / 2)*/, ((ModNPC)this).npc.frame, color45, ((ModNPC)this).npc.rotation, vector47, ((ModNPC)this).npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            if (npc.ai[0] == 2 || npc.ai[0] == 3)
            {
                return false;
            }

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedDarude = true;

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(4);
                // Always drops one of:
                if (choice == 0) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandstormSpinner>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandStaff>());
                }
                else if (choice == 2) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandThrower>());
                }
                else if (choice == 3) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DustStaff>());
                }
            }

            if (Sandstorm.Happening)
            {
                Sandstorm.Happening = false;
                Sandstorm.TimeLeft = 18000;
                SandstormStuff();
            }

            for (int num785 = 0; num785 < 4; num785++)
            {
                Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 100, default(Color), 1.5f);
            }

            for (int num788 = 0; num788 < 40; num788++)
            {
                int num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num797].noGravity = true;
                Dust dust24 = Main.dust[num797];
                dust24.velocity *= 3f;
                num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 100, default(Color), 1.5f);
                dust24 = Main.dust[num797];
                dust24.velocity *= 2f;
                Main.dust[num797].noGravity = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}