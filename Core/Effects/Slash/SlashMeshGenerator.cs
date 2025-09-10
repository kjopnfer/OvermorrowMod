using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Generates triangle strip meshes for slash effects
    /// </summary>
    public static class SlashMeshGenerator
    {
        /// <summary>
        /// Generates vertices for a triangle list that follows a slash path with tapering support
        /// </summary>
        /// <param name="path">The slash path to follow</param>
        /// <param name="width">Width of the slash</param>
        /// <param name="segments">Number of segments to divide the path into</param>
        /// <param name="color">Color to apply to all vertices</param>
        /// <param name="spriteEffects">Texture flipping options</param>
        /// <param name="startTaper">Width at start (0 = sharp point, 1 = full width)</param>
        /// <param name="endTaper">Width at end (0 = sharp point, 1 = full width)</param>
        /// <param name="taperLength">How much of slash length used for tapering each end (0-1)</param>
        /// <returns>List of vertices for rendering</returns>
        public static List<VertexPositionColorTexture> GenerateSlashMesh(SlashPath path, float width, int segments, Color color, SpriteEffects spriteEffects = SpriteEffects.None, float startTaper = 1f, float endTaper = 1f, float taperLength = 0.2f, float offset = 0f, float fadeProgress = 0f)
        {
            var vertices = new List<VertexPositionColorTexture>();
            for (int i = 0; i < segments; i++)
            {
                float t1 = (float)i / (float)segments;
                float t2 = (float)(i + 1) / (float)segments;

                // Calculate width with tapering
                float width1 = width * CalculateTaperFactor(t1, startTaper, endTaper, taperLength);
                float width2 = width * CalculateTaperFactor(t2, startTaper, endTaper, taperLength);

                // Get positions and directions at both points
                Vector2 pos1 = path.GetPointAt(t1);
                Vector2 pos2 = path.GetPointAt(t2);
                Vector2 direction1 = path.GetDirectionAt(t1);
                Vector2 direction2 = path.GetDirectionAt(t2);

                // Calculate perpendicular vectors for width
                Vector2 perpendicular1 = new Vector2(-direction1.Y, direction1.X);
                Vector2 perpendicular2 = new Vector2(-direction2.Y, direction2.X);

                // Create top and bottom points for both positions (using calculated widths)
                Vector2 centerOffset1 = perpendicular1 * offset; // Add this line
                Vector2 centerOffset2 = perpendicular2 * offset; // Add this line

                Vector2 pos1Top = pos1 + centerOffset1 + (perpendicular1 * width1 * 0.5f);
                Vector2 pos1Bottom = pos1 + centerOffset1 - (perpendicular1 * width1 * 0.5f);
                Vector2 pos2Top = pos2 + centerOffset2 + (perpendicular2 * width2 * 0.5f);
                Vector2 pos2Bottom = pos2 + centerOffset2 - (perpendicular2 * width2 * 0.5f);

                // Convert to screen space
                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                // Calculate UV coordinates with sprite effects
                float u1 = t1;
                float u2 = t2;
                float vTop = 0f;
                float vBottom = 1f;

                // Handle horizontal flip (flips along the arc direction)
                if ((spriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
                {
                    u1 = 1f - u1;
                    u2 = 1f - u2;
                }

                // Handle vertical flip (flips across the width)
                if ((spriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically)
                {
                    vTop = 1f;
                    vBottom = 0f;
                }

                float fadeAmount = EasingUtils.EaseOutQuint(fadeProgress);
                Color color1 = color * (1f - ((1f - t1) * fadeProgress));
                Color color2 = color * (1f - ((1f - t2) * fadeProgress));

                // Create two triangles for this segment (6 vertices total)
                // Triangle 1: pos1Top, pos1Bottom, pos2Top
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color1, new Vector2(u1, vTop)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(u1, vBottom)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(u2, vTop)));

                // Triangle 2: pos2Top, pos1Bottom, pos2Bottom
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(u2, vTop)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(u1, vBottom)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color2, new Vector2(u2, vBottom)));
            }
            return vertices;
        }

        /// <summary>
        /// Generates vertices with width variation along the path
        /// </summary>
        /// <param name="path">The slash path to follow</param>
        /// <param name="widthCurve">Function that returns width at progress t (0 to 1)</param>
        /// <param name="segments">Number of segments to divide the path into</param>
        /// <param name="color">Color to apply to all vertices</param>
        /// <returns>List of vertices ready for triangle strip rendering</returns>
        public static List<VertexPositionColorTexture> GenerateSlashMeshWithCurve(SlashPath path, Func<float, float> widthCurve, int segments, Color color, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;

                Vector2 centerPoint = path.GetPointAt(t);
                Vector2 direction = path.GetDirectionAt(t);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

                // Get width at this progress point
                float currentWidth = widthCurve(t);

                Vector2 topPoint = centerPoint + (perpendicular * currentWidth * 0.5f);
                Vector2 bottomPoint = centerPoint - (perpendicular * currentWidth * 0.5f);

                float u = t;

                vertices.Add(new VertexPositionColorTexture(
                    new Vector3(topPoint, 0),
                    color,
                    new Vector2(u, 0)
                ));

                vertices.Add(new VertexPositionColorTexture(
                    new Vector3(bottomPoint, 0),
                    color,
                    new Vector2(u, 1)
                ));
            }

            return vertices;
        }

        /// <summary>
        /// Calculates the width multiplier for a given position along the slash based on taper settings.
        /// 
        /// How tapering works:
        /// - First [taperLength] portion: transitions from startTaper to 1.0 (full width)
        /// - Middle portion: always 1.0 (full width)  
        /// - Last [taperLength] portion: transitions from 1.0 to endTaper
        /// 
        /// Common patterns:
        /// - Sharp start: startTaper=0f, endTaper=1f, taperLength=0.2f
        /// - Sharp end: startTaper=1f, endTaper=0f, taperLength=0.2f  
        /// - Sharp both: startTaper=0f, endTaper=0f, taperLength=0.3f
        /// - No taper: startTaper=1f, endTaper=1f, taperLength=any
        /// </summary>
        /// <param name="t">Position along slash (0 to 1)</param>
        /// <param name="startTaper">Width multiplier at start (0-1)</param>
        /// <param name="endTaper">Width multiplier at end (0-1)</param>
        /// <param name="taperLength">Length of taper regions (0-1)</param>
        /// <returns>Width multiplier for this position</returns>
        public static float CalculateTaperFactor(float t, float startTaper, float endTaper, float taperLength)
        {
            if (t <= taperLength)
            {
                // Start taper region: transition from startTaper to full width
                float taperProgress = t / taperLength;
                return MathHelper.Lerp(startTaper, 1f, taperProgress);
            }
            else if (t >= 1f - taperLength)
            {
                // End taper region: transition from full width to endTaper
                float taperProgress = (1f - t) / taperLength;
                return MathHelper.Lerp(endTaper, 1f, taperProgress);
            }
            else
            {
                // Middle region: always full width
                return 1f;
            }
        }

        /// <summary>
        /// Helper method to create a simple width curve that starts thin, gets thick in middle, ends thin
        /// </summary>
        public static float DefaultWidthCurve(float t, float maxWidth)
        {
            // Creates a curve that peaks in the middle
            float curve = (float)Math.Sin(t * Math.PI);
            return maxWidth * curve;
        }
    }
}