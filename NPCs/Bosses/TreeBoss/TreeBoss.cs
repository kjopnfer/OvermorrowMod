using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Consumable.Boss.TreeRune;
using OvermorrowMod.Particles;
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
    public partial class TreeBoss : ModNPC
    {
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
        public int ChosenSpiritAttack;
        public int PreviousSpirit = -1;
        public int RotationDirection;
        public float RotationOffset;

        public int AbsorbedEnergies;
        public int EnergyCount;
        public int ENERGY_THRESHOLD = 17;

        // Keeps track of attacks other than the rune attack
        // This is so we don't keep spamming the healing attack
        public int RuneCounter;
        public int MINIMUM_ATTACKS = 3;
        public int ChosenAttack;

        public bool RunAgain = false;
        public bool CanDie = false;
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
            npc.alpha = 255;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Silence");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale * 0.75f);
            npc.defense = 17;
        }

        public enum AIStates
        {
            Buffer = -2,
            Intro = -1,
            Selector = 0,
            Thorns = 1,
            Spirit = 2,
            Runes = 3,
            Energy = 4,
            Death = 5
        }

        public ref float AICase => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float MiscCounter => ref npc.ai[2];
        public ref float MiscCounter2 => ref npc.ai[3];
        public ref float VFXCounter => ref npc.localAI[1];

        public override void AI()
        {
            npc.velocity.Y += 40;
            npc.alpha = AICase == (int)AIStates.Intro ? 255 : 0;

            GlobalCounter++;
            VFXCounter++;

            npc.TargetClosest();
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

            // This is here because the boss heals
            if (npc.life >= npc.lifeMax) npc.life = npc.lifeMax;

            switch (AICase)
            {
                case (int)AIStates.Buffer: // Does literally nothing for 90 seconds
                    Buffer();
                    break;
                case (int)AIStates.Intro:
                    Intro();
                    break;
                case (int)AIStates.Selector: // Attack Selector
                    Selector();
                    break;
                case (int)AIStates.Thorns: // Spawn Thorns
                    ThornAttack(player);
                    break;
                case (int)AIStates.Spirit: // Spirit Attack
                    SpiritAttack(player);
                    break;
                case (int)AIStates.Runes: // Absorb Energy
                    RuneAttack();
                    break;
                case (int)AIStates.Energy: // Meteor Attack
                    EnergyAttack();
                    break;
                case (int)AIStates.Death:
                    Death();
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeBoss_Pulse");

            #region Spawn Drawcode
            if (MiscCounter > 60 && AICase == (int)AIStates.Intro)
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
                    scale3 *= num219 / 240f + 0.5f;
                }

                Color alpha13 = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));

                // This controls the speed that they rotate in as well as how it shifts forward
                float lerpValue2 = GetLerpValue(0f, 120f, num219, clamped: true);

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
            #endregion

            #region Pulse Drawcode
            if (AbsorbedEnergies > ENERGY_THRESHOLD)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);

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

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            // Draw the glowmask after the spawn animation has been completed
            if (AICase != (int)AIStates.Intro)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/TreeBoss_Glow");
                spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

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
                    xOffset = -1;
                    yOffset = -62 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, Color.Yellow);

                    break;
                case (int)AIStates.Spirit: // Right
                    xOffset = 12;
                    yOffset = -62 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, new Color(0, 255, 191));

                    break;
                case (int)AIStates.Runes: // Middle
                    xOffset = 5;
                    yOffset = -69 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);

                    break;
            }

            // Makes all three eyes glow during the spawning phase
            if (MiscCounter2 > 120 && MiscCounter2 < 180 && AICase == (int)AIStates.Intro)
            {
                xOffset = -1;
                yOffset = -62 + frameOffset;

                EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);

                xOffset = 12;
                yOffset = -62 + frameOffset;

                EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);

                xOffset = 5;
                yOffset = -69 + frameOffset;

                EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);
            }

            if (AICase == (int)AIStates.Death && MiscCounter > 60)
            {
                if (MiscCounter2 > 60)
                {
                    xOffset = -1;
                    yOffset = -62 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);
                }

                if (MiscCounter2 > 120)
                {

                    xOffset = 12;
                    yOffset = -62 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);

                }

                if (MiscCounter2 > 180)
                {
                    xOffset = 5;
                    yOffset = -69 + frameOffset;

                    EyeFlare(spriteBatch, xOffset, yOffset, Color.White, true);
                }
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
            if (!CanDie)
            {
                AICase = (int)AIStates.Death;
                GlobalCounter = 0;
                MiscCounter = 0;
                MiscCounter2 = 0;

                npc.damage = 0;
                npc.life = npc.lifeMax;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;

                ChosenAttack = -1;

                return false;
            }

            return true;
        }

        public override void NPCLoot()
        {
            // Spawn 2nd Phase
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<TreeBossP2>(), 0, 0f, 0f, 0f, 0f);

            if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
            {
                Main.NewText("Iorich has uprooted!", new Color(175, 75, 255));
            }
            else if (Main.netMode == NetmodeID.Server) // Server
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Iorich has uprooted!"), new Color(175, 75, 255));
            }
        }
    }
}