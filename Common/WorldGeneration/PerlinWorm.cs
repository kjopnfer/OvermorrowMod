using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common.WorldGeneration
{
    public class PerlinWorm
    {
        private Vector2 direction;
        private Vector2 position;
        private Vector2 endPosition;

        public float weight = 0.6f;

        public PerlinWorm(Vector2 startPosition, Vector2 endPosition)
        {
            position = startPosition;
            this.endPosition = endPosition;
        }

        public Vector2 MoveTowardsEndpoint()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            Vector2 direction = GetDirection();
            var directionToEndpoint = Vector2.Normalize(endPosition - position);
            var endDirection = Vector2.Normalize(direction * (1 - weight) + directionToEndpoint * weight);

            position += endDirection;

            return position;
        }

        private Vector2 GetDirection()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalOctaves(6);
            noise.SetFractalLacunarity(4f);
            noise.SetFrequency(0.025f);
            noise.SetFractalGain(0.1f);

            float degrees = MathHelper.Lerp(-90, 90, noise.GetNoise(position.X, position.Y));
            direction = Vector2.One.RotatedBy(degrees);

            return direction;
        }


        public virtual void OnRunStart(Vector2 position) { }

        public virtual void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, Main.rand.Next(4, 9), false);
        }

        public virtual void OnRunEnd(Vector2 position) { }

        protected int maxTries = 1000;
        public void Run(out Vector2 lastPosition)
        {
            OnRunStart(position);

            int currentIteration = 0;
            while (Vector2.Distance(endPosition, position) > 1 && currentIteration < maxTries)
            {
                MoveTowardsEndpoint();

                var logger = OvermorrowModFile.Instance.Logger;
                RunAction(position, endPosition, currentIteration);
                //WorldGen.PlaceTile((int)position.X, (int)position.Y, TileID.ObsidianBrick, true, true);
                currentIteration++;
            }

            lastPosition = position;
            OnRunEnd(position);
        }
    }
}