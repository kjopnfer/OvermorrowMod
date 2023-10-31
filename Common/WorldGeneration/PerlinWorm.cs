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

            return currentPosition += endDirection;
        }

        protected int invertDirection = 1;
        private Vector2 GetDirection()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            // Increasing turn amount makes it more jagged, decreasing it makes it smoother
            float turnAmount = 90; // 45
            float scale = 0.6f; // 0.4

            float degrees = MathHelper.Lerp(-turnAmount * invertDirection, turnAmount * invertDirection, noise.GetNoise(currentPosition.X, currentPosition.Y) * scale);
            direction = Vector2.One.RotatedBy(degrees);

            return direction;
        }


        public virtual void OnRunStart(Vector2 position) { }

        public virtual void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            //WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, Main.rand.Next(4, 9), false);
        }

        public virtual void OnRunEnd(Vector2 endPosition) { }

        protected int maxTries = 1000;
        public void Run(out Vector2 lastPosition)
        {
            OnRunStart(currentPosition);

            int currentIteration = 0;
            while (Vector2.Distance(currentPosition, endPosition) > 1 && currentIteration < maxTries)
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