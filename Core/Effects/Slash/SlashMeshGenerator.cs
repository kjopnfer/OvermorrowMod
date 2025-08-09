using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// Generates vertices for a triangle strip that follows a slash path
        /// </summary>
        /// <param name="path">The slash path to follow</param>
        /// <param name="width">Width of the slash</param>
        /// <param name="segments">Number of segments to divide the path into</param>
        /// <param name="color">Color to apply to all vertices</param>
        /// <returns>List of vertices ready for triangle strip rendering</returns>
        public static List<VertexPositionColorTexture> GenerateSlashMesh(SlashPath path, float width, int segments, Color color)
        {
            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 0; i < segments; i++)
            {
                float t1 = (float)i / (float)segments;
                float t2 = (float)(i + 1) / (float)segments;

                // Get positions and directions at both points
                Vector2 pos1 = path.GetPointAt(t1);
                Vector2 pos2 = path.GetPointAt(t2);
                Vector2 direction1 = path.GetDirectionAt(t1);
                Vector2 direction2 = path.GetDirectionAt(t2);

                // Calculate perpendicular vectors for width
                Vector2 perpendicular1 = new Vector2(-direction1.Y, direction1.X);
                Vector2 perpendicular2 = new Vector2(-direction2.Y, direction2.X);

                // Create top and bottom points for both positions
                Vector2 pos1Top = pos1 + (perpendicular1 * width * 0.5f);
                Vector2 pos1Bottom = pos1 - (perpendicular1 * width * 0.5f);
                Vector2 pos2Top = pos2 + (perpendicular2 * width * 0.5f);
                Vector2 pos2Bottom = pos2 - (perpendicular2 * width * 0.5f);

                // Convert to screen space
                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                // UV coordinates
                float u1 = t1;
                float u2 = t2;

                // Create two triangles for this segment (6 vertices total)
                // Triangle 1: pos1Top, pos1Bottom, pos2Top
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color, new Vector2(u1, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color, new Vector2(u1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color, new Vector2(u2, 0f)));

                // Triangle 2: pos2Top, pos1Bottom, pos2Bottom
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color, new Vector2(u2, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color, new Vector2(u1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color, new Vector2(u2, 1f)));
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
        public static List<VertexPositionColorTexture> GenerateSlashMeshWithCurve(SlashPath path, Func<float, float> widthCurve, int segments, Color color)
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