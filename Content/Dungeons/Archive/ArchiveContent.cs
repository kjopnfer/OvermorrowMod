using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Content.Dungeons.Archive.Cells;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive
{
    public class ArchiveContent : DungeonContent
    {
        public override int Cols => 35;
        public override int Rows => 30;

        public override int FillTile => ModContent.TileType<CastleBrick>();
        public override int LiningTile => ModContent.TileType<ArchiveWood>();

        public override DungeonPalette Palette { get; } = new ArchivePalette();

        public override IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> SpawnBindings
        {
            get
            {
                ArchiveSpawnPool.Initialize();
                return new Dictionary<(byte R, byte G, byte B), SpawnPool>
                {
                    [(255, 0, 0)] = ArchiveSpawnPool.BaseGroundPool,
                    [(221, 255, 0)] = ArchiveSpawnPool.WallPool,
                };
            }
        }

        public override List<Func<GridRoom>> RequiredRooms => new()
        {
            () => new FireplaceRoom(),
            () => new CombatRoom(),
            () => new WritingRoom(),
        };

        public override GridRoom CreateCombat(bool isFeature) => new CombatRoom { IsFeature = isFeature };
        public override GridRoom CreateTreasure(bool isFeature) => new ChestRoom { IsFeature = isFeature };
        public override GridRoom CreateDoor(bool isFeature) => new DoorRoom { IsFeature = isFeature };
        public override GridRoom CreateFiller(bool isFeature) => new BookshelfCell { IsFeature = isFeature };
        public override GridRoom CreateVerticalConnector(bool isFeature) => new ShaftCell { IsFeature = isFeature };

        public override IReadOnlyDictionary<Type, double> TypeWeights => new Dictionary<Type, double>
        {
            [typeof(ShaftCell)] = 1.4,
            [typeof(DescendingStair)] = 0.7,
            [typeof(AscendingStair)] = 0.7,
            [typeof(FireplaceRoom)] = 1.5,
            [typeof(LoungeRoom)] = 0.3,
            [typeof(CombatRoom)] = 0.7
        };

        public override IReadOnlyDictionary<Type, int> StreakLimits => new Dictionary<Type, int>
        {
            [typeof(BookshelfCell)] = 4,
            [typeof(CorridorCell)] = 5,
            [typeof(FireplaceRoom)] = 1
        };

        public override IReadOnlyDictionary<Type, int> MinStreakLimits => new Dictionary<Type, int>
        {
            [typeof(BookshelfCell)] = 2
        };

        public override void Decorate(DungeonGrid grid)
        {
            int diagonalStairsType = ModContent.TileType<DiagonalStairs>();
            int stairCapType = ModContent.TileType<StairCap>();

            // A passage spans consecutive shafts and any bookshelf landings between them.
            var resolved = new HashSet<Point>();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty || slot.Room.Type != RoomType.VerticalConnector) continue;
                    if (resolved.Contains(new Point(col, row))) continue;

                    // Walk up through shafts and through a bookshelf landing with another shaft beyond.
                    int topRow = row;
                    while (topRow > 0)
                    {
                        var above = grid.GetSlot(col, topRow - 1);
                        if (above == null || above.IsEmpty) break;

                        if (above.Room.Type == RoomType.VerticalConnector)
                        {
                            topRow--;
                            continue;
                        }

                        if (above.Room.Type == RoomType.Filler && topRow >= 2)
                        {
                            var aboveAbove = grid.GetSlot(col, topRow - 2);
                            if (aboveAbove != null && !aboveAbove.IsEmpty && aboveAbove.Room.Type == RoomType.VerticalConnector)
                            {
                                topRow -= 2;
                                continue;
                            }
                        }

                        break;
                    }

                    // Walk down with the mirrored rule.
                    int bottomRow = row;
                    while (bottomRow < grid.Rows - 1)
                    {
                        var below = grid.GetSlot(col, bottomRow + 1);
                        if (below == null || below.IsEmpty) break;

                        if (below.Room.Type == RoomType.VerticalConnector)
                        {
                            bottomRow++;
                            continue;
                        }

                        if (below.Room.Type == RoomType.Filler && bottomRow + 2 < grid.Rows)
                        {
                            var belowBelow = grid.GetSlot(col, bottomRow + 2);
                            if (belowBelow != null && !belowBelow.IsEmpty && belowBelow.Room.Type == RoomType.VerticalConnector)
                            {
                                bottomRow += 2;
                                continue;
                            }
                        }

                        break;
                    }

                    for (int r = topRow; r <= bottomRow; r++)
                    {
                        var s = grid.GetSlot(col, r);
                        if (s != null && !s.IsEmpty && s.Room.Type == RoomType.VerticalConnector)
                            resolved.Add(new Point(col, r));
                    }

                    var topRoom = grid.GetSlot(col, topRow - 1);
                    var bottomRoom = grid.GetSlot(col, bottomRow + 1);

                    // Skip decoration if either end is empty; nothing for stairs to lead to.
                    if (topRoom == null || topRoom.IsEmpty || bottomRoom == null || bottomRoom.IsEmpty)
                        continue;

                    Point topRoomOrigin = grid.GridToWorld(col, topRow - 1);
                    Point bottomRoomOrigin = grid.GridToWorld(col, bottomRow + 1);

                    int topY = topRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;
                    int bottomY = bottomRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;

                    int segmentCount = (bottomY - topY) / 10;
                    int shaftCenterX = grid.GridToWorld(col, topRow).X + DungeonGrid.CellTileWidth / 2;
                    int stairX = shaftCenterX - 7;

                    for (int s = segmentCount - 1; s >= 0; s--)
                    {
                        int sy = topY + s * 10 + 10;
                        ClearObjectFootprint(stairX, sy, 14, 10);
                        WorldGen.PlaceObject(stairX, sy, diagonalStairsType);
                    }

                    ClearObjectFootprint(stairX, topY, 5, 4);
                    WorldGen.PlaceObject(stairX, topY, stairCapType);
                }
            }
        }

        private static void ClearObjectFootprint(int x, int yBottom, int width, int height)
        {
            int yTop = yBottom - (height - 1);
            for (int lx = 0; lx < width; lx++)
                for (int ly = 0; ly < height; ly++)
                    WorldGenUtils.ClearTile(x + lx, yTop + ly);
        }
    }
}
