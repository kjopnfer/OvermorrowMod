using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Armor.Masks;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

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
            Main.npcFrameCount[NPC.type] = 12;
        }

        public override void SetDefaults()
        {
            NPC.width = 320;
            NPC.height = 482;
            NPC.aiStyle = -1;
            NPC.damage = 59;//21;
            NPC.defense = 35;
            NPC.lifeMax = 6500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.DD2_BetsyDeath;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 2, silver: 50);
            NPC.npcSlots = 10f;
            NPC.chaseable = false;
            Music = SoundLoader.GetSoundSlot("Sounds/Music/DripplerBoss");

            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Frostburn] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.buffImmune[BuffID.Daybreak] = true;
            NPC.buffImmune[BuffID.Electrified] = true;
            NPC.buffImmune[BuffID.Webbed] = true;
            NPC.buffImmune[BuffID.Suffocation] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.buffImmune[BuffID.WeaponImbueConfetti] = true;
            NPC.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
            NPC.buffImmune[BuffID.WeaponImbueFire] = true;
            NPC.buffImmune[BuffID.WeaponImbueGold] = true;
            NPC.buffImmune[BuffID.WeaponImbueIchor] = true;
            NPC.buffImmune[BuffID.WeaponImbueNanites] = true;
            NPC.buffImmune[BuffID.WeaponImbuePoison] = true;
            NPC.buffImmune[BuffID.WeaponImbueVenom] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
            NPC.defense = 50;
        }

        public override void AI()
        {
            if (turnonattackindicator)
            {
                attackindicator = true;
                if (NPC.ai[3]++ > /*180*/ 480 /*240*/)
                {
                    attackindicator = false;
                    turnonattackindicator = false;
                    NPC.ai[3] = 0;
                }
            }

            if (NPC.life <= 0)
            {
                NPC.NPCLoot();
                SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
            }

            // Reinitialize variables if the previous checks didn't get them
            if (NPC.ai[2] == 0)
            {
                OvermorrowWorld.dripPhase2 = false;
                OvermorrowWorld.dripPhase3 = false;
                OvermorrowWorld.DripladShoot = false;
                NPC.ai[2]++;
            }

            Player player = Main.player[NPC.target];

            // Check that it is a Blood Moon & that it is night time
            if (!Main.bloodMoon)
            {
                NPC.TargetClosest(false);
                NPC.direction = 1;
                NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                if (NPC.timeLeft > 20)
                {
                    NPC.timeLeft = 20;
                    return;
                }

                OvermorrowWorld.dripPhase2 = false;
                OvermorrowWorld.dripPhase3 = false;
                OvermorrowWorld.DripladShoot = false;
            }

            // Handles Despawning
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest(false);
                NPC.direction = 1;
                NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                if (NPC.timeLeft > 20)
                {
                    NPC.timeLeft = 20;
                    return;
                }
            }

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                NPC.velocity.Y = -2000;
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

            switch (NPC.ai[0])
            {
                case 0: // Float towards the player
                    if (NPC.ai[0] == 0)
                    {
                        // Break statements to stop movement and continue to phase initializers
                        if (/*!LocalPhaseTwo*/ !OvermorrowWorld.dripPhase2 && !spawnedRotaters && /*!randomrotatorshootistrue &&*/ countDripplers <= 0 && NPC.life <= NPC.lifeMax * 0.66f)
                        {
                            NPC.ai[0] = 1;
                            NPC.ai[1] = 0;
                            break;
                        }
                        else if (/*LocalPhaseTwo*/ OvermorrowWorld.dripPhase2 && spawnedRotaters && countRotaters <= 0 /*&& randomrotatorshootistrue*/)
                        {
                            NPC.ai[0] = 2;
                            NPC.ai[1] = 0;
                            break;
                        }
                        if (!OvermorrowWorld.dripPhase2 || NPC.Distance(player.Center) > 333)//750)
                        {
                            Vector2 moveTo = player.Center;
                            var move = moveTo - NPC.Center;
                            var speed = 2;

                            float length = move.Length();
                            if (length > speed)
                            {
                                move *= speed / length;
                            }
                            var turnResistance = 45;
                            move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                            length = move.Length();
                            if (length > 10)
                            {
                                move *= speed / length;
                            }
                            NPC.velocity = move;
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
                            NPC.ai[0] = 4;
                            NPC.ai[1] = 0;
                        }

                        if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase3 && NPC.ai[0] == 4)
                        {
                            dripladCooldown = 1600;
                            OvermorrowWorld.DripladShoot = false;
                        }
                        else if (OvermorrowWorld.DripladShoot && OvermorrowWorld.dripPhase2 && NPC.ai[0] == 4)
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
                                if (NPC.ai[1] % 135 == 0)
                                {
                                    var source = NPC.GetSpawnSource_ForProjectile();
                                    // I'm lazy
                                    Projectile.NewProjectile(source, NPC.Center.X, NPC.Center.Y, -10f, 0f, ModContent.ProjectileType<SplittingBlood>(), NPC.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(source, NPC.Center.X, NPC.Center.Y, 0f, 10f, ModContent.ProjectileType<SplittingBlood>(), NPC.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(source, NPC.Center.X, NPC.Center.Y, 10f, 0f, ModContent.ProjectileType<SplittingBlood>(), NPC.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                    Projectile.NewProjectile(source, NPC.Center.X, NPC.Center.Y, 0f, -10f, ModContent.ProjectileType<SplittingBlood>(), NPC.damage / 3, 2f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }

                        if (NPC.ai[1] == 270)
                        {
                            // No dripplers present, spawn them in
                            if ((countDripplers <= 0 && countRotaters <= 0) || (countDripplers <= 0 && changedPhase3))
                            {
                                NPC.ai[0] = 3;
                                NPC.ai[1] = 0;
                            }
                            else // Dripplers present, resume movement
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                            }
                        }
                    }
                    break;
                case 1: // Phase 2 Initializer
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[1] == 140)
                    {
                        // Wall of Flesh scream sound
                        SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                            if (distance <= 1200)
                            {
                                //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                            }
                        }
                        /*
                        Vector2 origin = NPC.Center; // Origin of the circle
                        float radius = 450; // Distance from the circle
                        int numSpawns = 5; // Points spawned on the circle
                        */
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<RotatingDriplad>(), 0, 60f * i, NPC.whoAmI, 350);
                            }
                        }
                    }

                    if (NPC.ai[1] == 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        changedPhase2 = true;
                        OvermorrowWorld.dripPhase2 = true;
                        dripladCooldown = 400;
                    }
                    break;
                case 2: // Phase 3 Initializer
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[0] == 2)
                    {

                        if (NPC.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 1200)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                                }
                            }

                            Vector2 origin = NPC.Center; // Origin of the circle
                            float radius = 1050; // Distance from the circle
                            int numSpawns = 5; // Points spawned on the circle
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                    NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<RotatingDriplad>(), 0, 60f * i, NPC.whoAmI, radius);
                                }
                            }
                        }

                        if (NPC.ai[1] == 270)
                        {
                            changedPhase3 = true;
                            OvermorrowWorld.dripPhase3 = true;
                            dripladCooldown = 1600;
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Summon Dripplers
                    if (NPC.ai[0] == 3)
                    {
                        NPC.velocity = Vector2.Zero;
                        if (NPC.ai[1] == 89)
                        {
                            OvermorrowWorld.loomingdripplerdeadcount = 0;
                        }
                        if (NPC.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                            //float rotation = MathHelper.ToRadians(360);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 1200)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                                }
                            }

                            Vector2 origin = NPC.Center; // Origin of the circle
                            float radius = 975; // Distance from the circle
                            int numSpawns = 12; // Points spawned on the circle
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                    // Pass in AI[0] for Dripplers
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), position.X, position.Y, 0, 0, ModContent.ProjectileType<DripplerSpawner>(), 0, 0f, Main.myPlayer, 0, NPC.whoAmI);
                                }
                            }
                        }

                        if (NPC.ai[1] == 270)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 4: // Driplad actions
                    NPC.velocity = Vector2.Zero;
                    OvermorrowWorld.DripladShoot = true;

                    if (NPC.ai[1] == 1)
                    {
                        //OvermorrowWorld.DripladShoot = true;
                        while (choice == lastchoice)
                        {
                            choice = Main.rand.Next(3);
                        }
                        OvermorrowWorld.RotatingDripladAttackCounter = choice;
                        lastchoice = choice;
                    }

                    if (NPC.ai[1] == /*300*/ 480)
                    {
                        //OvermorrowWorld.DripladShoot = false;
                        OvermorrowWorld.RotatingDripladAttackCounter = 0;
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                    }
                    break;
            }

            NPC.ai[1]++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)NPC.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * (float)hitDirection, -2.5f);
            }

            Gore.NewGore(NPC.position, NPC.velocity, Find<ModGore>("Assets/Gores/DripplerBoss1").Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Find<ModGore>("Assets/Gores/DripplerBoss2").Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Find<ModGore>("Assets/Gores/DripplerBoss3").Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Find<ModGore>("Assets/Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Find<ModGore>("Assets/Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 6f == 5f)
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 12) // 6 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (attackindicator == true)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                Vector2 origin = NPC.frame.Size() / 2;
                int amount = 5;
                float scaleAmount = 1f;
                for (int i = 0; i < amount; i++)
                {
                    float progress = (float)i / (float)amount;
                    float scale = 1f + progress * scaleAmount;
                    spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White * (1f - progress), NPC.rotation, origin, scale * NPC.scale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/DripplerBoss/DripplerBoss_Glowmask").Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - screenPos.X, NPC.Center.Y - screenPos.Y + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void OnKill()
        {
            var loomingDripplerType = Mod.Find<ModNPC>("LoomingDrippler").Type;
            var rotatingDripladType = Mod.Find<ModNPC>("RotatingDriplad").Type;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && Main.npc[k].type == loomingDripplerType)
                {
                    ((LoomingDrippler)Main.npc[k].ModNPC).run = true;
                }
                if (Main.npc[k].active && Main.npc[k].type == rotatingDripladType)
                {
                    ((RotatingDriplad)Main.npc[k].ModNPC).run = true;
                }
            }
            OvermorrowWorld.downedDrippler = true;
            OvermorrowWorld.dripPhase2 = false;
            OvermorrowWorld.dripPhase3 = false;
            OvermorrowWorld.DripladShoot = false;
            OvermorrowWorld.loomingdripplerdeadcount = 0;

            Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss1").Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss1").Type, NPC.scale);
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss2").Type, NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss3").Type, NPC.scale);
            }
            Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
            Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DripplerBoss" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<DripplerBag>()));

            // If not expert mode, one off SinisterBlood, BloodyTeeth, DripplerEye and ShatteredOrb,
            // 1/5 chance of shark tooth neclace
            // 1/7 DripMask
            // 100% 6-10 CancerInABottle
            // 1/10 DripplerTrophy
            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert());
            nonExpert
                .OnSuccess(ItemDropRule.OneFromOptions(1,
                    ItemType<SinisterBlood>(),
                    ItemType<BloodyTeeth>(),
                    ItemType<DripplerEye>(),
                    ItemType<ShatteredOrb>()))
                .OnSuccess(ItemDropRule.Common(ItemID.SharkToothNecklace, 5))
                .OnSuccess(ItemDropRule.Common(ItemType<DripMask>(), 7))
                .OnSuccess(ItemDropRule.Common(ItemType<CancerInABottle>(), 1, 6, 10))
                .OnSuccess(ItemDropRule.Common(ItemType<DripplerTrophy>(), 10));

            npcLoot.Add(nonExpert);
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}