using System;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Utilities
{
    public static class WorldGenUtils
    {
        /// <summary>Sets tile type + HasTile directly. Skips framing/sync, for bulk worldgen.</summary>
        public static void PlaceTile(int x, int y, ushort tileType)
        {
            if (!WorldGen.InWorld(x, y)) return;
            Tile t = Main.tile[x, y];
            t.TileType = tileType;
            t.HasTile = true;
        }

        /// <summary>Clears HasTile directly. Skips drops/dust/sync, for bulk worldgen.</summary>
        public static void ClearTile(int x, int y)
        {
            if (!WorldGen.InWorld(x, y)) return;
            Tile t = Main.tile[x, y];
            t.HasTile = false;
        }

        /// <summary>Repaints an existing tile's type. No-op if the position is empty.</summary>
        public static void ReplaceTile(int x, int y, ushort tileType)
        {
            if (!WorldGen.InWorld(x, y)) return;
            Tile t = Main.tile[x, y];
            if (!t.HasTile) return;
            t.TileType = tileType;
        }

        /// <summary>Sets WallType directly. Skips framing/sync, for bulk worldgen.</summary>
        public static void SetWall(int x, int y, ushort wallType)
        {
            if (!WorldGen.InWorld(x, y)) return;
            Tile t = Main.tile[x, y];
            t.WallType = wallType;
        }

        /// <summary>Clears the wall at the position.</summary>
        public static void ClearWall(int x, int y)
        {
            if (!WorldGen.InWorld(x, y)) return;
            Tile t = Main.tile[x, y];
            t.WallType = WallID.None;
        }

        // tile/wall sentinels: -1 = no-op, -2 = explicit clear. slope: -2 keep, -1 halfbrick, else slope id.
        public static void GenerateTile(int x, int y, int tile, int wall, int tileStyle = 0, bool active = true, bool removeLiquid = true, int slope = -2, bool tileFrame = true, bool silent = false, bool sync = true)
        {
            try
            {
                Tile Mtile = Framing.GetTileSafely(x, y);

                if (!WorldGen.InWorld(x, y)) return;
                TileObjectData data = tile <= -1 ? null : TileObjectData.GetTileData(tile, tileStyle);
                int width = data == null ? 1 : data.Width;
                int height = data == null ? 1 : data.Height;
                int tileWidth = tile == -1 || data == null ? 1 : data.Width;
                int tileHeight = tile == -1 || data == null ? 1 : data.Height;
                byte oldSlope = (byte)Main.tile[x, y].Slope;
                bool oldHalfBrick = Main.tile[x, y].IsHalfBlock;
                if (tile != -1)
                {
                    WorldGen.destroyObject = true;
                    if (width > 1 || height > 1)
                    {
                        int xs = x, ys = y;
                        //Vector2 newPos = TileUtils.FindTopLeft(xs, ys);
                        Vector2 newPos = TileUtils.GetCornerOfMultiTile(xs, ys, TileUtils.CornerType.TopLeft).ToVector2();

                        for (int x1 = 0; x1 < width; x1++)
                        {
                            for (int y1 = 0; y1 < height; y1++)
                            {
                                int x2 = (int)newPos.X + x1;
                                int y2 = (int)newPos.Y + y1;
                                // Chest: kill items first to avoid dupe glitch.
                                if (x1 == 0 && y1 == 0 && Main.tile[x2, y2].TileType == 21)
                                {
                                    KillChestAndItems(x2, y2);
                                }

                                Main.tile[x, y].TileType = 0;
                                if (!silent) WorldGen.KillTile(x, y, false, false, true);

                                if (removeLiquid)
                                {
                                    GenerateLiquid(x2, y2, 0, true, 0, false);
                                }
                            }
                        }

                        for (int x1 = 0; x1 < width; x1++)
                        {
                            for (int y1 = 0; y1 < height; y1++)
                            {
                                int x2 = (int)newPos.X + x1;
                                int y2 = (int)newPos.Y + y1;
                                WorldGen.SquareTileFrame(x2, y2);
                                WorldGen.SquareWallFrame(x2, y2);
                            }
                        }
                    }
                    else if (!silent)
                    {
                        WorldGen.KillTile(x, y, false, false, true);
                    }

                    WorldGen.destroyObject = false;
                    if (active)
                    {
                        if (tileWidth <= 1 && tileHeight <= 1 && !Main.tileFrameImportant[tile])
                        {
                            Main.tile[x, y].TileType = (ushort)tile;
                            Mtile.HasTile = true;
                            if (slope == -2 && oldHalfBrick) { Mtile.IsHalfBlock = true; }
                            else
                            if (slope == -1) { Mtile.IsHalfBlock = true; }
                            else
                            { Mtile.Slope = (SlopeType)(slope == -2 ? oldSlope : (byte)slope); }
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else
                        {
                            WorldGen.destroyObject = true;
                            if (!silent)
                            {
                                for (int x1 = 0; x1 < tileWidth; x1++)
                                {
                                    for (int y1 = 0; y1 < tileHeight; y1++)
                                    {
                                        WorldGen.KillTile(x + x1, y + y1, false, false, true);
                                    }
                                }
                            }
                            WorldGen.destroyObject = false;
                            int genX = x;
                            int genY = tile == 10 ? y : y + height;
                            WorldGen.PlaceTile(genX, genY, tile, true, true, -1, tileStyle);
                            for (int x1 = 0; x1 < tileWidth; x1++)
                            {
                                for (int y1 = 0; y1 < tileHeight; y1++)
                                {
                                    WorldGen.SquareTileFrame(x + x1, y + y1);
                                }
                            }
                        }
                    }
                    else
                    {
                        Mtile.ClearTile();
                    }
                }

                if (wall != -1)
                {
                    if (wall == -2) { wall = 0; }
                    Main.tile[x, y].WallType = 0;
                    WorldGen.PlaceWall(x, y, wall, true);
                }

                if (sync && Main.netMode != NetmodeID.SinglePlayer)
                {
                    int sizeWidth = tileWidth + Math.Max(0, width - 1);
                    int sizeHeight = tileHeight + Math.Max(0, height - 1);
                    int size = sizeWidth > sizeHeight ? sizeWidth : sizeHeight;
                    NetMessage.SendTileSquare(-1, x + (int)(size * 0.5F), y + (int)(size * 0.5F), size + 1);
                }
            }
            catch (Exception e)
            {
                OvermorrowModFile.Instance.Logger.Error(e);
            }
        }

        /// <summary>Removes the chest at (x, y) and its contents. Does not remove the tile.</summary>
        public static bool KillChestAndItems(int x, int y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == x && Main.chest[i].y == y)
                {
                    Main.chest[i] = null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>Places a liquid tile. liquidType: 0=Water, 1=Lava, 2=Honey, 3=Shimmer.</summary>
        public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            Tile Mtile = Main.tile[x, y];

            if (!WorldGen.InWorld(x, y)) return;

            liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0, 255);
            Main.tile[x, y].LiquidAmount = (byte)liquidHeight;

            if (liquidType == 0) { Mtile.LiquidType = LiquidID.Water; }
            else if (liquidType == 1) { Mtile.LiquidType = LiquidID.Lava; }
            else if (liquidType == 2) { Mtile.LiquidType = LiquidID.Honey; }
            else if (liquidType == 3) { Mtile.LiquidType = LiquidID.Shimmer; }

            if (updateFlow) { Liquid.AddWater(x, y); }
            if (sync && Main.netMode != NetmodeID.SinglePlayer) { NetMessage.SendTileSquare(-1, x, y, 1); }
        }
    }
}