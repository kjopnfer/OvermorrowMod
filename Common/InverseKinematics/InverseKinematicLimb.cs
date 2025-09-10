using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.InverseKinematics;
using System;
using Terraria;

namespace OvermorrowMod.Common.InverseKinematics
{
    public class InverseKinematicLimb
    {
        public Vector2 BasePosition { get; set; }
        public InverseKinematicSegment[] Segments;

        public InverseKinematicLimb(float x, float y, int numSegments, float[] segmentLengths, float initialAngle, Texture2D[] segmentTextures, Vector2[] origins = null)
        {
            if (segmentLengths.Length != numSegments || (origins != null && origins.Length != numSegments))
            {
                Main.NewText("The number of segment lengths, origins, and offsets must match the number of segments.");
            }

            BasePosition = new Vector2(x, y);
            Segments = new InverseKinematicSegment[numSegments];

            // Default origins if not provided
            origins = origins ?? new Vector2[numSegments]; // Default to Vector2.Zero

            // Create the first segment at the base
            Segments[0] = new InverseKinematicSegment(x, y, segmentLengths[0], initialAngle, segmentTextures[0], origins[0]);

            // Create the remaining segments with their respective lengths
            for (int i = 1; i < numSegments; i++)
            {
                Segments[i] = new InverseKinematicSegment(0, 0, segmentLengths[i], 0, segmentTextures[i], origins[i]);
                Segments[i - 1].Parent = Segments[i];
            }
        }

        /// <summary>
        /// Moves it towards this position.
        /// </summary>
        /// <param name="target"></param>
        public void Update(Vector2 target)
        {
            // FABRIK algorithm with angle constraints

            // Step 1: Check if target is reachable
            float totalLength = 0;
            for (int i = 0; i < Segments.Length; i++)
            {
                totalLength += Segments[i].Length;
            }

            float distanceToTarget = Vector2.Distance(BasePosition, target);

            if (distanceToTarget > totalLength)
            {
                // Target is unreachable, stretch towards it
                Vector2 direction = (target - BasePosition).SafeNormalize(Vector2.Zero);
                Segments[0].A = BasePosition;
                for (int i = 0; i < Segments.Length; i++)
                {
                    float desiredAngle = direction.ToRotation();
                    Segments[i].Angle = ApplyAngleConstraints(i, desiredAngle);
                    Segments[i].Recalculate();
                    if (i < Segments.Length - 1)
                    {
                        Segments[i + 1].A = Segments[i].B;
                    }
                }
                return;
            }

            // Step 2: FABRIK forward pass (from end to base)
            Vector2[] positions = new Vector2[Segments.Length + 1];

            // Initialize positions with current segment positions
            positions[0] = BasePosition;
            for (int i = 0; i < Segments.Length; i++)
            {
                positions[i + 1] = Segments[i].B;
            }

            // Forward pass - start from target and work backwards
            positions[Segments.Length] = target;
            for (int i = Segments.Length - 1; i >= 0; i--)
            {
                Vector2 direction = (positions[i] - positions[i + 1]).SafeNormalize(Vector2.Zero);
                positions[i] = positions[i + 1] + direction * Segments[i].Length;
            }

            // Step 3: FABRIK backward pass with angle constraints (from base to end)
            positions[0] = BasePosition;
            for (int i = 0; i < Segments.Length; i++)
            {
                Vector2 direction = (positions[i + 1] - positions[i]).SafeNormalize(Vector2.Zero);
                float desiredAngle = direction.ToRotation();

                // Apply angle constraints
                float constrainedAngle = ApplyAngleConstraints(i, desiredAngle);

                // Recalculate position based on constrained angle
                Vector2 constrainedDirection = new Vector2((float)Math.Cos(constrainedAngle), (float)Math.Sin(constrainedAngle));
                positions[i + 1] = positions[i] + constrainedDirection * Segments[i].Length;

                // Update segment
                Segments[i].A = positions[i];
                Segments[i].Angle = constrainedAngle;
                Segments[i].Recalculate();
            }
        }

        private float ApplyAngleConstraints(int segmentIndex, float desiredAngle)
        {
            var segment = Segments[segmentIndex];

            while (desiredAngle > MathHelper.Pi) desiredAngle -= MathHelper.TwoPi;
            while (desiredAngle < -MathHelper.Pi) desiredAngle += MathHelper.TwoPi;

            // Apply constraints
            return MathHelper.Clamp(desiredAngle, segment.MinAngle, segment.MaxAngle);
        }

        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            // Draw each segment, customize the color as needed
            foreach (InverseKinematicSegment segment in Segments)
            {
                segment.Draw(spriteBatch, drawColor);
            }
        }

        /// <summary>
        /// Gets the position of the last segment's endpoint, which represents the moving limb's tip.
        /// </summary>
        /// <returns>The <see cref="Vector2"/> position of the last segment's endpoint.</returns>
        public Vector2 GetEndPosition()
        {
            // Return the 'B' position of the last segment, which is the endpoint of the last limb
            return Segments[Segments.Length - 1].B;
        }
    }
}