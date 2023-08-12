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

        // ai[0] is AI State
        // ai[2] is not used because for some reason vanilla decides to stick whatever bonus drop in here
        // ai[2] is AI Counter
        // ai[3] is an AI Cycle (how many times the NPC has run through a state)
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                // This is so stupid why would Red do this
                if (npc.netID == NPCID.GreenSlime)
                {
                    Player player = Main.player[npc.target];
                    if (!player.active) npc.target = 255;

                    switch (npc.ai[0])
                    {
                        case (int)AICase.Idle:
                            if (npc.ai[2] == 1) npc.velocity.X = 0.1f * idleJumpDirection;

                            if (npc.ai[2]++ >= 40)
                            {
                                npc.velocity.X = 0;
                                npc.ai[2] = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0

                                if (npc.ai[3]++ >= 3)
                                {
                                    npc.ai[0] = (int)AICase.PreJump;
                                    npc.ai[2] = 0;
                                    npc.ai[3] = 0;
                                }
                            }
                            break;
                        case (int)AICase.PreJump:
                            int bounceRate = 1;
                            if (npc.ai[3] >= 4) bounceRate = 2;

                            npc.ai[2] += bounceRate;

                            if (npc.ai[2] >= 12)
                            {
                                npc.ai[2] = 0;
                                if (npc.ai[3]++ >= 8)
                                {
                                    npc.ai[0] = (int)AICase.Jump;
                                    npc.ai[3] = 0;
                                }
                            }
                            break;
                        case (int)AICase.Jump:
                            if (npc.target != 255) idleJumpDirection = player.Center.X > npc.Center.X ? 1 : -1;

                            if (npc.ai[2]++ == 0)
                                npc.velocity = new Vector2(2 * idleJumpDirection, -7);

                            // Sometimes the NPC gets stuck on weird blocks or ledges and only ends up jumping straight up
                            // This nudges the NPC while in midair to get over these obstacles
                            if (npc.velocity.X == 0 && npc.ai[2] >= 5) npc.velocity.X = 2 * idleJumpDirection;

                            if (npc.collideY && npc.ai[2] >= 15)
                            {
                                if (npc.velocity.X != 0) npc.velocity.X = 0;

                                npc.ai[0] = (int)AICase.Land;
                                npc.ai[2] = 0;
                            }
                            break;
                        case (int)AICase.Land:
                            if (npc.ai[2]++ >= 10)
                            {
                                npc.ai[0] = (int)AICase.Idle;
                                npc.ai[2] = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0
                            }
                            break;
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
                // During the singular run instance, reset ai0 to be 0
                if (npc.netID == NPCID.GreenSlime)
                {
                    if (npc.ai[0] < 0) npc.ai[0] = 0;

                    // For some stupid reason I can't do this in SetDefaults or OnSpawn
                    npc.lifeMax = 30;
                    npc.life = 30;

                    idleJumpDirection = npc.Center.X / 16 > Main.maxTilesX / 2 ? -1 : 1;
                }
            }

            base.AI(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                    npc.TargetClosest();
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                    npc.TargetClosest();
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                }
            }
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                }
            }
        }

        int xFrame = 1;
        int yFrame = 2;
        int frameCounter;
        int frameTimer;

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

                    switch (npc.ai[0])
                    {
                        case (int)AICase.Idle:
                            xFrame = 0;
                            if (npc.ai[2] % 8 == 0)
                                yFrame++;

                            // For SOME FUCKING REASON THIS KEEPS HAPPENING
                            if (yFrame > 5) yFrame = 5;

                            if (npc.ai[2] >= 40) yFrame = 0;
                            break;
                        case (int)AICase.PreJump:
                            xFrame = 1;

                            if (npc.ai[2] == 0 || npc.ai[2] >= 12)
                                yFrame = 2;

                            if (npc.ai[2] == 6)
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
