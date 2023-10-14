using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Common.WorldGeneration
{
    public class PerlinWorm
    {
        private Vector2 direction;
        private Vector2 currentPosition;
        protected Vector2 startPosition;
        protected Vector2 endPosition;

        protected FastNoiseLite noise;

        /// <summary>
        /// Controls the curve amount, higher number means less curve. Lower number means more curve.
        /// </summary>
        public float weight = 0.6f;


        public PerlinWorm(Vector2 startPosition, Vector2 endPosition, FastNoiseLite noise)
        {
            currentPosition = startPosition;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.noise = noise;
        }

        public Vector2 MoveTowardsEndpoint()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            Vector2 direction = GetDirection();
            var directionToEndpoint = Vector2.Normalize(endPosition - currentPosition);
            var endDirection = Vector2.Normalize(direction * (1 - weight) + directionToEndpoint * weight);

            var newPosition = currentPosition + endDirection;
            int fail = 0; // Regenerates a new noise function based on the seed

            weight = 0.6f;
            //if (RetryCondition(newPosition, fail)) return currentPosition;

            /*while (RetryCondition(newPosition, fail))
            {
                direction = GetDirection();
                directionToEndpoint = Vector2.Normalize(endPosition - currentPosition);
                endDirection = Vector2.Normalize(direction * (1 - weight) + directionToEndpoint * weight);

                newPosition = currentPosition + endDirection;
                logger.Debug(newPosition);

                fail++;
            }*/

            return currentPosition += endDirection;
        }

        /// <summary>
        /// Returns false by default.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fail"></param>
        /// <returns></returns>
        private bool RetryCondition(Vector2 position, int fail)
        {
            if (position.Y > Main.worldSurface || position.Y < Main.worldSurface * 0.7f)
            {
                /*noise = new FastNoiseLite(WorldGen._genRandSeed + fail);
                noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                noise.SetFractalOctaves(6);
                noise.SetFractalLacunarity(4);
                noise.SetFrequency(0.025f);
                noise.SetFractalGain(0.1f);*/

                invertDirection *= -1;

                return true;
            }

            return false;
        }

        int invertDirection = 1;
        private Vector2 GetDirection()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            // Increasing turn amount makes it more jagged, decreasing it makes it smoother
            float turnAmount = 90; // 45
            float scale = 0.6f; // 0.4

            float degrees = MathHelper.Lerp(-turnAmount, turnAmount, noise.GetNoise(currentPosition.X, currentPosition.Y) * scale);
            direction = Vector2.One.RotatedBy(degrees);

            // if fail invert noise value?
            return direction;
        }


        public virtual void OnRunStart(Vector2 position) { }

        public virtual void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            //WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, Main.rand.Next(4, 9), false);
        }

        public virtual void OnRunEnd(Vector2 position) { }

        protected int maxTries = 1000;
        public void Run(out Vector2 lastPosition)
        {
            OnRunStart(currentPosition);

            int currentIteration = 0;
            while (Vector2.Distance(endPosition, currentPosition) > 1 && currentIteration < maxTries)
            {
                MoveTowardsEndpoint();

                var logger = OvermorrowModFile.Instance.Logger;
                RunAction(currentPosition, endPosition, currentIteration);
                //WorldGen.PlaceTile((int)position.X, (int)position.Y, TileID.ObsidianBrick, true, true);
                currentIteration++;
            }

            lastPosition = currentPosition;
            OnRunEnd(currentPosition);
        }
    }
}