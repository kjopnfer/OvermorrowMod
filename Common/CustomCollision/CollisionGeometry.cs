using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OvermorrowMod.Common.CustomCollision
{
    /// <summary>
    /// Shared geometry helpers for the line-segment colliders held by TileCollisionNPC.
    /// All endpoints are in world coordinates.
    /// </summary>
    public static class CollisionGeometry
    {
        /// <summary>
        /// Returns the surface height at the given world X along a line segment,
        /// or false if worldX is outside the segment's X range.
        /// </summary>
        public static bool TryGetSurfaceHeight(Vector2 segmentStart, Vector2 segmentEnd, float worldX, out float surfaceY)
        {
            float minX = segmentStart.X < segmentEnd.X ? segmentStart.X : segmentEnd.X;
            float maxX = segmentStart.X < segmentEnd.X ? segmentEnd.X : segmentStart.X;

            if (worldX < minX || worldX > maxX)
            {
                surfaceY = 0f;
                return false;
            }

            float dx = segmentEnd.X - segmentStart.X;
            if (dx == 0f)
            {
                surfaceY = segmentStart.Y < segmentEnd.Y ? segmentStart.Y : segmentEnd.Y;
                return true;
            }

            float t = (worldX - segmentStart.X) / dx;
            surfaceY = segmentStart.Y + (segmentEnd.Y - segmentStart.Y) * t;
            return true;
        }

        /// <summary>
        /// Treats a segment as a thin rotated rectangle of the given width and tests
        /// intersection against an axis-aligned hitbox.
        /// </summary>
        public static bool IsRectangleIntersectingSlope(Vector2 start, Vector2 end, float width, Rectangle hitbox)
        {
            Vector2 direction = Vector2.Normalize(end - start);
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * width * 0.5f;

            Vector2 p1 = start - perpendicular;
            Vector2 p2 = start + perpendicular;
            Vector2 p3 = end + perpendicular;
            Vector2 p4 = end - perpendicular;

            Vector2[] slopePolygon = new[] { p1, p2, p3, p4 };
            return PolygonIntersectsRectangle(slopePolygon, hitbox);
        }

        /// <summary>
        /// SAT test between a polygon and an axis-aligned rectangle.
        /// </summary>
        public static bool PolygonIntersectsRectangle(Vector2[] polygon, Rectangle rectangle)
        {
            Vector2[] rectPolygon = new Vector2[]
            {
                new Vector2(rectangle.Left, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Bottom),
                new Vector2(rectangle.Left, rectangle.Bottom)
            };

            return PolygonsIntersect(polygon, rectPolygon);
        }

        /// <summary>
        /// Separating Axis Theorem polygon intersection test.
        /// </summary>
        public static bool PolygonsIntersect(Vector2[] poly1, Vector2[] poly2)
        {
            void ProjectPolygon(Vector2 axis, Vector2[] polygon, out float min, out float max)
            {
                min = Vector2.Dot(axis, polygon[0]);
                max = min;
                for (int i = 1; i < polygon.Length; i++)
                {
                    float projection = Vector2.Dot(axis, polygon[i]);
                    if (projection < min) min = projection;
                    if (projection > max) max = projection;
                }
            }

            bool Overlaps(float minA, float maxA, float minB, float maxB) => maxA >= minB && maxB >= minA;

            List<Vector2> axes = new List<Vector2>();
            for (int i = 0; i < poly1.Length; i++)
                axes.Add(Vector2.Normalize(new Vector2(-(poly1[(i + 1) % poly1.Length] - poly1[i]).Y, (poly1[(i + 1) % poly1.Length] - poly1[i]).X)));

            for (int i = 0; i < poly2.Length; i++)
                axes.Add(Vector2.Normalize(new Vector2(-(poly2[(i + 1) % poly2.Length] - poly2[i]).Y, (poly2[(i + 1) % poly2.Length] - poly2[i]).X)));

            foreach (var axis in axes)
            {
                ProjectPolygon(axis, poly1, out float min1, out float max1);
                ProjectPolygon(axis, poly2, out float min2, out float max2);
                if (!Overlaps(min1, max1, min2, max2)) return false;
            }

            return true;
        }
    }
}
