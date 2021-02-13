using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class StormDrake : ModNPC
    {
        private bool createAfterimage = false;
        //private string chooseDirection; // Make this an enum later
        enum hoverDirection { left, right };
        private hoverDirection chooseDirection;
        private bool secondBalls = false;
        private bool hasCharged = false;
        private bool textSent = false;
        private int chargeCount = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Storm Drake");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 0;

            npc.width = 296;
            npc.height = 232;
            //npc.width = 416;
            //npc.height = 522;
            npc.aiStyle = -1;
            npc.damage = 45;
            npc.defense = 14;
            npc.lifeMax = 7300;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 5);
            npc.npcSlots = 5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3f);
            npc.defense = 22;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];

            // Handles Despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
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

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = -2000;
            }

            // PHASE 1 MOVESET:
            // Hover over on side of player for 300 seconds (case 0)
            // Position to side for 200 seconds (case 3)
            // Spawn balls (case 1)
            // Chase after player (case 5)
            // Position on top of player (case 4)

            // PHASE 2:
            // Maybe cause rain effect?
            if(npc.life <= npc.lifeMax * 0.5)
            {
                if (!textSent) // Print phase 2 notifier
                {
                    if (Main.netMode == 0) // Singleplayer
                    {
                        Main.NewText("The air crackles with electricity...", Color.Teal);
                    }
                    else if (Main.netMode == 2) // Multiplayer
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The air crackles with electricity..."), Color.Teal);
                    }
                    textSent = true;
                }

                // Lightning now rains down periodically
                // AI[0] = -4.75, AI[1] = Main.rand.Next(-10, 10)
                if (npc.ai[1] % 120 == 0)
                {

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int randCeiling = Main.expertMode ? 6 : 4;
                        for (int i = 0; i < Main.rand.Next(2, randCeiling); i++)
                        {
                            int randomX = Main.rand.Next(-500, 500);
                            npc.netUpdate = true;
                            Projectile.NewProjectile(player.Center - new Vector2(randomX, 500), new Vector2(0, 20), ModContent.ProjectileType<ChoreographLaser>(), 0, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(player.Center - new Vector2(randomX, 500), new Vector2(0, 4), ProjectileID.CultistBossLightningOrbArc, npc.damage / 4, 0f, Main.myPlayer, (float)-4.75, Main.rand.Next(-10, 10));
                        }
                    }
                }
            }

            switch (npc.ai[0])
            {
                case 0: // NEUTRAL POSITION: Hover above the player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (npc.ai[1] == 240)
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;

                            // Choose new direction
                            int randChoice = Main.rand.Next(2);
                            npc.netUpdate = true;
                            if (randChoice == 0)
                            {
                                chooseDirection = hoverDirection.left;
                            }
                            else
                            {
                                chooseDirection = hoverDirection.right;
                            }
                        }

                        Vector2 moveTo = player.Center + new Vector2(0, -300);
                        var move = moveTo - npc.Center;
                        var speed = 10;
                        
                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 30;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity = move;
                    }
                    break;
                case 1: // Begin aligning self to one side of the player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

                        if (npc.ai[1] == 240)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }

                        if (chooseDirection == hoverDirection.left)
                        {
                            Vector2 moveTo = player.Center - new Vector2(450, 0);
                            var move = moveTo - npc.Center;
                            //var move = target - npc.Center;
                            var speed = 8;
                            float length = move.Length();
                            if (length > speed)
                            {
                                move *= speed / length;
                            }
                            var turnResistance = 30;
                            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                            length = move.Length();
                            if (length > 10)
                            {
                                move *= speed / length;
                            }
                            npc.velocity = move;
                        }
                        else
                        {
                            Vector2 moveTo = player.Center + new Vector2(450, 0);
                            var move = moveTo - npc.Center;
                            //var move = target - npc.Center;
                            var speed = 8;
                            float length = move.Length();
                            if (length > speed)
                            {
                                move *= speed / length;
                            }
                            var turnResistance = 30;
                            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                            length = move.Length();
                            if (length > 10)
                            {
                                move *= speed / length;
                            }
                            npc.velocity = move;
                        }

                        if (npc.ai[1] == 190)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;
                case 2: // Position self to one side of the player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

                        if (npc.ai[1] == 240)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }

                        if (chooseDirection == hoverDirection.left)
                        {
                            Vector2 moveTo = player.Center - new Vector2(450, 0);
                            var move = moveTo - npc.Center;
                            //var move = target - npc.Center;
                            var speed = 7;
                            float length = move.Length();
                            if (length > speed)
                            {
                                move *= speed / length;
                            }
                            var turnResistance = 30;
                            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                            length = move.Length();
                            if (length > 10)
                            {
                                move *= speed / length;
                            }
                            npc.velocity = move;
                        }
                        else
                        {
                            Vector2 moveTo = player.Center + new Vector2(450, 0);
                            var move = moveTo - npc.Center;
                            var speed = 7;
                            float length = move.Length();
                            if (length > speed)
                            {
                                move *= speed / length;
                            }
                            var turnResistance = 30;
                            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                            length = move.Length();
                            if (length > 10)
                            {
                                move *= speed / length;
                            }
                            npc.velocity = move;
                        }

                        if (npc.ai[1] == 190)
                        {
                            if (chargeCount == 3) // The boss has already charged thrice and is now doing ball attack
                            {
                                npc.ai[0] = 4;
                                npc.ai[1] = 0;
                                chargeCount = 0; // Set to 0 so it doesn't repeat again
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                                chargeCount++;
                                npc.netUpdate = true;
                            }
                        }
                    }
                    break;
                case 3: // Charge at player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if(npc.ai[2] < 40) // Pause before charging, also get position to charge towards
                        {
                            // Process takes ~3/4ths a second
                            npc.velocity = new Vector2(0, 0);
                        }
                        else // Actual charge sequence
                        {
                            if (!hasCharged)
                            {
                                createAfterimage = true;
                                if (chooseDirection == hoverDirection.left) // Charge to right
                                {
                                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                                    float chargeSpeed = npc.life <= npc.lifeMax * 0.5 ? 20 : 15;
                                    Vector2 moveTo = npc.Center + new Vector2(450, 0);
                                    Vector2 move = moveTo - npc.Center;
                                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                                    move *= chargeSpeed / magnitude;
                                    npc.velocity = move;
                                }
                                else // Charge to left
                                {
                                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                                    float chargeSpeed = npc.life <= npc.lifeMax * 0.5 ? 20: 15;
                                    Vector2 moveTo = npc.Center - new Vector2(450, 0);
                                    Vector2 move = moveTo - npc.Center;
                                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                                    move *= chargeSpeed / magnitude;
                                    npc.velocity = move;
                                }
                                hasCharged = true;
                            }
                        }
                        npc.ai[2]++;

                        if (npc.ai[1] == 110) // Go back to neutral
                        {
                            // Ensures that the NPC is not rotated
                            npc.rotation = 0.0f;

                            // Halt velocity so the NPC doesn't go insane
                            npc.velocity = new Vector2(0, 0);

                            npc.ai[0] = 0;
                            npc.ai[1] = 0;

                            // Reset variables
                            hasCharged = false;
                            createAfterimage = false;

                            npc.netUpdate = true;
                        }
                    }
                    break;
                case 4: // Spawn electric balls
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        createAfterimage = false;

                        npc.velocity = new Vector2(0, 0);
                        if (npc.ai[1] % 301 == 1)
                        {
                            Main.PlaySound(SoundID.Item94, (int)npc.position.X, (int)npc.position.Y);
                            int damage = Main.expertMode ? 12 : 20;
                            Vector2 target = Main.player[npc.target].Center - npc.Center;
                            target.Normalize();

                            int direction = npc.direction == 1 ? 175 : -175; // Facing right, otherwise

                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 0, 0, ModContent.ProjectileType<ElectricBallCenter>(), 0, 1, Main.myPlayer, 0, damage);
                        }

                        if (npc.ai[1] == 20)
                        {
                            npc.ai[0] = 5;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;
                case 5: // Chase after the player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
                        var move = target - npc.Center;
                        var speed = 4;
                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 30;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity = move;

                        if (npc.ai[1] == 160) 
                        {
                            
                            if (!secondBalls && npc.life <= npc.lifeMax * 0.5f)
                            {
                                secondBalls = true;
                                npc.ai[0] = 4;
                                npc.ai[1] = 0;
                            }
                            else
                            {
                                secondBalls = false;
                                // Back to neutral
                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                            }
                        }
                    }
                    break;
            }

            if (npc.ai[0] == 0 || npc.ai[0] == 1 || npc.ai[0] == 2)
            {
                npc.FaceTarget();
                npc.spriteDirection = npc.direction;
            }
            npc.ai[1]++;

            // PHASE CHANGE: Duke Fishron pulse effect

            // Phase 2
            // Calls columns of lightning down from the sky
            // Dash from one side to the other, summons clone projectiles above and bottom to dash after a delay

            // EXPERT MODE: Phase 3
            // EXPERT VERSION: Summon a lightning clone with similar movements
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
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
            if (createAfterimage)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                //Vector2 drawOrigin = new Vector2(416, 522);
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(60f, 290);
                    Color color = npc.GetAlpha(drawColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
            
            return true;
        }

        public override void NPCLoot()
        {
            int choice = Main.rand.Next(2);

            // Always drops one of:
            if (choice == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LightningPiercer"));
            }
            else if (choice == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LightningPiercer"));
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}