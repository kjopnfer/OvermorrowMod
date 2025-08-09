using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Defines the elliptical path that a slash effect follows
    /// </summary>
    public struct SlashPath
    {
        /// <summary>
        /// Center point of the ellipse in world coordinates
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Horizontal radius of the ellipse
        /// </summary>
        public float RadiusX;

        /// <summary>
        /// Vertical radius of the ellipse  
        /// </summary>
        public float RadiusY;

        /// <summary>
        /// Rotation of the entire ellipse in radians
        /// </summary>
        public float EllipseRotation;

        /// <summary>
        /// Starting angle on the ellipse in radians
        /// </summary>
        public float StartAngle;

        /// <summary>
        /// Ending angle on the ellipse in radians
        /// </summary>
        public float EndAngle;

        public SlashPath(Vector2 center, float radiusX, float radiusY, float ellipseRotation, float startAngle, float endAngle)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            EllipseRotation = ellipseRotation;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        /// <summary>
        /// Samples points along the elliptical arc
        /// </summary>
        /// <param name="segments">Number of segments to divide the arc into</param>
        /// <returns>List of points along the arc</returns>
        public List<Vector2> SampleArcPoints(int segments)
        {
            var points = new List<Vector2>();

            float angleSpan = EndAngle - StartAngle;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float currentAngle = StartAngle + (angleSpan * t);

                // Calculate point on ellipse
                float x = RadiusX * (float)Math.Cos(currentAngle);
                float y = RadiusY * (float)Math.Sin(currentAngle);

                // Apply ellipse rotation
                Vector2 rotatedPoint = new Vector2(x, y).RotatedBy(EllipseRotation);

                // Translate to world position
                Vector2 worldPoint = Center + rotatedPoint;
                points.Add(worldPoint);
            }

            return points;
        }

        /// <summary>
        /// Gets the direction vector (tangent) at a specific point along the arc
        /// </summary>
        /// <param name="t">Progress along arc (0 to 1)</param>
        /// <returns>Normalized direction vector</returns>
        public Vector2 GetDirectionAt(float t)
        {
            float angleSpan = EndAngle - StartAngle;
            float currentAngle = StartAngle + (angleSpan * t);

            // Calculate derivative for tangent direction
            float dx = -RadiusX * (float)Math.Sin(currentAngle);
            float dy = RadiusY * (float)Math.Cos(currentAngle);

            Vector2 direction = new Vector2(dx, dy).RotatedBy(EllipseRotation);
            return Vector2.Normalize(direction);
        }

        /// <summary>
        /// Gets a point at a specific progress along the arc
        /// </summary>
        /// <param name="t">Progress along arc (0 to 1)</param>
        /// <returns>World position at that progress</returns>
        public Vector2 GetPointAt(float t)
        {
            float angleSpan = EndAngle - StartAngle;
            float currentAngle = StartAngle + (angleSpan * t);

            float x = RadiusX * (float)Math.Cos(currentAngle);
            float y = RadiusY * (float)Math.Sin(currentAngle);

            Vector2 rotatedPoint = new Vector2(x, y).RotatedBy(EllipseRotation);
            return Center + rotatedPoint;
        }
    }
}