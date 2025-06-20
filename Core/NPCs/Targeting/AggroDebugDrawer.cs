using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Helper class for drawing debug visualizations of AggroRadius shapes.
    /// </summary>
    public static class AggroDebugDrawer
    {
        /// <summary>
        /// Draws an AggroRadius shape for debugging purposes.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        /// <param name="aggroRadius">The AggroRadius to visualize</param>
        /// <param name="center">The center position to draw from</param>
        /// <param name="npcDirection">The NPC's facing direction</param>
        /// <param name="color">The color to draw the outline</param>
        /// <param name="segments">Number of segments to use for the outline (higher = smoother)</param>
        public static void DrawAggroRadius(SpriteBatch spriteBatch, AggroRadius aggroRadius, Vector2 center,
            int npcDirection, Color color, int segments = 32)
        {
            if (aggroRadius == null) return;

            Vector2[] points = new Vector2[segments + 1];

            // Calculate points around the perimeter of the aggro shape
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)(i * MathHelper.TwoPi / segments);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                float radius = aggroRadius.GetRadiusInDirection(direction, npcDirection);
                Vector2 point = center + direction * radius;

                // Convert world position to screen position
                points[i] = point - Main.screenPosition;
            }
            
            var pixel = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < segments; i++)
            {
                DrawLine(spriteBatch, pixel, points[i], points[i + 1], color, 2f);
            }
        }

        /// <summary>
        /// Draws multiple aggro radius shapes with different colors for a complete visualization.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        /// <param name="config">The targeting configuration to visualize</param>
        /// <param name="center">The center position to draw from</param>
        /// <param name="npcDirection">The NPC's facing direction</param>
        public static void DrawTargetingDebug(SpriteBatch spriteBatch, NPCTargetingConfig config, Vector2 center, int npcDirection)
        {
            if (!config.ShowDebugVisualization) return;

            // Draw alert radius in blue (outermost)
            if (config.AlertRadius != null)
            {
                DrawAggroRadius(spriteBatch, config.AlertRadius, center, npcDirection, Color.Blue * 0.6f);
            }

            // Draw target radius in red (middle)
            if (config.TargetRadius != null)
            {
                DrawAggroRadius(spriteBatch, config.TargetRadius, center, npcDirection, Color.Red * 0.8f);
            }

            // Draw attack radius in orange (innermost)
            if (config.AttackRadius != null)
            {
                DrawAggroRadius(spriteBatch, config.AttackRadius, center, npcDirection, Color.Orange);
            }

            // Draw center point
            var pixel = TextureAssets.MagicPixel.Value;
            Vector2 screenCenter = center - Main.screenPosition;
            spriteBatch.Draw(pixel, new Rectangle((int)screenCenter.X - 2, (int)screenCenter.Y - 2, 4, 4), Color.White);

            // Draw direction indicator
            Vector2 directionEnd = center + new Vector2(npcDirection * 20, 0);
            DrawLine(spriteBatch, pixel, screenCenter, directionEnd - Main.screenPosition, Color.Yellow, 3f);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        private static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(texture,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
                null,
                color,
                angle,
                new Vector2(0, 0.5f),
                SpriteEffects.None,
                0f);
        }

        /// <summary>
        /// Checks if a point is within the screen bounds (with some padding) to avoid unnecessary drawing.
        /// </summary>
        public static bool IsOnScreen(Vector2 worldPosition, float padding = 100f)
        {
            Vector2 screenPos = worldPosition - Main.screenPosition;
            return screenPos.X > -padding && screenPos.X < Main.screenWidth + padding &&
                   screenPos.Y > -padding && screenPos.Y < Main.screenHeight + padding;
        }
    }
}