using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract class OvermorrowNPC : ModNPC
    {
        public ref Player Player => ref Main.player[NPC.target];

        protected virtual void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor) { }

        /// <summary>
        /// Is called before <see cref="DrawOvermorrowNPC(SpriteBatch, Vector2, Color)"/>, which will always draw behind.
        /// The SpriteBatch calls here will not be captured by RenderTargets such as the NPCBarrierRenderer.
        /// </summary>
        public virtual void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }

        /// <summary>
        /// The replacement for PreDraw. Everything drawn in here can be captured by a RenderTarget.
        /// </summary>
        public virtual bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;
        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                DrawNPCBestiary(spriteBatch, drawColor);
                return false;
            }

            DrawBehindOvermorrowNPC(spriteBatch, screenPos, drawColor);

            return DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }
    }
}