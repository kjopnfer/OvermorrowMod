using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Core.Interfaces
{
    public interface IOutlineEntity
    {
        bool ShouldDrawOutline { get; }
        Color OutlineColor { get; }
        Color FillColor { get; }
        bool UseFillColor { get; }

        bool DrawForOutline(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor) => true;
    }
}