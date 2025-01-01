using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract class OvermorrowNPC : ModNPC
    {
        protected virtual void DrawBestiary(SpriteBatch spriteBatch, Color drawColor) { }

        public virtual bool DrawNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;
        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                DrawBestiary(spriteBatch, drawColor);
                return false;
            }

            return DrawNPC(spriteBatch, screenPos, drawColor);
        }
    }
}