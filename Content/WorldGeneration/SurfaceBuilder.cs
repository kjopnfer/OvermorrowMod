using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.WorldGeneration;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class SurfaceBuilder : PerlinWorm
    {

        public int repeatWorm = 0;
        public int timesRepeated = 0;
        public Vector2 trueStartPoint = Vector2.Zero;
        public SurfaceBuilder(Vector2 startPosition, Vector2 endPosition, FastNoiseLite noise, int repeatWorm = 1) : base(startPosition, endPosition, noise)
        {
            this.repeatWorm = repeatWorm;
        }


        private bool endBranch = false;
        public int endDistance = 300;
        public override void OnRunStart(Vector2 position)
        {
            var logger = OvermorrowModFile.Instance.Logger;
            logger.Error("run: " + repeatWorm);

            endBranch = repeatWorm > 1;
        }

        public override void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            int size = 9;
            var logger = OvermorrowModFile.Instance.Logger;

            // Make the middle spawn area flatter 
            if (position.X >= (Main.maxTilesX / 7 * 3) && position.X <= (Main.maxTilesX / 7 * 4)) weight = 0.7f;
            else weight = 0.6f;

            bool withinBounds = position.X > 0 && position.X < Main.maxTilesX && position.Y > 0 && position.Y < Main.maxTilesY;

            // Fill in tiles below
            ushort tileType = TileID.Dirt;
            int runnerBlock = 20; // Don't use TileRunner for 20 tiles, which would make the surface more jagged
            //int runnerBlock = 1;

            int yOffset = 0;
            for (int y = (int)position.Y; y < Main.rockLayer; y++)
            {
                if (Framing.GetTileSafely((int)position.X, y + yOffset).HasTile) continue;

                if (runnerBlock > 0)
                {
                    if (runnerBlock == 20)
                    {
                        // This makes the terrain smoother by removing any sudden 1 block holes
                        if (Framing.GetTileSafely((int)position.X - 1, y - 1).HasTile)
                        {
                            //logger.Error("go up");
                            yOffset--;
                        }
                    }

                    if (withinBounds) WorldGen.PlaceTile((int)position.X, y + yOffset, tileType, true, true);

                    runnerBlock--;
                }
                else
                {
                    WorldGen.TileRunner((int)position.X, y + yOffset, size, 1, tileType, true);
                }
            }
        }

        public override void OnRunEnd(Vector2 endPosition)
        {
            if (repeatWorm > 1)
            {
                int yOffset = 0;
                if (endPosition.X <= (Main.maxTilesX / 7 * 3)) yOffset = 15; // Make the surface go down on the left side
                else if (endPosition.X >= (Main.maxTilesX / 7 * 4)) yOffset = -15; // Then make it go back up on the right side

                Vector2 nextEndpoint = endPosition + new Vector2(endDistance, yOffset);

                SurfaceBuilder branchWorm = new SurfaceBuilder(endPosition, nextEndpoint, noise, --repeatWorm);
                branchWorm.Run(out _);

                //SurfaceTunneler tunnel = new SurfaceTunneler(startPosition, branchEndpoint, noise);
                SurfaceTunneler tunnel = new SurfaceTunneler(endPosition, nextEndpoint + new Vector2(0, 75), noise);
                tunnel.Run(out _);
            }

            base.OnRunEnd(endPosition);
        }
    }

    public class SurfaceTunneler : PerlinWorm
    {
        public int repeatWorm = 0;

        public SurfaceTunneler(Vector2 startPosition, Vector2 endPosition, FastNoiseLite noise, int repeatWorm = 1) : base(startPosition, endPosition, noise)
        {
            invertDirection = -1;
            this.repeatWorm = repeatWorm;
        }


        private bool endBranch = false;
        public int endDistance = 300;
        public override void OnRunStart(Vector2 position)
        {
        }

        public override void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            int size = Main.rand.Next(2, 6);

            if (position.X >= (Main.maxTilesX / 7 * 3) && position.X <= (Main.maxTilesX / 7 * 4)) weight = 0.7f;
            else weight = 0.6f;

            // Push the tunnels downwards so they don't intersect with the spawn
            int yOffset = 65;
            if (position.X < (Main.maxTilesX / 7 * 3) || position.X > (Main.maxTilesX / 7 * 4))
            {

                float progress = ModUtils.EaseInQuart(position.X / (Main.maxTilesX / 7 * 3));
                int lerpOffset = (int)MathHelper.Lerp(75, 0, progress);
                if (position.X >= (Main.maxTilesX / 7 * 4))
                {
                    progress = ModUtils.EaseOutQuart((position.X - Main.maxTilesX / 7 * 4) / Main.maxTilesX);
                    lerpOffset = (int)MathHelper.Lerp(0, 75, progress);
                }

                WorldGen.digTunnel((int)position.X, (int)position.Y + lerpOffset, 0, 0, 1, size, false);

                if (Main.rand.NextBool(300) && lerpOffset > 25)
                {
                    Vector2 branchEndpoint = position + new Vector2(0, lerpOffset) + new Vector2(0, yOffset).RotatedBy(MathHelper.PiOver2);

                    SurfaceTunnelBranch branchWorm = new SurfaceTunnelBranch(position + new Vector2(0, lerpOffset), branchEndpoint, noise, Main.rand.Next(3, 5));
                    branchWorm.toSurface = true;
                    branchWorm.Run(out _);
                }
                //WorldGen.digTunnel((int)position.X, (int)position.Y + yOffset, 0, 0, 1, size, false);
            }
        }

        public override void OnRunEnd(Vector2 endPosition)
        {
            /*if (endPosition.X <= (Main.maxTilesX / 7 * 3) || endPosition.X >= (Main.maxTilesX / 7 * 4))
            {
                //if (Main.rand.NextBool(2))
                {
                    int lerpOffset = (int)MathHelper.Lerp(Main.rand.Next(50, 75), 0, ModUtils.EaseInQuart(endPosition.X / (Main.maxTilesX / 7 * 3)));
                    if (endPosition.X >= (Main.maxTilesX / 7 * 4)) lerpOffset = (int)MathHelper.Lerp(0, 75, ModUtils.EaseOutQuart((endPosition.X - Main.maxTilesX / 7 * 4) / Main.maxTilesX));

                    //bool toSurface = Main.rand.NextBool() ? true : false;
                    bool toSurface = false;
                    int yOffset = toSurface ? -100 : 50;
                    Vector2 branchEndpoint = endPosition + new Vector2(0, lerpOffset) + new Vector2(0, yOffset).RotateRandom(MathHelper.Pi);

                    SurfaceTunnelBranch branchWorm = new SurfaceTunnelBranch(endPosition + new Vector2(0, lerpOffset), branchEndpoint, noise, Main.rand.Next(2, 4));
                    //branchWorm.toSurface = toSurface;
                    branchWorm.Run(out _);
                }
            }*/

            base.OnRunEnd(endPosition);
        }
    }

    public class SurfaceTunnelBranch : PerlinWorm
    {
        public int repeatWorm = 0;
        public bool toSurface = false;
        public SurfaceTunnelBranch(Vector2 startPosition, Vector2 endPosition, FastNoiseLite noise, int repeatWorm = 1) : base(startPosition, endPosition, noise)
        {
            invertDirection = -1;
            weight = Main.rand.NextFloat(0.5f, 0.6f);
            this.repeatWorm = repeatWorm;
        }

        public override void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            int size = Main.rand.Next(3, 7);
            if (toSurface) size = Main.rand.Next(1, 4);
            //if (Vector2.Distance(position, endPosition) < 25) size = Main.rand.Next(8, 12);

            WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, size);
        }

        public int endDistance = 50;
        public override void OnRunEnd(Vector2 position)
        {
            if (repeatWorm > 1)
            {
                //Vector2 branchEndpoint = position + new Vector2(endDistance * 0.5f, 0).RotateRandom(MathHelper.Pi);
                int invert = toSurface ? -1 : -1;
                int startBound = toSurface ? 75 : 50;
                int endBound = toSurface ? 150 : 75;

                Vector2 branchEndpoint = endPosition + new Vector2(Main.rand.Next(startBound, endBound) * invert, Main.rand.Next(startBound, endBound) * invert).RotateRandom(MathHelper.Pi);
                if (toSurface) branchEndpoint = endPosition + new Vector2(Main.rand.Next(startBound, endBound) * invert, Main.rand.Next(startBound, endBound) * invert).RotatedBy(MathHelper.PiOver2);

                SurfaceTunnelBranch branchWorm = new SurfaceTunnelBranch(position, branchEndpoint, noise, --repeatWorm);
                //weight = Main.rand.NextFloat(0.2f, 0.6f);
                //branchWorm.branchChance = branchChance * 2;
                branchWorm.toSurface = toSurface;
                branchWorm.Run(out _);
            }
            else
            {
                if (toSurface) return;

                // Creates an open chamber at the end of the run
                bool withinBounds = position.X > 0 && position.X < Main.maxTilesX && position.Y > 0 && position.Y < Main.maxTilesY;
                //WorldGen.digTunnel(position.X, position.Y, -8, 0, Main.rand.Next(20, 28), Main.rand.Next(4, 8), true);
                if (withinBounds)
                {
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        ShapeData slimeShapeData = new ShapeData();

                        float xScale = 0.8f + Main.rand.NextFloat() * 0.5f; // Randomize the width of the shrine area
                        float yScale = Main.rand.NextFloat(0.6f, 0.8f);
                        int radius = Main.rand.Next(32, 48);

                        WorldUtils.Gen(new Point((int)position.X + -15 * i, (int)position.Y + Main.rand.Next(-5, 5)), new Shapes.Slime(20, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));
                    }
                }
            }

            base.OnRunEnd(position);
        }
    }
}