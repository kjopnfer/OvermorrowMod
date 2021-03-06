using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    [AutoloadBossHead]
    public class StormDrake : ModNPC
    {
        private bool createAfterimage = false;
        enum hoverDirection { left, right };
        private hoverDirection chooseDirection;
        private bool secondBalls = false;
        private bool hasCharged = false;
        private bool textSent = false;
        private bool changedPhase = false;
        private bool phaseAnimation = false;
        private bool canPulse = false;
        private bool playChargeSound = false; // Because multiplayer and sounds are annoying
        private int chargeCount = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Storm Drake");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[npc.type] = 14;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            npc.width = 296;
            npc.height = 232;

            // Actual sprite dimensions:
            //npc.width = 416;
            //npc.height = 522;

            npc.aiStyle = -1;
            npc.damage = 45;
            npc.defense = 14;
            npc.lifeMax = 7600;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/StormDrake");
            bossBag = ModContent.ItemType<DrakeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 19;
        }

        public override void AI()
        {
            //npc.netUpdate = true;
            Player player = Main.player[npc.target];

            // Force player to be in the sky
            if (!player.ZoneSkyHeight)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }

            // Handles Despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20)
                {
                    if (Main.raining)
                    {
                        Main.raining = false;
                        Main.rainTime = 0;
                    }

                    npc.timeLeft = 20;
                    return;
                }
            }

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = -2000;
            }

            // General MOVESET:
            // Position on top of player
            // Hover over on side of player
            // Position to side
            // Charge thrice
            // Position on top of player
            // Hover over on side of player
            // Position to side
            // Spawn balls
            // Chase after player

            // Handles PHASE 2:
            if (npc.life <= npc.lifeMax * 0.5 && changedPhase)
            {
                // Causes rain effect
                if (!Main.raining)
                {
                    Main.raining = true;
                    Main.rainTime = 180;
                }
                else
                {
                    Main.rainTime += 120;
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
                            //Projectile.NewProjectile(player.Center - new Vector2(randomX, 500), new Vector2(0, 4), ModContent.ProjectileType<Lightning>(), npc.damage / 4, 0f, Main.myPlayer, (float)-4.75, Main.rand.Next(-10, 10));
                            Projectile.NewProjectile(player.Center - new Vector2(randomX, 500), new Vector2(0, 4), ProjectileID.CultistBossLightningOrbArc, npc.damage / 4, 0f, Main.myPlayer, (float)-4.75, Main.rand.Next(-10, 10));
                        }
                    }
                }
            }

            switch (npc.ai[0])
            {
                case 0: // NEUTRAL POSITION: Hover above the player
                    if (npc.ai[0] == 0)
                    {
                        if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                        {
                            // If the NPC reaches below half health, break out of switch statement and become stationary
                            npc.ai[0] = 6;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            break;
                        }

                        if (npc.ai[1] == 240)
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;

                            // Choose new direction
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int randChoice = Main.rand.Next(2);
                                npc.netUpdate = true;
                                if (randChoice == 0)
                                {
                                    chooseDirection = hoverDirection.left;
                                    npc.netUpdate = true;
                                }
                                else
                                {
                                    chooseDirection = hoverDirection.right;
                                    npc.netUpdate = true;
                                }
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
                    //if (Main.netMode != NetmodeID.MultiplayerClient)
                    if (npc.ai[0] == 1)
                    {
                        if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                        {
                            // If the NPC reaches below half health, break out of switch statement and become stationary
                            npc.ai[0] = 6;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            break;
                        }

                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

                        if (chooseDirection == hoverDirection.left)
                        {
                            Vector2 moveTo = player.Center - new Vector2(450, 0);
                            var move = moveTo - npc.Center;
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

                        if (chooseDirection == hoverDirection.right)
                        {
                            Vector2 moveTo = player.Center + new Vector2(450, 0);
                            var move = moveTo - npc.Center;
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
                            npc.netUpdate = true;
                        }

                        if (npc.ai[1] == 190)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 2: // Position self to one side of the player
                    if (npc.ai[0] == 2)
                    {
                        if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                        {
                            // If the NPC reaches below half health, break out of switch statement and become stationary
                            npc.ai[0] = 6;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            break;
                        }
                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

                        int distanceBuffer = npc.life <= npc.lifeMax * 0.5 ? 125 : 95;
                        if (chooseDirection == hoverDirection.left)
                        {
                            Vector2 moveTo = player.Center - new Vector2(450 + distanceBuffer, 0);
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

                        if (chooseDirection == hoverDirection.right)
                        {
                            Vector2 moveTo = player.Center + new Vector2(450 + distanceBuffer, 0);
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
                            npc.netUpdate = true;
                        }

                        // Shoot projectiles at player
                        if (npc.ai[1] % 40 == 0)
                        {
                            Main.PlaySound(SoundID.Item109, (int)npc.position.X, (int)npc.position.Y);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = npc.life <= npc.lifeMax * 0.70 ? 15 : 10;
                                if (npc.direction == 1) // Facing right
                                {
                                    Vector2 headPosition = new Vector2(npc.position.X - 165, npc.position.Y + 74);
                                    float speedX = player.Center.X - headPosition.X;
                                    float speedY = player.Center.Y - headPosition.Y;
                                    float length = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                                    float num12 = speed / length;
                                    speedX = speedX * num12;
                                    speedY = speedY * num12;

                                    Vector2 position = npc.position - new Vector2(-165 * 2, (-74 / 2) - 20);
                                    Vector2 targetPosition = Main.player[npc.target].Center;
                                    Vector2 direction = targetPosition - position;
                                    direction.Normalize();

                                    Projectile.NewProjectile(npc.position - new Vector2(-165 * 2, (-74 / 2) - 20), direction * speed, ModContent.ProjectileType<LightningBreath>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                                }
                                else // Facing left
                                {
                                    Vector2 headPosition = new Vector2(npc.position.X - 165, npc.position.Y - 74);
                                    headPosition += new Vector2((npc.width / 2), (npc.height / 2));

                                    float speedX = player.Center.X - headPosition.X;
                                    float speedY = player.Center.Y - headPosition.Y;
                                    float length = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                                    float num12 = speed / length;
                                    speedX = speedX * num12;
                                    speedY = speedY * num12;

                                    Vector2 position = npc.position - new Vector2((-165 / 4) + 90, (-74 / 2) - 18);
                                    Vector2 targetPosition = Main.player[npc.target].Center;
                                    Vector2 direction = targetPosition - position;
                                    direction.Normalize();

                                    Projectile.NewProjectile(npc.position - new Vector2((-165 / 4) + 90, (-74 / 2) - 18), direction * speed, ModContent.ProjectileType<LightningBreath>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                                }
                            }
                        }

                        if (npc.ai[1] == 190)
                        {
                            if (chargeCount == 3) // The boss has already charged thrice and is now doing ball attack
                            {
                                npc.ai[0] = 4;
                                npc.ai[1] = 0;
                                chargeCount = 0; // Set to 0 so it doesn't repeat again
                            }
                            else
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                                chargeCount++;
                            }
                        }
                    }
                    break;
                case 3: // Charge at player
                    createAfterimage = true;

                    if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                    {
                        // If the NPC reaches below half health, break out of switch statement and become stationary
                        npc.ai[0] = 6;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        break;
                    }

                    int delayTime = npc.life <= npc.lifeMax * 0.5 ? 13 : 20;
                    if (npc.ai[2] < delayTime) // Pause before charging, also get position to charge towards
                    {
                        // Process takes ~3/4ths a second
                        npc.velocity = new Vector2(0, 0);
                    }
                    else // Actual charge sequence
                    {
                        Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {

                            if (!hasCharged)
                            {
                                createAfterimage = true;
                                if (chooseDirection == hoverDirection.left) // Charge to right
                                {
                                    float chargeSpeed = npc.life <= npc.lifeMax * 0.5 ? 21 : 16;
                                    Vector2 moveTo = npc.Center + new Vector2(450, 0);
                                    Vector2 move = moveTo - npc.Center;
                                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                                    move *= chargeSpeed / magnitude;
                                    npc.velocity = move;
                                    playChargeSound = true;
                                    npc.netUpdate = true; // Update the server with the NPC's velocity
                                }
                                else // Charge to left
                                {
                                    float chargeSpeed = npc.life <= npc.lifeMax * 0.5 ? 21 : 16;
                                    Vector2 moveTo = npc.Center - new Vector2(450, 0);
                                    Vector2 move = moveTo - npc.Center;
                                    float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                                    move *= chargeSpeed / magnitude;
                                    npc.velocity = move;
                                    playChargeSound = true;
                                    npc.netUpdate = true; // Update the server with the NPC's velocity
                                }

                                playChargeSound = true;
                                hasCharged = true;
                            }

                            if (hasCharged && npc.life <= npc.lifeMax * 0.5)
                            {
                                int numSparks = npc.life <= npc.lifeMax * 0.25 ? 3 : 6;
                                if (npc.ai[1] % numSparks == 0)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            int damage = Main.expertMode ? npc.damage : npc.damage / 2;
                                            if (chooseDirection == hoverDirection.left)
                                            {
                                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, -4, Main.rand.Next(-3, -1), ModContent.ProjectileType<ElectricSparks>(), damage, 1, Main.myPlayer, 0, 0);
                                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, -4, Main.rand.Next(1, 3), ModContent.ProjectileType<ElectricSparks>(), damage, 1, Main.myPlayer, 0, 0);
                                            }
                                            else
                                            {
                                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 4, Main.rand.Next(-3, -1), ModContent.ProjectileType<ElectricSparks>(), damage, 1, Main.myPlayer, 0, 0);
                                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 4, Main.rand.Next(1, 3), ModContent.ProjectileType<ElectricSparks>(), damage, 1, Main.myPlayer, 0, 0);
                                            }
                                        }
                                    }
                                }
                            }
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
                        npc.ai[2] = 0;

                        // Reset variables
                        hasCharged = false;
                        createAfterimage = false;
                    }
                    break;
                case 4: // Spawn electric balls
                    if (npc.ai[0] == 4)
                    {
                        if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                        {
                            // If the NPC reaches below half health, break out of switch statement and become stationary
                            npc.ai[0] = 6;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            break;
                        }

                        createAfterimage = false;

                        npc.velocity = new Vector2(0, 0);
                        if (npc.ai[1] == 1)
                        {
                            Main.PlaySound(SoundID.Item94, (int)npc.position.X, (int)npc.position.Y);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = Main.expertMode ? 12 : 20;
                                Vector2 target = Main.player[npc.target].Center - npc.Center;
                                target.Normalize();

                                int direction = npc.direction == 1 ? 175 : -175; // Facing right, otherwise

                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 0, 0, ModContent.ProjectileType<ElectricBallCenter>(), damage, 1, Main.myPlayer, 0, npc.whoAmI);
                            }
                        }

                        if (npc.ai[1] == 20)
                        {
                            createAfterimage = false;
                            npc.ai[0] = 5;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 5: // Chase after the player
                    if (npc.ai[0] == 5)
                    {
                        if (npc.life <= npc.lifeMax * 0.5 && !changedPhase) // Break out condition
                        {
                            // If the NPC reaches below half health, break out of switch statement and become stationary
                            npc.ai[0] = 6;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            break;
                        }

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
                            
                            if (!secondBalls && npc.life <= npc.lifeMax * 0.35f)
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
                case 6: // Phase change
                    npc.velocity = new Vector2(0, 0);
                    npc.dontTakeDamage = true;

                    if (npc.ai[1] == 180)
                    {
                        npc.dontTakeDamage = false;

                        // Go back to neutral
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        createAfterimage = false;

                        changedPhase = true;
                    }

                    if (npc.ai[1] == 70)
                    {
                        Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                        phaseAnimation = true;
                    }

                    if(npc.ai[1] == 120)
                    {
                        phaseAnimation = false;
                        canPulse = true;
                    }

                    // Causes rain effect
                    if (!Main.raining)
                    {
                        Main.raining = true;
                        Main.rainTime = 180;
                    }
                    else
                    {
                        Main.rainTime += 120;
                    }

                    if (!textSent) // Print phase 2 notifier
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
                        {
                            Main.NewText("The air crackles with electricity...", Color.Teal);
                        }
                        else if (Main.netMode == NetmodeID.Server) // Server
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The air crackles with electricity..."), Color.Teal);
                        }
                        textSent = true;
                    }
                    break;
            }

            if (npc.ai[0] == 0 || npc.ai[0] == 1 || npc.ai[0] == 2 || npc.ai[0] == 5) 
            {
                npc.FaceTarget();
                npc.spriteDirection = npc.direction;
            }

            // Eye dust
            if (!hasCharged && canPulse)
            {
                if (npc.direction == 1)
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = npc.position - new Vector2(-165, 74);
                        vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustPerfect(vector304 + new Vector2((npc.width / 2), (npc.height / 2)), 206, null, 0, default, 1.5f);
                    }
                }
                else
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = npc.position + new Vector2(-165, -74);
                        vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustPerfect(vector304 + new Vector2((npc.width / 2), (npc.height / 2)), 206, null, 0, default, 1.5f);
                    }
                }
            }

            npc.ai[1]++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if(npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 206, 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * npc.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * npc.scale;
                    }
                }

                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeHead"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeWing"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeLeg"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeTail"), npc.scale);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            // Phase 2 Debuff
            if (npc.life <= npc.lifeMax * 0.5)
            {
                target.AddBuff(BuffID.Electrified, Main.expertMode ? 240 : 120);
            }
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
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            Vector2 drawPos2 = Main.screenPosition + drawOrigin - new Vector2(60f, 290);

            if (createAfterimage)
            {
                //Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                //Vector2 drawOrigin = new Vector2(416, 522);
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(60f, 290);
                    Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.LightCyan : drawColor;
                    Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            Texture2D texture2D16 = mod.GetTexture("NPCs/Bosses/StormDrake/StormDrake");

            // this controls the passive pulsing effect
            if (canPulse)
            {
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
                float scaleFactor10 = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

                Color color47 = Color.Lerp(Color.White, Color.Blue, 0.5f);
                color55 = Color.Cyan;
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
                    spriteBatch.Draw(texture2D16, vector45 - new Vector2(0, 290 / 2), ((ModNPC)this).npc.frame, color45, ((ModNPC)this).npc.rotation, vector47, ((ModNPC)this).npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            // this controls the phase change animation
            if (phaseAnimation)
            {
                // this gets the npc's frame
                Vector2 vector472 = drawOrigin;
                Color color552 = Color.White; // This is just white lol
                float amount102 = 0f; // I think this controls amount of color
                int num1782 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num1792 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num1772 = 6; // ok i think this controls the number of afterimage frames
                float num1762 = 1f - (float)Math.Cos((npc.ai[1] - (float)num1782) / (float)num1792 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num1762 /= 3f;
                float scaleFactor102 = 40f; // Change scale factor of the pulsing effect and how far it draws outwards

                Color color472 = Color.Lerp(Color.White, Color.Blue, 0.5f);
                color552 = Color.Cyan;
                amount102 = 1f;

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num1772; num164++)
                {
                    // these assign the color of the pulsing
                    Color color452 = color472;
                    color452 = Color.Lerp(color452, color552, amount102);
                    color452 = ((ModNPC)this).npc.GetAlpha(color452);
                    color452 *= 1f - num1762; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector452 = ((Entity)((ModNPC)this).npc).Center + Utils.ToRotationVector2((float)num164 / (float)num1772 * ((float)Math.PI * 2f) + ((ModNPC)this).npc.rotation) * scaleFactor102 * num1762 - Main.screenPosition;
                    vector452 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).npc.type]) * ((ModNPC)this).npc.scale / 2f;
                    vector452 += vector472 * ((ModNPC)this).npc.scale + new Vector2(0f, 4f + ((ModNPC)this).npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture2D16, vector452 - new Vector2(0, 290 / 2), ((ModNPC)this).npc.frame, color452, ((ModNPC)this).npc.rotation, vector472, ((ModNPC)this).npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/StormDrake/StormDrake_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y- 141), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if (Main.raining)
            {
                Main.raining = false;
                Main.rainTime = 0;
            }

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(5);
                // Always drops one of:
                if (choice == 0) // Warden
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<LightningPiercer>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BoltStream>());
                }
                else if (choice == 2) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<StormTalon>());
                }
                else if (choice == 3) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TempestGreatbow>());
                }
                else if (choice == 4) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeStaff>());
                }

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeTrophy>());
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}