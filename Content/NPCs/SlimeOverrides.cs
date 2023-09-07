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

namespace OvermorrowMod.Content.NPCs
{
    public class SlimeOverrides : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return base.CanBeHitByItem(npc, player, item);
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                    npc.lifeMax = 30;
                }
            }
        }

        int idleJumpDirection = 1;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            // This shit doesn't run
            if (npc.type == NPCID.BlueSlime)
            {

                if (npc.netID == NPCID.GreenSlime)
                {
                    npc.lifeMax = 30;
                }
            }

            base.OnSpawn(npc, source);
        }

        public enum AICase
        {
            Idle = 0,
            PreJump = 1,
            Jump = 2,
            Land = 3,
        }

        private float AIState = 0;
        private float AICycles = 0;
        private float AICounter = 0;
        private float FrameCounter = 0;

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                // This is so stupid why would Red do this
                if (npc.netID == NPCID.GreenSlime)
                {
                    Player player = Main.player[npc.target];
                    if (!player.active) npc.target = 255;

                    npc.velocity.X *= 0.98f;
                    Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);

                    //Main.NewText("collideX: " + npc.collideX + " collideY: " + npc.collideY + " fC: " + FrameCounter + " aC: " + AICounter);

                    switch (AIState)
                    {
                        #region Idle
                        case (int)AICase.Idle:
                            Main.NewText("idle");

                            if (AICounter == 1) npc.velocity.X = 0.1f * idleJumpDirection;
                            if (npc.collideY) AICounter++;

                            if (FrameCounter++ >= 40) FrameCounter = 1;

                            if (AICounter++ >= 40)
                            {
                                if (npc.collideY) npc.velocity.X = 0;

                                AICounter = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0
                                //FrameCounter = 1;
                                if (AICycles++ >= 3)
                                {
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

                            Tile bottomLeftTile = Main.tile[(int)npc.Hitbox.BottomLeft().X / 16, (int)npc.Hitbox.BottomLeft().Y / 16];
                            Tile bottomRightTile = Main.tile[(int)npc.Hitbox.BottomRight().X / 16, (int)npc.Hitbox.BottomRight().Y / 16];

                            bool checkOnSlope = (bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) || (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType]);

                            Main.NewText("prejump " + npc.collideY + " / " + checkOnSlope);

                            if (npc.collideY || checkOnSlope)
                            {
                                AICounter += bounceRate;
                            }


                            // These are for weird slopes that don't trigger the collision code normally

                            if (npc.velocity.X != 0) npc.velocity.X *= 0.98f;

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
                            Main.NewText("jump");

                            if (npc.target != 255) idleJumpDirection = player.Center.X > npc.Center.X ? 1 : -1;

                            if (AICounter++ == 0)
                                npc.velocity = new Vector2(2 * idleJumpDirection, -7);

                            FrameCounter++;

                            // Sometimes the NPC gets stuck on weird blocks or ledges and only ends up jumping straight up
                            // This nudges the NPC while in midair to get over these obstacles
                            if (npc.velocity.X == 0 && AICounter >= 5) npc.velocity.X = 2 * idleJumpDirection;

                            if (npc.collideY && AICounter >= 15)
                            {
                                if (npc.velocity.X != 0) npc.velocity.X = 0;

                                AIState = (int)AICase.Land;
                                AICounter = 0;
                                FrameCounter = 0;
                            }
                            break;
                        #endregion
                        #region Land
                        case (int)AICase.Land:
                            Main.NewText("land");

                            if (AICounter++ >= 10)
                            {
                                AIState = (int)AICase.Idle;
                                AICounter = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0
                                FrameCounter = 1;
                            }
                            break;
                            #endregion
                    }

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
                if (npc.netID == NPCID.BlueSlime)
                {
                    npc.lifeMax = npc.life = 600;
                }

                // During the singular run instance, reset ai0 to be 0
                if (npc.netID == NPCID.GreenSlime)
                {
                    if (AIState < 0) AIState = 0;

                    // For some stupid reason I can't do this in SetDefaults or OnSpawn
                    npc.lifeMax = npc.life = 600;
                    idleJumpDirection = npc.Center.X / 16 > Main.maxTilesX / 2 ? -1 : 1;
                }
            }

            base.AI(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime) npc.TargetClosest();
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime) npc.TargetClosest();
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
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
                if (npc.netID == NPCID.GreenSlime)
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
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                    npc.direction = idleJumpDirection;

                    switch (AIState)
                    {
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
                    }
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

                if (npc.netID == NPCID.GreenSlime)
                {
                    Color color = npc.color * Lighting.Brightness((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                    float alpha = npc.alpha / 255f;
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
                        spriteBatch.Draw(bonusDrop, npc.Center + new Vector2(drawOffset * drawDirection, 0) - Main.screenPosition, null, Color.White, npc.rotation, bonusDrop.Size() / 2, dropScale, SpriteEffects.None, 0);
                    }

                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, drawRectangle, color, npc.rotation, drawRectangle.Size() / 2, npc.scale, spriteEffects, 0);
                    return false;
                }
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
