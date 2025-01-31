using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            // Default origins and offsets if not provided
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
            // Follow target starting from the last segment
            for (int i = Segments.Length - 1; i >= 0; i--)
            {
                if (i == Segments.Length - 1)
                {
                    Segments[i].Follow(target);
                }
                else
                {
                    Segments[i].Follow(Segments[i + 1].A);
                }
                Segments[i].Update();
            }

            // Recalculate positions starting from the base
            Segments[0].A = BasePosition;
            Segments[0].Recalculate();
            for (int i = 1; i < Segments.Length; i++)
            {
                Segments[i].A = Segments[i - 1].B;
                Segments[i].Recalculate();
            }
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