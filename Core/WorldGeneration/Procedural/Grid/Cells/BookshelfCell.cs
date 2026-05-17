using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class BookshelfCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
            new FireplaceRoom(),
            new LoungeRoom(),
            new WritingRoom(),
        };

        private static readonly GridRoom[] VerticalNeighbors = { new ShaftCell() };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            Direction.Top or Direction.Bottom => VerticalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        /// <summary>
        /// Bookshelves are architectural rooms and accept connections on
        /// every cardinal side. The actual neighbor type is constrained
        /// separately by AllowedNeighbors.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) => true;

        public override bool OwnsPadding => true;

        private const string AsepritePath = AssetDirectory.GrandArchives + "BookshelfCell.aseprite";

        // Color maps

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(54, 36, 11)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWallFrame>()),
            [(118, 66, 138)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWall>()),
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
            [(74, 15, 56)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar>()),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y, hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, BuildWallMap(), hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, BuildTileMap(), hp, vp, cw, ch);
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            // Skip the vertical strip when a shaft is on that side; the shaft
            // carves the passage and we'd otherwise paint over it.
            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
            {
                int neighborRow = ctx.Side == Direction.Top ? ctx.Row - 1 : ctx.Row + 1;
                var neighbor = ctx.Grid.GetSlot(ctx.Col, neighborRow);
                if (neighbor != null && !neighbor.IsEmpty && neighbor.Room is ShaftCell)
                    return;
            }

            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

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
                    srcH = ch;
                    break;
                case Direction.Right:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = hp + cw;
                    srcY = vp;
                    srcW = hp;
                    srcH = ch;
                    break;
                case Direction.Top:
                    // Extend horizontally to claim the two corner squares.
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = 0;
                    srcW = 2 * hp + cw;
                    srcH = vp;
                    break;
                case Direction.Bottom:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp + ch;
                    srcW = 2 * hp + cw;
                    srcH = vp;
                    break;
                default:
                    return;
            }

            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, BuildWallMap(), srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, BuildTileMap(), srcX, srcY, srcW, srcH);

            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
                PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());

            var above = ctx.Grid.GetSlot(ctx.Col, ctx.Row - 1);
            var below = ctx.Grid.GetSlot(ctx.Col, ctx.Row + 1);
            bool shaftAbove = above != null && !above.IsEmpty && above.Room is ShaftCell;
            bool shaftBelow = below != null && !below.IsEmpty && below.Room is ShaftCell;

            // Arch sits at the top row; a shaft above would route through it.
            if (!shaftAbove)
                PlaceBookshelfArch(ctx.Origin.X + 2, ctx.Origin.Y);

            if (shaftAbove || shaftBelow)
            {
                // Sconces are mutually exclusive with book piles; a shaft
                // running through this cell breaks the floor that piles
                // would otherwise rest on.
                int sconceRow = ctx.Origin.Y + DungeonGrid.CellTileHeight - 12;
                int leftStripCol = ctx.Origin.X - DungeonGrid.HorizontalPadding + 3;
                int rightStripCol = ctx.Origin.X + DungeonGrid.CellTileWidth + 3;
                int sconceType = ModContent.TileType<WaxSconceEven>();

                WorldGen.PlaceObject(leftStripCol, sconceRow, sconceType);
                WorldGen.PlaceObject(rightStripCol, sconceRow, sconceType);
                return;
            }

            int pileBottomY = ctx.Origin.Y + DungeonGrid.CellTileHeight - 1;
            int pileStartX = ctx.Origin.X + 1;
            int pileSpaceWidth = DungeonGrid.CellTileWidth - 2;

            // Place 1-2 floor candles first; book piles skip the columns
            // they end up occupying so they don't conflict.
            HashSet<int> blockedColumns = new();
            int targetCandles = Main.rand.Next(1, 3);
            int placedCandles = 0;
            int candleAttempts = 0;
            while (placedCandles < targetCandles && candleAttempts < 8)
            {
                int candleX = pileStartX + Main.rand.Next(pileSpaceWidth);
                if (!blockedColumns.Contains(candleX))
                {
                    if (WorldGen.PlaceObject(candleX, pileBottomY, ModContent.TileType<FloorCandles>(), true, Main.rand.Next(0, 6)))
                    {
                        blockedColumns.Add(candleX);
                        placedCandles++;
                    }
                }
                candleAttempts++;
            }

            // Stagger book piles across the cell's footprint, sitting on
            // the wood floor of the padding strip below the bookshelf.
            PlaceMultiBookPiles(pileStartX, pileBottomY, pileSpaceWidth, blockedColumns);
        }

        /// <summary>
        /// 14-tile-wide wooden arch with a 7-tile gap in the middle for
        /// objects underneath. Mirrored from GrandArchiveRoom.PlaceBookshelfArch.
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

        /// <summary>
        /// Places a single vertical pile of BookPile tiles starting at (x, y)
        /// and stacking upward. Optionally caps the pile with a BookCandleholder.
        /// </summary>
        private static bool PlaceBookPile(int x, int y, int stackSize, bool withCandle = false)
        {
            if (stackSize < 1) stackSize = 1;

            bool success = WorldGen.PlaceObject(x, y, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
            if (!success) return false;

            for (int i = 1; i < stackSize; i++)
                WorldGen.PlaceObject(x, y - i, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));

            if (withCandle)
                WorldGen.PlaceObject(x, y - stackSize, ModContent.TileType<BookCandleholder>(), true);

            return true;
        }

        /// <summary>
        /// Scatters up to 4 staggered BookPile stacks across a horizontal
        /// strip starting at (x, y), where y is the row the bottom of each
        /// pile occupies (the row above the floor tile).
        /// At most one pile is capped with a BookCandleholder.
        /// </summary>
        private static void PlaceMultiBookPiles(int x, int y, int spaceWidth, HashSet<int> blockedColumns = null)
        {
            const int pileWidth = 2;
            const int maxPiles = 4;
            const int maxAttempts = 50;
            bool candlePlaced = false;

            List<int> possibleOffsets = new();
            for (int i = 0; i <= spaceWidth - pileWidth; i++)
            {
                if (i % 2 == 0 || Main.rand.NextBool()) possibleOffsets.Add(i);
            }

            if (possibleOffsets.Count < maxPiles)
            {
                possibleOffsets = Enumerable.Range(0, spaceWidth / pileWidth + 1).Select(i => i * pileWidth).ToList();
            }

            for (int i = possibleOffsets.Count - 1; i > 0; i--)
            {
                int j = Main.rand.Next(i + 1);
                (possibleOffsets[i], possibleOffsets[j]) = (possibleOffsets[j], possibleOffsets[i]);
            }

            int placedCount = 0;
            int attemptIndex = 0;
            int failCount = 0;

            while (placedCount < maxPiles && attemptIndex < possibleOffsets.Count && failCount < maxAttempts)
            {
                int offsetX = possibleOffsets[attemptIndex];
                int pileX = x + offsetX;

                if (blockedColumns != null && blockedColumns.Contains(pileX))
                {
                    attemptIndex++;
                    failCount++;
                    continue;
                }

                int stackSize;
                bool withCandle = false;

                if (!candlePlaced && (maxPiles - placedCount <= 1 || Main.rand.NextBool(4)))
                {
                    stackSize = Main.rand.Next(8, 13);
                    withCandle = true;
                    candlePlaced = true;
                }
                else
                {
                    int style = Main.rand.Next(3);
                    stackSize = style switch
                    {
                        0 => Main.rand.Next(2, 4),
                        1 => Main.rand.Next(5, 7),
                        _ => Main.rand.Next(6, 9)
                    };
                }

                bool success = PlaceBookPile(pileX, y, stackSize, withCandle);
                if (success) placedCount++;
                else failCount++;

                attemptIndex++;
            }
        }
    }
}
