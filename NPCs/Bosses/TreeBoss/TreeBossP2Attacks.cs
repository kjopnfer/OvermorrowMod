using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Consumable.Boss.TreeRune;
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
        private void Intro()
        {
            npc.dontTakeDamage = true;

            if (MiscCounter++ == 0)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (npc.Distance(Main.player[i].Center) < 900)
                    {
                        Main.player[i].GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(npc.Center, 90, 60f, 20f);
                        Main.player[i].GetModPlayer<OvermorrowModPlayer>().TitleID = (int)OvermorrowModFile.TitleID.IorichP2; // Turn this into an enum one day omg this is so unreadable
                                                                                                                              //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ShowText = true;
                    }
                }
            }

            if (MiscCounter % 100 == 0)
            {
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TreeRune_Pulse>(), 0, 0f, Main.myPlayer, 1);
            }

            if (MiscCounter == 300)
            {
                AICase = (int)AIStates.Buffer;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                npc.dontTakeDamage = false;
            }
        }

        private void Selector(Player player)
        {
            #region Movement
            if (MiscCounter2 == 0 && GlobalCounter == 0)
            {
                SelectorAttacks[] attacks = (SelectorAttacks[])Enum.GetValues(typeof(SelectorAttacks));
                attacks.Shuffle();

                int NumSelected = npc.life <= npc.lifeMax * 0.5f ? 3 : 2;
                for (int i = 0; i < NumSelected; i++)
                {
                    SelectedAttacks[i] = (int)attacks[i];
                }
            }

            if (MiscCounter2 <= (npc.life <= npc.lifeMax * 0.5f ? 2 : 1))
            {
                switch (SelectedAttacks[(int)MiscCounter2])
                {
                    case (int)SelectorAttacks.Charge:
                        if (GlobalCounter < 40)
                        {
                            if (GlobalCounter == 0)
                            {
                                PlayerPosition = player.Center;
                                //Projectile.NewProjectile(npc.Center, npc.DirectionTo(PlayerPosition), ModContent.ProjectileType<BodyWarning>(), npc.damage, 0, Main.myPlayer);

                                int warning = Projectile.NewProjectile(npc.Center, npc.DirectionTo(PlayerPosition), ModContent.ProjectileType<TreeWarning>(), 0, 0f, Main.myPlayer);
                                ((TreeWarning)Main.projectile[warning].modProjectile).DrawColor = Color.LightGreen;
                                ((TreeWarning)Main.projectile[warning].modProjectile).Length = 1280;
                                ((TreeWarning)Main.projectile[warning].modProjectile).CastRay = false;
                            }

                            if (GlobalCounter % 2 == 0)
                            {
                                float RADIUS = 90;
                                int NUM_LOCATIONS = 30;
                                for (int i = 0; i < NUM_LOCATIONS; i++)
                                {
                                    Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i)) * RADIUS;
                                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }

                            npc.velocity = Vector2.Zero;

                            InitialPosition = npc.Center;
                            MoveDirection = npc.direction;

                            npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                        }

                        if (GlobalCounter == 40)
                        {
                            DrawAfterimage = true;

                            npc.velocity = npc.DirectionTo(PlayerPosition) * 36;
                        }

                        if (GlobalCounter > 70)
                        {
                            if (MiscCounter < 20)
                            {
                                npc.velocity = Vector2.SmoothStep(npc.velocity, Vector2.Zero, MiscCounter++ / 20f);
                            }
                            else
                            {
                                AICase = (int)AIStates.Buffer;
                                GlobalCounter = 0;
                                MiscCounter = 0;
                                MiscCounter2++;

                                DrawAfterimage = false;
                            }
                        }
                        break;
                    case (int)SelectorAttacks.Bezier:
                        if (GlobalCounter < 60)
                        {
                            if (GlobalCounter == 0)
                            {
                                float RADIUS = 90;
                                int NUM_LOCATIONS = 30;
                                for (int i = 0; i < NUM_LOCATIONS; i++)
                                {
                                    for (int ii = 0; ii < 3; ii++)
                                    {
                                        Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i)) * RADIUS;
                                        Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i));
                                        int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                        Main.dust[dust].noGravity = true;
                                    }
                                }

                                PlayerPosition = player.Center;
                                InitialPosition = npc.Center;
                                MoveDirection = npc.direction;
                            }

                            DrawAfterimage = true;

                            Vector2 cp1 = PlayerPosition - Vector2.UnitY * 700;
                            npc.Center = ModUtils.Bezier(InitialPosition, PlayerPosition + Vector2.UnitX * (1500 * MoveDirection), cp1, cp1, Utils.Clamp(GlobalCounter, 0, 60) / 60f);

                            if (GlobalCounter >= 10 && GlobalCounter <= 60 && GlobalCounter % 10 == 0)
                            {
                                Main.PlaySound(SoundID.DD2_DrakinShot, npc.Center);
                                Projectile.NewProjectile(npc.Center, Vector2.UnitY * 8, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                                Projectile.NewProjectile(npc.Center - (Vector2.UnitY * 2500), Vector2.UnitY, ModContent.ProjectileType<ScytheWarning>(), npc.damage, 0, Main.myPlayer);
                            }
                        }
                        else
                        {
                            AICase = (int)AIStates.Buffer;
                            GlobalCounter = 0;
                            MiscCounter = 0;
                            MiscCounter2++;

                            DrawAfterimage = false;
                        }
                        break;
                    case (int)SelectorAttacks.Spread:
                        npc.velocity = Vector2.Zero;

                        if (GlobalCounter == 30 || GlobalCounter == 60)
                        {
                            float numberProjectiles = 3;
                            float rotation = MathHelper.ToRadians(GlobalCounter == 30 ? 30 : 15);
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

                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Main.PlaySound(SoundID.DD2_DrakinShot, npc.Center);
                                for (int i = 0; i < numberProjectiles; i++)
                                {
                                    Vector2 perturbedSpeed = new Vector2(delta.X, delta.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                                    Projectile.NewProjectile(npc.Center, perturbedSpeed * 4, ModContent.ProjectileType<NatureScythe>(), npc.damage, 2f, Main.myPlayer, 0f, 0f);
                                    //int warning = Projectile.NewProjectile(npc.Center, perturbedSpeed, ModContent.ProjectileType<TreeWarning>(), 0, 0f, Main.myPlayer);
                                    //((TreeWarning)Main.projectile[warning].modProjectile).DrawColor = Color.LightGreen;

                                    Projectile.NewProjectile(npc.Center, perturbedSpeed * 4, ModContent.ProjectileType<ScytheWarning>(), npc.damage, 0, Main.myPlayer);
                                }
                            }

                            if (GlobalCounter == 60)
                            {
                                AICase = (int)AIStates.Buffer;
                                GlobalCounter = 0;
                                MiscCounter = 0;
                                MiscCounter2++;
                            }
                        }
                        break;
                    case (int)SelectorAttacks.Scythes:
                        DrawAfterimage = true;
                        switch (MiscCounter)
                        {
                            case 0:
                                if (GlobalCounter == 0)
                                {
                                    float RADIUS = 90;
                                    int NUM_LOCATIONS = 30;
                                    for (int i = 0; i < NUM_LOCATIONS; i++)
                                    {
                                        for (int ii = 0; ii < 3; ii++)
                                        {
                                            Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i)) * RADIUS;
                                            Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i));
                                            int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                            Main.dust[dust].noGravity = true;
                                        }
                                    }

                                    MoveDirection = -npc.direction;
                                    InitialPosition = npc.Center;
                                    PlayerPosition = player.Center;
                                }

                                if (GlobalCounter < 20)
                                {
                                    npc.Center = Vector2.Lerp(InitialPosition, PlayerPosition + Vector2.UnitX * (900 * MoveDirection), GlobalCounter / 20f);
                                }
                                else
                                {
                                    GlobalCounter = 0;
                                    MiscCounter++;

                                    InitialPosition = npc.Center;

                                    float RADIUS = 90;
                                    int NUM_LOCATIONS = 30;
                                    for (int i = 0; i < NUM_LOCATIONS; i++)
                                    {
                                        for (int ii = 0; ii < 3; ii++)
                                        {
                                            Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i)) * RADIUS;
                                            Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i));
                                            int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                            Main.dust[dust].noGravity = true;
                                        }
                                    }
                                }
                                break;
                            case 1:
                                if (GlobalCounter < 10)
                                {
                                    npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + new Vector2(-400 * MoveDirection, -500), GlobalCounter / 10f);
                                }
                                else
                                {
                                    GlobalCounter = 0;
                                    MiscCounter++;

                                    InitialPosition = npc.Center;

                                    float RADIUS = 90;
                                    int NUM_LOCATIONS = 30;
                                    for (int i = 0; i < NUM_LOCATIONS; i++)
                                    {
                                        for (int ii = 0; ii < 3; ii++)
                                        {
                                            Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i)) * RADIUS;
                                            Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / NUM_LOCATIONS * i));
                                            int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                            Main.dust[dust].noGravity = true;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                if (GlobalCounter % 10 == 0)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.UnitX * (-15 * MoveDirection), ModContent.ProjectileType<NatureScythe>(), npc.damage, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(npc.Center - (Vector2.UnitX * 5000 * -MoveDirection), Vector2.UnitX * (-12 * MoveDirection), ModContent.ProjectileType<ScytheWarning>(), npc.damage, 0, Main.myPlayer);
                                }

                                if (GlobalCounter < 30)
                                {
                                    npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + new Vector2(-300 * MoveDirection, 800), GlobalCounter / 30f);

                                }
                                else
                                {
                                    AICase = (int)AIStates.Buffer;
                                    GlobalCounter = 0;
                                    MiscCounter = 0;
                                    MiscCounter2++;

                                    DrawAfterimage = false;
                                }
                                break;
                        }
                        break;
                }

                GlobalCounter++;
                #endregion
            }
            else
            {
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 3, 0.05f);
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (player.Center.Y > npc.Center.Y ? 2.5f : -2.5f), 0.02f);

                #region Attack Selection
                int[] Attacks = new int[] { (int)AIStates.Teleport, (int)AIStates.Spirit, (int)AIStates.Runes };

                if (GlobalCounter++ == 120)
                {
                    Main.PlaySound(SoundID.Item4, npc.Center);

                    // Chooses the attack from the list, guaranteed meteor attack after 4 attacks
                    if (RuneCounter == 4)
                    {
                        ChosenAttack = (int)AIStates.Runes;
                    }
                    else
                    {
                        ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];
                    }

                    // Makes sure the healing attack doesn't have a chance to be chosen unless conditions are met
                    while (ChosenAttack == (int)AIStates.Runes && RuneCounter < MINIMUM_ATTACKS)
                    {
                        ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];
                    }

                    // If the boss is below 50%, make them do the Rune attack at least once
                    if (npc.life <= npc.lifeMax * 0.5f && !HealthRune)
                    {
                        ChosenAttack = (int)AIStates.Runes;
                        RuneCounter = 0;
                        HealthRune = true;
                    }

                    // This prevents an attack from running multiple times in a row by forcing it to be the other attack
                    switch (ChosenAttack)
                    {
                        case (int)AIStates.Teleport:
                            // If the attack has repeated twice in a row, force it to the other attack
                            if (TeleportCounter >= 2)
                            {
                                ChosenAttack = (int)AIStates.Spirit;
                                TeleportCounter = 0;
                                SpiritCounter = 0;
                            }
                            else
                            {
                                // Otherwise, reset the other attack since it's no longer consecutive
                                SpiritCounter = 0;
                                TeleportCounter++;
                            }

                            // Increment the non-healing attack counter
                            RuneCounter++;
                            break;
                        case (int)AIStates.Spirit:
                            if (TeleportCounter >= 2)
                            {
                                ChosenAttack = (int)AIStates.Teleport;
                                SpiritCounter = 0;
                                TeleportCounter = 0;
                            }
                            else
                            {
                                TeleportCounter = 0;
                                SpiritCounter++;
                            }

                            // Increment the non-healing attack counter
                            RuneCounter++;
                            break;
                    }

                    // Increment the non-healing attack counter
                    if (ChosenAttack != (int)AIStates.Runes)
                    {
                        RuneCounter++;
                    }
                }
                #endregion

                if (GlobalCounter >= 300)
                {
                    // If the condition was satisfied and you DID choose the rune attack, now reset the counter
                    if (ChosenAttack == (int)AIStates.Runes)
                    {
                        RuneCounter = 0;
                    }

                    //AICase = ChosenAttack;
                    AICase = (int)AIStates.Energy;
                    GlobalCounter = 0;
                    MiscCounter = 0;
                    MiscCounter2 = 0;

                    switch (AICase)
                    {
                        case (int)AIStates.Teleport:
                            MiscCounter2 = 120;
                            ChosenPortal = Main.rand.Next(1, 3);
                            break;
                        case (int)AIStates.Spirit:
                            if (!RunAgain) RunAgain = true;

                            break;
                    }

                    // Keep the scythe visual for the runes attack, otherwise turn it off
                    if (ChosenAttack != (int)AIStates.Runes)
                    {
                        ChosenAttack = 0;
                    }
                }
            }
        }

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
                    npc.Center = player.Center - Vector2.UnitY * 2250;


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
                        AICase = (int)AIStates.Buffer;
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
            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 4, 0.05f);
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
                    AICase = (int)AIStates.Buffer;
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
            npc.dontTakeDamage = true;

            if (MiscCounter++ == 0)
            {
                BossText("Meteoric Burst!");
                npc.alpha = 255;

                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MeteoricBurst>(), npc.damage * 3, 60f, Main.myPlayer, npc.whoAmI);
                npc.velocity = Vector2.Zero;

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/IorichMeteor"), npc.Center);
            }

            if (MiscCounter < 60 && !MeteorLight)
            {
                LightValue = Utils.Clamp(MiscCounter, 0, 60) / 60f;
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

            // Hover aiming part of the code
            if (MiscCounter > 360)
            {
                if (MiscCounter == 361)
                {
                    npc.velocity = Vector2.Zero;

                    int tracking = Projectile.NewProjectile(npc.Center, Vector2.UnitY * 20, ModContent.ProjectileType<MeteorWarning>(), npc.damage, 0f, Main.myPlayer, player.whoAmI);
                    ((MeteorWarning)Main.projectile[tracking].modProjectile).ParentNPC = npc;
                }
            }

            if (MeteorLanded)
            {
                // Turn off the scythe indicators
                ChosenAttack = 0;

                if (MiscCounter2++ == 240)
                {
                    AICase = RepeatMeteors-- > 0 ? (int)AIStates.Energy : (int)AIStates.Buffer;
                    GlobalCounter = 0;
                    MiscCounter = 0;

                    // Set the progression value for the lighting to return to normal if going back to the selector
                    MiscCounter2 = RepeatMeteors-- != 0 ? 0 : 60;

                    MeteorLanded = false;
                    MeteorLight = true;
                }
            }
        }

        private void Death()
        {
            npc.velocity = Vector2.Zero;

            string[] Texts = new string[]
            {
                        "wow ur strong",
                        "now u get to use the resonance altar!!",
                        "too bad its not released yet lol",
                        "u can have this dad joke instead",
                        "do you know what genre of music national anthems are?",
                        "...country music"
            };

            if (MiscCounter++ % 180 == 0 && GlobalCounter < 6)
            {
                if (GlobalCounter < 6)
                {
                    BossText(Texts[(int)GlobalCounter]);
                }

                GlobalCounter++;
            }

            if (GlobalCounter == 6 && MiscCounter > 1200)
            {
                if (MiscCounter == 1201)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EntrancePortal>(), 0, 0f, Main.myPlayer);
                }

                if (MiscCounter > 1340)
                {
                    npc.alpha = 255;

                    MiscCounter2--;

                    if (MiscCounter2 % 10 == 0 && MiscCounter2 > 20)
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
                                Projectile.NewProjectile(RandomPosition, Vector2.Zero, ModContent.ProjectileType<AbsorbEnergyCinematic>(), 0, 0f, Main.myPlayer, npc.whoAmI, 1);
                            }
                        }
                    }

                    if (MiscCounter2 == 20)
                    {
                        //Particle.CreateParticle(Particle.ParticleType<ReverseShockwave2>(), npc.Center, Vector2.Zero, Color.Lime, 1, 16f);
                    }
                }

                if (MiscCounter2 == 0)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 0);
                    CanDie = true;
                    npc.checkDead();
                }

            }
        }
    }
}
