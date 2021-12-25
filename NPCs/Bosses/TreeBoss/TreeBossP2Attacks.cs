using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Particles;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public partial class TreeBossP2 : ModNPC
    {
        private void Teleport_Attacks(Player player)
        {
            // Spawn an entrance portal behind the boss
            if (MiscCounter++ == 0)
            {
                npc.velocity = Vector2.Zero;
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EntrancePortal>(), 0, 0f, Main.myPlayer);
            }

            if (MiscCounter2 > 0 && MiscCounter > 320)
            {
                npc.alpha = 255;
                MiscCounter2--; // Decrement the counter to cause the boss to fade out

                // Spawn the exit portal near the player
                if (MiscCounter2 == 0)
                {
                    npc.dontTakeDamage = true;
                    npc.Center = player.Center - Vector2.UnitY * 6000f;


                    if (ChosenPortal == (int)PortalAttacks.Scythes)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Projectile.NewProjectile(player.Center - Vector2.UnitY * 500, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, 1, npc.whoAmI);
                        }
                        else
                        {
                            Projectile.NewProjectile(player.Center + Vector2.UnitY * 500, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, 2, npc.whoAmI);
                        }
                    }
                    else
                    {
                        Vector2 RandomCenter = player.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 400;
                        npc.netUpdate = true;

                        Projectile.NewProjectile(RandomCenter, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, -1, npc.whoAmI);
                        int tracking = Projectile.NewProjectile(RandomCenter, Vector2.Zero, ModContent.ProjectileType<TrackingWarning>(), 0, 1f, Main.myPlayer, player.whoAmI, 1);
                        ((TrackingWarning)Main.projectile[tracking].modProjectile).ParentNPC = npc;
                    }
                }
            }

            if (PortalLaunched)
            {

                if (ChosenPortal == (int)PortalAttacks.Scythes)
                {
                    if (GlobalCounter % 10 == 0)
                    {
                        Projectile.NewProjectile(npc.Center - Vector2.UnitX * 5000, Vector2.UnitX * 20, ModContent.ProjectileType<ScytheWarning>(), npc.damage, 0, Main.myPlayer);
                        //Projectile.NewProjectile(npc.Center, Vector2.UnitX * -20, ModContent.ProjectileType<ScytheWarning>(), npc.damage, 0, Main.myPlayer);

                        Projectile.NewProjectile(npc.Center, Vector2.UnitX * 20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center, Vector2.UnitX * -20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                    }
                }

                if (GlobalCounter++ == 50)
                {
                    PortalLaunched = false;
                    npc.velocity = Vector2.Zero;

                    int RepeatRuns = Main.expertMode ? 2 : 1;
                    if (PortalRuns < RepeatRuns)
                    {
                        AICase = (int)AIStates.Teleport;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 120;

                        ChosenPortal = Main.rand.Next(1, 3);

                        // I don't know why this doesn't spawn if it repeats the attack cycle again?
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EntrancePortal>(), 0, 0f, Main.myPlayer);
                    }
                    else
                    {
                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        PortalRuns = 0;
                    }
                }

            }

            // I don't know why I put a second counter here but I guess everything is twice as fast
            // And I'm too lazy to remove to and recondition the code
            MiscCounter++;
        }

        private void Spirit(Player player)
        {
            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, player.Center.X > npc.Center.X ? 2 : -2, 0.025f);
            npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, player.Center.Y > npc.Center.Y ? 2.5f : -2.5f, 0.01f);

            if (MiscCounter == 0)
            {
                // Various nondeterministic selections for this attack
                int[] Attacks = new int[] { (int)SpiritAttacks.Circular, (int)SpiritAttacks.Randomized, (int)SpiritAttacks.Combined };

                // Chooses the attack from the list
                ChosenSpiritAttack = Attacks[Main.rand.Next(Attacks.Length)];

                //ChosenSpiritAttack = (int)(Main.rand.NextBool(2) ? SpiritAttacks.Circular : SpiritAttacks.Randomized);

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
                        if (MiscCounter == 30)
                        {
                            float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                            int RADIUS = 100 + ((int)MiscCounter2 * 50);

                            for (int i = 0; i < 4; i++)
                            {
                                Rotation += MathHelper.PiOver2 * i;

                                int proj = Projectile.NewProjectile(player.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).RotationCenter = player;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                            }

                            MiscCounter2++;
                        }
                    }
                    else
                    {
                        if (MiscCounter == 30)
                        {
                            float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                            int RADIUS = 100 + ((int)MiscCounter2 * 50);

                            for (int i = 0; i < 2; i++)
                            {
                                Rotation += MathHelper.Pi * i;

                                int proj = Projectile.NewProjectile(player.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).RotationCenter = player;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                            }

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

                    if (MiscCounter == 7)
                    {
                        float RandomOffset = Main.rand.Next(0, 10) * 40; // Random number between 0 and 360 with 40 degree increments
                        for (int i = 0; i < values.Length; i++)
                        {
                            int RADIUS = 175;

                            float Rotation = RotationDirection * (int)values[(int)i] * MathHelper.PiOver4;
                            Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                            int proj = Projectile.NewProjectile(player.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2Circle>(), npc.damage, RandomOffset, Main.myPlayer, Rotation + RotationOffset, RADIUS);
                            ((GreenSpiritP2Circle)Main.projectile[proj].modProjectile).RotationCenter = player;
                            ((GreenSpiritP2Circle)Main.projectile[proj].modProjectile).RandomOffset = RandomOffset;
                        }
                    }

                    MiscCounter++;
                    break;
                case (int)SpiritAttacks.Combined:
                    if (MiscCounter++ == 0)
                    {
                        values = values.Shuffle();

                        // Populate with 4 random values
                        for (int i = 0; i < SpawnDirections.Count; i++)
                        {
                            // Add a random value to the list from the shuffled enum array
                            SpawnDirections[i] = (SpiritPoints)values.GetValue(i);
                        }

                        RotationDirection = Main.rand.NextBool(2) ? 1 : -1;
                        RotationOffset = Main.rand.Next(4) * MathHelper.PiOver2;

                        npc.netUpdate = true;
                    }

                    if (MiscCounter == 7)
                    {
                        float RandomOffset = Main.rand.Next(0, 10) * 40; // Random number between 0 and 360 with 40 degree increments
                        for (int i = 0; i < values.Length; i++)
                        {
                            int RADIUS = 175;

                            float Rotation = RotationDirection * (int)values[(int)i] * MathHelper.PiOver4;
                            Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                            int proj = Projectile.NewProjectile(player.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2Circle>(), npc.damage, RandomOffset, Main.myPlayer, Rotation + RotationOffset, RADIUS);
                            ((GreenSpiritP2Circle)Main.projectile[proj].modProjectile).RotationCenter = player;
                            ((GreenSpiritP2Circle)Main.projectile[proj].modProjectile).RandomOffset = RandomOffset;
                        }
                    }

                    if (npc.life < npc.lifeMax * 0.5f)
                    {
                        if (MiscCounter == 30)
                        {
                            float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                            int RADIUS = 100 + ((int)MiscCounter2 * 50);

                            for (int i = 0; i < 4; i++)
                            {
                                Rotation += MathHelper.PiOver2 * i;

                                int proj = Projectile.NewProjectile(player.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).RotationCenter = player;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                            }

                            MiscCounter2++;
                        }
                    }
                    else
                    {
                        if (MiscCounter == 30)
                        {
                            float Rotation = (int)SpawnDirections[(int)MiscCounter2] * MathHelper.PiOver4;
                            int RADIUS = 100 + ((int)MiscCounter2 * 50);

                            for (int i = 0; i < 2; i++)
                            {
                                Rotation += MathHelper.Pi * i;

                                int proj = Projectile.NewProjectile(player.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation), Vector2.Zero, ModContent.ProjectileType<GreenSpiritP2>(), npc.damage, 0f, Main.myPlayer, Rotation, RADIUS);
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).RotationCenter = player;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                                ((GreenSpiritP2)Main.projectile[proj].modProjectile).Converge = true;
                            }

                            MiscCounter2++;
                        }
                    }


                    break;
            }

            if (MiscCounter == 150)
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

            }
        }

        private void RuneAttack()
        {
            GlobalCounter++;

            npc.velocity = Vector2.Zero;

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
                            float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

                            // Generates in a half-arc 200 pixels below the NPC's center
                            Vector2 RandomPosition = npc.Center + new Vector2(0, 200) + new Vector2(1200, 0).RotatedBy(-RandomRotation);
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.NewNPC((int)RandomPosition.X, (int)RandomPosition.Y, ModContent.NPCType<AbsorbEnergyP2>(), 0, npc.whoAmI);
                            }
                        }
                    }
                }
            }

            EnergyCount = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AbsorbEnergyP2>())
                {
                    EnergyCount++;
                }
            }

            npc.localAI[0] += 0.04f; // Rotation code for the rune

            if (Utils.Clamp(++MiscCounter, 0, 600) == 600 && EnergyCount == 0)
            {
                RepeatMeteors = (int)Math.Floor(AbsorbedEnergies / 12f);

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

        private void EnergyAttack(Player player)
        {
            if (MiscCounter++ == 0)
            {
                BossText("Meteoric Burst!");
                npc.alpha = 255;

                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MeteoricBurst>(), npc.damage * 3, 60f, Main.myPlayer, npc.whoAmI);
                npc.velocity = Vector2.Zero;
            }

            // Nudge the boss in a random direction
            if (MiscCounter == 60)
            {
                FlyDistance = npc.Center - Vector2.UnitY * 2250;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (npc.Distance(Main.player[i].Center) < 800)
                    {
                        Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                    }
                }

                npc.velocity = Vector2.One.RotatedByRandom(-MathHelper.PiOver4) * 15;
            }

            if (MiscCounter > 60 && MiscCounter < 180)
            {
                npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(2f));
            }

            if (MiscCounter > 180 && MiscCounter < 360)
            {
                if (MiscCounter == 240)
                {
                    float radius = 60;
                    int numLocations = 6;

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 position = npc.Center + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                        Vector2 dustvelocity = new Vector2(0f, 12f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));

                        Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Main.DiscoColor, 1, 4f, MathHelper.ToRadians(360f / numLocations * i), 1f);
                    }


                }

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (npc.Distance(Main.player[i].Center) < 1600)
                    {
                        Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 15;
                    }
                }

                Vector2 direction = FlyDistance - npc.Center;
                float distanceTo = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

                direction.SafeNormalize(Vector2.Zero);
                float launchSpeed = distanceTo < 400 ? 0.0125f : 0.025f;
                direction *= launchSpeed;

                float inertia = distanceTo < 400 ? 20f : 150f;
                npc.velocity = (npc.velocity * (inertia - 1) + direction) / inertia;
            }

            if (MiscCounter > 360)
            {
                if (MiscCounter == 361)
                {
                    npc.velocity = Vector2.Zero;

                    int tracking = Projectile.NewProjectile(npc.Center, Vector2.UnitY * 20, ModContent.ProjectileType<MeteorWarning>(), 0, 0f, Main.myPlayer, player.whoAmI);
                    ((MeteorWarning)Main.projectile[tracking].modProjectile).ParentNPC = npc;
                }
            }

            if (MeteorLanded)
            {
                // Turn off the scythe indicators
                ChosenAttack = 0;

                if (MiscCounter2++ == 240)
                {
                    // Repeat the meteor attack again for each 12 absorbed energies
                    if (RepeatMeteors == 0)
                    {
                        AbsorbedEnergies = 0;
                    }

                    AICase = RepeatMeteors-- != 0 ? (int)AIStates.Energy : (int)AIStates.Selector;
                    GlobalCounter = 0;
                    MiscCounter = 0;
                    MiscCounter2 = 0;

                    MeteorLanded = false;
                }
            }
        }

        private void Death()
        {
            // Death animation code
            if (npc.ai[3] > 0f)
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[2] > 0)
                {
                    npc.ai[2]--;

                    if (npc.ai[2] == 480)
                    {
                        BossText("I deem thee fit to inherit their powers.");
                    }

                    if (npc.ai[2] == 300)
                    {
                        BossText("Thou Dryad shalt guide thee.");
                    }

                    if (npc.ai[2] == 120)
                    {
                        BossText("Fare thee well.");
                    }
                }
                else
                {
                    npc.dontTakeDamage = true;
                    npc.ai[3]++; // Death timer
                    npc.velocity.X *= 0.95f;

                    if (npc.velocity.Y < 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                    }

                    if (npc.velocity.X > 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                    }

                    if (npc.ai[3] > 120f)
                    {
                        npc.Opacity = 1f - (npc.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && npc.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(npc.Left, npc.width, npc.height / 2, DustID.TerraBlade, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = npc.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(npc.width * 1.5f, npc.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + npc.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (npc.ai[3] % 30f == 1f)
                    {
                        //Main.PlaySound(4, npc.Center, 22);
                        Main.PlaySound(SoundID.Item25, npc.Center); // every half second while dying, play a sound
                    }

                    if (npc.ai[3] >= 180f)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 0);
                        npc.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
                    }
                }
                return;
            }
        }

        /*switch (npc.ai[0])
           {
               case -1: // case switching
                   {
                       if (movement == true)
                       {
                           if (changedPhase2 == true) { RandomCeiling = 4; }
                           else { RandomCeiling = 3; }
                           while (RandomCase == LastCase)
                           {
                               RandomCase = Main.rand.Next(1, RandomCeiling);
                           }
                           LastCase = RandomCase;
                           movement = false;
                           npc.ai[0] = RandomCase;
                       }
                       else
                       {
                           movement = true;
                           npc.ai[0] = 0;
                       }
                   }
                   break;
               case 0: // Follow player
                   if (npc.ai[0] == 0)
                   {
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

                       if (npc.ai[1] > (changedPhase2 ? 90 : 120))
                       {
                           npc.ai[0] = -1;
                           npc.ai[1] = 0;
                       }
                   }
                   break;
               case 1: // Shoot scythes
                   if (npc.ai[0] == 1)
                   {
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

                       if (npc.ai[1] % 90 == 0)
                       {
                           int shootSpeed = Main.rand.Next(8, 12);
                           Vector2 position = npc.Center;
                           Vector2 targetPosition = Main.player[npc.target].Center;
                           Vector2 direction = targetPosition - position;
                           direction.Normalize();
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                           }
                       }

                       if (npc.ai[1] > 600)
                       {
                           npc.ai[0] = -1;
                           npc.ai[1] = 0;
                       }
                   }
                   break;
               case 2: // Absorb energy
                   npc.velocity = Vector2.Zero;

                   // Summon projectiles from off-screen that move towards the boss
                   if (npc.ai[1] % 20 == 0 && (energiesAbsorbed + energiesKilled) < 33 && npc.ai[1] <= 660)
                   {
                       for (int i = 0; i < 6; i++)
                       {
                           float randPositionX = npc.Center.X + Main.rand.Next(-10, 10) * 600;
                           float randPositionY = npc.Center.Y + Main.rand.Next(-10, 10) * 600;
                           npc.netUpdate = true;
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               NPC.NewNPC((int)randPositionX, (int)randPositionY, ModContent.NPCType<AbsorbEnergy>(), 0, 0f, npc.whoAmI, 0, npc.damage / 3, Main.myPlayer);
                           }
                       }
                   }

                   if (energiesKilled <= 5 && npc.ai[1] > 660) // punish
                   {
                       npc.ai[2] = 1;
                       Main.NewText("u suk");
                   }
                   else if (npc.ai[1] > 660) // else
                   {
                       npc.ai[2] = 1;
                   }

                   if (npc.ai[1] > 660 && npc.ai[3] == 1)
                   {
                       energiesAbsorbed = 0;
                       energiesKilled = 0;
                       npc.ai[0] = 4;
                       npc.ai[1] = 0;
                       npc.ai[2] = 0;
                   }
                   break;
               case 4: // Shoot nature blasts
                   npc.velocity = Vector2.Zero;

                   if (npc.ai[1] == 120)
                   {
                       int projectiles = Main.rand.Next((changedPhase2 ? 13 : 9), (changedPhase2 ? 18 : 13));
                       npc.netUpdate = true;

                       for (int i = 0; i < projectiles; i++)
                       {
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               Projectile.NewProjectile(npc.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<NatureBlast>(), 19, 2, Main.myPlayer);
                           }
                       }
                   }

                   if (npc.ai[1] > 240)
                   {
                       npc.ai[0] = -1;
                       npc.ai[1] = 0;
                   }
                   break;
               case 3: // scythes
                   {
                       Vector2 moveTo = player.Center;
                       moveTo.X += 50 * (npc.Center.X < moveTo.X ? -1 : 1);
                       var move = moveTo - npc.Center;
                       var speed = 1;

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


                       if (npc.ai[1] == 180)
                       {
                           for (int i = -2; i < 3; i++)
                           {
                               if (Main.netMode != NetmodeID.MultiplayerClient)
                               {
                                   Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y - 200), Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                                   Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y + 200), -Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                               }
                           }
                       }
                       if (npc.ai[1] > 540)
                       {
                           npc.ai[0] = 0;
                           npc.ai[1] = 0;
                       }
                       break;
                   }
           }*/
    }
}
