using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class DoorLock : ModNPC
    {
        public override bool CheckActive() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 56;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;

            SafeSetDefaults();
        }

        public ModTileEntity tileEntity;
        ArchiveDoor_TE DoorInstance => (ArchiveDoor_TE)TileEntity.ByID[tileEntity.ID];

        // Death animation states
        private bool isDying = false;
        public ref float DeathAnimationTimer => ref NPC.ai[0];
        private readonly int DEATH_ANIMATION_DURATION = ModUtils.SecondsToTicks(1);

        public override void AI()
        {
            if (!DoorInstance.IsLocked && !isDying) return;

            if (!isDying)
            {
                Lighting.AddLight(NPC.Center, 0f, 1f, 0.5f);
                NPC.localAI[0]++;
            }
            else
            {
                DeathAnimationTimer++;

                if (DeathAnimationTimer >= DEATH_ANIMATION_DURATION)
                {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }

                if (!NPC.noGravity && NPC.collideY && NPC.velocity.Y >= 0)
                {
                    NPC.life = 0;
                    NPC.HitEffect(new NPC.HitInfo());
                    NPC.active = false;

                    DoorInstance.UnlockDoors();
                }
            }
        }

        public void StartDeathAnimation()
        {
            if (!isDying)
            {
                isDying = true;
                DeathAnimationTimer = 0;
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                float randomScale = Main.rand.NextFloat(10f, 20f);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_01", AssetRequestMode.ImmediateLoad).Value;

                Color color = Color.LimeGreen;
                var lightOrb = new Circle(texture, ModUtils.SecondsToTicks(0.7f), canGrow: true, useSineFade: true)
                {
                    rotationAmount = 0.05f
                };

                float orbScale = 0.5f;
                ParticleManager.CreateParticleDirect(lightOrb, NPC.Center, Vector2.Zero, color, 1f, orbScale, 0.2f, useAdditiveBlending: true);

                lightOrb = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value, ModUtils.SecondsToTicks(0.6f), canGrow: true, useSineFade: true) {
                    rotationAmount = 0.05f
                };
                ParticleManager.CreateParticleDirect(lightOrb, NPC.Center, Vector2.Zero, color, 1f, scale: 0.6f, 0.2f, useAdditiveBlending: true);

                Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
                for (int i = 0; i < 16; i++)
                {
                    randomScale = Main.rand.NextFloat(2f, 7f);

                    Vector2 randomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 7);

                    var lightSpark = new Spark(sparkTexture, 0f, true, 0f)
                    {
                        endColor = Color.LimeGreen
                    };
                    ParticleManager.CreateParticleDirect(lightSpark, NPC.Center, randomVelocity * 2, color, 1f, randomScale, 0f, useAdditiveBlending: true);
                }


                Vector2 upwardVelocity = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-8f, -3f));
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, upwardVelocity, Mod.Find<ModGore>($"{Name}Gore1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, upwardVelocity, Mod.Find<ModGore>($"{Name}Gore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, upwardVelocity, Mod.Find<ModGore>($"{Name}Gore3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, upwardVelocity, Mod.Find<ModGore>($"{Name}Gore4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, upwardVelocity, Mod.Find<ModGore>($"{Name}Gore5").Type, NPC.scale);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D lockTexture = ModContent.Request<Texture2D>(AssetDirectory.Misc + Name).Value;
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Misc + "LockChain").Value;

            Vector2[] chainDirections = {
                Vector2.Normalize(new Vector2(-1, -1.05f)),
                Vector2.Normalize(new Vector2(1, -1.05f)),
                Vector2.Normalize(new Vector2(-1, 1f)),
                Vector2.Normalize(new Vector2(1, 1))
            };

            float max = 5;

            // Calculate death animation progress (0 to 1)
            float deathProgress = isDying ? MathHelper.Clamp(DeathAnimationTimer / DEATH_ANIMATION_DURATION, 0f, 1f) : 0f;

            for (int direction = 0; direction < chainDirections.Length; direction++)
            {
                Vector2 chainDirection = chainDirections[direction];

                for (int i = 1; i <= max; i++)
                {
                    Vector2 chainPosition = NPC.Center + chainDirection * chainTexture.Height * i;

                    // Add falling effect during death animation
                    if (isDying)
                    {
                        // Add slight random sway
                        float sway = MathF.Sin(DeathAnimationTimer * 0.2f + i) * deathProgress * 5f;
                        chainPosition.X += sway;
                    }

                    // Calculate rotation angle for the chain to match the direction
                    float rotation = (float)Math.Atan2(chainDirection.Y, chainDirection.X) + MathHelper.PiOver2;


                    // Calculate fade-out alpha based on distance from center
                    float fadeProgress = (float)i / max;
                    float baseAlpha = MathHelper.Lerp(1f, 0.15f, fadeProgress);

                    float deathFade = isDying ? (1f - deathProgress) : 1f;
                    float alpha = baseAlpha * deathFade;

                    // Skip drawing if completely faded
                    if (alpha <= 0.01f) continue;

                    // Calculate pulsing shader progress (only if not dying)
                    float shaderProgress = 0f;
                    if (!isDying)
                    {
                        float timeOffset = (float)i * 25f;
                        float pulseSpeed = 0.05f;
                        shaderProgress = MathF.Sin((NPC.localAI[0] - timeOffset) * pulseSpeed) * 0.5f + 0.5f;
                        shaderProgress = MathHelper.Clamp(shaderProgress, 0f, 1f);
                    }

                    // Apply shader if progress is above threshold
                    if (shaderProgress > 0.1f)
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                        Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
                        effect.Parameters["ColorFillColor"].SetValue(Color.LightGreen.ToVector3());
                        effect.Parameters["ColorFillProgress"].SetValue(shaderProgress);
                        effect.CurrentTechnique.Passes["ColorFill"].Apply();
                    }

                    spriteBatch.Draw(
                        chainTexture,
                        chainPosition - screenPos,
                        new Rectangle(0, 0, chainTexture.Width, chainTexture.Height),
                        Color.White * alpha,
                        rotation,
                        chainTexture.Size() / 2f,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    // Reset spritebatch if shader was applied
                    if (shaderProgress > 0.1f)
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
            }

            spriteBatch.Draw(lockTexture, NPC.Center - screenPos, null, drawColor, 0f, lockTexture.Size() / 2f, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}