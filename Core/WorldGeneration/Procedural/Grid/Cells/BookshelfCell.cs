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
            // Skip the vertical strip when a shaft is on that side; the shaft
            // carves the passage and we'd otherwise paint over it.
            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
            {
                int neighborRow = ctx.Side == Direction.Top ? ctx.Row - 1 : ctx.Row + 1;
                var neighbor = ctx.Grid.GetSlot(ctx.Col, neighborRow);
                if (neighbor != null && !neighbor.IsEmpty && neighbor.Room is ShaftCell)
                    return;
            }

            var wallMap = new Dictionary<(byte, byte, byte), (int, int)>
            {
                [(67, 84, 50)]  = (ModContent.WallType<ArchiveWoodWallBlack>(), 0),
                [(101, 66, 14)] = (ModContent.WallType<ArchiveWoodWall>(), 0),
            };
            var tileMap = new Dictionary<(byte, byte, byte), (int, int)>
            {
                [(105, 106, 106)] = (ModContent.TileType<CastleBrick>(), 0),
                [(89, 86, 82)]    = (ModContent.TileType<DarkCastleBrick>(), 0),
                [(138, 111, 48)]  = (ModContent.TileType<CastlePlatform>(), 0),
                [(74, 47, 33)]    = (ModContent.TileType<ArchiveWood>(), 0),
            };

            // Top/Bottom extend horizontally to cover both corner squares.
            int worldX;
            int worldY;
            int worldW;
            int worldH;
            int srcX;
            int srcY;
            int srcW;
            int srcH;
            switch (ctx.Side)
            {
                case Direction.Left:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    worldW = ctx.Width;
                    worldH = ctx.Height;
                    srcX = 0;
                    srcY = DungeonGrid.VerticalPadding;
                    srcW = DungeonGrid.HorizontalPadding;
                    srcH = DungeonGrid.CellTileHeight;
                    break;
                case Direction.Right:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    worldW = ctx.Width;
                    worldH = ctx.Height;
                    srcX = DungeonGrid.HorizontalPadding + DungeonGrid.CellTileWidth;
                    srcY = DungeonGrid.VerticalPadding;
                    srcW = DungeonGrid.HorizontalPadding;
                    srcH = DungeonGrid.CellTileHeight;
                    break;
                case Direction.Top:
                    worldX = ctx.X - DungeonGrid.HorizontalPadding;
                    worldY = ctx.Y;
                    worldW = ctx.Width + 2 * DungeonGrid.HorizontalPadding;
                    worldH = ctx.Height;
                    srcX = 0;
                    srcY = 0;
                    srcW = DungeonGrid.HorizontalPadding * 2 + DungeonGrid.CellTileWidth;
                    srcH = DungeonGrid.VerticalPadding;
                    break;
                case Direction.Bottom:
                    worldX = ctx.X - DungeonGrid.HorizontalPadding;
                    worldY = ctx.Y;
                    worldW = ctx.Width + 2 * DungeonGrid.HorizontalPadding;
                    worldH = ctx.Height;
                    srcX = 0;
                    srcY = DungeonGrid.VerticalPadding + DungeonGrid.CellTileHeight;
                    srcW = DungeonGrid.HorizontalPadding * 2 + DungeonGrid.CellTileWidth;
                    srcH = DungeonGrid.VerticalPadding;
                    break;
                default:
                    return;
            }

            for (int lx = 0; lx < worldW; lx++)
                for (int ly = 0; ly < worldH; ly++)
                    WorldGenUtils.ClearTile(worldX + lx, worldY + ly);

            string asepritePath = AssetDirectory.GrandArchives + "BookshelfCell.aseprite";
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, asepritePath, worldX, worldY, wallMap, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, asepritePath, worldX, worldY, tileMap, srcX, srcY, srcW, srcH);

            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
                PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            var objectMap = new Dictionary<(byte, byte, byte), (int, int)>
            {
                [(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
            };
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AssetDirectory.GrandArchives + "BookshelfCell.aseprite", paintX, paintY, objectMap);

            var above = ctx.Grid.GetSlot(ctx.Col, ctx.Row - 1);
            var below = ctx.Grid.GetSlot(ctx.Col, ctx.Row + 1);
            bool shaftAbove = above != null && !above.IsEmpty && above.Room is ShaftCell;
            bool shaftBelow = below != null && !below.IsEmpty && below.Room is ShaftCell;

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

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            var wallMap = new Dictionary<(byte, byte, byte), (int, int)>
            {
                [(54, 36, 11)]   = (ModContent.WallType<ArchiveBookWallFrame>(), 0),
                [(118, 66, 138)] = (ModContent.WallType<ArchiveBookWall>(), 0),
                [(101, 66, 14)]  = (ModContent.WallType<ArchiveWoodWall>(), 0),
            };
            var tileMap = new Dictionary<(byte, byte, byte), (int, int)>
            {
                [(105, 106, 106)] = (ModContent.TileType<CastleBrick>(), 0),
                [(89, 86, 82)]    = (ModContent.TileType<DarkCastleBrick>(), 0),
                [(138, 111, 48)]  = (ModContent.TileType<CastlePlatform>(), 0),
                [(74, 47, 33)]    = (ModContent.TileType<ArchiveWood>(), 0),
            };
            string asepritePath = AssetDirectory.GrandArchives + "BookshelfCell.aseprite";
            int sx = DungeonGrid.HorizontalPadding;
            int sy = DungeonGrid.VerticalPadding;
            int sw = DungeonGrid.CellTileWidth;
            int sh = DungeonGrid.CellTileHeight;

            TexGen.PaintAsepriteLayer(SheetLayer.Walls, asepritePath, origin.X, origin.Y, wallMap, sx, sy, sw, sh);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, asepritePath, origin.X, origin.Y, tileMap, sx, sy, sw, sh);
        }

        /// <summary>
        /// 14-tile-wide wooden arch with a 7-tile gap in the middle for
        /// objects underneath. Mirrored from GrandArchiveRoom.PlaceBookshelfArch.
        /// </summary>
        private static void PlaceBookshelfArch(int x, int y)
        {
            WorldGen.PlaceObject(x,      y, ModContent.TileType<WoodenArchL1>());
            WorldGen.PlaceObject(x + 1,  y, ModContent.TileType<WoodenArchL2>());
            WorldGen.PlaceObject(x + 2,  y, ModContent.TileType<WoodenArchL3>());
            WorldGen.PlaceObject(x + 3,  y, ModContent.TileType<WoodenArchSplit>());
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
