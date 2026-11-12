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

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
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

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(134, 42, 104)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>()),
            [(131, 42, 134)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>(), direction: -1),
            [(171, 73, 94)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArchSmall>()),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(BuildContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            TexGen.PaintClearLayer(AsepritePath, ctx.Origin.X, ctx.Origin.Y, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Walls, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Tiles, hp, vp, interiorW, interiorH);
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
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, ctx.Palette.Walls, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, ctx.Palette.Tiles, srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());

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
    }
}
