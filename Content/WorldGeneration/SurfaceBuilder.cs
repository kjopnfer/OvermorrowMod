using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Common.WorldGeneration;
using OvermorrowMod.Content.Tiles.Ambient;
using OvermorrowMod.Content.Tiles.Underground;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
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

            if (!endBranch)
            {
                float progress = Utils.Clamp((currentIteration) / (maxTries * 0.2f), 0, 1);

                //logger.Error("create minSize: " + minSize + " maxSize: " + maxSize);
                int minSize = (int)MathHelper.Lerp(4, 2, progress);
                int maxSize = (int)MathHelper.Lerp(9, 5, progress);

                //size = Main.rand.Next(minSize, maxSize);

                if (currentIteration > maxTries * 0.2f)
                {
                }
            }

            var logger = OvermorrowModFile.Instance.Logger;

            // Make the middle spawn area flatter 
            if (position.X >= (Main.maxTilesX / 7 * 3) && position.X <= (Main.maxTilesX / 7 * 4)) weight = 0.7f;
            else weight = 0.6f;

            Vector2 tileLocation = new Vector2((int)position.X, (int)position.Y);
            bool withinBounds = position.X > 0 && position.X < Main.maxTilesX && position.Y > 0 && position.Y < Main.maxTilesY;

            // Fill in tiles below
            ushort tileType = TileID.Dirt;
            int runnerBlock = 20; // Don't use TileRunner for 10 tiles, which would make the surface more jagged
            //int runnerBlock = 1;

            int yOffset = 0;
            for (int y = (int)position.Y; y < Main.rockLayer; y++)
            {
                if (Framing.GetTileSafely((int)position.X, y + yOffset).HasTile) continue;

                if (runnerBlock > 0)
                {
                    bool checkOrphanTile = !Framing.GetTileSafely((int)position.X - 1, y).HasTile ||
                        !Framing.GetTileSafely((int)position.X - 1, y - 1).HasTile ||
                        !Framing.GetTileSafely((int)position.X, y - 1).HasTile;

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

            Tile tile = Framing.GetTileSafely((int)tileLocation.X, (int)tileLocation.Y + 1);
            bool checkTileType = tile.TileType != TileID.ObsidianBrick;

        }

        public override void OnRunEnd(Vector2 position)
        {
            if (repeatWorm > 1)
            {
                int yOffset = 0;
                if (position.X <= (Main.maxTilesX / 7 * 3)) yOffset = 15;
                else if (position.X >= (Main.maxTilesX / 7 * 4)) yOffset = -15;

                Vector2 branchEndpoint = position + new Vector2(endDistance, yOffset);

                //Vector2 branchEndpoint = trueStartPoint + new Vector2(endDistance, 0);

                ShapeData slimeShapeData = new ShapeData();
                float xScale = 0.4f + Main.rand.NextFloat() * 0.5f; // Randomize the width of the shrine area
                float yScale = Main.rand.NextFloat(0.6f, 0.8f);
                int radius = Main.rand.Next(32, 48);

                //if (Framing.GetTileSafely((int)branchEndpoint.X, (int)branchEndpoint.Y).HasTile)
                //    WorldUtils.Gen(new Point((int)branchEndpoint.X, (int)branchEndpoint.Y), new Shapes.Slime(radius, xScale, yScale), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));

                SurfaceBuilder branchWorm = new SurfaceBuilder(position, branchEndpoint, noise, --repeatWorm);
                branchWorm.Run(out _);

                if (branchEndpoint.X >= (Main.maxTilesX / 7 * 3) - 50 && branchEndpoint.X <= (Main.maxTilesX / 7 * 4))
                {
                    branchEndpoint += new Vector2(0, -25);
                }

                int startOffset = 0;
                if (position.X >= (Main.maxTilesX / 7 * 4) - 25 && position.X <= (Main.maxTilesX / 7 * 4) + 50)
                {
                    startOffset = -100;
                }

                SurfaceTunneler tunnel = new SurfaceTunneler(startPosition, branchEndpoint, noise);
                //SurfaceTunneler tunnel = new SurfaceTunneler(position, branchEndpoint, noise);
                tunnel.Run(out _);
            }

            base.OnRunEnd(position);
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

            if (!endBranch)
            {
                float progress = Utils.Clamp((currentIteration) / (maxTries * 0.2f), 0, 1);

                //logger.Error("create minSize: " + minSize + " maxSize: " + maxSize);
                int minSize = (int)MathHelper.Lerp(4, 2, progress);
                int maxSize = (int)MathHelper.Lerp(9, 5, progress);

                //size = Main.rand.Next(minSize, maxSize);

                if (currentIteration > maxTries * 0.2f)
                {


                }
            }

            if (position.X >= (Main.maxTilesX / 7 * 3) && position.X <= (Main.maxTilesX / 7 * 4)) weight = 0.7f;
            else weight = 0.6f;

            // Push the tunnels downwards so they don't intersect with the spawn
            int yOffset = 65;
            if (position.X >= (Main.maxTilesX / 7 * 3) && position.X <= (Main.maxTilesX / 7 * 4))
            {
                // The distance between the flat area of the spawn
                int totalDistance = (Main.maxTilesX / 7 * 4) - (Main.maxTilesX / 7 * 3);
                //yOffset = (int)MathHelper.Lerp(45, 75, (position.X - Main.maxTilesX / 7 * 4) / totalDistance);
            }
            else
            {

                float progress = ModUtils.EaseInQuart(position.X / (Main.maxTilesX / 7 * 3));
                int lerpOffset = (int)MathHelper.Lerp(75, 0, progress);
                int branchProbability = (int)MathHelper.Lerp(1000, 550, progress);

                if (position.X >= (Main.maxTilesX / 7 * 4))
                {
                    progress = ModUtils.EaseOutQuart((position.X - Main.maxTilesX / 7 * 4) / Main.maxTilesX);
                    branchProbability = (int)MathHelper.Lerp(1000, 550, progress);
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

        public override void OnRunEnd(Vector2 position)
        {
            if (position.X <= (Main.maxTilesX / 7 * 3) || position.X >= (Main.maxTilesX / 7 * 4))
            {
                //if (Main.rand.NextBool(2))
                {
                    int lerpOffset = (int)MathHelper.Lerp(Main.rand.Next(50, 75), 0, ModUtils.EaseInQuart(position.X / (Main.maxTilesX / 7 * 3)));
                    if (position.X >= (Main.maxTilesX / 7 * 4)) lerpOffset = (int)MathHelper.Lerp(0, 75, ModUtils.EaseOutQuart((position.X - Main.maxTilesX / 7 * 4) / Main.maxTilesX));

                    //bool toSurface = Main.rand.NextBool() ? true : false;
                    bool toSurface = false;
                    int yOffset = toSurface ? -100 : 50;
                    Vector2 branchEndpoint = position + new Vector2(0, lerpOffset) + new Vector2(0, yOffset).RotateRandom(MathHelper.Pi);

                    SurfaceTunnelBranch branchWorm = new SurfaceTunnelBranch(position + new Vector2(0, lerpOffset), branchEndpoint, noise, Main.rand.Next(2, 4));
                    //branchWorm.toSurface = toSurface;
                    branchWorm.Run(out _);
                }
            }

            base.OnRunEnd(position);
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