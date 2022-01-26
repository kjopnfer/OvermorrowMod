using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Effects.Explosions
{
    public class CircularExplosionGenerator
    {
        /// <summary>
        /// Set this to true once the generator can be removed if there are no more
        /// animations playing.
        /// </summary>
        public bool Finished { get; set; }
        /// <summary>
        /// Current animation tick.
        /// </summary>
        public int CurrentTick { get; set; }
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Radius in pixels.
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// Color of effects.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Duration in ticks. Can be modified to cut off all animations early.
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Maximum life time in ticks, generator will not be removed until all animations are done.
        /// </summary>
        public int MaxLifeTime { get; set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">Texture to use, pixels are read column-wise</param>
        /// <param name="radius">Radius in pixels of resulting circular pattern</param>
        /// <param name="color">Multiplicative color</param>
        /// <param name="maxLifeTime">Maximum life time of this generator. No matter what
        /// it will last until all animations have played out. Set this to make sure that
        /// the generator is killed once it is done, even if it is never manually set to
        /// "Finished".
        /// For example in projectiles, since Kill might not be called.</param>
        public CircularExplosionGenerator(Texture2D texture, float radius, Color color, int maxLifeTime)
        {
            Texture = texture;
            Radius = radius;
            Color = color;
            CurrentTick = 0;
            Duration = texture.Width;
            MaxLifeTime = maxLifeTime;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">Texture to use given by index in ExplosionManager texture list
        /// , pixels are read column-wise</param>
        /// <param name="radius">Radius in pixels of resulting circular pattern</param>
        /// <param name="color">Multiplicative color</param>
        /// <param name="maxLifeTime">Maximum life time of this generator. No matter what
        /// it will last until all animations have played out. Set this to make sure that
        /// the generator is killed once it is done, even if it is never manually set to
        /// "Finished".
        /// For example in projectiles, since Kill might not be called.</param>
        public CircularExplosionGenerator(int texture, float radius, Color color, int maxLifeTime) :
            this(ExplosionManager.CircularExplosionTextures[texture], radius, color, maxLifeTime)
        {
        }

        public VertexPositionCenterTick[] GenerateVertices(Vector2 center)
        {
            var res = new VertexPositionCenterTick[4];
            res[0] = new VertexPositionCenterTick(
                new Vector3(center.X - Radius, center.Y + Radius, 0), center, CurrentTick);
            res[1] = new VertexPositionCenterTick(
                new Vector3(center.X - Radius, center.Y - Radius, 0), center, CurrentTick);
            res[2] = new VertexPositionCenterTick(
                new Vector3(center.X + Radius, center.Y + Radius, 0), center, CurrentTick);
            res[3] = new VertexPositionCenterTick(
                new Vector3(center.X + Radius, center.Y - Radius, 0), center, CurrentTick);
            return res;
        }

        public static int[] GetIndices(int count)
        {
            var indices = new int[count * 6];
            // Two triangles, need to do it this way instead of strips in order to batch sprites
            for (int i = 0; i < count; i++)
            {
                indices[i * 6 + 0] = i * 4 + 0;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;

                indices[i * 6 + 3] = i * 4 + 1;
                indices[i * 6 + 4] = i * 4 + 2;
                indices[i * 6 + 5] = i * 4 + 3;
            }
            return indices;
        }
    }
}
