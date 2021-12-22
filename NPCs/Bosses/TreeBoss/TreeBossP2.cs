using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    [AutoloadBossHead]
    public partial class TreeBossP2 : ModNPC
    {
        public Vector2 InitialPosition;

        public bool PortalLaunched;
        public int PortalRuns = 0;

        public enum PortalAttacks { Charge = 1, Scythes = 2 }
        public int ChosenPortal;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, Scythe of the Dryads");
            Main.npcFrameCount[npc.type] = 6;
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 368;
            npc.height = 338;
            npc.damage = 20;
            npc.defense = 14;
            npc.lifeMax = 3300;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.Item25;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TreeBoss");
            bossBag = ModContent.ItemType<TreeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale * 0.65f);
            npc.defense = 17;
        }

        public enum AIStates
        {
            Intro = -1,
            Selector = 0,
            Teleport = 1,
            Scythes = 2,
            Spirit = 3,
            Runes = 4,
            Energy = 5,
            Death = 6
        }

        public ref float AICase => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float MiscCounter => ref npc.ai[2];
        public ref float MiscCounter2 => ref npc.ai[3];

        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Green);
            }
        }

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
            // Also get the player's position during this run so it doesn't get constantly offset while they move

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

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

            if (npc.life > npc.lifeMax)
            {
                npc.life = npc.lifeMax;
            }

            switch (AICase)
            {
                case (int)AIStates.Selector:
                    npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 3, 0.05f);
                    npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (player.Center.Y > npc.Center.Y ? 5 : -5), 0.02f);

                    if (MiscCounter++ == 60)
                    {
                        AICase = (int)AIStates.Teleport;
                        MiscCounter = 0;
                        MiscCounter2 = 120;

                        ChosenPortal = Main.rand.Next(1, 3);
                    }
                    break;
                case (int)AIStates.Teleport:
                    // Spawn an entrance portal behind the boss
                    if (MiscCounter++ == 0)
                    {
                        npc.velocity = Vector2.Zero;
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EntrancePortal>(), 0, 0f, Main.myPlayer);
                    }

                    if (MiscCounter2 > 0 && MiscCounter > 320)
                    {
                        npc.alpha = 255;
                        MiscCounter2--;

                        // Spawn the exit portal near the player
                        if (MiscCounter2 == 0)
                        {
                            npc.dontTakeDamage = true;
                            npc.Center = player.Center - Vector2.UnitY * 6000f;

                            
                            if (ChosenPortal == (int)PortalAttacks.Scythes)
                            {
                                Projectile.NewProjectile(player.Center - Vector2.UnitY * 500, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, player.whoAmI, npc.whoAmI);
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
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX * 20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX * -20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                            }
                        }

                        if (GlobalCounter++ == 50)
                        {
                            PortalLaunched = false;
                            npc.velocity = Vector2.Zero;

                            if (PortalRuns < 3)
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

                    break;
                case (int)AIStates.Scythes:
                    if (MiscCounter++ == 0)
                    {
                        npc.velocity = Vector2.Zero;
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EntrancePortal>(), 0, 0f, Main.myPlayer);
                    }

                    if (MiscCounter2 > 0 && MiscCounter > 320)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.UnitX * 20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center, Vector2.UnitX * -20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                    }


                    /*if (MiscCounter == 120)
                    {
                        Projectile.NewProjectile(player.Center - Vector2.UnitY * 500, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, player.whoAmI, npc.whoAmI);
                    }

                    if (MiscCounter2 > 0 && MiscCounter > 320)
                    {
                        npc.alpha = 255;
                        MiscCounter2--;

                        // Spawn the exit portal near the player
                        if (MiscCounter2 == 0)
                        {
                            npc.dontTakeDamage = true;
                            npc.Center = player.Center - Vector2.UnitY * 6000f;

                            Vector2 RandomCenter = player.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 400;
                            npc.netUpdate = true;

                            Projectile.NewProjectile(RandomCenter, Vector2.Zero, ModContent.ProjectileType<ExitPortal>(), 0, 0f, Main.myPlayer, -1, npc.whoAmI);
                            int tracking = Projectile.NewProjectile(RandomCenter, Vector2.Zero, ModContent.ProjectileType<TrackingWarning>(), 0, 1f, Main.myPlayer, player.whoAmI, 1);
                            ((TrackingWarning)Main.projectile[tracking].modProjectile).ParentNPC = npc;
                        }
                    }


                    if (MiscCounter > 350)
                    {
                        if (MiscCounter2++ % 10 == 0)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX * 20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX * -20, ModContent.ProjectileType<NatureScythe>(), npc.damage, 0, Main.myPlayer);
                        }
                    }

                    if (MiscCounter == 400)
                    {
                        npc.velocity = Vector2.Zero;

                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        PortalRuns = 0;
                    }*/
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.rotation = npc.velocity.X * 0.015f;

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
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeBossP2_Trail");

            if ((AICase == (int)AIStates.Teleport && PortalLaunched) || (AICase == (int)AIStates.Scythes && PortalLaunched))
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.Green : Color.LightGreen;
                    Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            // Teleportation animation
            if (AICase == (int)AIStates.Teleport || AICase == (int)AIStates.Scythes)
            {
                Vector2 vector59 = npc.Center + new Vector2(0, 10) - Main.screenPosition;
                Rectangle frame8 = npc.frame;
                //Vector2 origin21 = new Vector2(70f, 127f);
                Vector2 origin21 = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                //origin21.Y += 8f;
                Vector2 scale3 = new Vector2(npc.scale);
                float num219 = MiscCounter2;
                if (num219 < 120f)
                {
                    scale3 *= num219 / 120f;
                }

                Color alpha13 = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));

                // This controls the speed that they rotate in as well as how it shifts forward
                float lerpValue2 = ModUtils.GetLerpValue(0f, 120f, num219, clamped: true);

                // This controls the initial distance away from the body before it converges to the center
                float num220 = MathHelper.Lerp(32f/*32f*/, 0f, lerpValue2);

                Color color = alpha13;

                if (num219 >= 120f)
                {
                    color = alpha13;
                }

                // Draw the NPC with the silhouette after it has rotated into existence
                if (MiscCounter2 > 120 && MiscCounter2 < 180)
                {
                    spriteBatch.Draw(texture, vector59, frame8, new Color(149, 252, 173) * 0.75f, npc.rotation, origin21, scale3, SpriteEffects.None, 0f);
                }

                // AI counter
                if (num219 < 120f)
                {
                    float num229 = (float)Math.PI * 2f * lerpValue2 * (float)Math.Pow(lerpValue2, 2.0) * 2f + lerpValue2;

                    // This controls the number of afterimage frames the spiral around
                    float num230 = 6f/*3f*/;
                    for (float num231 = 0f; num231 < num230; num231 += 1f)
                    {
                        spriteBatch.Draw(texture, vector59 + (num229 + (float)Math.PI * 2f / num230 * num231).ToRotationVector2() * num220, frame8, Color.Lerp(new Color(204, 252, 149), new Color(149, 252, 173), num231 / num230) * 0.15f /*color * 0.5f*/, npc.rotation, origin21, scale3, SpriteEffects.None, 0f);
                    }
                }
            }

            /*
                Vector2 origin = npc.Center + new Vector2(0, 10) - Main.screenPosition;
                Rectangle frame = npc.frame;
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                Vector2 scale = new Vector2(npc.scale);

                if (MiscCounter2 < 120f)
                {
                    scale *= MiscCounter2 / 240f + 0.5f;
                }

                // This controls the speed that they rotate in as well as how it shifts forward
                float lerpValue2 = ModUtils.GetLerpValue(0f, 120f, MiscCounter2, clamped: true);

                // This controls the initial distance away from the body before it converges to the center
                float distance = MathHelper.Lerp(32f, 0f, lerpValue2);

                // Draw the NPC with the silhouette after it has rotated into existence
                if (MiscCounter2 > 120 && MiscCounter2 < 180)
                {
                    spriteBatch.Draw(texture, origin, frame, new Color(149, 252, 173) * 0.75f, npc.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
                }

                // AI counter
                if (MiscCounter2 < 120f)
                {
                    float num229 = (float)Math.PI * 2f * lerpValue2 * (float)Math.Pow(lerpValue2, 2.0) * 2f + lerpValue2;

                    // This controls the number of afterimage frames the spiral around
                    float numFrames = 6f;
                    for (float num231 = 0f; num231 < numFrames; num231 += 1f)
                    {
                        spriteBatch.Draw(texture, origin + (num229 + (float)Math.PI * 2f / numFrames * num231).ToRotationVector2() * distance, frame, Color.Lerp(new Color(204, 252, 149), new Color(149, 252, 173), num231 / numFrames) * 0.15f, npc.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
                    }
                }
             */
            return base.PreDraw(spriteBatch, drawColor);
        }

        public override bool CheckActive()
        {
            if (AICase == (int)AIStates.Teleport)
            {
                return true;
            }

            return base.CheckActive();
        }

        public override bool CheckDead()
        {
            if (npc.ai[3] == 0f)
            {
                npc.ai[2] = OvermorrowWorld.downedTree ? 0 : 540;
                npc.ai[3] = 1f;
                npc.damage = 0;
                npc.life = npc.lifeMax;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedTree = true;

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
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<EarthCrystal>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichStaff>());
                }
                if (choice == 2) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichHarvester>());
                }
                else if (choice == 3) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichBow>());
                }
                else if (choice == 4) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichWand>());
                }

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TreeTrophy>());
                }

                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SapStone>(), Main.rand.Next(1, 4));
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}