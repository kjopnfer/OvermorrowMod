using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Buffs;
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
        public int MaxBarrierPoints { get; private set; } = 0;

        /// <summary>
        /// Time until barrier decays.
        /// </summary>
        public int BarrierDecayTimer { get; private set; } = 0;
        public bool HasBarrier => BarrierPoints > 0;


        /// <summary>
        /// Sets the barrier state for the NPC.
        /// </summary>
        /// <param name="amount">The total Barrier Points (BP) to assign. This determines how much damage the barrier can absorb.</param>
        /// <param name="duration">The duration of the barrier in ticks (1/60th of a second). After this duration, the barrier will decay.</param>
        public void SetBarrier(int amount, int duration)
        {
            MaxBarrierPoints = amount;
            BarrierPoints = amount;
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

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            HandleBarrier(ref modifiers);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            HandleBarrier(ref modifiers);
        }

        private void HandleBarrier(ref NPC.HitModifiers modifiers)
        {
            if (HasBarrier)
            {
                float incomingDamage = modifiers.FinalDamage.Base; // Get the base damage

                if (incomingDamage <= BarrierPoints)
                {
                    BarrierPoints -= (int)incomingDamage; // Fully absorbed
                    modifiers.FinalDamage.Base = 0; // No damage passes through
                }
                else
                {
                    float excessDamage = incomingDamage - BarrierPoints;
                    BarrierPoints = 0; // Deplete the barrier
                    modifiers.FinalDamage.Base = excessDamage; // Remaining damage applies
                }
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (HasBarrier)
            {
                Effect barrier = OvermorrowModFile.Instance.BarrierShader.Value;

                float alpha = 1f;

                barrier.Parameters["Time"].SetValue(Main.GameUpdateCount / 60f); // Time in seconds
                barrier.Parameters["NoiseSeed"].SetValue((float)(Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f)); // Random noise seed
                barrier.Parameters["Ratio"].SetValue(0.8f); // Ratio for noise blend
                barrier.Parameters["TintColor"].SetValue(new Vector4(0.3f, 0.6f, 1.0f, 1f)); // Tint color (RGBA)
                barrier.Parameters["Alpha"].SetValue(alpha);

                Main.graphics.GraphicsDevice.Textures[1] = OvermorrowModFile.Instance.BarrierNoiseTexture.Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                barrier.CurrentTechnique.Passes[0].Apply();

                //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Assets + "Sprites/NPCs/RAT").Value;
                Texture2D texture = TextureAssets.Npc[npc.type].Value;
                Rectangle drawRectangle = new Rectangle(0, 0, (int)(texture.Width), 70);
                spriteBatch.Draw(texture, npc.Center - screenPos, npc.frame, drawColor * alpha, npc.rotation, texture.Size() / 2, npc.scale, SpriteEffects.None,  0f);

                // Reset the sprite batch to avoid affecting other rendering
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}