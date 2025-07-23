using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Priority for fill rendering:
    /// <para>
    /// 1. CustomDrawFunction - if not null, calls this function to draw custom fill content
    /// </para>
    /// <para>
    /// 2. FillTexture - if CustomDrawFunction is null but FillTexture is not null, uses this texture
    /// </para>
    /// <para>
    /// 3. FillColor - if both above are null but FillColor has a value, uses solid color fill
    /// </para>
    /// 4. Original sprite - if all above are null, shows the original sprite texture inside the outline
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
        /// Return null to skip solid color fill (allows CustomDrawFunction, FillTexture, or original sprite to be used).
        /// Return a Color value to fill with that solid color.
        /// Note: CustomDrawFunction and FillTexture take priority over this if specified.
        /// </summary>
        Color? FillColor { get; }

        /// <summary>
        /// Optional custom texture to fill the inside of the outline.
        /// Return null to allow CustomDrawFunction, FillColor, or original sprite to be used.
        /// Return a Texture2D to use that texture as the fill.
        /// Note: CustomDrawFunction takes priority over this if both are specified.
        /// </summary>
        Texture2D FillTexture { get; }

        /// <summary>
        /// Optional custom drawing function for complete control over the fill content.
        /// This function is called when rendering the fill layer and allows you to draw anything you want.
        /// The shader will then use this rendered content as the fill that gets revealed through entity shapes.
        /// 
        /// Parameters:
        /// - SpriteBatch: The spriteBatch to draw with (already begun with appropriate settings)
        /// - GraphicsDevice: The graphics device for advanced operations
        /// - int screenWidth: Width of the render target
        /// - int screenHeight: Height of the render target
        /// - Entity entity: The entity being drawn (for position, scale, etc.)
        /// 
        /// Return null to use FillTexture, FillColor, or original sprite instead.
        /// 
        /// Example uses:
        /// - Animated textures with custom frame logic
        /// - Scrolling backgrounds with custom speed/direction
        /// - Procedural patterns
        /// - Multi-layered effects
        /// - Particle effects
        /// </summary>
        Action<SpriteBatch, GraphicsDevice, int, int, Entity> CustomDrawFunction { get; }
    }
}