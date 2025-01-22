using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using OvermorrowMod.Common;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Localization;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public abstract class ChairSummon : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (AICase)AIState != AICase.Summon;
 
        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        /// <summary>
        /// Chairs are spawned by a parent which caps summons to only three active.
        /// </summary>
        public ref float ParentID => ref NPC.ai[2];

        public enum AICase
        {
            Summon = -1,
            Idle = 0,
            Jump = 1
        }

        int maxAttempts = 1000;
        private int failCount = 0;
        protected float maxSpeed = 2f;
        public override void OnSpawn(IEntitySource source)
        {
            AIState = (int)AICase.Summon;
            while (!Collision.SolidTiles((int)(NPC.position.X / 16), (int)((NPC.position.X + NPC.width) / 16), (int)((NPC.position.Y + NPC.height) / 16), (int)((NPC.position.Y + NPC.height + 1) / 16)) && failCount < maxAttempts)
            {
                NPC.position.Y += 1; // Move the NPC downward
                failCount++; // Increment the fail count to avoid infinite loops
            }

            maxSpeed = Main.rand.NextFloat(2f, 3.5f);
        }

        protected int idleTime = 30;
        public sealed override void AI()
        {
            NPC.knockBackResist = (AICase)AIState == AICase.Summon ? 0f : 1f;

            switch ((AICase)AIState)
            {
                case AICase.Summon:
                    while (!Collision.SolidTiles((int)(NPC.position.X / 16), (int)((NPC.position.X + NPC.width) / 16), (int)((NPC.position.Y + NPC.height) / 16), (int)((NPC.position.Y + NPC.height + 1) / 16)) && failCount < maxAttempts)
                    {
                        NPC.position.Y += 1; // Move the NPC downward
                        failCount++; // Increment the fail count to avoid infinite loops
                    }

                    Vector3 originalColor = new Vector3(0.5f, 0.3765f, 0.3980f);
                    float lerpFactor = MathHelper.Clamp((AICounter - 60f) / 60f, 0f, 1f);
                    Vector3 lerpedColor = Vector3.Lerp(originalColor, Vector3.Zero, lerpFactor);

                    Lighting.AddLight(NPC.Center, lerpedColor);

                    if (AICounter++ >= 120)
                    {
                        AIState = (int)AICase.Idle;
                        AICounter = 0;
                    }
                    break;
                case AICase.Idle:
                    if (AICounter++ >= idleTime)
                    {
                        AIState = (int)AICase.Jump;
                        AICounter = 0;
                    }
                    break;
                case AICase.Jump:
                    MovementAI();
                    break;
            }
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
            if (AIState == (int)AICase.Summon)
            {
                HandleParticleEffects();
                DrawSpawnAura(spriteBatch, spriteEffects);
            }

            return false;
        }

        private Rectangle GetDrawRectangle()
        {
            Rectangle drawRectangle = NPC.frame;
            if (AIState == (int)AICase.Summon && AICounter < 60)
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
                    Particle.CreateParticleDirect(Particle.ParticleType<LightOrb>(), spawnPosition, -Vector2.UnitY, color, 1f, scale, 0f, 0, scale * 0.5f);
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