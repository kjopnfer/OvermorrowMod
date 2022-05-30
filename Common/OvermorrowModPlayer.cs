using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Content.Projectiles.Accessory;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    partial class OvermorrowModPlayer : ModPlayer
    {
        // Accessories
        public bool ArmBracer;
        public bool ArtemisAmulet;
        public bool BloodyHeart;
        public bool BloodyTeeth;
        public bool DripplerEye;
        public bool EruditeDamage;
        public bool SerpentTooth;
        public bool ShatteredOrb;
        public bool StormScale;
        public bool StormShield;
        public bool TreeNecklace;
        public bool PredatorTalisman;
        public bool Bloodmana;

        // Set Bonuses
        public bool BMSet;
        public bool graniteSet;
        private int minionCounts;
        public bool MarbleTrail;
        public bool SkyArmor;


        // Shield Variables
        public int DashType;

        public static readonly int DashRight = 2;
        public static readonly int DashLeft = 3;

        // The direction the player is currently dashing towards.  Defaults to -1 if no dash is ocurring.
        public int DashDir = -1;

        //The fields related to the dash accessory
        public bool DashActive = false;
        public int DashDelay = MAX_DASH_DELAY;
        public int DashTimer = MAX_DASH_TIMER;

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public readonly float DashVelocity = 10f;
        // These two fields are the max values for the delay between dashes and the length of the dash in that order
        // The time is measured in frames
        public static readonly int MAX_DASH_DELAY = 50;
        public static readonly int MAX_DASH_TIMER = 35;

        // Accessory Counters
        public int dripplerStack;
        public int shieldCounter;
        public int sandCount;
        public int sandMode = 0;
        public bool launchSand = false;
        private int sparkCounter;
        private int treeCounter;
        private int treeDefenseStack;
        public bool MouseLampPlay;
        public float storedDamage = 0;
        public float amuletCounter;

        // Buffs
        public bool atomBuff;
        public bool explosionBuff;
        public bool graniteSpearBuff;
        public bool goldWind;
        public bool iorichGuardianShield;
        public bool lightningCloud;
        public bool mirrorBuff;
        public bool moonBuff;
        public bool slimeBuff;
        public bool smolBoi;
        public bool shroomBuff;
        public bool treeBuff;
        public bool vineBuff;
        // public bool windBuff;

        // Misc
        public int BowEnergyCount = 0;
        public int IorichGuardianEnergy = 0;
        public int PlatformTimer = 0;
        public int ScytheHitCount = 0;

        public Vector2 AltarCoordinates;
        public bool UIToggled = false;
        public bool StoleArtifact = false;

        public override void ResetEffects()
        {
            ArmBracer = false;
            ArtemisAmulet = false;
            BloodyHeart = false;
            BloodyTeeth = false;
            DripplerEye = false;
            EruditeDamage = false;
            SerpentTooth = false;
            ShatteredOrb = false;
            StormScale = false;
            StormShield = false;
            TreeNecklace = false;
            PredatorTalisman = false;
            Bloodmana = false;

            BMSet = false;
            graniteSet = false;
            MarbleTrail = false;
            SkyArmor = false;

            atomBuff = false;
            explosionBuff = false;
            graniteSpearBuff = false;
            goldWind = false;
            lightningCloud = false;
            mirrorBuff = false;
            moonBuff = false;
            slimeBuff = false;
            smolBoi = false;
            shroomBuff = false;
            treeBuff = false;
            vineBuff = false;
            // windBuff = false;
            MouseLampPlay = false;

            minionCounts = 0;

            bool dashAccessoryEquipped = false;

            //This is the loop used in vanilla to update/check the not-vanity accessories
            for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
            {
                Item item = Player.armor[i];

                //Set the flag for the ExampleDashAccessory being equipped if we have it equipped OR immediately return if any of the accessories are
                // one of the higher-priority ones
                if (item.type == ModContent.ItemType<StormShield>())
                    dashAccessoryEquipped = true;
                else if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
                    return;
            }

            //If we don't have the ExampleDashAccessory equipped or the player has the Solor armor set equipped, return immediately
            //Also return if the player is currently on a mount, since dashes on a mount look weird, or if the dash was already activated
            if (!dashAccessoryEquipped || Player.setSolar || Player.mount.Active || DashActive)
                return;

            //When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            //If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                return;  //No dash was activated, return

            DashActive = true;
            DashType = 0;
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (BMSet && item.DamageType == DamageClass.Melee)
            {
                Player.statMana += damage / 2;
                Player.ManaEffect(damage / 2);
            }

            if (BloodyTeeth)
            {
                int bleedChance = Main.rand.Next(4);
                if (bleedChance == 0 && item.DamageType == DamageClass.Melee)
                {
                    target.AddHex(Hex.HexType<Bleeding>(), 60 * 6);
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (BMSet && proj.DamageType == DamageClass.Melee)
            {
                Player.statMana += damage / 5;
                Player.ManaEffect(damage / 5);
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (BloodyHeart)
            {
                int projectiles = 3;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_OnHurt(null), Player.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<BouncingBlood>(), 19, 2, Player.whoAmI);
                    }
                }
                NPC.NewNPC(null, (int)Player.position.X, (int)Player.position.Y, ModContent.NPCType<BloodHeal>());
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {

            // creating cross on the npc
            if (storedDamage > 0)
            {
                Vector2 anchor = npc.Center;
                float angle;
                float gap; // gap between each particle

                for (int i = 0; i < 4; i++)
                {
                    angle = (i * 90 + 45) * (float)(Math.PI / 180);
                    gap = 20f;

                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 dustPos = anchor + j * (new Vector2(gap, 0).RotatedBy(angle));
                        Dust dust = Main.dust[Terraria.Dust.NewDust(dustPos, 15, 15, DustID.Electric, 0f, 0f, 0, default, 1.25f)];
                        dust.noGravity = true;
                    }
                }

                float hitDirection = (float)Math.Atan2(Player.Center.Y - npc.Center.Y, Player.Center.X - npc.Center.X);
                npc.StrikeNPC((int)storedDamage, 3f, (int)hitDirection);

                storedDamage = 0;
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            // creating cross on the projectile
            if (storedDamage > 0)
            {
                Vector2 anchor = proj.Center;
                float angle;
                float gap;

                for (int i = 0; i < 4; i++)
                {
                    angle = (i * 90 + 45) * (float)(Math.PI / 180);
                    gap = 20f;

                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 dustPos = anchor + j * (new Vector2(gap, 0).RotatedBy(angle));
                        Dust dust = Main.dust[Terraria.Dust.NewDust(dustPos, 15, 15, DustID.Electric, 0f, 0f, 0, default, 1.25f)];
                        dust.noGravity = true;
                    }
                }

                if (storedDamage > damage)
                {
                    proj.Kill();
                }

                storedDamage = 0;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (mirrorBuff)
            {
                damage /= 2;
            }
        }

        public override void PostUpdate()
        {
            if (Bloodmana)
            {
                if (Player.statMana < Player.statManaMax)
                {
                    int ManaDMG = Player.statManaMax - Player.statMana;

                    if (ManaDMG > 5)
                    {
                        Player.statLife = Player.statLife - ManaDMG / 2;
                        CombatText.NewText(Player.getRect(), Color.Red, ManaDMG / 2);
                    }
                    else
                    {
                        Player.statLife = Player.statLife - ManaDMG;
                        CombatText.NewText(Player.getRect(), Color.Red, ManaDMG);

                        if (Player.statLife < 0)
                        {
                            Player.Hurt(
                                PlayerDeathReason.ByCustomReason($"{Player.name} drank too deply from the blood mana ring."),
                                0,
                                0);
                        }
                    }


                    Player.statMana = Player.statManaMax;
                }
                if (Player.statMana > Player.statManaMax)
                {
                    Player.statMana = Player.statManaMax;
                }
            }
            if (amuletCounter > 0)
            {
                amuletCounter--;
            }
        }

        public override void PreUpdate()
        {
            PlatformTimer--;
        }


        public override void UpdateEquips()
        {
            if (DripplerEye)
            {
                Player.GetCritChance(DamageClass.Ranged) += dripplerStack;
            }

            if (StormScale) // Create sparks while moving, increase defense if health is below 50%
            {
                if (Player.statLife <= Player.statLifeMax2 * 0.5f)
                {
                    Player.statDefense += 5;
                }

                if (Player.velocity.X != 0 || Player.velocity.Y != 0)
                {
                    if (sparkCounter % 30 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                        {
                            for (int i = 0; i < Main.rand.Next(1, 3); i++)
                            {
                                Projectile.NewProjectile(Player.GetSource_Misc("OvermorrowStormScale"), Player.Center.X, Player.Center.Y + Main.rand.Next(-15, 18), 0, 0, ModContent.ProjectileType<ElectricSparksFriendly>(), 20, 1, Player.whoAmI, 0, 0);
                            }
                        }
                    }
                    sparkCounter++;
                }
            }

            if (TreeNecklace)
            {
                Lighting.AddLight(Player.Center, 0f, 1.5f, 0f);

                // The player is standing still
                if (Player.velocity == Vector2.Zero)
                {
                    treeCounter++;
                    if (treeCounter % 60 == 0 && treeDefenseStack <= 15)
                    {
                        treeDefenseStack++;
                    }
                }
                else // Reset the counter
                {
                    treeCounter = 0;
                    treeDefenseStack = 0;
                }

                Player.statDefense += treeDefenseStack;
            }


            if (graniteSpearBuff)
            {
                var damage = Player.GetDamage(DamageClass.Summon) += 0.1f;
            }

            /* Unused?
             * if (windBuff)
            {
                if (Player.HeldItem.DamageType == DamageClass.Ranged)
                {
                    Player.moveSpeed += 0.85f;
                    TrailTimer++;
                    if (TrailTimer > 3 && Player.velocity.X > 0 || TrailTimer > 3 && Player.velocity.X < 0 || TrailTimer > 3 && Player.velocity.Y < 0 || TrailTimer > 3 && Player.velocity.Y > 0)
                    {
                        Projectile.NewProjectile(Player.GetProjectileSource_Buff(Player.FindBuffIndex(ModContent.BuffType<WindBuff>()), Player.Center.X, Player.Center.Y, Player.velocity.X, Player.velocity.Y, ModContent.ProjectileType<PlayerMoveWave>(), 15, 0, Main.myPlayer);
                        TrailTimer = 0;
                    }
                }
            } */

            if (slimeBuff)
            {
                Player.jumpSpeedBoost += 3f;
            }

            if (goldWind)
            {
                Player.moveSpeed += 1f;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (treeBuff)
            {
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 2 life gained per second.
                Player.lifeRegen += 4;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Player.pulley && DashType > 0)
            {
                DashMovement();
            }
        }

        public override void PostUpdateEquips()
        {
            if (Player.mount.Active || Player.mount.Cart)
            {
                Player.dashDelay = 10;
                DashType = 0;
            }

            // Armor Sets
            if (graniteSet)
            {
                // When minions are despawned, the count does not reset
                //minionCounts = 0;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].minion && Main.projectile[i].active)
                    {
                        minionCounts++;
                    }
                }

                Player.statDefense += 1 * minionCounts;
                var damage = Player.GetDamage(DamageClass.Melee) += 0.03f * minionCounts;
            }
        }

        private bool IsInRange(Vector2 coordinates)
        {
            float distance = Vector2.Distance(coordinates, Player.Center);
            if (distance <= 80)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int slimeCounter = 0;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (slimeBuff && slimeCounter > 0)
            {
                slimeCounter--;
            }

            if (Player.justJumped && slimeBuff && slimeCounter == 0)
            {
                Vector2 vector4 = new Vector2(Player.position.X + (float)Player.width * 0.5f, Player.position.Y + (float)Player.height * 0.5f);

                for (int j = 0; j < 5; j++)
                {
                    Vector2 vector5 = new Vector2(j - 2, -4f);
                    vector5.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                    vector5.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                    vector5.Normalize();
                    vector5 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                    int proj = Projectile.NewProjectile(null, vector4.X, vector4.Y, vector5.X, vector5.Y, ProjectileID.SpikedSlimeSpike, 16, 0f, Main.myPlayer);

                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                }

                Player.statLife += 2;
                Player.HealEffect(2);
                slimeCounter = 45;
            }

            if (OvermorrowModFile.SandModeKey.JustPressed && ArmBracer)
            {
                if (sandMode == 0) // Defense
                {
                    sandMode = 1;
                    Main.NewText("Swapped to Attack Mode", Color.Yellow);
                }
                else // Attack
                {
                    sandMode = 0;
                    Main.NewText("Swapped to Defense Mode", Color.Yellow);
                }
            }

            if (OvermorrowModFile.AmuletKey.JustPressed && ArtemisAmulet && amuletCounter == 0)
            {
                Vector2 position = Main.MouseWorld;

                Projectile.NewProjectile(null, position, new Vector2(0), ModContent.ProjectileType<ArtemisRune>(), 0, 5f, Main.myPlayer);

                amuletCounter = 900;
            }

            if (OvermorrowModFile.ToggleUI.JustPressed)
            {
                Main.NewText("a");
                //ModContent.GetInstance<OvermorrowModFile>().ShowAltar();
            }

            if (UIToggled && IsInRange(AltarCoordinates))
            {
                ModContent.GetInstance<OvermorrowModSystem>().ShowAltar();
            }
            /*else
            {
                ModContent.GetInstance<OvermorrowModFile>().HideAltar();
            }*/
        }

        public void DashMovement()
        {
            int cShoe = 0;
            if (DashType == 1 && Player.eocDash > 0)
            {
                if (Player.eocHit < 0)
                {
                    Rectangle dashHitbox = new Rectangle((int)((double)Player.position.X + (double)Player.velocity.X * 0.5 - 4.0), (int)((double)Player.position.Y + (double)Player.velocity.Y * 0.5 - 4.0), Player.width + 8, Player.height + 8);
                    for (int i = 0; i < 200; i++)
                    {
                        if (!(Main.npc[i]).active || Main.npc[i].dontTakeDamage || Main.npc[i].friendly || Main.npc[i].immune[(Player).whoAmI] > 0)
                        {
                            continue;
                        }

                        NPC npc = Main.npc[i];
                        Rectangle npcHitbox = npc.getRect();
                        if (dashHitbox.Intersects(npcHitbox) && (npc.noTileCollide || Player.CanHit(npc)))
                        {
                            float damage = Player.GetTotalDamage(DamageClass.Melee).ApplyTo(36f);
                            float knockback = 9f;
                            bool crit = false;

                            if (Player.kbGlove)
                            {
                                knockback *= 2f;
                            }

                            if (Player.kbBuff)
                            {
                                knockback *= 1.5f;
                            }

                            if (Main.rand.Next(100) < Player.GetCritChance(DamageClass.Melee))
                            {
                                crit = true;
                            }

                            int direction = Player.direction;

                            if (Player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (Player.velocity.X > 0f)
                            {
                                direction = 1;
                            }

                            if (Player.whoAmI == Main.myPlayer)
                            {
                                Player.ApplyDamageToNPC(npc, (int)damage, knockback, direction, crit);
                            }

                            Player.eocDash = 10;
                            Player.dashDelay = 30;
                            Player.velocity.X = (0f - (float)direction) * 9f;
                            Player.velocity.Y = -4f;
                            Player.immune = true;
                            Player.immuneNoBlink = true;
                            Player.immuneTime = 4;
                            Player.eocHit = i;
                        }
                    }
                }
                else if ((!Player.controlLeft || Player.velocity.X >= 0f) && (!Player.controlRight || Player.velocity.X <= 0f))
                {
                    Player.velocity.X *= 0.95f;
                }
            }

            if (Player.dashDelay > 0)
            {
                if (Player.eocDash > 0)
                {
                    Player.eocDash--;
                }
                if (Player.eocDash == 0)
                {
                    Player.eocHit = -1;
                }
                Player.dashDelay--;
            }
            else if (Player.dashDelay < 0)
            {
                float num47 = 12f;
                float num46 = 0.992f;
                float num45 = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
                float num44 = 0.96f;
                int num43 = 20;
                if (Player.dash == 1)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        int num42 = (Player.velocity.Y != 0f) ? Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + (float)(Player.height / 2) - 8f), Player.width, 16, DustID.Smoke, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + (float)Player.height - 4f), Player.width, 8, DustID.Smoke, 0f, 0f, 100, default(Color), 1.4f);
                        Main.dust[num42].velocity *= 0.1f;
                        Main.dust[num42].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num42].shader = GameShaders.Armor.GetSecondaryShader(cShoe, Player);
                    }
                }

                if (Player.dash <= 0)
                {
                    return;
                }

                if (Player.velocity.X > num47 || Player.velocity.X < 0f - num47)
                {
                    Player.velocity.X *= num46;
                    return;
                }
                if (Player.velocity.X > num45 || Player.velocity.X < 0f - num45)
                {
                    Player.velocity.X *= num44;
                    return;
                }
                Player.dashDelay = num43;
                if (Player.velocity.X < 0f)
                {
                    Player.velocity.X = 0f - num45;
                }
                else if (Player.velocity.X > 0f)
                {
                    Player.velocity.X = num45;
                }
            }
            else
            {
                if (Player.dash <= 0 || Player.mount.Active)
                {
                    return;
                }
                if (Player.dash == 1)
                {
                    int num36 = 0;
                    bool flag5 = false;
                    if (Player.dashTime > 0)
                    {
                        Player.dashTime--;
                    }
                    if (Player.dashTime < 0)
                    {
                        Player.dashTime++;
                    }
                    if (Player.controlRight && Player.releaseRight)
                    {
                        if (Player.dashTime > 0)
                        {
                            num36 = 1;
                            flag5 = true;
                            Player.dashTime = 0;
                        }
                        else
                        {
                            Player.dashTime = 15;
                        }
                    }
                    else if (Player.controlLeft && Player.releaseLeft)
                    {
                        if (Player.dashTime < 0)
                        {
                            num36 = -1;
                            flag5 = true;
                            Player.dashTime = 0;
                        }
                        else
                        {
                            Player.dashTime = -15;
                        }
                    }
                    if (flag5)
                    {
                        Player.velocity.X = 16.9f * (float)num36;
                        Point point11 = (Player.Center + new Vector2(num36 * Player.width / 2 + 2, Player.gravDir * (0f - (float)Player.height) / 2f + Player.gravDir * 2f)).ToTileCoordinates();
                        Point point10 = (Player.Center + new Vector2(num36 * Player.width / 2 + 2, 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point11.X, point11.Y) || WorldGen.SolidOrSlopedTile(point10.X, point10.Y))
                        {
                            Player.velocity.X /= 2f;
                        }
                        Player.dashDelay = -1;
                        for (int num35 = 0; num35 < 20; num35++)
                        {
                            int num31 = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
                            Dust expr_CDB_cp_0 = Main.dust[num31];
                            expr_CDB_cp_0.position.X = expr_CDB_cp_0.position.X + (float)Main.rand.Next(-5, 6);
                            Dust expr_D02_cp_0 = Main.dust[num31];
                            expr_D02_cp_0.position.Y = expr_D02_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num31].velocity *= 0.2f;
                            Main.dust[num31].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num31].shader = GameShaders.Armor.GetSecondaryShader(cShoe, Player);
                        }
                        var source = Player.GetSource_Misc("OvermorrowDash");

                        int num33 = Gore.NewGore(source, new Vector2(Player.position.X + (float)(Player.width / 2) - 24f, Player.position.Y + (float)(Player.height / 2) - 34f), default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[num33].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity *= 0.4f;
                        num33 = Gore.NewGore(source, new Vector2(Player.position.X + (float)(Player.width / 2) - 24f, Player.position.Y + (float)(Player.height / 2) - 14f), default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[num33].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity *= 0.4f;
                    }
                }/*
                else if (dash == 2)
                {
                    int num30 = 0;
                    bool flag4 = false;
                    if (dashTime > 0)
                    {
                        dashTime--;
                    }
                    if (dashTime < 0)
                    {
                        dashTime++;
                    }
                    if (controlRight && releaseRight)
                    {
                        if (dashTime > 0)
                        {
                            num30 = 1;
                            flag4 = true;
                            dashTime = 0;
                        }
                        else
                        {
                            dashTime = 15;
                        }
                    }
                    else if (controlLeft && releaseLeft)
                    {
                        if (dashTime < 0)
                        {
                            num30 = -1;
                            flag4 = true;
                            dashTime = 0;
                        }
                        else
                        {
                            dashTime = -15;
                        }
                    }
                    if (flag4)
                    {
                        velocity.X = 14.5f * (float)num30;
                        Point point9 = (projectile.Center + new Vector2(num30 * width / 2 + 2, gravDir * (0f - (float)height) / 2f + gravDir * 2f)).ToTileCoordinates();
                        Point point8 = (projectile.Center + new Vector2(num30 * width / 2 + 2, 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point9.X, point9.Y) || WorldGen.SolidOrSlopedTile(point8.X, point8.Y))
                        {
                            velocity.X /= 2f;
                        }
                        dashDelay = -1;
                        eocDash = 15;
                        for (int num29 = 0; num29 < 0; num29++)
                        {
                            int num28 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 31, 0f, 0f, 100, default(Color), 2f);
                            Dust expr_1106_cp_0 = Main.dust[num28];
                            expr_1106_cp_0.position.X = expr_1106_cp_0.position.X + (float)Main.rand.Next(-5, 6);
                            Dust expr_112D_cp_0 = Main.dust[num28];
                            expr_112D_cp_0.position.Y = expr_112D_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num28].velocity *= 0.2f;
                            Main.dust[num28].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num28].shader = GameShaders.Armor.GetSecondaryShader(cShield, this);
                        }
                    }
                }*/
            }
        }
    }
}
