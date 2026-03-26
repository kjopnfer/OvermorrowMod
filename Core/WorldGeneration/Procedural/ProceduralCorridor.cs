using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Content.Tiles.Archives;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public enum CorridorType
    {
        Flat,
        Slope,
        Stairs,
        Bridge
    }

    public struct CorridorPlan
    {
        public CorridorType Type;
        public int StartX;
        public int StartFloorY;
        public int EndFloorY;
        public int TotalWidth;
        public int HeightChange;

        public List<int> BridgeSpans;

        public int EndX => StartX + TotalWidth;
    }

    public static class ProceduralCorridor
    {
        private const int DiagonalStairWidth = 14;
        private const int DiagonalStairHeight = 10;
        private const int FlatPad = 5;
        private const int MinStepWidth = 3;

        private const int BridgeWidth = 17;
        private const int BridgePitDepth = 60;
        private const int BridgeCeilingHeight = 25;
        private const int MinBridgeCount = 2;
        private const int MaxBridgeCount = 4;
        private const int BridgeWallWidth = 1;
        private const int MinBridgeGap = 24;
        private const int MaxBridgeGap = 34;

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
                bool useBridge = rand.Next(3) == 0;
                if (useBridge)
                {
                    plan.Type = CorridorType.Bridge;
                    int bridgeCount = rand.Next(MinBridgeCount, MaxBridgeCount + 1);

                    plan.BridgeSpans = new List<int>();
                    int cursor = BridgeWallWidth;

                    for (int i = 0; i < bridgeCount; i++)
                    {
                        int gap = rand.Next(MinBridgeGap, MaxBridgeGap);
                        cursor += gap;
                        plan.BridgeSpans.Add(cursor);
                        cursor += BridgeWidth;
                    }

                    int finalGap = rand.Next(MinBridgeGap, MaxBridgeGap);
                    cursor += finalGap + BridgeWallWidth;
                    plan.TotalWidth = cursor;
                }
                else
                {
                    plan.Type = CorridorType.Flat;
                    plan.TotalWidth = rand.Next(10, 21);
                }
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

        public static void Clear(CorridorPlan plan, int corridorHeight, int fillTileType)
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
                case CorridorType.Bridge:
                    ClearBridge(plan, fillTileType);
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
            else if (plan.Type == CorridorType.Bridge)
            {
                PlaceBridges(plan);
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

            ClearFlat(plan.StartX, stairStartX, plan.StartFloorY, corridorHeight);

            for (int x = stairStartX; x < stairEndX; x++)
            {
                for (int y = higherFloor - corridorHeight - DiagonalStairHeight; y <= lowerFloor; y++)
                {
                    WorldGen.KillTile(x, y, false, false, true);
                }
            }

            ClearFlat(stairEndX, plan.EndX, plan.EndFloorY, corridorHeight);
        }

        private static void ClearBridge(CorridorPlan plan, int fillTileType)
        {
            int floorY = plan.StartFloorY;

            // Fill the entire pit area with solid tiles first, then selectively clear
            // This ensures pillars, outer walls, and pit floor all exist
            for (int x = plan.StartX; x <= plan.EndX; x++)
            {
                for (int y = floorY + 1; y <= floorY + BridgePitDepth; y++)
                {
                    WorldGen.PlaceTile(x, y, fillTileType, true, true);
                }
            }

            // Clear the walkable area above the bridge across the full width
            // Start from StartX so room border detection sees air and opens the wall
            for (int x = plan.StartX; x <= plan.EndX; x++)
            {
                for (int y = floorY - BridgeCeilingHeight; y <= floorY; y++)
                {
                    WorldGen.KillTile(x, y, false, false, true);
                }
            }

            // Clear the pit only under bridge spans (leave pillar columns solid below floor)
            foreach (int spanOffset in plan.BridgeSpans)
            {
                int spanStartX = plan.StartX + spanOffset;
                int spanEndX = spanStartX + BridgeWidth;

                for (int x = spanStartX; x < spanEndX; x++)
                {
                    for (int y = floorY + 1; y < floorY + BridgePitDepth; y++)
                    {
                        WorldGen.KillTile(x, y, false, false, true);
                    }
                }
            }
        }

        private const int DarknessOffset = 14;
        private const int DarknessRows = 3;

        private static void PlaceBridges(CorridorPlan plan)
        {
            int floorY = plan.StartFloorY;
            int darknessType = ModContent.TileType<Content.Tiles.DarknessBlock>();

            foreach (int spanOffset in plan.BridgeSpans)
            {
                int bridgeX = plan.StartX + spanOffset;
                WorldGen.PlaceObject(bridgeX, floorY + 1, ModContent.TileType<ArchiveBridge>());

                for (int row = 0; row < DarknessRows; row++)
                {
                    int y = floorY + DarknessOffset + row;
                    for (int x = bridgeX; x < bridgeX + BridgeWidth; x++)
                    {
                        WorldGen.PlaceTile(x, y, darknessType, true, true);
                    }
                }
            }
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
