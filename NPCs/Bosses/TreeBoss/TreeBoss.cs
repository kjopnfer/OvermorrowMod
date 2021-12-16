using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    [AutoloadBossHead]
    public class TreeBoss : ModNPC
    {
        private bool changedPhase2 = false;
        private bool introMessage = true;
        private Vector2 playerPos;

        public enum SpawnDirection { Left, Right }
        public SpawnDirection Direction;

        public enum SpikeAttacks { Wave = 0, Alternating = 1 }
        public int SpikeAttack;
        public int PreviousSpike = -1;

        // Enums don't take floating point values :V
        // Therefore these will be used to calculate in 45 degree increments according to the unit circle
        public enum SpiritPoints
        {
            East = 0,
            NorthEast = 1,
            North = 2,
            NorthWest = 3,
            West = 4,
            SouthWest = 5,
            South = 6,
            SouthEast = 7
        }
        List<SpiritPoints> SpawnDirections = new List<SpiritPoints>(new SpiritPoints[4]);
        public enum SpiritAttacks { Randomized = 0, Circular = 1 }
        public int SpiritAttack;
        public int PreviousSpirit = -1;
        public int RotationDirection;
        public float RotationOffset;

        public int AbsorbedEnergies;
        public int EnergyCount;
        public int ENERGY_THRESHOLD = 14;

        // Keeps track of attacks other than the rune attack
        // This is so we don't keep spamming the healing attack
        public int RuneCounter;
        public int MINIMUM_ATTACKS = 3;
        public int ChosenAttack;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, the Guardian");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            // Reduced size
            npc.width = 203;
            npc.height = 298;

            // Actual dimensions
            //npc.width = 372;
            //npc.height = 300;

            npc.aiStyle = -1;
            //npc.damage = 31;
            npc.damage = 17;
            npc.defense = 14;
            npc.lifeMax = 3300;
            npc.HitSound = SoundID.NPCHit1;
            npc.knockBackResist = 0f;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.boss = true;
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TreeBoss");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 17;
        }

        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                CombatText.NewText(npc.getRect(), new Color(0, 255, 191), text, true);
                Main.NewText(text, new Color(0, 255, 191));
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                CombatText.NewText(npc.getRect(), new Color(0, 255, 191), text, true);
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(0, 255, 191));
            }
        }

        public float GetLerpValue(float from, float to, float t, bool clamped = false)
        {
            if (clamped)
            {
                if (from < to)
                {
                    if (t < from)
                    {
                        return 0f;
                    }
                    if (t > to)
                    {
                        return 1f;
                    }
                }
                else
                {
                    if (t < to)
                    {
                        return 1f;
                    }
                    if (t > from)
                    {
                        return 0f;
                    }
                }
            }
            return (t - from) / (to - from);
        }

        public enum AIStates
        {
            Intro = -1,
            Selector = 0,
            Thorns = 1,
            Spirit = 2,
            Runes = 3,
            Energy = 4
        }

        public ref float AICase => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float MiscCounter => ref npc.ai[2];
        public ref float MiscCounter2 => ref npc.ai[3];


        public override void AI()
        {
            // The cool plans that I write down and forget to remove in the final version of the reworks
            // Iorich has three attack types indicated by his eyes
            // 1. A thorns attack, that can fire in segments, diagonally, or in waves
            // 2. A rune attack, that can fire in bursts or a spread
            // 3. An energy attack, that will follow the player and shoot horizontally, or vertically at their position

            // These coincide with phase 2 attacks that are essentially upgraded versions sans the thorns
            // 1. A physical attack, which would involve various back-and-forth charges
            // 2. A rune attack, which in this case would be the absorption-healing attack, it has two versions:
            // 2a. If it absorbs enough energy, will summon projectiles that rain from the sky
            // 2b. If it doesn't, will fire energy thorns in all directions in quick even-spread bursts
            // 3. An energy attack, which would spawn lights that circle around before firing at their initial position after a full rotation

            if (!OvermorrowWorld.downedTree && introMessage)
            {
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                MiscCounter++;
                npc.localAI[0]++;

                if (MiscCounter == 60)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<IorichRune>(), 0, 0, Main.myPlayer, 1f);
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<IorichRune>(), 0, 0, Main.myPlayer, 0.5f);
                }

                if (MiscCounter == 180)
                {
                    BossText("I heed thy call.");
                }

                if (MiscCounter == 360)
                {
                    BossText("Thou wishes to unlock the secrets of the Dryads?");
                }

                if (MiscCounter == 540)
                {
                    BossText("Very well, I shalt test thy resolve.");
                }

                if (MiscCounter <= 600)
                {
                    return;
                }
                else
                {
                    introMessage = false;
                    npc.dontTakeDamage = false;
                    npc.netUpdate = true;
                }
            }

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
                npc.velocity.Y = 2000;
            }

            if (npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }

            // This is here because the boss heals
            if (npc.life >= npc.lifeMax) npc.life = npc.lifeMax;

            GlobalCounter++;
            npc.localAI[1]++;

            switch (AICase)
            {
                case (int)AIStates.Selector: // General case
                    // TODO: Turn these into NPCs that can be killed and make this Expert exclusive
                    if (MiscCounter % 75 == 0 && MiscCounter > 60 && Main.expertMode)
                    {
                        int numSeeds = npc.life <= npc.lifeMax * 0.25f ? 16 : 13;
                        float numberProjectiles = Main.rand.Next(7, numSeeds);
                        Vector2 position = npc.Center;
                        int speedX = 1;
                        int speedY = Main.rand.Next(-25, -15);
                        float rotation = MathHelper.ToRadians(45);

                        position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f;
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 85, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<FloatingSeeds>(), 17, 1f, Main.myPlayer);
                            }
                        }
                    }

                    // Probably something to choose one of three attack versions
                    int[] Attacks = new int[] { (int)AIStates.Thorns, (int)AIStates.Spirit, (int)AIStates.Runes };

                    if (MiscCounter++ == 0)
                    {
                        // Chooses the attack from the list
                        ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];
                        //ChosenAttack = (int)AIStates.Thorns;

                        // Makes sure the healing attack doesn't have a chance to be chosen unless conditions are met
                        while (ChosenAttack == (int)AIStates.Runes && RuneCounter < MINIMUM_ATTACKS)
                        {
                            ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];
                        }

                        // Increment the non-healing attack counter
                        if (ChosenAttack != (int)AIStates.Runes)
                        {
                            RuneCounter++;
                        }
                    }

                    if (GlobalCounter % 300 == 0)
                    {
                        // If the condition was satisfied and you DID choose the rune attack, now reset the counter
                        if (ChosenAttack == (int)AIStates.Runes)
                        {
                            RuneCounter = 0;
                        }
                        AICase = ChosenAttack;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;
                    }

                    break;
                case (int)AIStates.Thorns: // Spawn Thorns
                    // First find whether the offset position is to the left or the right of the player
                    if (MiscCounter2 == 0)
                    {
                        // Various nondeterministic selections for this attack
                        Direction = Main.rand.NextBool(2) ? SpawnDirection.Left : SpawnDirection.Right;
                        SpikeAttack = (int)(Main.rand.NextBool(2) ? SpikeAttacks.Wave : SpikeAttacks.Alternating);

                        // Check to see if the previous attack was the same
                        while (PreviousSpike == SpikeAttack)
                        {
                            SpikeAttack = (int)(Main.rand.NextBool(2) ? SpikeAttacks.Wave : SpikeAttacks.Alternating);
                        }

                        npc.netUpdate = true; // Multiplayer code stinky

                        // Also get the player's position during this run so it doesn't get constantly offset while they move
                        playerPos = new Vector2(player.position.X / 16, player.position.Y / 16);
                        Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);

                        // Get the ground beneath the player
                        while (!tile.active() || tile.type == TileID.Trees || tile.collisionType != 1)
                        {
                            playerPos.Y += 1;
                            tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                        }
                    }

                    int THORN_OFFSET = 120;
                    int SPAWN_OFFSET = 950;

                    switch (SpikeAttack)
                    {
                        case (int)SpikeAttacks.Wave:
                            if (++MiscCounter % 15 == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 calculatedOffset = Direction == SpawnDirection.Left ? new Vector2(SPAWN_OFFSET + (THORN_OFFSET * -MiscCounter2), 0) : new Vector2(-SPAWN_OFFSET + (THORN_OFFSET * MiscCounter2), 0);
                                    Projectile.NewProjectile(playerPos * 16 + calculatedOffset, new Vector2(0, -10), ModContent.ProjectileType<SpikeStrip>(), 26, 2.5f, Main.myPlayer, 900f, 0f);
                                }

                                MiscCounter2++;
                            }
                            break;
                        case (int)SpikeAttacks.Alternating:
                            int AlternatingSpawnOffset = (SPAWN_OFFSET / 3) * 2; // It was too far away lol

                            if (MiscCounter == 0)
                            {
                                for (int iterations = 0; iterations < 12; iterations++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 calculatedOffset = Direction == SpawnDirection.Left ? new Vector2(AlternatingSpawnOffset + (THORN_OFFSET * -iterations), 0) : new Vector2(-AlternatingSpawnOffset + (THORN_OFFSET * iterations), 0);
                                        Projectile.NewProjectile(playerPos * 16 + calculatedOffset, new Vector2(0, -10), ModContent.ProjectileType<SpikeStrip>(), 26, 2.5f, Main.myPlayer, 900f, 0f);
                                    }
                                }
                            }

                            if (MiscCounter == 140)
                            {
                                AlternatingSpawnOffset -= THORN_OFFSET / 2; // Update the offsets to spawn inbetween the safe areas
                                for (int iterations = 0; iterations < 12; iterations++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 calculatedOffset = Direction == SpawnDirection.Left ? new Vector2(AlternatingSpawnOffset + (THORN_OFFSET * -iterations), 0) : new Vector2(-AlternatingSpawnOffset + (THORN_OFFSET * iterations), 0);
                                        Projectile.NewProjectile(playerPos * 16 + calculatedOffset, new Vector2(0, -10), ModContent.ProjectileType<SpikeStrip>(), 26, 2.5f, Main.myPlayer, 900f, 0f);
                                    }
                                }
                            }

                            MiscCounter++;
                            MiscCounter2++;
                            break;
                    }


                    if (MiscCounter == 300)
                    {
                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        // Store the current attack for next iteration
                        PreviousSpike = SpikeAttack;
                    }

                    break;
                case (int)AIStates.Spirit: // Spirit Attack
                    if (MiscCounter2 == 0)
                    {
                        // Various nondeterministic selections for this attack
                        SpiritAttack = (int)(Main.rand.NextBool(2) ? SpiritAttacks.Circular : SpiritAttacks.Randomized);

                        // Check to see if the previous attack was the same
                        while (PreviousSpirit == SpiritAttack)
                        {
                            SpiritAttack = SpiritAttack = (int)(Main.rand.NextBool(2) ? SpiritAttacks.Circular : SpiritAttacks.Randomized);
                        }

                        npc.netUpdate = true; // Multiplayer code stinky
                    }

                    SpiritPoints[] values = (SpiritPoints[])Enum.GetValues(typeof(SpiritPoints));

                    switch (SpiritAttack)
                    {
                        case (int)SpiritAttacks.Randomized:
                            if (MiscCounter++ == 0)
                            {
                                values = values.Shuffle();

                                // Populate with 4 random values
                                for (int i = 0; i < SpawnDirections.Count; i++)
                                {
                                    // Add a random value to the list from the shuffled enum array
                                    SpawnDirections[i] = (SpiritPoints)values.GetValue(i);
                                }
                            }

                            if (MiscCounter % 15 == 0)
                            {
                                float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                                int RADIUS = 100;

                                int proj = Projectile.NewProjectile(player.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpirit>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpirit)Main.projectile[proj].modProjectile).RotationCenter = player;

                                MiscCounter2++;
                            }

                            break;
                        case (int)SpiritAttacks.Circular:
                            // Determine the rotation direction and offset (where it starts spawning)
                            if (MiscCounter++ == 0)
                            {
                                RotationDirection = Main.rand.NextBool(2) ? 1 : -1;
                                RotationOffset = Main.rand.Next(4) * MathHelper.PiOver2;

                                npc.netUpdate = true;
                            }

                            if (MiscCounter % 7 == 0)
                            {
                                int RADIUS = 135;

                                float Rotation = RotationDirection * (int)values[(int)MiscCounter2] * MathHelper.PiOver4;
                                Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                                int proj = Projectile.NewProjectile(player.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<GreenSpirit>(), npc.damage, 0f, Main.myPlayer, Rotation + RotationOffset, RADIUS);
                                ((GreenSpirit)Main.projectile[proj].modProjectile).RotationCenter = player;

                                MiscCounter2++;
                            }
                            break;
                    }

                    if (MiscCounter == 60)
                    {
                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        // Store the current attack for next iteration
                        PreviousSpirit = SpiritAttack;
                    }
                    break;
                case (int)AIStates.Runes: // Multiple thorns
                    // The initializer for the spinny visuals
                    if (MiscCounter == 0)
                    {
                        int RADIUS = 300;

                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.PiOver2 * i);

                            int proj = Projectile.NewProjectile(npc.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<RuneSpinner>(), npc.damage, 0f, Main.myPlayer, MathHelper.PiOver2 * i, RADIUS);
                            ((RuneSpinner)Main.projectile[proj].modProjectile).RotationCenter = npc;
                        }
                    }

                    if (MiscCounter != 600)
                    {
                        npc.chaseable = false;
                        npc.dontTakeDamage = true;

                        // Spawning timeframe for the energies
                        if (MiscCounter > 60 && MiscCounter < 480)
                        {
                            // Spawns killable NPCs that heal the boss after 60 seconds
                            if (MiscCounter % 60 == 0)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    //float randPositionX = npc.Center.X + Main.rand.Next(-10, 10) * 800;
                                    //float randPositionY = npc.Center.Y + Main.rand.Next(-10, 10) * 800;
                                    float RandomRotation = Main.rand.NextFloat(0, MathHelper.Pi);

                                    // Generates in a half-arc 200 pixels below the NPC's center
                                    Vector2 RandomPosition = npc.Center + new Vector2(0, 200) + new Vector2(1200, 0).RotatedBy(-RandomRotation);
                                    npc.netUpdate = true;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        NPC.NewNPC((int)RandomPosition.X, (int)RandomPosition.Y, ModContent.NPCType<AbsorbEnergy>(), 0, npc.whoAmI);
                                    }
                                }
                            }
                        }

                        if (MiscCounter > 540)
                        {
                            MiscCounter2++;
                        }
                    }

                    EnergyCount = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AbsorbEnergy>())
                        {
                            EnergyCount++;
                        }
                    }

                    npc.localAI[0] += 0.04f; // Rotation code for the rune

                    if (Utils.Clamp(++MiscCounter, 0, 600) == 600 && EnergyCount == 0)
                    {
                        Main.NewText("abosrbed energies: " + AbsorbedEnergies);
                        AICase = (int)AIStates.Energy;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        // Reset properties
                        npc.chaseable = true;
                        npc.dontTakeDamage = false;
                    }
                    break;
                case (int)AIStates.Energy:
                    if (MiscCounter == 0)
                    {
                        if (AbsorbedEnergies > ENERGY_THRESHOLD)
                        {
                            BossText("Vis Inberux");
                        }
                        else
                        {
                            BossText("Mis Inberux");
                        }
                    }

                    if (AbsorbedEnergies > ENERGY_THRESHOLD)
                    {
                        if (MiscCounter % 30 == 0)
                        {
                            for (int i = 0; i < Main.rand.Next(5, 9); i++)
                            {
                                // Choose a position above the player with random x-axis offsets
                                Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-18, 18) * 80, Main.rand.Next(-1600, -1200));
                                npc.netUpdate = true;

                                Projectile.NewProjectile(RandomPosition, new Vector2(0, Main.rand.Next(3, 6) * 2), ModContent.ProjectileType<PrismaMeteor>(), npc.damage * 2, 5f, Main.myPlayer, Main.rand.NextFloat(0.04f, 0.085f));
                            }
                        }
                    }
                    else
                    {
                        if (MiscCounter % 30 == 0)
                        {
                            for (int i = 0; i < Main.rand.Next(5, 9); i++)
                            {
                                // Choose a position above the player with random x-axis offsets
                                Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-18, 18) * 80, Main.rand.Next(-1600, -1200));
                                npc.netUpdate = true;

                                Projectile.NewProjectile(RandomPosition, new Vector2(0, Main.rand.Next(3, 6) * 2), ModContent.ProjectileType<LesserPrismaMeteor>(), npc.damage, 5f, Main.myPlayer, Main.rand.NextFloat(0.04f, 0.085f));
                            }
                        }
                    }

                    if (++MiscCounter == 600)
                    {
                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        // Reset properties
                        AbsorbedEnergies = 0;
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D value71 = mod.GetTexture("NPCs/Bosses/TreeBoss/TreeBoss");
            Vector2 vector59 = npc.Center - Main.screenPosition;
            Rectangle frame8 = npc.frame;
            Vector2 origin21 = new Vector2(70f, 127f);
            origin21.Y += 8f;
            Vector2 scale3 = new Vector2(npc.scale);
            float num219 = npc.localAI[0];
            if (num219 < 120f)
            {
                scale3 *= num219 / 240f + 0.5f;
            }
            Color alpha13 = npc.GetAlpha(npc.color);
            float lerpValue2 = GetLerpValue(0f, 120f, num219, clamped: true);
            float num220 = MathHelper.Lerp(32f, 0f, lerpValue2);
            Color color45 = alpha13;
            color45.A = (byte)MathHelper.Lerp((int)color45.A, 0f, lerpValue2);
            color45 *= lerpValue2;
            if (num219 >= 120f)
            {
                color45 = alpha13;
            }
            spriteBatch.Draw(value71, vector59, frame8, color45, npc.rotation, origin21, scale3, SpriteEffects.None, 0f);

            // AI Counter
            float y2 = (((MiscCounter + 54f) % 180f - 120f) / 180f * 2f * ((float)Math.PI * 2f)).ToRotationVector2().Y;
            if (num219 >= 120f)
            {
                num220 = y2 * 0f;
                color45.A = (byte)((float)(int)color45.A * 0.5f);
                color45 *= y2 / 2f + 0.5f;
                float num221 = 1f;
                for (float num222 = 0f; num222 < num221; num222 += 1f)
                {
                    spriteBatch.Draw(value71, vector59 + ((float)Math.PI * 2f / num221 * num222).ToRotationVector2() * num220, frame8, color45, npc.rotation, origin21, scale3, SpriteEffects.None, 0f);
                }
            }

            // AI counter
            float num223 = MiscCounter / 180f - 0.76f;
            if (num223 < 0f)
            {
                num223 += 1f;
            }
            float num224 = 0f;
            float num225 = 0f;
            float num226 = 0.6f;
            float num227 = 0.8f;
            if (num223 >= num226 && num223 <= num227)
            {
                num224 = GetLerpValue(num226, num227, num223);
                num225 = MathHelper.Lerp(0.75f, 0.85f, num224);
            }
            num226 = num227;
            num227 = num226 + 0.13f;
            if (num223 >= num226 && num223 <= num227)
            {
                num224 = 1f - GetLerpValue(num226, num227, num223);
                num225 = MathHelper.Lerp(1.3f, 0.85f, num224);
            }
            int frameNumber = frame8.Y / frame8.Height;

            if (num219 < 120f)
            {
                float num229 = (float)Math.PI * 2f * lerpValue2 * (float)Math.Pow(lerpValue2, 2.0) * 2f + lerpValue2;
                color45.A = (byte)((float)(int)alpha13.A * (float)Math.Pow(lerpValue2, 2.0) * 0.5f);
                float num230 = 3f;
                for (float num231 = 0f; num231 < num230; num231 += 1f)
                {
                    spriteBatch.Draw(value71, vector59 + (num229 + (float)Math.PI * 2f / num230 * num231).ToRotationVector2() * num220, frame8, color45, npc.rotation, origin21, scale3, SpriteEffects.None, 0f);
                }
            }

            if (AbsorbedEnergies > ENERGY_THRESHOLD)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeBoss_Pulse");


                // this gets the npc's frame
                int num178 = 60; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num177 = 6; // ok i think this controls the number of afterimage frames
                float num176 = 1f - (float)Math.Cos((GlobalCounter - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num176 /= 3f;
                float scaleFactor10 = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num177; num164++)
                {
                    // these assign the color of the pulsing
                    Color spriteColor = Color.LightGreen;
                    spriteColor = npc.GetAlpha(spriteColor);
                    spriteColor *= 1f - num176; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector45 = npc.Center + Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + npc.rotation) * scaleFactor10 * num176 - Main.screenPosition;
                    vector45 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector45 += drawOrigin * npc.scale + new Vector2(0f, npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture, vector45, npc.frame, spriteColor, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            if (AICase == (int)AIStates.Runes)
            {
                Texture2D RuneTexture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeRune");
                Texture2D RuneTextureGlow = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeRune_Glow");

                // Draw the glow texture when it is night-time because it is gross as fuck during the day
                if (!Main.dayTime)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    Main.spriteBatch.Draw(RuneTextureGlow, npc.Center - Main.screenPosition, null, Color.Lerp(new Color(0, 255, 191) * 0.75f, Color.Transparent, Utils.Clamp(MiscCounter2, 0, 60) / 60f), npc.localAI[0], RuneTextureGlow.Size() / 2, MathHelper.Lerp(0, 1.25f, Utils.Clamp(GlobalCounter, 0, 60) / 60f), SpriteEffects.None, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }

                Main.spriteBatch.Draw(RuneTexture, npc.Center - Main.screenPosition, null, Color.Lerp(new Color(0, 255, 191), Color.Transparent, Utils.Clamp(MiscCounter2, 0, 60) / 60f), npc.localAI[0], RuneTexture.Size() / 2, MathHelper.Lerp(0, 1.25f, Utils.Clamp(GlobalCounter, 0, 60) / 60f), SpriteEffects.None, 0f);
            }

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Texture2D texture2 = ModContent.GetTexture("OvermorrowMod/Textures/test2");
            Rectangle rect2 = new Rectangle(0, 0, texture2.Width, texture2.Height);
            Vector2 drawOrigin2 = new Vector2(texture2.Width / 2, texture2.Height / 2);

            int xOffset;
            int yOffset;

            int frameOffset = 0;
            // The tree bounces up and down so we need to increase the height when he goes up
            if (npc.frame.Y == 300 || npc.frame.Y == 600)
            {
                frameOffset = -2;
            }

            switch (ChosenAttack)
            {
                case (int)AIStates.Thorns: // Left
                    xOffset = 1;
                    yOffset = -65 + frameOffset;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    // Side and top flares
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, MathHelper.PiOver2, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(4, 5, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, 0, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(0.75f, 1f, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);

                    // The center circle
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, npc.rotation, drawOrigin2, 0.3f, SpriteEffects.None, 0);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    break;
                case (int)AIStates.Spirit: // Right
                    xOffset = 11;
                    yOffset = -65 + frameOffset;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    // Side and top flares
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, MathHelper.PiOver2, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(4, 5, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, 0, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(0.75f, 1f, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);

                    // The center circle
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, npc.rotation, drawOrigin2, 0.3f, SpriteEffects.None, 0);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    break;
                case (int)AIStates.Runes: // Middle
                    xOffset = 5;
                    yOffset = -69 + frameOffset;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    // Side and top flares
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, MathHelper.PiOver2, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(4, 5, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, 0, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(0.75f, 1f, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);

                    // The center circle
                    spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), Main.DiscoColor, npc.rotation, drawOrigin2, 0.3f, SpriteEffects.None, 0);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    break;
            }

            base.PostDraw(spriteBatch, drawColor);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 4) // 4 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool CheckDead()
        {
            npc.boss = false;
            return base.CheckDead();
        }

        public override void NPCLoot()
        {
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<TreeBossP2>(), 0, 0f, 0f, 0f, 0f);

            // Spawn 2nd Phase
            if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
            {
                Main.NewText("Iorich has uprooted!", Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server) // Server
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Iorich has uprooted!"), Color.Green);
            }
        }
    }
}