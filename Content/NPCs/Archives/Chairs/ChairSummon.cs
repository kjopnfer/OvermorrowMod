using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public abstract class ChairSummon : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState is not ChairSummonAnimation;

        /// <summary>
        /// Chairs are spawned by a parent which caps summons to only three active.
        /// </summary>
        public ref float ParentID => ref NPC.ai[2];


        protected float maxSpeed = 2f;
        public override void OnSpawn(IEntitySource source)
        {
            AIStateMachine.SetSubstate<ChairSummonAnimation>(AIStateType.Idle, NPC.ModNPC as OvermorrowNPC);
        }

        protected int idleTime = 30;

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new ChairSummonAnimation(this)
        };

        public override List<BaseAttackState> InitializeAttackStates() => null;

        public override List<BaseMovementState> InitializeMovementStates()
        {
            switch (NPC.ModNPC)
            {
                case AnimatedSofa:
                    return new List<BaseMovementState> {
                        new Hop(this)
                    };
                default:
                    return new List<BaseMovementState> {
                        new MeleeWalk(this)
                    };
            }
        }

        public override void SafeSetDefaults()
        {

        }

        public override NPCTargetingConfig TargetingConfig()
        {
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(10f),
                aggroLossRate: 1f,
                aggroCooldownTime: ModUtils.SecondsToTicks(4f),
                aggroRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(25),            // Far right detection
                    left: ModUtils.TilesToPixels(25),             // Close left detection
                    up: ModUtils.TilesToPixels(15),               // Medium up detection
                    down: ModUtils.TilesToPixels(15),             // Far down detection
                    flipWithDirection: true                       // Flip based on NPC direction
                ),
                attackRadius: AggroRadius.Circle(ModUtils.TilesToPixels(35)),
                alertRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(35),
                    left: ModUtils.TilesToPixels(35),
                    up: ModUtils.TilesToPixels(25),
                    down: ModUtils.TilesToPixels(25),
                    flipWithDirection: true
                ),
                prioritizeAggro: true
            )
            {
                ShowDebugVisualization = true
            };
        }

        protected State AIState => AIStateMachine.GetCurrentSubstate();

        public override void AI()
        {
            State substate = AIStateMachine.GetCurrentSubstate();

            NPC.knockBackResist = 1f;
            if (substate is ChairSummonAnimation)
                NPC.knockBackResist = 0f;

            if (TargetingModule.HasTarget())
            {
                Vector2 targetPosition = TargetingModule.Target.Center;
                NPC.direction = NPC.GetDirectionFrom(targetPosition);
            }

            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);
        }

        public abstract void MovementAI();

        protected int xFrame = 0;
        protected int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 4;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        public abstract void SetFrame();
        /// <summary>
        /// Defines the portal width that the NPC spawns from.
        /// </summary>
        protected abstract int GlowRectangleWidth { get; }
        /// <summary>
        /// Defines the randomized position ranges that the particles spawn from.
        /// </summary>
        protected abstract Vector2 ParticleSpawnOffset { get; }
        protected abstract int AuraHeightOffset { get; }

        public sealed override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle drawRectangle = GetDrawRectangle();

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, drawRectangle, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            // Handle summon effects
            if (AIState is ChairSummonAnimation)
            {
                HandleParticleEffects();
                DrawSpawnAura(spriteBatch, spriteEffects);
            }

            return false;
        }

        private Rectangle GetDrawRectangle()
        {
            Rectangle drawRectangle = NPC.frame;
            if (AIState is ChairSummonAnimation && AICounter < 60)
            {
                float rectangleHeight = MathHelper.SmoothStep(-NPC.frame.Height, 0, AICounter / 60f);
                drawRectangle = new Rectangle(0, (int)rectangleHeight, NPC.frame.Width, NPC.frame.Height);
            }
            return drawRectangle;
        }

        private void HandleParticleEffects()
        {
            float scale = 0.1f;
            float particleSpawnRate = MathHelper.Lerp(6, 15, MathHelper.Clamp(AICounter - 60, 0, 60f) / 60f);
            if (AICounter % particleSpawnRate == 0 && !Main.gamePaused && AICounter < 110)
            {
                int randomIterations = Main.rand.Next(1, 3);
                for (int i = 0; i < randomIterations; i++)
                {
                    Color color = Color.Lerp(Color.Orange, Color.HotPink, Main.rand.NextFloat(0, 1f));
                    Vector2 spawnPosition = NPC.Center + ParticleSpawnOffset + new Vector2(Main.rand.Next(-3, 4) * 6, 20);

                    Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;

                    var lightOrb = new Circle(texture, 0f);
                    ParticleManager.CreateParticleDirect(lightOrb, spawnPosition, -Vector2.UnitY, color, 1f, scale * 0.5f, 0f, useAdditiveBlending: true);
                }
            }
        }

        private void DrawSpawnAura(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            Rectangle glowRectangle = new Rectangle(0, 0, GlowRectangleWidth, 1);

            float baseAlpha = 0.8f;
            if (AICounter >= 60) baseAlpha = MathHelper.Lerp(0.8f, 0, (AICounter - 60f) / 60f);

            for (int i = 0; i < 30; i++)
            {
                Vector2 drawOffset = new Vector2(0, AuraHeightOffset - i); // Default offset
                float drawAlpha = baseAlpha - (i / 30f);
                Color auraColor = Color.Lerp(Color.Orange, Color.HotPink, i / 30f);

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, NPC.Center + drawOffset - Main.screenPosition, glowRectangle, auraColor * drawAlpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }
        }
    }
}