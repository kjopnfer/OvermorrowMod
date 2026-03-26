using System;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Content.Tiles.Archives;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public enum CorridorType
    {
        Flat,
        Slope,
        Stairs
    }

    public struct CorridorPlan
    {
        public CorridorType Type;
        public int StartX;
        public int StartFloorY;
        public int EndFloorY;
        public int TotalWidth;
        public int HeightChange;

        public int EndX => StartX + TotalWidth;
    }

    public static class ProceduralCorridor
    {
        private const int DiagonalStairWidth = 14;
        private const int DiagonalStairHeight = 10;
        private const int FlatPad = 5;
        private const int MinStepWidth = 3;

        public static CorridorPlan Plan(int startX, int floorY, int heightChange, Random rand)
        {
            int absChange = Math.Abs(heightChange);
            var plan = new CorridorPlan
            {
                StartX = startX,
                StartFloorY = floorY,
                EndFloorY = floorY + heightChange,
                HeightChange = heightChange
            };

            if (absChange == 0)
            {
                plan.Type = CorridorType.Flat;
                plan.TotalWidth = rand.Next(10, 21);
            }
            else if (absChange <= 8)
            {
                plan.Type = CorridorType.Slope;
                int slopeWidth = absChange * MinStepWidth;
                plan.TotalWidth = FlatPad * 2 + slopeWidth;
            }
            else
            {
                plan.Type = CorridorType.Stairs;
                int stairsNeeded = (int)Math.Ceiling((double)absChange / DiagonalStairHeight);
                plan.TotalWidth = FlatPad * 2 + stairsNeeded * DiagonalStairWidth;
            }

            return plan;
        }

        public static void Clear(CorridorPlan plan, int corridorHeight)
        {
            switch (plan.Type)
            {
                case CorridorType.Flat:
                    ClearFlat(plan.StartX, plan.EndX, plan.StartFloorY, corridorHeight);
                    break;
                case CorridorType.Slope:
                    ClearSlope(plan, corridorHeight);
                    break;
                case CorridorType.Stairs:
                    ClearStairs(plan, corridorHeight);
                    break;
            }
        }

        public static void PlaceObjects(CorridorPlan plan, int corridorHeight)
        {
            if (plan.Type == CorridorType.Stairs)
            {
                int absChange = Math.Abs(plan.HeightChange);
                int stairsNeeded = (int)Math.Ceiling((double)absChange / DiagonalStairHeight);

                int stairStartX = plan.StartX + FlatPad;
                int lowerFloor = Math.Max(plan.StartFloorY, plan.EndFloorY);

                PlaceDiagonalStairStack(stairStartX, lowerFloor, stairsNeeded);
            }
        }

        private static void ClearFlat(int startX, int endX, int floorY, int corridorHeight)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = floorY - corridorHeight; y <= floorY; y++)
                {
                    WorldGen.KillTile(x, y, false, false, true);
                }
            }
        }

        private static void ClearSlope(CorridorPlan plan, int corridorHeight)
        {
            int absChange = Math.Abs(plan.HeightChange);
            int direction = plan.HeightChange > 0 ? 1 : -1;

            int slopeStart = plan.StartX + FlatPad;
            int slopeEnd = plan.EndX - FlatPad;
            int stepWidth = Math.Max(MinStepWidth, (slopeEnd - slopeStart) / absChange);

            for (int x = plan.StartX; x <= plan.EndX; x++)
            {
                int currentFloor;

                if (x < slopeStart)
                    currentFloor = plan.StartFloorY;
                else if (x >= slopeEnd)
                    currentFloor = plan.EndFloorY;
                else
                {
                    int posInSlope = x - slopeStart;
                    int step = Math.Min(posInSlope / stepWidth, absChange);
                    currentFloor = plan.StartFloorY + step * direction;
                }

                for (int y = currentFloor - corridorHeight; y <= currentFloor; y++)
                {
                    WorldGen.KillTile(x, y, false, false, true);
                }
            }
        }

        private static void ClearStairs(CorridorPlan plan, int corridorHeight)
        {
            int absChange = Math.Abs(plan.HeightChange);
            int stairsNeeded = (int)Math.Ceiling((double)absChange / DiagonalStairHeight);
            int stairTotalWidth = stairsNeeded * DiagonalStairWidth;

            int stairStartX = plan.StartX + FlatPad;
            int stairEndX = stairStartX + stairTotalWidth;

            int higherFloor = Math.Min(plan.StartFloorY, plan.EndFloorY);
            int lowerFloor = Math.Max(plan.StartFloorY, plan.EndFloorY);

            // Flat from room exit to stair base at the starting floor level
            ClearFlat(plan.StartX, stairStartX, plan.StartFloorY, corridorHeight);

            // Clear the stair area — full height between both floors plus headroom
            for (int x = stairStartX; x < stairEndX; x++)
            {
                for (int y = higherFloor - corridorHeight - DiagonalStairHeight; y <= lowerFloor; y++)
                {
                    WorldGen.KillTile(x, y, false, false, true);
                }
            }

            // Flat pad from stair exit to next room at the end floor level
            ClearFlat(stairEndX, plan.EndX, plan.EndFloorY, corridorHeight);
        }

        private static void PlaceDiagonalStairStack(int x, int y, int stack)
        {
            int height = stack * 10;
            for (int i = 0; i < height; i += 10)
            {
                WorldGen.PlaceObject(x, y - i, ModContent.TileType<DiagonalStairs>());
            }

            WorldGen.PlaceObject(x, y - height, ModContent.TileType<StairCap>());
        }
    }
}
