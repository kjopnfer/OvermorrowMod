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
        // oh my god -Japan
        /*
         *  Generates a single tile and wall at the given coordinates. (if the tile is > 1 x 1 it assumes the passed in coordinate is the top left)
         *  tile : type of tile to place. -1 means don't do anything tile related, -2 is used in conjunction with active == false to make air.
         *  wall : type of wall to place. -1 means don't do anything wall related. -2 is used to remove the wall already there.
         *  tileStyle : the style of the given tile. 
         *  active : If false, will make the tile 'air' and show the wall only.
         *  removeLiquid : If true, it will remove liquids in the generating area.
		 *  slope : if -2, keep the current slope. if -1, make it a halfbrick, otherwise make it the slope given.
		 *  tileFrame: if true and tile is a 1x1 block, will frame it and its neighbours
		 *  silent : If true, will not display dust nor sound.
         *  sync : If true, will sync the client and server.
         */
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
                                if (x1 == 0 && y1 == 0 && Main.tile[x2, y2].TileType == 21) //is a chest, special case to prevent dupe glitch
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

        /// <summary>
        /// Removes a chest at the specified coordinates and clears all items inside it.
        /// This method does not remove the tile itself, only the chest and its contents.
        /// </summary>
        /// <param name="x">The X coordinate of the chest.</param>
        /// <param name="y">The Y coordinate of the chest.</param>
        /// <returns>True if the chest was found and removed, otherwise false.</returns>
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

        /// <summary>
        /// Generates a single tile of liquid at the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="liquidType">The type of liquid: 0 = Water, 1 = Lava, 2 = Honey, 3 = Shimmer.</param>
        /// <param name="updateFlow">Whether to update the liquid flow after placement. Default is true.</param>
        /// <param name="liquidHeight">The height of the liquid (0 - 255). Default is 255.</param>
        /// <param name="sync">Whether to sync the change with other clients in multiplayer. Default is true.</param>
        public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            Tile Mtile = Main.tile[x, y];

            if (!WorldGen.InWorld(x, y)) return;

            liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0, 255);
            Main.tile[x, y].LiquidAmount = (byte)liquidHeight;

            // Set the liquid type based on the liquidType argument
            if (liquidType == 0) { Mtile.LiquidType = LiquidID.Water; }
            else if (liquidType == 1) { Mtile.LiquidType = LiquidID.Lava; }
            else if (liquidType == 2) { Mtile.LiquidType = LiquidID.Honey; }
            else if (liquidType == 3) { Mtile.LiquidType = LiquidID.Shimmer; }

            // Update the liquid flow if requested
            if (updateFlow) { Liquid.AddWater(x, y); }

            // Sync the change with other clients if multiplayer and requested
            if (sync && Main.netMode != NetmodeID.SinglePlayer) { NetMessage.SendTileSquare(-1, x, y, 1); }
        }
    }
}