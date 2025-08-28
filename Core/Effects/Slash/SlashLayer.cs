using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Defines a single visual layer of a slash effect
    /// </summary>
    public struct SlashLayer
    {
        /// <summary>
        /// Texture to use for this layer
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Color tint for this layer
        /// </summary>
        public Color Color;

        /// <summary>
        /// Width scale multiplier for this layer (1.0 = same as base width)
        /// </summary>
        public float WidthScale;

        /// <summary>
        /// Opacity of this layer (0.0 to 1.0)
        /// </summary>
        public float Opacity;

        /// <summary>
        /// Blend state for rendering this layer
        /// </summary>
        public BlendState BlendState;

        /// <summary>
        /// Texture flipping options
        /// </summary>
        public SpriteEffects SpriteEffects;

        /// <summary>
        /// Width multiplier at the start of the slash (0 = sharp point, 1 = full width)
        /// Use 0f for sharp sword-like start, 1f for no tapering at start
        /// </summary>
        public float StartTaper;

        /// <summary>
        /// Width multiplier at the end of the slash (0 = sharp point, 1 = full width)
        /// Use 0f for sharp ending point, 1f for no tapering at end
        /// </summary>
        public float EndTaper;

        /// <summary>
        /// How much of the slash length (0 to 1) is used for tapering on EACH end.
        /// 0.1f = 10% at start + 10% at end = 20% total tapering
        /// 0.2f = 20% at start + 20% at end = 40% total tapering
        /// 
        /// Examples:
        /// - Sharp start only: StartTaper=0f, EndTaper=1f, TaperLength=0.2f (start tapers over first 20%)
        /// - Sharp end only: StartTaper=1f, EndTaper=0f, TaperLength=0.2f (end tapers over last 20%)
        /// - No tapering: StartTaper=1f, EndTaper=1f, TaperLength=any (rectangle shape)
        /// - Both ends sharp: StartTaper=0f, EndTaper=0f, TaperLength=0.3f (diamond/leaf shape)
        /// </summary>
        public float TaperLength;

        /// <summary>
        /// Offset from center line (0 = center, positive = towards outer edge, negative = towards inner edge)
        /// </summary>
        public float Offset;

        /// <summary>
        /// Optional width curve function for variable width along the slash
        /// If null, uses constant width
        /// </summary>
        public Func<float, float> WidthCurve;

        /// <summary>
        /// Creates a basic slash layer
        /// </summary>
        public SlashLayer(Texture2D texture, Color color, float widthScale = 1f, float opacity = 1f)
        {
            Texture = texture;
            Color = color;
            WidthScale = widthScale;
            Opacity = opacity;
            Offset = 0f; // Default: no offset
            BlendState = BlendState.AlphaBlend;
            SpriteEffects = SpriteEffects.None;
            WidthCurve = null;
            StartTaper = 1f;  // Default: no tapering
            EndTaper = 1f;    // Default: no tapering
            TaperLength = 0.2f; // Default: 20% taper length
        }

        public SlashLayer(Texture2D texture, Color color, float widthScale, float opacity, BlendState blendState, SpriteEffects spriteEffects = SpriteEffects.None, Func<float, float> widthCurve = null)
        {
            Texture = texture;
            Color = color;
            WidthScale = widthScale;
            Opacity = opacity;
            Offset = 0f; // Default: no offset
            BlendState = blendState;
            SpriteEffects = spriteEffects;
            WidthCurve = widthCurve;
            StartTaper = 1f;  // Default: no tapering
            EndTaper = 1f;    // Default: no tapering
            TaperLength = 0.2f;
        }

        /// <summary>
        /// Gets the final color with opacity applied
        /// </summary>
        public Color FinalColor => Color * Opacity;

        /// <summary>
        /// Calculates the actual width for this layer given a base width
        /// </summary>
        public float GetWidth(float baseWidth, float t = 0f)
        {
            float scaledWidth = baseWidth * WidthScale;

            if (WidthCurve != null)
            {
                return WidthCurve(t);
            }

            return scaledWidth;
        }
    }
}