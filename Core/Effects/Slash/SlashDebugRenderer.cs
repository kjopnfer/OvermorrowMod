using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Helper class for debugging slash paths visually
    /// </summary>
    public static class SlashDebugRenderer
    {
        /// <summary>
        /// Draws debug visualization of a SlashPath
        /// Call this in your projectile's PreDraw method
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        /// <param name="path">SlashPath to visualize</param>
        /// <param name="segments">Number of debug points to draw</param>
        public static void DrawPath(SpriteBatch spriteBatch, SlashPath path, int segments = 20)
        {
            var points = path.SampleArcPoints(segments);

            // Draw center point in yellow
            DrawDebugDot(spriteBatch, path.Center, Color.Yellow, 4f);

            // Draw arc points in white
            for (int i = 0; i < points.Count; i++)
            {
                Color pointColor = Color.White;

                // Start point in green, end point in red
                if (i == 0) pointColor = Color.Green;
                else if (i == points.Count - 1) pointColor = Color.Red;

                DrawDebugDot(spriteBatch, points[i], pointColor, 3f);
            }

            // Draw direction arrows at a few points
            for (int i = 0; i < 5; i++)
            {
                float t = i / 4f; // 0, 0.25, 0.5, 0.75, 1.0
                Vector2 point = path.GetPointAt(t);
                Vector2 direction = path.GetDirectionAt(t);

                DrawDebugArrow(spriteBatch, point, direction, Color.Cyan, 20f);
            }
        }

        private static void DrawDebugDot(SpriteBatch spriteBatch, Vector2 worldPosition, Color color, float size)
        {
            Vector2 screenPosition = worldPosition - Main.screenPosition;

            var dotRect = new Rectangle(
                (int)(screenPosition.X - size / 2),
                (int)(screenPosition.Y - size / 2),
                (int)size,
                (int)size
            );

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dotRect, color);
        }

        private static void DrawDebugArrow(SpriteBatch spriteBatch, Vector2 worldPosition, Vector2 direction, Color color, float length)
        {
            Vector2 screenPosition = worldPosition - Main.screenPosition;
            Vector2 endPoint = screenPosition + (direction * length);

            DrawDebugLine(spriteBatch, screenPosition, endPoint, color, 2f);

            // Draw arrowhead
            Vector2 arrowHead1 = endPoint + (direction.RotatedBy(MathHelper.Pi * 0.75f) * 8f);
            Vector2 arrowHead2 = endPoint + (direction.RotatedBy(-MathHelper.Pi * 0.75f) * 8f);

            DrawDebugLine(spriteBatch, endPoint, arrowHead1, color, 2f);
            DrawDebugLine(spriteBatch, endPoint, arrowHead2, color, 2f);
        }

        private static void DrawDebugLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 direction = end - start;
            float distance = direction.Length();
            float rotation = direction.ToRotation();

            var lineRect = new Rectangle(
                (int)start.X,
                (int)(start.Y - thickness / 2),
                (int)distance,
                (int)thickness
            );

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, lineRect, null, color, rotation, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}