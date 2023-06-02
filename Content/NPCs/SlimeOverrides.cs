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
            switch (npc.type)
            {
                case NPCID.GreenSlime:
                    npc.aiStyle = -1;
                    break;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                    npc.ai[1] = 1;
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

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                // This is so stupid why would Red do this
                if (npc.netID == NPCID.GreenSlime)
                {
                    switch (npc.ai[0])
                    {
                        case (int)AICase.Idle:
                            if (npc.ai[1] == 1) npc.velocity.X = 0.1f;

                            if (npc.ai[1]++ >= 40)
                            {
                                npc.velocity.X = 0;
                                npc.ai[1] = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0

                                if (npc.ai[2]++ >= 3)
                                {
                                    npc.ai[0] = (int)AICase.PreJump;
                                    npc.ai[1] = 0;
                                    npc.ai[2] = 0;
                                }
                            }
                            break;
                        case (int)AICase.PreJump:
                            int bounceRate = 1;
                            if (npc.ai[2] >= 4) bounceRate = 2;

                            npc.ai[1] += bounceRate;

                            if (npc.ai[1] >= 12)
                            {
                                npc.ai[1] = 0;
                                if (npc.ai[2]++ >= 8)
                                {
                                    npc.ai[0] = (int)AICase.Jump;
                                    npc.ai[2] = 0;
                                }
                            }
                            break;
                        case (int)AICase.Jump:
                            if (npc.ai[1]++ == 0)
                                npc.velocity = new Vector2(2, -7);

                            // Sometimes the NPC gets stuck on weird blocks or ledges and only ends up jumping straight up
                            // This nudges the NPC while in midair to get over these obstacles
                            if (npc.velocity.X == 0 && npc.ai[1] >= 5) npc.velocity.X = 2;

                            if (npc.collideY && npc.ai[1] >= 15)
                            {
                                if (npc.velocity.X != 0) npc.velocity.X = 0;

                                npc.ai[0] = (int)AICase.Land;
                                npc.ai[1] = 0;
                            }
                            break;
                        case (int)AICase.Land:
                            if (npc.ai[1]++ >= 10)
                            {
                                npc.ai[0] = (int)AICase.Idle;
                                npc.ai[1] = 1; // Set the AICounter to 1 so it doesnt immediately change frames when set to 0
                            }
                            break;
                    }
                    return false;
                }
            }


            return base.PreAI(npc);
        }

        public override void AI(NPC npc)
        {


            base.AI(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(npc, player, item, damage, knockback, crit);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
        }


        int xFrame = 1;
        int yFrame = 2;
        int frameCounter;
        int frameTimer;

        private const int MAX_FRAMES = 6;
        private const int FRAME_HEIGHT = 36;
        private const int FRAME_WIDTH = 40;

        // ai[0] is AI State
        // ai[1] is AI Counter
        // ai[2] is an AI Cycle (how many times the NPC has run through a state)
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                if (npc.netID == NPCID.GreenSlime)
                {
                    Main.NewText("ai1: " + npc.ai[1] + " frame: " + yFrame);

                    switch (npc.ai[0])
                    {
                        case (int)AICase.Idle:
                            xFrame = 0;
                            if (npc.ai[1] % 8 == 0)
                                yFrame++;

                            // For SOME FUCKING REASON THIS KEEPS HAPPENING
                            if (yFrame > 5) yFrame = 5;

                            if (npc.ai[1] >= 40) yFrame = 0;
                            break;
                        case (int)AICase.PreJump:
                            xFrame = 1;

                            if (npc.ai[1] == 0 || npc.ai[1] >= 12)
                                yFrame = 2;

                            if (npc.ai[1] == 6)
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
                var spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Rectangle drawRectangle = new Rectangle(xFrame * FRAME_WIDTH, yFrame * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);

                if (npc.netID == NPCID.GreenSlime)
                {
                    Color color = npc.color * Lighting.Brightness((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                    float alpha = npc.alpha / 255f;

                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, drawRectangle, color, npc.rotation, drawRectangle.Size() / 2, npc.scale, spriteEffects, 0);
                    return false;
                }
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
