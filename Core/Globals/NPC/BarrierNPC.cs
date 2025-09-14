using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    public class BarrierNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// Current BP.
        /// </summary>
        public int BarrierPoints { get; private set; } = 0;

        /// <summary>
        /// Max BP.
        /// </summary>
        public int MaxBarrierPoints { get; set; } = 100;

        /// <summary>
        /// Time until barrier decays.
        /// </summary>
        public int BarrierDecayTimer { get; private set; } = 0;
        public bool HasBarrier => BarrierPoints > 0;

        /// <summary>
        /// Determines whether the NPC can gain barrier.
        /// </summary>
        public bool CanGainBarrier { get; set; } = true;

        /// <summary>
        /// Sets the barrier state for the NPC.
        /// </summary>
        /// <param name="amount">The total Barrier Points (BP) to assign. This determines how much damage the barrier can absorb.</param>
        /// <param name="duration">The duration of the barrier in ticks (1/60th of a second). After this duration, the barrier will decay.</param>
        public void SetBarrier(int amount, int duration)
        {
            if (!CanGainBarrier) return;

            BarrierPoints += amount;
            BarrierPoints = (int)MathHelper.Clamp(BarrierPoints, 0, MaxBarrierPoints);

            BarrierDecayTimer = duration;
        }

        public override void ResetEffects(NPC npc)
        {
            if (HasBarrier)
            {
                /*BarrierDecayTimer--;
                if (BarrierDecayTimer <= 0)
                {
                    BarrierPoints = 0; // Decay barrier
                }*/
            }
        }

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (HasBarrier && !npc.IsStealthed())
            {
                float alpha = Lighting.Brightness((int)npc.Center.X / 16, (int)npc.Center.Y / 16);

                Texture2D healthBar = TextureAssets.Hb1.Value;
                Texture2D healthContainer = TextureAssets.Hb2.Value;

                Rectangle drawRectangle = new Rectangle(0, 0, (int)(healthBar.Width * (BarrierPoints / (float)MaxBarrierPoints)), healthBar.Height);
                //Main.spriteBatch.Draw(healthBar, new Vector2((int)position.X, (int)position.Y) - Main.screenPosition, null, Color.White * alpha * 1.5f, 0, new Vector2(healthBar.Width / 2, 0), 0, 0);
                Main.spriteBatch.Draw(healthContainer, new Vector2((int)position.X, (int)position.Y - 8) - Main.screenPosition, null, Color.White * alpha, 0f, new Vector2(healthBar.Width, 0) / 2, new Vector2(1f, 0.8f) * scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Effect barrier = OvermorrowModFile.Instance.BarrierShader.Value;
                barrier.Parameters["Time"].SetValue(Main.GameUpdateCount / 60f);
                barrier.Parameters["NoiseSeed"].SetValue((float)(Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f));
                barrier.Parameters["Ratio"].SetValue(0.8f);
                barrier.Parameters["TintColor"].SetValue(new Vector4(0.3f, 0.6f, 1.0f, 1f));
                barrier.Parameters["Alpha"].SetValue(alpha);

                Main.graphics.GraphicsDevice.Textures[1] = OvermorrowModFile.Instance.BarrierNoiseTexture.Value;

                barrier.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(healthBar, new Vector2((int)position.X, (int)position.Y - 6) - Main.screenPosition, drawRectangle, Color.Cyan * alpha, 0f, new Vector2(healthBar.Width, 0) / 2, new Vector2(1f, 0.5f) * scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            }

            return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                binaryWriter.Write(MaxBarrierPoints);
                binaryWriter.Write(BarrierPoints);
                binaryWriter.Write(BarrierDecayTimer);
            }

            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                MaxBarrierPoints = binaryReader.ReadInt32();
                BarrierPoints = binaryReader.ReadInt32();
                BarrierDecayTimer = binaryReader.ReadInt32();
            }

            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }


        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => HandleBarrier(npc, ref n);
        }

        private void HandleBarrier(NPC npc, ref NPC.HitInfo info)
        {
            if (HasBarrier)
            {
                int incomingDamage = info.Damage;
                if (incomingDamage <= BarrierPoints) // Fully absorbed
                {
                    BarrierPoints -= incomingDamage;
                    CombatText.NewText(npc.Hitbox, Color.GhostWhite, incomingDamage);
                    info.Damage = 1; // Damage cannot be zero.
                }
                else
                {
                    int excessDamage = incomingDamage - BarrierPoints;
                    CombatText.NewText(npc.Hitbox, Color.WhiteSmoke, BarrierPoints);

                    BarrierPoints = 0;
                    info.Damage = excessDamage; // Remaining damage applies
                }

            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            /*if (HasBarrier)
            {
                RenderTarget2D renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

                // Set the render target
                Main.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent); // Clear the target to make sure it's empty before drawing

                // Start drawing to the render target
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                // Draw the NPC (this will include its textures such as wings, body, etc.)
                PreDraw(npc, spriteBatch, screenPos, drawColor);

                // End drawing to the render target
                spriteBatch.End();

                // Reset the render target to draw to the screen again
                Main.graphics.GraphicsDevice.SetRenderTarget(null);

                // Apply your custom shader (if any)
                if (HasBarrier) // Assuming you want to apply this shader conditionally
                {
                    Effect barrier = OvermorrowModFile.Instance.BarrierShader.Value;
                    barrier.Parameters["Time"].SetValue(Main.GameUpdateCount / 60f); // Time in seconds
                    barrier.Parameters["NoiseSeed"].SetValue((float)(Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f)); // Random noise seed
                    barrier.Parameters["Ratio"].SetValue(0.8f); // Ratio for noise blend
                    barrier.Parameters["TintColor"].SetValue(new Vector4(0.3f, 0.6f, 1.0f, 1f)); // Tint color (RGBA)
                    barrier.Parameters["Alpha"].SetValue(1f);

                    Main.graphics.GraphicsDevice.Textures[1] = OvermorrowModFile.Instance.BarrierNoiseTexture.Value;

                    barrier.CurrentTechnique.Passes[0].Apply();
                }

                // Draw the captured render target to the screen
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                // Draw the captured render target (as a texture)
                spriteBatch.Draw(renderTarget, npc.Center - screenPos, npc.frame, drawColor * npc.Opacity, npc.rotation, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 0f);

                // End the drawing and reset the sprite batch
                spriteBatch.End();
            }*/

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}