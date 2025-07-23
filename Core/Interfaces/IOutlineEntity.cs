using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Priority:
    /// 1. FillTexture - if not null, uses this texture
    /// 2. FillColor - if FillTexture is null but FillColor has a value, uses solid color fill
    /// 3. Original sprite - if both are null, shows the original sprite texture inside the outline
    /// </summary>
    public interface IOutlineEntity
    {
        /// <summary>
        /// Whether this entity should have an outline drawn.
        /// Return false to skip outline rendering entirely.
        /// </summary>
        bool ShouldDrawOutline { get; }

        /// <summary>
        /// The color of the outline border.
        /// This color is always used when ShouldDrawOutline is true.
        /// </summary>
        Color OutlineColor { get; }

        /// <summary>
        /// Optional solid color to fill the inside of the outline.
        /// Return null to skip solid color fill (allows FillTexture or original sprite to be used).
        /// Return a Color value to fill with that solid color.
        /// Note: FillTexture takes priority over this if both are specified.
        /// </summary>
        Color? FillColor { get; }

        /// <summary>
        /// Optional custom texture to fill the inside of the outline.
        /// Return null to allow FillColor or original sprite to be used.
        /// Return a Texture2D to use that texture as the fill.
        /// Note: This has the highest priority - if specified, FillColor is ignored.
        /// </summary>
        Texture2D FillTexture { get; }
    }
}