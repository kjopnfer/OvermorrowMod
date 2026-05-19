using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public abstract class StairBlock : GridRoom
    {
        /// <summary>
        /// <c>true</c> for descending (top-left to bottom-right),
        /// <c>false</c> for ascending (bottom-left to top-right).
        /// </summary>
        protected abstract bool DescendsLeftToRight { get; }

        /// <summary>Path to the aseprite covering this 2x2 stair block.</summary>
        protected abstract string AsepritePath { get; }

        private bool _descendLeftToRight => DescendsLeftToRight;

        public override int CellWidth => 2;
        public override int CellHeight => 2;

        public bool IsBottomLandingSubCell(int subCol, int subRow)
        {
            if (subRow != 1) return false;
            return _descendLeftToRight ? subCol == 1 : subCol == 0;
        }

        public bool IsTopLandingSubCell(int subCol, int subRow)
        {
            if (subRow != 0) return false;
            return _descendLeftToRight ? subCol == 0 : subCol == 1;
        }

        private static readonly HashSet<Type> FloorAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(CorridorCell),
            typeof(CombatRoom),
            typeof(WritingRoom),
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side))
                return null;

            if (_descendLeftToRight)
            {
                if (subCol == 0 && subRow == 0 && side == Direction.Left) return FloorAccepted;
                if (subCol == 1 && subRow == 1 && side == Direction.Right) return FloorAccepted;
            }
            else
            {
                if (subCol == 0 && subRow == 1 && side == Direction.Left) return FloorAccepted;
                if (subCol == 1 && subRow == 0 && side == Direction.Right) return FloorAccepted;
            }

            return null;
        }

        /// <summary>
        /// Descending: cursor sits at the anchor (top-left of 2x2).
        /// Ascending:  cursor sits at bottom-left, so the anchor is one row above.
        /// </summary>
        public override Point AnchorOffsetFromCursor =>
            _descendLeftToRight ? Point.Zero : new Point(0, -1);

        /// <summary>
        /// Stairs are inherently directional: one exit only.
        /// Descending: cursor moves (+2, +1) to the lower-right landing's east neighbor.
        /// Ascending:  cursor moves (+2, -1) to the upper-right landing's east neighbor.
        /// </summary>
        public override CellExit[] Exits => new[]
        {
            new CellExit(
                _descendLeftToRight ? new Point(2, 1) : new Point(2, -1),
                // CombatRoom intentionally excluded: planner-placed only.
                new GridRoom[] { new BookshelfCell(), new CorridorCell() })
        };

        /// <summary>
        /// A stair's 2x2 top row cannot sit immediately below a shaft, since
        /// shafts only accept bookshelves on their vertical ends. The same
        /// rule applies to the bottom row sitting above a shaft.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, Func<int, int, GridRoom> pendingLookup = null)
        {
            for (int dc = 0; dc < 2; dc++)
            {
                var above = GetEffectiveRoomAt(grid, pendingLookup, anchor.X + dc, anchor.Y - 1);
                if (above is ShaftCell) return false;

                var below = GetEffectiveRoomAt(grid, pendingLookup, anchor.X + dc, anchor.Y + 2);
                if (below is ShaftCell) return false;
            }
            return true;
        }

        public override bool IsOpenSide(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return false;

            if (_descendLeftToRight)
            {
                if (subCol == 0 && subRow == 0 && side == Direction.Left) return true;
                if (subCol == 1 && subRow == 1 && side == Direction.Right) return true;
            }
            else
            {
                if (subCol == 0 && subRow == 1 && side == Direction.Left) return true;
                if (subCol == 1 && subRow == 0 && side == Direction.Right) return true;
            }
            return false;
        }

        public override bool AllowsEmptyNeighbors => false;

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && side == Direction.Left) return true;
            if (subRow == 0 && side == Direction.Bottom) return true;
            if (subRow == 1 && side == Direction.Top) return true;
            return false;
        }

        public override bool OwnsPadding => true;

        // Color maps

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(70, 67, 117)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlue>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildTileMap() => new()
        {
            [(105, 106, 106)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastleBrick>()),
            [(89, 86, 82)] = TexPlaceAction.PlaceTile(ModContent.TileType<DarkCastleBrick>()),
            [(138, 111, 48)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastlePlatform>()),
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(237, 152, 93)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxSconce>()),
            [(251, 242, 54)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchivePotSmall>()),
            [(215, 186, 87)] = TexPlaceAction.PlaceObject(ModContent.TileType<FatVase>()),
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
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
                    // Extend horizontally to claim the two corner squares.
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
        }
    }

    /// <summary>
    /// A 2x2 staircase that goes down from left to right. Cursor enters at
    /// the top-left landing and exits at the bottom-right landing.
    /// </summary>
    public class DescendingStair : StairBlock
    {
        protected override bool DescendsLeftToRight => true;
        protected override string AsepritePath => AssetDirectory.GrandArchives + "StairBlockRight.aseprite";
    }

    /// <summary>
    /// A 2x2 staircase that goes up from left to right. Cursor enters at
    /// the bottom-left landing and exits at the top-right landing.
    /// </summary>
    public class AscendingStair : StairBlock
    {
        protected override bool DescendsLeftToRight => false;
        protected override string AsepritePath => AssetDirectory.GrandArchives + "StairBlockLeft.aseprite";
    }
}
