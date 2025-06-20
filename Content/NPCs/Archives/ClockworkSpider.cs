using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.DataStructures;
using OvermorrowMod.Content.Biomes;
using System.Collections.Generic;
using OvermorrowMod.Core.NPCs;
using System;


namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ClockworkSpider : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            State currentState = AIStateMachine.GetCurrentState();

            return currentState is AttackState;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            NPC.width = 44;
            NPC.height = 44;
            NPC.lifeMax = 200;
            NPC.defense = 18;
            NPC.damage = 23;
            NPC.knockBackResist = 0.2f;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override bool CheckActive() => false;
        public override NPCTargetingConfig TargetingConfig()
        {
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(6.5f),
                aggroLossRate: 1f,
                aggroCooldownTime: 180f,
                maxTargetRange: ModUtils.TilesToPixels(35),
                maxAttackRange: ModUtils.TilesToPixels(35),
                alertRange: ModUtils.TilesToPixels(40),
                prioritizeAggro: true
            );
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new ClockworkSpiderPinball(this, Main.rand.Next(14, 17)),
            new ClockworkSpiderRoll(this),
            new ClockworkSpiderSwap(this)
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new CeilingWalk(this, 0.05f, 4.25f),
            new MeleeWalk(this, 0.05f, 4.25f),
        };


        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public bool IsOnCeiling => NPC.noGravity;
        public override void AI()
        {
            State currentState = AIStateMachine.GetCurrentSubstate();
            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);

            NPC.knockBackResist = 0.2f;
            if (currentState is not ClockworkSpiderRoll)
            {
                if (currentState is ClockworkSpiderPinball || currentState is ClockworkSpiderSwap)
                {
                    NPC.knockBackResist = 0f;

                    if (AICounter > 10)
                        NPC.rotation += 0.6f * NPC.direction;
                }
                else
                {
                    NPC.rotation = 0;
                }
                //else if (currentState is ClockworkSpiderSwap)
                //{
                //    if (AICounter > 50)
                //        NPC.rotation += 0.6f * NPC.direction;

                //    NPC.knockBackResist = 0f;
                //}
                //else
                //{
                //    NPC.rotation = 0;
                //}
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy)
            {
                xFrame = 0;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 8) yFrame = 0;
                }

                return;
            }

            State currentState = AIStateMachine.GetCurrentState();
            switch (currentState)
            {
                case MovementState:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame = (yFrame + 1) % 8;
                    }

                    if (Math.Abs(NPC.velocity.X) > 3)
                    {
                        xFrame = 1;

                    }
                    break;
                case AttackState attackState:
                    switch (attackState.currentSubstate)
                    {
                        case ClockworkSpiderSwap:
                        case ClockworkSpiderPinball:
                        case ClockworkSpiderRoll:
                            //Main.NewText("xFrame: " + xFrame + " yFrame: " + yFrame + " COUNTER: " + NPC.frameCounter + " AI " + AICounter);
                            xFrame = 2;

                            //int delay = attackState.currentSubstate is ClockworkSpiderSwap ? 30 : 0;
                            int delay = 0;
                            if (AICounter < delay)
                            {
                                yFrame = 0;
                            }

                            if (AICounter > delay)
                            {
                                if (yFrame < 4)
                                {
                                    NPC.frameCounter++;
                                }

                                if (NPC.frameCounter % 6 == 0)
                                {
                                    yFrame++;
                                }

                                // Don't know why this keeps going
                                if (yFrame > 4)
                                {
                                    yFrame = 4;
                                }
                            }

                            if (AICounter > delay + 40)
                            {
                                yFrame = 4;
                            }
                            break;
                    }
                    break;
                default:
                    xFrame = 2;
                    yFrame = 0;
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 8;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center + new Vector2(0, -8), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            State currentState = AIStateMachine.GetCurrentSubstate();
            State currentSuperState = AIStateMachine.GetCurrentState();
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOffset = new Vector2(0, -4);

            var swapCondition = currentState is ClockworkSpiderSwap && NPC.noGravity;
            if (currentState is CeilingWalk || swapCondition)
            {
                spriteEffects |= SpriteEffects.FlipVertically; // Add vertical flip
                drawOffset = new Vector2(0, 4);
            }

            if (currentState is ClockworkSpiderRoll && AICounter > 30)
                drawOffset = new Vector2(0, 2);

            if (AICounter > 30 && currentSuperState is AttackState)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 origin = NPC.frame.Size() / 2f;
                    SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    Vector2 offset = new Vector2(-42, -12);
                    Vector2 drawPos = NPC.oldPos[k] + origin - screenPos + new Vector2(0f, NPC.gfxOffY);
                    float opacity = (NPC.oldPos.Length - k) / (float)NPC.oldPos.Length;
                    Color afterImageColor = drawColor * 0.25f * opacity;
                    //Color afterImageColor = Color.Purple * 0.25f * opacity;

                    spriteBatch.Draw(
                        texture,
                        drawPos + offset,
                        NPC.frame,
                        afterImageColor,
                        NPC.oldRot[k],
                        origin,
                        NPC.scale,
                        effects,
                        0f
                    );
                }
            }

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }
    }
}