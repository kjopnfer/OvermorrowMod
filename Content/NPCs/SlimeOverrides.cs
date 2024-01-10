using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria.Audio;
using OvermorrowMod.Common.Particles;
using Terraria.ModLoader.IO;
using System.IO;
using OvermorrowMod.Common.VanillaOverrides;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.UI.SpeechBubble;

namespace OvermorrowMod.Content.NPCs
{
    public class SlimeOverrides : VanillaNPCOverride
    {
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return base.CanBeHitByItem(npc, player, item);
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => AIState != (int)AICase.Spawn;

        public override void SetDefaults(NPC npc)
        {
            // This doesn't do anything
        }

        int idleJumpDirection = 1;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    npc.scale = 0;
                    AIState = (int)AICase.Spawn;
                }
            }
            base.OnSpawn(npc, source);
        }

        public enum AICase
        {
            Spawn = -1,
            Idle = 0,
            PreJump = 1,
            Jump = 2,
            Land = 3,
            Swim = 4,
        }

        List<int> SlimeOverrideIDs = new List<int>()
        {
            NPCID.GreenSlime, NPCID.BlueSlime, NPCID.RedSlime, NPCID.PurpleSlime, NPCID.YellowSlime, NPCID.BlackSlime, NPCID.JungleSlime, NPCID.Pinky
        };

        public float AIState = 0;
        public float AICycles = 0;
        public float AICounter = 0;
        public float FrameCounter = 0;

        private bool oldCollision = true;
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                // This is so stupid why would Red do this
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    npc.velocity.X *= 0.98f;
                    float moveSpeed = 0.25f * idleJumpDirection;
                    Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
                    //Collision.StepDown(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY, 1, true);
                    //Main.NewText("collideX: " + npc.collideX + " collideY: " + npc.collideY + " fC: " + FrameCounter + " aC: " + AICounter);
                    if (npc.wet && AIState != (int)AICase.Swim)
                    {
                        AICounter = 0;
                        FrameCounter = 0;
                        AICycles = 0;
                        AIState = (int)AICase.Swim;
                    }

                    switch (AIState)
                    {
                        #region Spawn
                        case (int)AICase.Spawn:

                            FrameCounter++;
                            npc.scale = MathHelper.Lerp(0, 1f, AICounter / 60f);
                            Dust.NewDust(npc.BottomLeft, npc.width, 10, DustID.t_Slime, 0, Main.rand.NextFloat(-3f, -1f), 0, npc.color, npc.scale * 1.2f);
                            if (AICounter++ >= 60)
                            {
                                AIState = (int)AICase.Idle;
                                AICounter = 0;
                                FrameCounter = 0;
                                AICycles = 0;
                            }
                            break;
                        #endregion
                        #region Idle
                        case (int)AICase.Idle:
                            //Main.NewText("idle " + npc.collideY + ", velocity:" + npc.velocity.ToString());

                            if (npc.collideY)
                            {
                                if (AICounter == 1 && npc.velocity.X == 0)
                                {
                                    npc.velocity.X = moveSpeed;
                                    AICounter++;
                                }

                                if (!oldCollision)
                                {
                                    npc.velocity.X = moveSpeed;
                                }
                            }

                            if (FrameCounter++ >= 40)
                            {
                                // Set the NPC's direction towards the target periodically
                                if (target != null)
                                {
                                    idleJumpDirection = npc.Center.X > target.Center.X ? -1 : 1;
                                }

                                FrameCounter = 1;
                            }

                            if (AICounter++ >= 40)
                            {
                                if (npc.collideY) npc.velocity.X = moveSpeed;

                                AICounter = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0
                                //FrameCounter = 1;
                                if (AICycles++ >= 5)
                                {
                                    if (npc.collideY) npc.velocity.X = 0;

                                    AIState = (int)AICase.PreJump;
                                    AICounter = 0;
                                    FrameCounter = 0;
                                    AICycles = 0;
                                }
                            }
                            break;
                        #endregion
                        #region PreJump
                        case (int)AICase.PreJump:
                            int bounceRate = 1;
                            if (AICycles >= 4) bounceRate = 2;

                            //Main.NewText("prejump " + npc.collideY + " / " + ModUtils.CheckEntityBottomSlopeCollision(npc));

                            if (npc.collideY || npc.CheckEntityBottomSlopeCollision())
                            {
                                AICounter += bounceRate;
                            }

                            // Prevent the NPC from sliding after being hit into the air and landing
                            if (npc.collideY && npc.velocity.X != 0 && npc.oldVelocity.X != 0)
                            {
                                npc.velocity.X = 0;
                            }

                            FrameCounter += bounceRate;

                            if (AICounter >= 12)
                            {
                                AICounter = 0;
                                FrameCounter = 0;

                                if (AICycles++ >= 8)
                                {
                                    AIState = (int)AICase.Jump;
                                    AICycles = 0;
                                }
                            }
                            break;
                        #endregion
                        #region Jump
                        case (int)AICase.Jump:
                            if (target != null) idleJumpDirection = target.Center.X > npc.Center.X ? 1 : -1;

                            if (AICounter++ == 0)
                                npc.velocity = new Vector2(2 * idleJumpDirection, -7);

                            FrameCounter++;

                            // Sometimes the NPC gets stuck on weird blocks or ledges and only ends up jumping straight up
                            // This nudges the NPC while in midair to get over these obstacles
                            if (npc.velocity.X == 0 && AICounter >= 5) npc.velocity.X = 2 * idleJumpDirection;

                            if (npc.collideY && AICounter >= 15)
                            {
                                //if (npc.velocity.X != 0) npc.velocity.X = 0;

                                AIState = (int)AICase.Land;
                                AICounter = 0;
                                FrameCounter = 0;
                            }
                            break;
                        #endregion
                        #region Land
                        case (int)AICase.Land:
                            if (npc.collideY && npc.velocity.X != 0 && npc.oldVelocity.X != 0)
                            {
                                npc.velocity.X = 0;
                            }

                            if (AICounter++ >= 10)
                            {
                                AIState = (int)AICase.Idle;
                                AICounter = 1;
                                FrameCounter = 1; // Set the FrameCounter to 1 so it doesnt immediately change frames when set to 0
                            }
                            break;
                        #endregion
                        #region Swim
                        case (int)AICase.Swim:
                            if (npc.wet)
                            {
                                if (npc.collideY)
                                {
                                    npc.velocity.Y = -2f;
                                }

                                if (npc.velocity.Y > 2f)
                                {
                                    npc.velocity.Y *= 0.9f;
                                }

                                npc.velocity.Y -= 0.5f;
                                if (npc.velocity.Y < -4f)
                                {
                                    npc.velocity.Y = -4f;
                                }
                            }

                            npc.velocity.X = moveSpeed * 5f;

                            if (npc.collideY && !npc.wet)
                            {
                                AIState = (int)AICase.Idle;
                            }
                            break;
                        #endregion
                        default:
                            break;

                    }

                    oldCollision = npc.collideY;

                    // Allow the vanilla AI to run ONCE, after which the condition
                    // will no longer be true after being assigned a bonus drop (which will never be 0)
                    return npc.ai[1] == 0;
                }
            }


            return base.PreAI(npc);
        }

        public override void AI(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                // During the singular run instance, reset ai0 to be 0
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    //if (AIState < 0) AIState = 0;

                    // For some stupid reason I can't do this in SetDefaults or OnSpawn
                    //npc.lifeMax = npc.life = 100;
                    Player nearestPlayer = ModUtils.GetNearestPlayer(npc);
                    idleJumpDirection = npc.Center.X > nearestPlayer.Center.X ? -1 : 1;
                }
            }

            base.AI(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    if (target == null)
                    {
                        target = Main.player[player.whoAmI];
                    }
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    if (target == null)
                    {
                        Entity ownerEntity = projectile.GlobalProjectile().ownerEntity;
                        target = ownerEntity;
                    }

                    FeydenKillSpeechBubble(npc, projectile);         
                }
            }
        }

        private void FeydenKillSpeechBubble(NPC npc, Projectile projectile)
        {
            NPC feyden = ModUtils.FindFirstNPC(ModContent.NPCType<Feyden>());
            if (feyden == null) return;


            if (projectile.ModProjectile is FeydenAttack && npc.life <= 0)
            {
                Main.NewText("what");

                BaseSpeechBubble speechBubble = new BaseSpeechBubble();
                string[] randomText = {
                    "Eat my dust, slimeballs!",
                    "Down you go!",
                    "It's all in the footwork.",
                    //"Oh sorry was that your friend? Don't worry, you're next!",
                    "Slime your way out of this!",
                    "Gooey pest!",
                    "Like dicing onions in the kitchen!",
                    //"Nice try, but I've seen scarier jelly at the dessert table!",
                };

                int textIndex = Main.rand.Next(0, randomText.Length);
                string text = randomText[textIndex];
                speechBubble.Add(new Text(text, 45, 100));

                UISpeechBubbleSystem.Instance.SpeechBubbleState.AddSpeechBubble(feyden, speechBubble);
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    binaryWriter.Write(AIState);
                    binaryWriter.Write(AICycles);
                    binaryWriter.Write(AICounter);
                    binaryWriter.Write(FrameCounter);
                }
            }
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    AIState = binaryReader.ReadInt32();
                    AICycles = binaryReader.ReadInt32();
                    AICounter = binaryReader.ReadInt32();
                    FrameCounter = binaryReader.ReadInt32();
                }
            }
        }

        int xFrame = 1;
        int yFrame = 2;

        private const int MAX_FRAMES = 6;
        private const int FRAME_HEIGHT = 36;
        private const int FRAME_WIDTH = 40;
        public override void FindFrame(NPC npc, int frameHeight)
        {
            // For some reason, SOMETHING is being called that messes with the frames if I use npc.frame.
            // Therefore, I have to manually do them myself using my own counters.
            // Vanilla can go fuck itself.

            if (npc.type == NPCID.BlueSlime)
            {
                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    //Main.NewText(npc.frameCounter + " " + npc.frame.Y);
                    npc.direction = idleJumpDirection;

                    switch (AIState)
                    {
                        case (int)AICase.Spawn:
                            xFrame = 1;
                            if (FrameCounter % 8 == 0) yFrame = yFrame == 2 ? 1 : 2;
                            break;
                        case (int)AICase.Idle:
                            xFrame = 0;
                            if (FrameCounter % 8 == 0)
                                yFrame++;

                            // For SOME FUCKING REASON THIS KEEPS HAPPENING
                            if (yFrame > 5) yFrame = 5;

                            if (FrameCounter >= 40) yFrame = 0;
                            break;
                        case (int)AICase.PreJump:
                            xFrame = 1;

                            if (FrameCounter == 0 || FrameCounter >= 12)
                                yFrame = 2;

                            if (FrameCounter == 6)
                                yFrame = 1;
                            break;
                        case (int)AICase.Jump:
                            xFrame = 1;
                            yFrame = 0;
                            break;
                        case (int)AICase.Land:
                            xFrame = 1;
                            yFrame = 1;
                            break;
                        case (int)AICase.Swim:
                            xFrame = 1;
                            yFrame = npc.wet ? 2 : 0;
                            break;
                    }

                    return;
                }
            }

            base.FindFrame(npc, frameHeight);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Slime").Value;
                var spriteEffects = npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Rectangle drawRectangle = new Rectangle(xFrame * FRAME_WIDTH, yFrame * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);

                if (SlimeOverrideIDs.Contains(npc.netID))
                {
                    Color color = npc.color * Lighting.Brightness((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                    float alpha = npc.alpha / 255f;
                    float scale = npc.scale;
                    #region Bonus Drop
                    if (npc.ai[1] != -1)
                    {
                        Texture2D bonusDrop = TextureAssets.Item[(int)npc.ai[1]].Value;
                        float drawOffset = 0;
                        if (xFrame == 0)
                        {
                            switch (yFrame)
                            {
                                case 0:
                                case 1:
                                    drawOffset = -4;
                                    break;
                                case 5:
                                    drawOffset = -8;
                                    break;
                                case 2:
                                case 3:
                                    drawOffset = 4;
                                    break;
                                case 4:
                                    drawOffset = -4;
                                    break;
                                default:
                                    drawOffset = 0;
                                    break;
                            }
                        }

                        float dropScale;
                        switch (npc.ai[1])
                        {
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 699:
                            case 700:
                            case 701:
                            case 702:
                                // Ores
                                dropScale = 0.75f;
                                break;
                            case 71:
                            case 72:
                            case 73:
                                // Coins
                                dropScale = 0.9f;
                                break;
                            default:
                                dropScale = 0.6f;
                                break;
                        }

                        float drawDirection = npc.direction == 0 ? 1 : -1;
                        spriteBatch.Draw(bonusDrop, npc.Center + new Vector2(drawOffset * drawDirection, 0) - screenPos, null, Color.White, npc.rotation, bonusDrop.Size() / 2, dropScale, SpriteEffects.None, 0);
                    }
                    #endregion

                    float spawnOffset = 0;
                    if (AIState == (int)AICase.Spawn) spawnOffset = MathHelper.Lerp(8f, 0f, AICounter / 60f);

                    // This is the pickaxe slime in the Feyden cave
                    if (npc.ai[1] == ItemID.IronPickaxe) scale = 1.5f;

                    spriteBatch.Draw(texture, npc.Center + new Vector2(0, spawnOffset) - screenPos, drawRectangle, color, npc.rotation, drawRectangle.Size() / 2, scale, spriteEffects, 0);
                    return npc.IsABestiaryIconDummy;
                }
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
