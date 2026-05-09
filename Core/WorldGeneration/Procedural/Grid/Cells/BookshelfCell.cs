using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

        public override void BuildPadding(PaddingContext ctx)
        {
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                    PaddingBuilder.PlaceWoodPanelPadding(
                        ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);
                    break;
                case Direction.Top:
                case Direction.Bottom:
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    break;
            }
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            var above = ctx.Grid.GetSlot(ctx.Col, ctx.Row - 1);
            var below = ctx.Grid.GetSlot(ctx.Col, ctx.Row + 1);
            bool shaftAbove = above != null && !above.IsEmpty && above.Room is ShaftCell;
            bool shaftBelow = below != null && !below.IsEmpty && below.Room is ShaftCell;
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

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            PlaceBookPanel(origin.X, origin.Y, width, height);
        }

        /// <summary>
        /// Places a bookshelf wall panel: wood frame on the sides, book wall
        /// fill with shelf rows every 4 tiles from the bottom.
        /// </summary>
        private static void PlaceBookPanel(int startX, int startY, int w, int h)
        {
            ushort frameWall = (ushort)ModContent.WallType<ArchiveBookWallFrame>();
            ushort bookWall = (ushort)ModContent.WallType<ArchiveBookWall>();
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            int bookHeight = 20;

            // 1-tile wood panel on each side, 16-tile book area in the middle
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < h; ly++)
                {
                    int worldX = startX + lx;
                    int worldY = startY + ly;

                    if (lx == 0 || lx == w - 1)
                    {
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                        continue;
                    }

                    int bookStart = h - bookHeight;
                    if (ly < bookStart)
                    {
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                        continue;
                    }

                    int bookLx = lx - 1;
                    int bookW = w - 2;
                    int bookLy = ly - bookStart;
                    bool isBorder = (bookLx == 0 || bookLx == bookW - 1 || bookLy == 0 || bookLy == bookHeight - 1);
                    int fromBottom = (bookHeight - 1) - bookLy;
                    bool isShelfRow = (fromBottom % 4 == 0);
                    WorldGenUtils.SetWall(worldX, worldY, (isBorder || isShelfRow) ? frameWall : bookWall);
                }
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

            // Generate staggered possible offsets for pile placement
            List<int> possibleOffsets = new();
            for (int i = 0; i <= spaceWidth - pileWidth; i++)
            {
                if (i % 2 == 0 || Main.rand.NextBool()) possibleOffsets.Add(i);
            }

            if (possibleOffsets.Count < maxPiles)
            {
                possibleOffsets = Enumerable.Range(0, spaceWidth / pileWidth + 1).Select(i => i * pileWidth).ToList();
            }

            // Shuffle so the staggered offsets are tried in a random order
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

                // Skip columns already taken by a floor candle so the
                // pile's base doesn't overlap the candle's footprint.
                if (blockedColumns != null && blockedColumns.Contains(pileX))
                {
                    attemptIndex++;
                    failCount++;
                    continue;
                }

                int stackSize;
                bool withCandle = false;

                // Assign a single candle pile randomly: either the last
                // available slot, or a 1-in-4 chance per attempt.
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
