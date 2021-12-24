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
using System.Collections.Generic;
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
        public int ChosenSpiritAttack;
        public int RotationDirection;
        public float RotationOffset;

        public bool RunAgain = true;

        public int AbsorbedEnergies;
        public int EnergyCount;
        public int ENERGY_THRESHOLD = 12;

        public Vector2 FlyDistance;
        public bool MeteorLanded;
        public int RepeatMeteors = 0;

        public int RuneCounter;
        public int MINIMUM_ATTACKS = 3;
        public int ChosenAttack;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AICase != (int)AIStates.Energy && AICase != (int)AIStates.Runes;

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
            Spirit = 2,
            Runes = 3,
            Energy = 4,
            Death = 5
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

            npc.localAI[1]++;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

            // Handles Despawning
            if (AICase != (int)AIStates.Teleport && AICase != (int)AIStates.Energy)
            {
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

                    int[] Attacks = new int[] { (int)AIStates.Teleport, (int)AIStates.Spirit, (int)AIStates.Runes };

                    if (MiscCounter++ == 120)
                    {
                        Main.PlaySound(SoundID.Item4, npc.Center);

                        // Chooses the attack from the list
                        ChosenAttack = Attacks[Main.rand.Next(Attacks.Length)];

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

                    if (MiscCounter % 100 == 0)
                    {
                        int ShootSpeed = Main.rand.Next(8, 12);
                        Vector2 PlayerDistance = player.Center - npc.Center;
                        PlayerDistance.Normalize();

                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, PlayerDistance * ShootSpeed, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (MiscCounter == 600)
                    {
                        // If the condition was satisfied and you DID choose the rune attack, now reset the counter
                        if (ChosenAttack == (int)AIStates.Runes)
                        {
                            RuneCounter = 0;
                        }

                        AICase = ChosenAttack;
                        MiscCounter = 0;
                        MiscCounter2 = 0;

                        if (AICase == (int)AIStates.Teleport)
                        {
                            MiscCounter2 = 120;
                            ChosenPortal = Main.rand.Next(1, 3);
                        }

                        // Keep the eye visual for the runes attack, otherwise turn it off
                        if (ChosenAttack != (int)AIStates.Runes)
                        {
                            ChosenAttack = 0;
                        }
                    }
                    break;
                case (int)AIStates.Teleport:
                    Teleport_Attacks(player);
                    break;
                case (int)AIStates.Spirit:
                    Spirit(player);
                    break;
                case (int)AIStates.Runes:
                    RuneAttack();
                    break;
                case (int)AIStates.Energy:
                    EnergyAttack(player);
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

            #region Teleportation Drawcode
            if (AICase == (int)AIStates.Teleport && PortalLaunched)
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
            if (AICase == (int)AIStates.Teleport)
            {
                Vector2 origin = npc.Center + new Vector2(0, 10) - Main.screenPosition;
                Rectangle frame = npc.frame;
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                Vector2 scale = new Vector2(npc.scale);
                if (MiscCounter2 < 120f)
                {
                    scale *= MiscCounter2 / 120f;
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
                        spriteBatch.Draw(texture, origin + (num229 + (float)Math.PI * 2f / numFrames * num231).ToRotationVector2() * distance, frame, Color.Lerp(new Color(204, 252, 149), new Color(149, 252, 173), num231 / numFrames) * 0.15f /*color * 0.5f*/, npc.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            #endregion

            #region Pulse Drawcode
            if (AbsorbedEnergies > ENERGY_THRESHOLD)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);

                // this gets the npc's frame
                int num178 = 60; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                if (AbsorbedEnergies > 36)
                {
                    num178 = 15;
                }
                else if (AbsorbedEnergies > 24)
                {
                    num178 = 30;
                }

                int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher
                if (AbsorbedEnergies > 36)
                {
                    num179 = 15;
                }
                else if (AbsorbedEnergies > 24)
                {
                    num179 = 30;
                }

                // default value
                int num177 = 6; // ok i think this controls the number of afterimage frames
                float num176 = 1f - (float)Math.Cos((npc.localAI[1] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num176 /= 3f;
                float scaleFactor = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num177; num164++)
                {
                    // these assign the color of the pulsing
                    Color spriteColor = Main.DiscoColor;

                    spriteColor = npc.GetAlpha(spriteColor);
                    spriteColor *= 1f - num176; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 position = npc.Center + Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + npc.rotation) * scaleFactor * num176 - Main.screenPosition;
                    position -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    position += drawOrigin * npc.scale + new Vector2(0f, 5 + npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture, position, npc.frame, spriteColor, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
            #endregion

            #region Rune Drawcode
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
            #endregion

            if (AICase == (int)AIStates.Energy)
            {
                spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 5), npc.frame, Main.DiscoColor, npc.rotation, npc.frame.Size() / 2f, MathHelper.Lerp(npc.scale, 0, Utils.Clamp(MiscCounter, 0, 30) / 30f), npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (AICase != (int)AIStates.Energy)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeBossP2_Glow");
                spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 5), npc.frame, Color.White * npc.Opacity, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            base.PostDraw(spriteBatch, drawColor);
        }

        public override bool CheckActive()
        {
            if (AICase == (int)AIStates.Teleport || AICase == (int)AIStates.Energy)
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