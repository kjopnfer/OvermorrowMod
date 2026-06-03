using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.Tiles.Misc;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class CombatRoom : GridRoom
    {
        public override int CellWidth => 3;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.Combat;

        // Prevents combat-corridor-combat clusters.
        private const int MinSpacingBetweenCombatRooms = 3;
        private const int MaxInstancesPerDungeon = 3;
        private const int MinDistanceFromDoor = 5;

        public override int PaddingPriority => 10;

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return false;
            if (subCol == 0 && side == Direction.Left) return true;
            if (subCol == 2 && side == Direction.Right) return true;
            return false;
        }

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && (side == Direction.Left || side == Direction.Right)) return true;
            if (subCol == 2 && side == Direction.Left) return true;
            return false;
        }

        /// <summary>Enforce minimum spacing between combat rooms and the cap.</summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, Func<int, int, GridRoom> pendingLookup = null)
        {
            // Door-proximity check.
            for (int dc = 0; dc < grid.Cols; dc++)
            {
                for (int dr = 0; dr < grid.Rows; dr++)
                {
                    var dSlot = grid.GetSlot(dc, dr);
                    if (dSlot == null || dSlot.IsEmpty) continue;
                    if (dSlot.Room is not DoorRoom) continue;

                    for (int sc = 0; sc < CellWidth; sc++)
                    {
                        for (int sr = 0; sr < CellHeight; sr++)
                        {
                            int dist = Math.Max(
                                Math.Abs((anchor.X + sc) - dc),
                                Math.Abs((anchor.Y + sr) - dr));
                            if (dist < MinDistanceFromDoor) return false;
                        }
                    }
                }
            }

            int radius = MinSpacingBetweenCombatRooms;
            int cMin = Math.Max(0, anchor.X - radius);
            int cMax = Math.Min(grid.Cols - 1, anchor.X + radius);
            int rMin = Math.Max(0, anchor.Y - radius);
            int rMax = Math.Min(grid.Rows - 1, anchor.Y + radius);

            for (int c = cMin; c <= cMax; c++)
            {
                for (int r = rMin; r <= rMax; r++)
                {
                    var room = GetEffectiveRoomAt(grid, pendingLookup, c, r);
                    if (room is CombatRoom)
                    {
                        int dist = Math.Max(Math.Abs(c - anchor.X), Math.Abs(r - anchor.Y));
                        if (dist < MinSpacingBetweenCombatRooms) return false;
                    }
                }
            }

            var seen = new HashSet<GridRoom>();
            int combatCount = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot == null || slot.IsEmpty) continue;

                    if (slot.Room is CombatRoom && seen.Add(slot.Room))
                        combatCount++;
                }
            }

            return combatCount < MaxInstancesPerDungeon;
        }

        private const string AsepritePath = AssetDirectory.GrandArchives + "CombatRoom.aseprite";

        // Color maps

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(66, 64, 61)] = TexPlaceAction.PlaceWall(ModContent.WallType<CastleWall>()),
            [(54, 36, 11)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWallFrame>()),
            [(118, 66, 138)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWall>()),
            [(86, 0, 251)] = TexPlaceAction.PlaceWall(ModContent.WallType<InvisibleWall>()),
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildTileMap() => new()
        {
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
            [(105, 106, 106)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastleBrick>()),
            [(89, 86, 82)] = TexPlaceAction.PlaceTile(ModContent.TileType<DarkCastleBrick>()),
            [(138, 111, 48)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastlePlatform>()),
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
            [(74, 15, 56)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar>()),
            [(134, 42, 104)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>()),
            [(131, 42, 134)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>(), direction: -1),
            [(69, 40, 60)] = TexPlaceAction.PlaceObject(ModContent.TileType<BanquetTable>()),
            [(159, 131, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandelabra>()),
            [(75, 105, 47)] = TexPlaceAction.PlaceObject(ModContent.TileType<BookPileTable>()),
            [(159, 183, 204)] = TexPlaceAction.PlaceObject(ModContent.TileType<Bismarck>()),
            [(99, 49, 110)] = TexPlaceAction.PlaceObject(ModContent.TileType<FireplacePillar>()),
            [(180, 58, 0)] = TexPlaceAction.PlaceObject(ModContent.TileType<Fireplace>()),
            [(208, 61, 125)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>()),
            [(237, 86, 227)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>(), direction: 1),
            [(171, 73, 94)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArchSmall>()),
            [(199, 158, 59)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchiveBanner>()),
            [(153, 229, 80)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxChandelier>()),
            [(237, 157, 102)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxSconceEven>()),
            [(0, 255, 255)] = TexPlaceAction.PlaceObject(ModContent.TileType<TallWindow>()),

            // Random painting from width-grouped pools.
            [(19, 215, 73)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width4;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),
            [(101, 224, 135)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width8;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),

            [(91, 110, 225)] = TexPlaceAction.CustomPlaceObject((x, y) => PlaceBookshelfArch(x, y)),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, BuildWallMap(), hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, BuildTileMap(), hp, vp, interiorW, interiorH);
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            int worldX;
            int worldY;
            int srcX;
            int srcY;
            int srcW;
            int srcH;
            switch (ctx.Side)
            {
                case Direction.Left:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp;
                    srcW = hp;
                    srcH = interiorH;
                    break;
                case Direction.Right:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = hp + interiorW;
                    srcY = vp;
                    srcW = hp;
                    srcH = interiorH;
                    break;
                case Direction.Top:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = 0;
                    srcW = 2 * hp + interiorW;
                    srcH = vp;
                    break;
                case Direction.Bottom:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp + interiorH;
                    srcW = 2 * hp + interiorW;
                    srcH = vp;
                    break;
                default:
                    return;
            }

            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, BuildWallMap(), srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, BuildTileMap(), srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());

            // Combat door pair: placed in the L/R padding strips, linked as
            // siblings so the pair opens/closes together. Runs in PlaceFurniture
            // (after Build + BuildPadding) so the aseprite paint can't overwrite
            // the door tile entities.
            int floorRow = ctx.Origin.Y + FootprintHeight - 1;
            int leftTile = ctx.Origin.X - 2;
            int rightTile = ctx.Origin.X + FootprintWidth + 1;

            int leftId = ModContent.GetInstance<CombatDoor_TE>().Place(leftTile, floorRow);
            int rightId = ModContent.GetInstance<CombatDoor_TE>().Place(rightTile, floorRow);

            if (TileEntity.ByID.TryGetValue(leftId, out var leftTE) && leftTE is CombatDoor_TE leftDoor)
                leftDoor.SiblingTEID = rightId;

            if (TileEntity.ByID.TryGetValue(rightId, out var rightTE) && rightTE is CombatDoor_TE rightDoor)
                rightDoor.SiblingTEID = leftId;
        }

        /// <summary>
        /// 14-tile-wide wooden arch with a 7-tile gap in the middle for shelf
        /// objects underneath.
        /// </summary>
        private static void PlaceBookshelfArch(int x, int y)
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<WoodenArchL1>());
            WorldGen.PlaceObject(x + 1, y, ModContent.TileType<WoodenArchL2>());
            WorldGen.PlaceObject(x + 2, y, ModContent.TileType<WoodenArchL3>());
            WorldGen.PlaceObject(x + 3, y, ModContent.TileType<WoodenArchSplit>());
            WorldGen.PlaceObject(x + 11, y, ModContent.TileType<WoodenArchR1>());
            WorldGen.PlaceObject(x + 12, y, ModContent.TileType<WoodenArchR2>());
            WorldGen.PlaceObject(x + 13, y, ModContent.TileType<WoodenArchR3>());

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 1, y + 5, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 10, y + 5, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));

            PlaceShelfArchObjects(x + 3, y + 5);
            PlaceShelfArchObjects(x + 5, y + 5);
            PlaceShelfArchObjects(x + 8, y + 5);
        }

        private static void PlaceShelfArchObjects(int x, int y)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Globe>());
                    break;
                case 1:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Telescope>());
                    break;
                case 2:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    WorldGen.PlaceObject(x, y - 1, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    if (Main.rand.NextBool())
                        WorldGen.PlaceObject(x, y - 2, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    break;
                case 3:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Crates>(), true, Main.rand.Next(0, 3));
                    break;
            }
        }
    }
}
