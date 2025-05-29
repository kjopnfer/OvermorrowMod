using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using OvermorrowMod.Content.Biomes;
using Terraria.Localization;
using System;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Particles;
using Terraria.Map;
using OvermorrowMod.Content.Misc;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public abstract class LivingGrimoire : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(8, 8),
                PortraitPositionXOverride = 8,
                PortraitPositionYOverride = -6,
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 110;
            NPC.defense = 8;
            NPC.damage = 15;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public virtual int CastTime => 120;
        //public ref float AIState => ref NPC.ai[0];
        //public ref float AICounter => ref NPC.ai[1];

        protected int distanceFromGround = 180;
        protected int aggroDelayTime = 60;
        protected int tileAttackDistance = 24;
        protected Vector2 targetPosition;
        public override void OnSpawn(IEntitySource source)
        {
            AIStateMachine.SetSubstate<GrimoireHidden>(AIStateType.Idle, NPC.ModNPC as OvermorrowNPC);
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new GrimoireHidden(this)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GrimoireSpellCast(this)
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new BasicFly(this),
        };

        public override NPCTargetingConfig TargetingConfig()
        {
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(15),
                aggroLossRate: 1f,
                aggroCooldownTime: 180f,
                maxTargetRange: ModUtils.TilesToPixels(26),
                maxAttackRange: ModUtils.TilesToPixels(30),
                alertRange: null,
                prioritizeAggro: true
            );
        }

        /// <summary>
        /// Exposes the config in order to allow for the NPC to have an alert indicator when it is no longer hidden.
        /// </summary>
        public void ReenableAlertIndicator()
        {
            TargetingModule = new NPCTargetingModule(NPC, new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(15),
                aggroLossRate: 1f,
                aggroCooldownTime: 180f,
                maxTargetRange: ModUtils.TilesToPixels(20),
                maxAttackRange: ModUtils.TilesToPixels(30),
                alertRange: ModUtils.TilesToPixels(30),
                prioritizeAggro: true
            ));
        }

        public override void AI()
        {
            State substate = AIStateMachine.GetCurrentSubstate();

            if (substate is not GrimoireSpellCast)
            {
                if (TargetingModule.HasTarget())
                {
                    Vector2 targetPosition = TargetingModule.Target.Center;
                    NPC.direction = NPC.GetDirection(targetPosition);
                }
                else if (TargetingModule.MiscTargetPosition.HasValue)
                {
                    Vector2 targetPosition = TargetingModule.MiscTargetPosition.Value;
                    bool canTurn = substate is not GrimoireIdle;
                    if (canTurn)
                        NPC.direction = NPC.GetDirection(targetPosition);
                }
            }

            NPC.dontTakeDamage = substate is GrimoireHidden;

            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);

            NPC.rotation = NPC.velocity.Y * 0.08f;

            base.AI();
        }

        bool drawWings = true;
        bool drawRuneCircle = false;
        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy)
            {
                yFrame = 8;

                if (NPC.frameCounter++ % 3 == 0)
                {
                    yFrameWing++;
                    if (yFrameWing >= 3) yFrameWing = 0;
                }
            }

            drawWings = true;
            drawRuneCircle = false;

            State currentState = AIStateMachine.GetCurrentState();
            switch (currentState)
            {
                case IdleState idleState:

                    switch (idleState.currentSubstate)
                    {
                        case GrimoireHidden:
                            drawWings = false;

                            if (TargetingModule.HasTarget())
                            {
                                if (NPC.frameCounter++ % 4 == 0)
                                {
                                    yFrame++;
                                    if (yFrameWing >= 8) yFrameWing = 8;
                                }
                            }
                            else
                            {
                                yFrame = 0;
                            }
                            break;
                        case GrimoireIdle:
                            if (NPC.frameCounter++ % 3 == 0)
                            {
                                yFrameWing++;
                                if (yFrameWing >= 3) yFrameWing = 0;
                            }
                            break;
                    }
                    break;
                case MovementState movementState when movementState.currentSubstate is BasicFly:
                    yFrame = 8;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
                case AttackState attackState when attackState.currentSubstate is GrimoireSpellCast:
                    drawRuneCircle = true;
                    yFrame = AICounter <= 6 || AICounter >= CastTime - 6 ? 9 : 10;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
            }

            /*switch ((AICase)AIState)
            {
                case AICase.Hidden:
                    yFrame = 0;
                    break;
                case AICase.Fall:
                    if (NPC.frameCounter++ % 4 == 0)
                    {
                        yFrame++;
                        if (yFrameWing >= 8) yFrameWing = 8;
                    }
                    break;
                case AICase.Fly:
                    yFrame = 8;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
                case AICase.Cast:
                    yFrame = AICounter <= 6 || AICounter >= CastTime - 6 ? 9 : 10;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
            }*/
        }

        int xFrame = 0;
        int yFrame = 0;
        int yFrameWing = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 11;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected virtual void DrawCastEffect(SpriteBatch spriteBatch) { }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D wingTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BookWings").Value;

            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int wingTextureHeight = (int)(wingTexture.Height / 3f);
            Vector2 bestiaryDrawOffset = new Vector2(-6, -28);
            spriteBatch.Draw(wingTexture, NPC.Center + bestiaryDrawOffset, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center + new Vector2(-8, 0), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (drawRuneCircle)
            {
                DrawCastEffect(spriteBatch);
            }
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.4f);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D wingTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BookWings").Value;

            int wingTextureHeight = (int)(wingTexture.Height / 3f);
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
           {
               drawModifiers.Position = new Vector2(8, 8);
               drawModifiers.PortraitPositionXOverride = 8;
               drawModifiers.PortraitPositionYOverride = -6;

               // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
               NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
               NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
           }*/

            int xOffset = NPC.direction == -1 ? 4 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -28);

            var lightAverage = (drawColor.R / 255f + drawColor.G / 255f + drawColor.B / 255f) / 3;
            if (Main.LocalPlayer.HasBuff(BuffID.Hunter))
            {
                drawColor = Color.Lerp(new Color(255, 50, 50), drawColor, lightAverage);
            }

            Color wingColor = Color.Lerp(drawColor, Color.White, 0.7f);

            State currentState = AIStateMachine.GetCurrentState();
            if (drawWings)
            {
                spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), wingColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }

            //if ((AICase)AIState != AICase.Fall && (AICase)AIState != AICase.Hidden)
            //    spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), wingColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}