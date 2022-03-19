using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Armor.Masks;
using OvermorrowMod.Content.Items.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class DripplerBoss : ModNPC
    {
        private bool spawnedRotaters = false; // Makes it so that it cannot skip phase 2, even if 0 rotaters
        private bool changedPhase2 = false;
        private bool changedPhase3 = false;
        private int circleCooldown = 0;
        private int dripladCooldown = 0;
        private bool attackindicator = false;
        public bool turnonattackindicator = false;
        int lastchoice;
        int choice;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplord, the Bloody Assimilator");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.width = 320;
            npc.height = 482;
            npc.aiStyle = -1;
            npc.damage = 59;//21;
            npc.defense = 35;
            npc.lifeMax = 6500;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 2, silver: 50);
            npc.npcSlots = 10f;
            npc.chaseable = false;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DripplerBoss");
            bossBag = ModContent.ItemType<DripplerBag>();

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Frostburn] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Daybreak] = true;
            npc.buffImmune[BuffID.Electrified] = true;
            npc.buffImmune[BuffID.Webbed] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.Venom] = true;
            npc.buffImmune[BuffID.WeaponImbueConfetti] = true;
            npc.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
            npc.buffImmune[BuffID.WeaponImbueFire] = true;
            npc.buffImmune[BuffID.WeaponImbueGold] = true;
            npc.buffImmune[BuffID.WeaponImbueIchor] = true;
            npc.buffImmune[BuffID.WeaponImbueNanites] = true;
            npc.buffImmune[BuffID.WeaponImbuePoison] = true;
            npc.buffImmune[BuffID.WeaponImbueVenom] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 50;
        }

        public override void AI()
        {
            if (turnonattackindicator)
            {
                attackindicator = true;
                if (npc.ai[3]++ > /*180*/ 480 /*240*/)
                {
                    attackindicator = false;
                    turnonattackindicator = false;
                    npc.ai[3] = 0;
                }
            }

            if (npc.life <= 0)
            {
                npc.NPCLoot();
                Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
            }

            // Reinitialize variables if the previous checks didn't get them
            if (npc.ai[2] == 0)
            {
                OvermorrowWorld.dripPhase2 = false;
                OvermorrowWorld.dripPhase3 = false;
                OvermorrowWorld.DripladShoot = false;
                npc.ai[2]++;
            }

            Player player = Main.player[npc.target];

            // Check that it is a Blood Moon & that it is night time
            if (!Main.bloodMoon)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }

                OvermorrowWorld.dripPhase2 = false;
                OvermorrowWorld.dripPhase3 = false;
                OvermorrowWorld.DripladShoot = false;
            }

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


            // Inflicts 'Congealed Blood' debuff that slows down player movement

            // General MOVESET:
            // Float towards the player
            // Pauses and roars, summoning Looming Dripplers from offscreen

            // PHASE 2: (66%)
            // Chase the player
            // Summon 5 Driplads that orbit the boss

            // PHASE 3: (33%)
            // Initial switch: Spawn stationary Driplads
            // Repeat spawning Looming Dripplers
            // Shoot out Splitting BLood

            // Dripplers that die damage the boss

            // PHASE CHECKS
            // Check number of spawned NPCs before switching phase

            // Reset counters
            int countDripplers = 0;
            int countRotaters = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<LoomingDrippler>())
                {
                    countDripplers++;
                }

                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RotatingDriplad>())
                {
                    countRotaters++;
                }
            }

            // Sometimes health shenanigans happen and the rotaters don't get spawned
            if (countRotaters >= 1)
            {
                spawnedRotaters = true;
            }

            // AI[0] is the phase
            // AI[1] is the counter

            switch (npc.ai[0])
            {
                case 0: // Float towards the player
                    if (npc.ai[0] == 0)
                    {
                        // Break statements to stop movement and continue to phase initializers
                        if (/*!LocalPhaseTwo*/ !OvermorrowWorld.dripPhase2 && !spawnedRotaters && /*!randomrotatorshootistrue &&*/ countDripplers <= 0 && npc.life <= npc.lifeMax * 0.66f)
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;
                            break;
                        }
                        else if (/*LocalPhaseTwo*/ OvermorrowWorld.dripPhase2 && spawnedRotaters && countRotaters <= 0 /*&& randomrotatorshootistrue*/)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                            break;
                        }
                        if (!OvermorrowWorld.dripPhase2 || npc.Distance(player.Center) > 333)//750)
                        {
                            Vector2 moveTo = player.Center;
                            var move = moveTo - npc.Center;
                            var speed = 2;

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
                            npc.velocity = move;
                        }
                        if (countDripplers > 0 && !changedPhase3)
                        {
                            if (Main.rand.Next(100) == 0 && circleCooldown == 0 && !OvermorrowWorld.DripplerCircle)
                            {
                                OvermorrowWorld.DripplerCircle = true;
                            }
                            else
                            {
                                if (circleCooldown > 0)
                                {
                                    circleCooldown--;
                                }
                            }
                        }

                        if (OvermorrowWorld.DripplerCircle)
                        {
                            circleCooldown = 2400;
                        }

                        if (countRotaters > 0 && changedPhase2 /*&& Main.rand.Next(100) == 0*/ && dripladCooldown <= 0)
                        {
                            npc.ai[0] = 4;
                            npc.ai[1] = 0;
                        }

                        if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase3 && npc.ai[0] == 4)
                        {
                            dripladCooldown = 1600;
                            OvermorrowWorld.DripladShoot = false;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && npc.ai[0] == 4)
                        {
                            dripladCooldown = 400;
                            OvermorrowWorld.DripladShoot = false;
                        }
                        else if (dripladCooldown > 0)
                        {
                            dripladCooldown--;
                            OvermorrowWorld.DripladShoot = false;
                        }

                        if (changedPhase3)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode)
                            {
                                if (npc.ai[1] % 135 == 0)
                                {
                                    // I'm lazy
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -10f, 0f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 10f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 10f, 0f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, -10f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            // No dripplers present, spawn them in
                            if ((countDripplers <= 0 && countRotaters <= 0) || (countDripplers <= 0 && changedPhase3))
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                            }
                            else // Dripplers present, resume movement
                            {
                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                            }
                        }
                    }
                    break;
                case 1: // Phase 2 Initializer
                    npc.velocity = Vector2.Zero;

                    if (npc.ai[1] == 140)
                    {
                        // Wall of Flesh scream sound
                        Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                            if (distance <= 1200)
                            {
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                            }
                        }
                        /*
                        Vector2 origin = npc.Center; // Origin of the circle
                        float radius = 450; // Distance from the circle
                        int numSpawns = 5; // Points spawned on the circle
                        */
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<RotatingDriplad>(), 0, 60f * i, npc.whoAmI, 350);
                            }
                        }
                    }

                    if (npc.ai[1] == 300)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                        changedPhase2 = true;
                        OvermorrowWorld.dripPhase2 = true;
                        dripladCooldown = 400;
                    }
                    break;
                case 2: // Phase 3 Initializer
                    npc.velocity = Vector2.Zero;

                    if (npc.ai[0] == 2)
                    {

                        if (npc.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                                if (distance <= 1200)
                                {
                                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                                }
                            }

                            Vector2 origin = npc.Center; // Origin of the circle
                            float radius = 1050; // Distance from the circle
                            int numSpawns = 5; // Points spawned on the circle
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<RotatingDriplad>(), 0, 60f * i, npc.whoAmI, radius);
                                }
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            changedPhase3 = true;
                            OvermorrowWorld.dripPhase3 = true;
                            dripladCooldown = 1600;
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Summon Dripplers
                    if (npc.ai[0] == 3)
                    {
                        npc.velocity = Vector2.Zero;
                        if (npc.ai[1] == 89)
                        {
                            OvermorrowWorld.loomingdripplerdeadcount = 0;
                        }
                        if (npc.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                            //float rotation = MathHelper.ToRadians(360);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                                if (distance <= 1200)
                                {
                                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                                }
                            }

                            Vector2 origin = npc.Center; // Origin of the circle
                            float radius = 975; // Distance from the circle
                            int numSpawns = 12; // Points spawned on the circle
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                    // Pass in AI[0] for Dripplers
                                    Projectile.NewProjectile(position.X, position.Y, 0, 0, ModContent.ProjectileType<DripplerSpawner>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                                }
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 4: // Driplad actions
                    npc.velocity = Vector2.Zero;
                    OvermorrowWorld.DripladShoot = true;

                    if (npc.ai[1] == 1)
                    {
                        //OvermorrowWorld.DripladShoot = true;
                        while (choice == lastchoice)
                        {
                            choice = Main.rand.Next(3);
                        }
                        OvermorrowWorld.RotatingDripladAttackCounter = choice;
                        lastchoice = choice;
                    }

                    if (npc.ai[1] == /*300*/ 480)
                    {
                        //OvermorrowWorld.DripladShoot = false;
                        OvermorrowWorld.RotatingDripladAttackCounter = 0;
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                    }
                    break;
            }

            npc.ai[1]++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
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

            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Assets/Gores/DripplerBoss1"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Assets/Gores/DripplerBoss2"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Assets/Gores/DripplerBoss3"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Assets/Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Assets/Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 12) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (attackindicator == true)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                Vector2 origin = npc.frame.Size() / 2;
                int amount = 5;
                float scaleAmount = 1f;
                for (int i = 0; i < amount; i++)
                {
                    float progress = (float)i / (float)amount;
                    float scale = 1f + progress * scaleAmount;
                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White * (1f - progress), npc.rotation, origin, scale * npc.scale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("Content/NPCs/Bosses/DripplerBoss/DripplerBoss_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && Main.npc[k].type == mod.NPCType("LoomingDrippler"))
                {
                    ((LoomingDrippler)Main.npc[k].modNPC).run = true;
                }
                if (Main.npc[k].active && Main.npc[k].type == mod.NPCType("RotatingDriplad"))
                {
                    ((RotatingDriplad)Main.npc[k].modNPC).run = true;
                }
            }

            OvermorrowWorld.downedDrippler = true;
            OvermorrowWorld.dripPhase2 = false;
            OvermorrowWorld.dripPhase3 = false;
            OvermorrowWorld.DripladShoot = false;
            OvermorrowWorld.loomingdripplerdeadcount = 0;

            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss1"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss1"), npc.scale);
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss3"), npc.scale);
            }
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()), npc.scale);

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(4);
                if (choice == 0) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SinisterBlood>());
                }
                else if (choice == 1) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BloodyTeeth>());
                }
                else if (choice == 2) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DripplerEye>());
                }
                else if (choice == 3) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<ShatteredOrb>());
                }

                int necklaceChance = Main.rand.Next(5);
                if (necklaceChance == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SharkToothNecklace);
                }

                if (Main.rand.Next(7) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DripMask>());
                }

                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<CancerInABottle>(), Main.rand.Next(6, 10));

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DripplerTrophy>());
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}