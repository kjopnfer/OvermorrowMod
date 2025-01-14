using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class Test : ModNPC
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AbigailMinion}";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.aiStyle = 3;

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Effect barrier = OvermorrowModFile.Instance.BarrierShader.Value;

            barrier.Parameters["Time"].SetValue(Main.GameUpdateCount / 60f); // Time in seconds
            barrier.Parameters["NoiseSeed"].SetValue((float)(Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f)); // Random noise seed
            barrier.Parameters["Ratio"].SetValue(0.6f); // Ratio for noise blend
            barrier.Parameters["TintColor"].SetValue(new Vector4(0.3f, 0.6f, 1.0f, 1.0f)); // Tint color (RGBA)

            Main.graphics.GraphicsDevice.Textures[1] = OvermorrowModFile.Instance.BarrierNoiseTexture.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            barrier.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(
                TextureAssets.Npc[NPC.type].Value,  // NPC texture
                NPC.Center - screenPos,             // Screen position
                null,                          // Source rectangle
                drawColor,                          // Color tint
                NPC.rotation,                       // Rotation
                NPC.frame.Size() / 2,               // Origin point
                NPC.scale,                          // Scale
                SpriteEffects.None,                 // Sprite effects
                0f                                  // Layer depth
            );

            // Reset the sprite batch to avoid affecting other rendering
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            return false;
        }
    }
}