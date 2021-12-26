using Microsoft.Xna.Framework;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public partial class TreeBoss : ModNPC
    {
        private void Buffer()
        {
            if (MiscCounter++ == 90)
            {
                AICase = (int)AIStates.Selector;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                npc.dontTakeDamage = false;
            }
        }

        private void Intro()
        {
            npc.dontTakeDamage = true;
            npc.netUpdate = true;

            // 1 second buffer before the animation begins
            if (MiscCounter++ > 60)
            {
                // Start the fading in animation
                if (MiscCounter2 < 120)
                {
                    MiscCounter2 += 0.5f;

                    if (MiscCounter2 % 10 == 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            // Randomized rotation in with Pi radians because spawning energy underground is literally impossible to kill
                            float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

                            // Generates in a circle 200 pixels below the NPC's center
                            Vector2 RandomPosition = npc.Center + new Vector2(0, 200) + new Vector2(600, 0).RotatedBy(-RandomRotation);
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(RandomPosition, Vector2.Zero, ModContent.ProjectileType<AbsorbEnergyCinematic>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }
                    }
                }
                else
                {
                    // Show all three of the eyes
                    if (MiscCounter2++ == 120)
                    {
                        Main.PlaySound(SoundID.Item4, npc.Center);
                    }

                    // Start the music earlier before the title card shows up
                    if (MiscCounter2 == 155)
                    {
                        music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Iorich");
                    }

                    if (MiscCounter2 == 180)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (npc.Distance(Main.player[i].Center) < 900)
                            {
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 15;
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(npc.Center, 90, 10f, 20f);
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().TitleID = (int)OvermorrowModFile.TitleID.Iorich; // Turn this into an enum one day omg this is so unreadable
                            }
                        }

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText("Iorich, the Guardian has awoken!", 175, 75, 255);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Iorich, the Guardian has awoken!"), new Color(175, 75, 255));
                        }

                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/IorichExplosion"), npc.Center);
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), npc.Center, Vector2.Zero, Color.Lime, 1f, 6f);
                    }
                }
            }


            if (MiscCounter == 361)
            {
                AICase = (int)AIStates.Buffer;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                npc.netUpdate = true;
            }
        }

        private void Selector()
        {
            if (LightValue > 0)
            {
                LightValue = Utils.Clamp(MiscCounter2--, 0, 60) / 60f;
            }

            if (MiscCounter % 75 == 0 && MiscCounter > 60 && Main.expertMode)
            {
                int numSeeds = npc.life <= npc.lifeMax * 0.25f ? 16 : 13;
                float numberProjectiles = Main.rand.Next(7, numSeeds);
                int speedX = 1;
                int speedY = Main.rand.Next(-25, -15);
                float rotation = MathHelper.ToRadians(45);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f;
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 85, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<FloatingSeeds>(), (int)(npc.damage / 2f), 1f, Main.myPlayer);
                    }
                }
            }

            // Probably something to choose one of three attack versions
            int[] Attacks = new int[] { (int)AIStates.Thorns, (int)AIStates.Spirit, (int)AIStates.Runes };

            if (MiscCounter++ == 120)
            {
                Main.PlaySound(SoundID.Item4, npc.Center);

                // Chooses the attack from the list
                ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];
                //ChosenAttack = (int)AIStates.Spirit;

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
                switch (ChosenAttack)
                {
                    // This does various tweaks and settings for attack selections depending on npc status
                    case (int)AIStates.Runes:
                        // If the condition was satisfied and you DID choose the rune attack, now reset the counter
                        RuneCounter = 0;
                        break;
                    case (int)AIStates.Spirit:
                        // Makes it so the spirit attack runs twice if below 50% hp
                        if (npc.life < npc.lifeMax * 0.5f)
                        {
                            RunAgain = true;
                        }
                        break;
                }

                //AICase = ChosenAttack;
                AICase = (int)AIStates.Runes;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                // Keep the eye visual for the runes attack, otherwise turn it off
                if (ChosenAttack != (int)AIStates.Runes)
                {
                    ChosenAttack = 0;
                }
            }
        }

        private void ThornAttack(Player target)
        {
            // First find whether the offset position is to the left or the right of the target
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

                // Also get the target's position during this run so it doesn't get constantly offset while they move
                playerPos = new Vector2(target.position.X / 16, target.position.Y / 16);
                Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);

                // Get the ground beneath the target
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
                            Projectile.NewProjectile(playerPos * 16 + calculatedOffset, new Vector2(0, -5), ModContent.ProjectileType<SpikeStrip>(), 26, 2.5f, Main.myPlayer, 900f, 0f);
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
                                Projectile.NewProjectile(playerPos * 16 + calculatedOffset, new Vector2(0, -5), ModContent.ProjectileType<SpikeStrip>(), 26, 2.5f, Main.myPlayer, 900f, 0f);
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
        }

        private void SpiritAttack(Player target)
        {
            if (MiscCounter2 == 0)
            {
                // Various nondeterministic selections for this attack
                ChosenSpiritAttack = (int)(Main.rand.NextBool(2) ? SpiritAttacks.Circular : SpiritAttacks.Randomized);

                // Check to see if the previous attack was the same
                while (PreviousSpirit == ChosenSpiritAttack)
                {
                    ChosenSpiritAttack = ChosenSpiritAttack = (int)(Main.rand.NextBool(2) ? SpiritAttacks.Circular : SpiritAttacks.Randomized);
                }

                npc.netUpdate = true; // Multiplayer code stinky
            }

            SpiritPoints[] values = (SpiritPoints[])Enum.GetValues(typeof(SpiritPoints));

            switch (ChosenSpiritAttack)
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

                    // If npc's life is below 50%, spawn them all at the same time. Otherwise, spawn them one-by-one.
                    if (npc.life < npc.lifeMax * 0.5f)
                    {
                        if (MiscCounter == 45)
                        {
                            for (int index = 0; index < 4; index++)
                            {
                                float Rotation = (int)SpawnDirections[index] * MathHelper.PiOver4;
                                int RADIUS = 100;

                                int proj = Projectile.NewProjectile(target.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpirit>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpirit)Main.projectile[proj].modProjectile).RotationCenter = target;
                            }
                        }
                    }
                    else
                    {

                        if (MiscCounter % 15 == 0)
                        {
                            float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                            int RADIUS = 100;

                            int proj = Projectile.NewProjectile(target.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpirit>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                            ((GreenSpirit)Main.projectile[proj].modProjectile).RotationCenter = target;

                            MiscCounter2++;
                        }
                    }

                    break;
                case (int)SpiritAttacks.Circular:
                    // Determine the rotation direction and offset (where it starts spawning)
                    if (MiscCounter == 0)
                    {
                        RotationDirection = Main.rand.NextBool(2) ? 1 : -1;
                        RotationOffset = Main.rand.Next(4) * MathHelper.PiOver2;

                        npc.netUpdate = true;
                    }

                    if (MiscCounter % 7 == 0 && MiscCounter <= 49)
                    {
                        int RADIUS = 135;

                        float Rotation = RotationDirection * (int)values[(int)MiscCounter2] * MathHelper.PiOver4;
                        Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                        int proj = Projectile.NewProjectile(target.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<GreenSpirit>(), npc.damage, 0f, Main.myPlayer, Rotation + RotationOffset, RADIUS);
                        ((GreenSpirit)Main.projectile[proj].modProjectile).RotationCenter = target;

                        MiscCounter2++;
                    }

                    MiscCounter++;
                    break;
            }

            if (MiscCounter == (npc.life < npc.lifeMax * 0.5f ? 150 : 60))
            {
                // Run the attack again but set it to false
                if (RunAgain)
                {
                    AICase = (int)AIStates.Spirit;
                    RunAgain = false;
                }
                else
                {
                    AICase = (int)AIStates.Selector;
                }

                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                // Store the current attack for next iteration
                PreviousSpirit = ChosenSpiritAttack;
            }
        }

        private void RuneAttack()
        {
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
                            // Randomized rotation in with Pi radians because spawning energy underground is literally impossible to kill
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
                AICase = (int)AIStates.Energy;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                // Reset properties
                npc.chaseable = true;
                npc.dontTakeDamage = false;

                // Start despawn code for the spinners
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<RuneSpinner>())
                    {
                        ((RuneSpinner)Main.projectile[i].modProjectile).CanDespawn = true;
                    }
                }
            }
        }

        private void EnergyAttack()
        {
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

            if (MiscCounter < 60)
            {
                LightValue = Utils.Clamp(MiscCounter, 0, 60) / 60f;
                MiscCounter2++;
            }

            if (AbsorbedEnergies > ENERGY_THRESHOLD)
            {
                // 1 second opening before he starts the attack
                if (MiscCounter > 60 && MiscCounter % 30 == 0)
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
                if (MiscCounter > 60 && MiscCounter % 30 == 0)
                {
                    // Increase the amount of stars spawned based on the absorbed energies
                    int energyIncrementer = (int)MathHelper.Lerp(0, 3, AbsorbedEnergies / 17f);
                    int maxIterations = Main.rand.Next(3 + energyIncrementer, 7 + energyIncrementer);
                    for (int i = 0; i < maxIterations; i++)
                    {
                        // Choose a position above the player with random x-axis offsets
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-18, 18) * 80, Main.rand.Next(-1600, -1200));
                        npc.netUpdate = true;

                        Projectile.NewProjectile(RandomPosition, new Vector2(0, Main.rand.Next(3, 6) * 2), ModContent.ProjectileType<LesserPrismaMeteor>(), npc.damage, 5f, Main.myPlayer, Main.rand.NextFloat(0.04f, 0.085f));
                    }
                }
            }

            if (++MiscCounter == 660)
            {
                AICase = (int)AIStates.Selector;
                GlobalCounter = 0;
                MiscCounter = 0;

                // This is to return the brightness back to normal
                MiscCounter2 = 60;

                // Reset properties
                AbsorbedEnergies = 0;
            }
        }

        private void Death()
        {
            int SPAWN_OFFSET = 116;
            int DUST_OFFSET = 15;
            int AlternatingSpawnOffset = (SPAWN_OFFSET / 3) * 2; // It was too far away lol

            for (int iterations = 0; iterations < 26; iterations++)
            {
                Vector2 calculatedOffset = new Vector2(-AlternatingSpawnOffset + (DUST_OFFSET * iterations), 0);

                // Alternate dust between green/brown
                if (npc.localAI[2]++ > 3f)
                {
                    if (Main.rand.NextBool(2))
                    {
                        int dust = Dust.NewDust(npc.getRect().BottomLeft() + calculatedOffset, 28, 28, DustID.Dirt, 0, 0, 0, default, Main.rand.NextFloat(1, 1.84f));
                        Main.dust[dust].noGravity = true;
                    }
                    else
                    {
                        int dust = Dust.NewDust(npc.getRect().BottomLeft() + calculatedOffset, 28, 28, DustID.Grass, 0, 0, 0, default, Main.rand.NextFloat(1, 1.84f));
                        Main.dust[dust].noGravity = true;
                    }
                }
            }

            if (MiscCounter++ == 60)
            {
                BossText("this isnt even my FINAL form!!!!!");
            }

            if (MiscCounter > 60 && MiscCounter2 <= 180)
            {
                if (++MiscCounter2 % 60 == 0)
                {
                    Main.PlaySound(SoundID.Item4, npc.Center);
                }
            }

            if (MiscCounter2 > 180)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (npc.Distance(Main.player[i].Center) < 900)
                    {
                        Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                    }
                }
            }

            if (MiscCounter == 300)
            {
                for (int i = 0; i < 200; i++)
                {
                    Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(40, 60);
                    Dust.NewDust(npc.Center, 2, 2, DustID.TerraBlade, RandomVelocity.X, RandomVelocity.Y);
                }

                CanDie = true;
                npc.life = 0;
                npc.checkDead();
            }
        }
    }
}