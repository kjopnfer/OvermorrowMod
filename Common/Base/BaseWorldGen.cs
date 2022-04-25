using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Common.Base
{
    public class BaseWorldGen
    {
        //------------------------------------------------------//
        //---------------BASE WORLDGEN CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods for generating various things into  //
        // the world.                                           //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public static int GetFirstTileFloor(int x, int startY, bool solid = true)
        {
            for (int y = startY; y < Main.maxTilesY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile != null && tile.HasUnactuatedTile && (!solid || Main.tileSolid[(int)tile.TileType])) { return y; }
            }
            return Main.maxTilesY;
        }

        /*
         * Iterates upwards and returns the first Y position that has a tile in it.
         * startY : The y to begin iteration at.
         * solid : True if the tile must be solid.
         */
        public static int GetFirstTileCeiling(int x, int startY, bool solid = true)
        {
            for (int y = startY; y > 0; y--)
            {
                Tile tile = Main.tile[x, y];
                if (tile != null && tile.HasUnactuatedTile && (!solid || Main.tileSolid[(int)tile.TileType])) { return y; }
            }
            return 0;
        }


        public static int GetFirstTileSide(int startX, int y, bool left, bool solid = true)
        {
            if (left)
            {
                for (int x = startX; x > 0; x--)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile != null && tile.HasUnactuatedTile && (!solid || Main.tileSolid[(int)tile.TileType])) { return x; }
                }
                return 0;
            }
            else
            {
                for (int x = startX; x < Main.maxTilesX; x++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile != null && tile.HasUnactuatedTile && (!solid || Main.tileSolid[(int)tile.TileType])) { return x; }
                }
                return Main.maxTilesX;
            }
        }


        public static void ReplaceTiles(Vector2 position, int radius, int[] tiles, int[] replacements, bool silent = false, bool sync = true)
        {
            int radiusLeft = (int)(position.X / 16f - (float)radius);
            int radiusRight = (int)(position.X / 16f + (float)radius);
            int radiusUp = (int)(position.Y / 16f - (float)radius);
            int radiusDown = (int)(position.Y / 16f + (float)radius);
            if (radiusLeft < 0) { radiusLeft = 0; }
            if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
            if (radiusUp < 0) { radiusUp = 0; }
            if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

            float distRad = radius * 16f;
            for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
            {
                for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                {
                    double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, y1 * 16f + 8f), position);
                    if (dist < distRad && Main.tile[x1, y1] != null && Main.tile[x1, y1].HasUnactuatedTile)
                    {
                        int currentType = Main.tile[x1, y1].TileType;
                        int index = 0;
                        if (BaseUtility.InArray(tiles, currentType, ref index))
                        {
                            GenerateTile(x1, y1, replacements[index], -1, 0, true, false, -2, silent, false);
                        }
                    }
                }
            }
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, (int)(position.X / 16f), (int)(position.Y / 16f), (radius * 2) + 2);
            }
        }

        /**
         *  Completely kills a chest at X, Y and removes all items within it.
         *  (note this does not remove the tile itself)
         */
        public static bool KillChestAndItems(int X, int Y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
                {
                    Main.chest[i] = null;
                    return true;
                }
            }
            return false;
        }

        /**
         *  Generates a single tile of liquid.
         *  isLava == true if you want lava instead of water.
         *  updateFlow == true if you want the flow to update after placement. (almost definitely yes)
         *  liquidHeight is the height given to the liquid. (0 - 255)
         */
        public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0, 255);


            var tile = Main.tile[x, y];
            tile.LiquidAmount = (byte)liquidHeight;
            tile.LiquidType = liquidType;

            if (updateFlow) { Liquid.AddWater(x, y); }
            if (sync && Main.netMode != NetmodeID.SinglePlayer) { NetMessage.SendTileSquare(-1, x, y, 1); }
        }

        /**
         *  Generates a width by height block of liquid with x, y being the top-left corner. 
         *  isLava == true if you want lava instead of water.
         */
        public static void GenerateLiquid(int x, int y, int width, int height, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int y1 = 0; y1 < height; y1++)
                {
                    GenerateLiquid(x1 + x, y1 + y, liquidType, updateFlow, liquidHeight, false);
                }
            }
            int size = width > height ? width : height;
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x + (int)(width * 0.5F) - 1, y + (int)(height * 0.5F) - 1, size + 4);
            }
        }

        /*
         *  Generates a single tile and wall at the given coordinates. (if the tile is > 1 x 1 it assumes the passed in coordinate is the top left)
         *  tile : type of tile to place. -1 means don't do anything tile related, -2 is used in conjunction with active == false to make air.
         *  wall : type of wall to place. -1 means don't do anything wall related. -2 is used to remove the wall already there.
         *  tileStyle : the style of the given tile. 
         *  active : If false, will make the tile 'air' and show the wall only.
         *  removeLiquid : If true, it will remove liquids in the generating area.
		 *  slope : if -2, keep the current slope. if -1, make it a halfbrick, otherwise make it the slope given.
		 *  silent : If true, will not display dust nor sound.
         *  sync : If true, will sync the client and server.
         */
        public static void GenerateTile(int x, int y, int tile, int wall, int tileStyle = 0, bool active = true, bool removeLiquid = true, int slope = -2, bool silent = false, bool sync = true)
        {
            try
            {
                var tileInstance = Main.tile[x, y];
                TileObjectData data = (tile <= -1 ? null : TileObjectData.GetTileData(tile, tileStyle, 0));
                int width = (data == null ? 1 : data.Width);
                int height = (data == null ? 1 : data.Height);
                int tileWidth = (tile == -1 || data == null ? 1 : data.Width);
                int tileHeight = (tile == -1 || data == null ? 1 : data.Height);
                var oldSlope = tileInstance.Slope;
                bool oldHalfBrick = tileInstance.IsHalfBlock;
                if (tile != -1)
                {
                    WorldGen.destroyObject = true;
                    if (width > 1 || height > 1)
                    {
                        int xs = x, ys = y;
                        Vector2 newPos = BaseTile.FindTopLeft(xs, ys);
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
                                tileInstance.TileType = 0;
                                tileInstance.HasTile = false;
                                if (!silent) { WorldGen.KillTile(x, y, false, false, true); }
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
                                WorldGen.SquareTileFrame(x2, y2, true);
                                WorldGen.SquareWallFrame(x2, y2, true);
                            }
                        }
                    }
                    else
                    if (!silent)
                    {
                        WorldGen.KillTile(x, y, false, false, true);
                    }
                    WorldGen.destroyObject = false;
                    if (active)
                    {
                        if (tileWidth <= 1 && tileHeight <= 1 && !Main.tileFrameImportant[tile])
                        {
                            tileInstance.TileType = (ushort)tile;
                            tileInstance.HasTile = true;
                            if (slope == -2 && oldHalfBrick || slope == -1) { tileInstance.IsHalfBlock = true; }
                            else
                            {
                                tileInstance.Slope = slope == -2 ? oldSlope : (SlopeType)slope;
                            }
                            WorldGen.SquareTileFrame(x, y, true);
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
                            int genY = (tile == BaseConstants.TILEID_DOORCLOSED ? y : y + height);
                            WorldGen.PlaceTile(genX, genY, tile, true, true, -1, tileStyle);
                            for (int x1 = 0; x1 < tileWidth; x1++)
                            {
                                for (int y1 = 0; y1 < tileHeight; y1++)
                                {
                                    WorldGen.SquareTileFrame(x + x1, y + y1, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        tileInstance.HasTile = false;
                    }
                }
                if (wall != -1)
                {
                    if (wall == -2) { wall = 0; }
                    tileInstance.WallType = 0;
                    WorldGen.PlaceWall(x, y, wall, true);
                }
                if (sync && Main.netMode != NetmodeID.SinglePlayer)
                {
                    int sizeWidth = tileWidth + (int)Math.Max(0, (width - 1));
                    int sizeHeight = tileHeight + (int)Math.Max(0, (height - 1));
                    int size = sizeWidth > sizeHeight ? sizeWidth : sizeHeight;
                    NetMessage.SendTileSquare(-1, x + (int)(size * 0.5F), y + (int)(size * 0.5F), size + 1);
                }
            }
            catch (Exception e)
            {
                //ErrorLogger.Log("TILEGEN ERROR: " + e.Message); ErrorLogger.Log(e.StackTrace); ErrorLogger.Log("--------");
            }
        }

        #region worldgen

        /**
		 *  Generates a line of tiles/walls from one point to another point.
		 *  
		 *  thickness: How thick to make the walls of the line.
	     *  sync : If true, will sync the client and server.
		 */
        public static void GenerateLine(GenConditions gen, int x, int y, int endX, int endY, int thickness, bool sync = true)
        {
            if (gen == null) throw new Exception("GenConditions cannot be null!");
            if (endX < x) { int temp = x; x = endX; endX = temp; }
            bool negativeY = endY < y; if (negativeY) x += Math.Abs(endX - x); //move it back since this essentially flips it on the X axis
            if (x == endX && y == endY) //it's just one tile...lol
            {
                int tileID = gen.GetTile(0), wallID = gen.GetWall(0);
                if ((tileID > -1 && gen.CanPlace != null && !gen.CanPlace(x, y, tileID, wallID)) || (wallID > -1 && gen.CanPlaceWall != null && !gen.CanPlaceWall(x, y, tileID, wallID))) return;
                GenerateTile(x, y, tileID, wallID, 0, tileID != -1, true, 0, false, sync);
                if (gen.slope) SmoothTiles(x, y, x, y);
            }
            else
            if (x == endX || y == endY) //check to see if it's a straight line. If it is, use the less expensive method of genning.
            {
                if (endY < y) { int temp = y; y = endY; endY = temp; }
                bool vertical = x == endX;
                int tileIndex = -1, wallIndex = -1;
                for (int m = 0; m < (vertical ? endY - y : endX - x); m++)
                {
                    for (int n = 0; n < thickness; n++)
                    {
                        tileIndex = (gen.tiles == null ? -1 : (gen.orderTiles ? tileIndex + 1 : WorldGen.genRand.Next(gen.tiles.Length)));
                        wallIndex = (gen.walls == null ? -1 : (gen.orderWalls ? wallIndex + 1 : WorldGen.genRand.Next(gen.walls.Length)));
                        if (tileIndex != -1 && tileIndex >= gen.tiles.Length) tileIndex = 0;
                        if (wallIndex != -1 && wallIndex >= gen.walls.Length) wallIndex = 0;
                        int addonX = (vertical ? n : m), addonY = (vertical ? m : n);
                        int x2 = x + addonX, y2 = y + addonY;

                        bool tileValid = tileIndex == -1 || (gen.CanPlace == null || gen.CanPlace(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex)));
                        bool wallValid = wallIndex == -1 || (gen.CanPlaceWall == null || gen.CanPlaceWall(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex)));
                        if (tileValid && wallValid)
                        {
                            GenerateTile(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex), 0, gen.GetTile(tileIndex) != -1, true, 0, false, false);
                        }
                    }
                }
                if (gen.slope)
                {
                    //SmoothTiles(x, y, x + (vertical ? thickness : Math.Abs(endX - x)), y + (vertical ? Math.Abs(endY - y) : thickness));
                }
                if (sync && Main.netMode != NetmodeID.SinglePlayer)
                {
                    int size = ((endY - y) > (endX - x) ? (endY - y) : (endX - x));
                    if (thickness > size) size = thickness;
                    NetMessage.SendData(MessageID.TileSquare, -1, -1, NetworkText.FromLiteral(""), size, (float)x, (float)y, 0f, 0);
                }
            }
            else //genning a line that isn't straight
            {
                Vector2 start = new Vector2(x, y), end = new Vector2(endX, endY), dir = new Vector2(endX, endY) - new Vector2(x, y);
                dir.Normalize();
                float length = Vector2.Distance(start, end);
                float way = 0f;

                float rot = (float)BaseUtility.RotationTo(start, end); if (rot < 0f) rot = (float)(Math.PI * 2f) - Math.Abs(rot);
                float rotPercent = MathHelper.Lerp(0f, 1f, rot / (float)(Math.PI * 2f));
                bool horizontal = rotPercent < 0.125f || (rotPercent > 0.375f && rotPercent < 0.625f) || rotPercent > 0.825f;
                int tileIndex = -1, wallIndex = -1;
                int lastX = x, lastY = y;
                while (way < length)
                {
                    Vector2 v = (start + dir * way);
                    Point point = new Point((int)v.X, (int)v.Y);
                    for (int n = 0; n < thickness; n++)
                    {
                        tileIndex = (gen.tiles == null ? -1 : (gen.orderTiles ? tileIndex + 1 : WorldGen.genRand.Next(gen.tiles.Length)));
                        wallIndex = (gen.walls == null ? -1 : (gen.orderWalls ? wallIndex + 1 : WorldGen.genRand.Next(gen.walls.Length)));
                        if (tileIndex != -1 && tileIndex >= gen.tiles.Length) tileIndex = 0;
                        if (wallIndex != -1 && wallIndex >= gen.walls.Length) wallIndex = 0;

                        int addonX = (horizontal ? 0 : n), addonY = (horizontal ? n : 0);
                        int x2 = point.X + addonX, y2 = (negativeY ? point.Y - addonY : point.Y + addonY);

                        bool tileValid = tileIndex == -1 || (gen.CanPlace == null || gen.CanPlace(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex)));
                        bool wallValid = wallIndex == -1 || (gen.CanPlaceWall == null || gen.CanPlaceWall(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex)));
                        if (tileValid && wallValid)
                        {
                            GenerateTile(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex), 0, gen.GetTile(tileIndex) != -1, true, 0, false, false);
                            //if (gen.slope) SmoothTiles(x2, y2, x2 + 1, y2 + 1);
                        }
                    }
                    if (sync && Main.netMode != NetmodeID.SinglePlayer && ((!horizontal && Math.Abs(lastY - point.Y) >= 5) || (horizontal && Math.Abs(lastY - point.Y) >= 5) || (way + 1 > length)))
                    {
                        int size = (int)Math.Max(5, thickness);
                        NetMessage.SendData(MessageID.TileSection, -1, -1, NetworkText.FromLiteral(""), lastX, lastY, size, size, 0);
                        lastX = point.X; lastY = point.Y;
                    }
                    way += 1;
                }
            }
        }

        /**
		 *  Generates a hollow hallway with (x, y) as the top left. 
		 *  Note that (endX, endY) is NOT the actual end of the hallway, but the end of the inner wall.
		 *  
		 *  thickness: How thick to make the walls of the hallway.
		 *  height: The height of the hallway. (width if it's going up/down)
         *  sync : If true, will sync the client and server.
		 */
        public static void GenerateHall(GenConditions gen, int x, int y, int endX, int endY, int thickness, int height, bool sync = true)
        {
            if (gen == null) throw new Exception("GenConditions cannot be null!");
            if (endX < x) { int temp = x; x = endX; endX = temp; }
            //if (endY < y) { int temp = y; y = endY; endY = temp; }
            bool negativeX = endX < x, negativeY = endY < y;
            int nx = (negativeX ? -1 : 1), ny = (negativeY ? -1 : 1);
            Vector2 start = new Vector2(x, y), end = new Vector2(endX, endY);
            float rotPercent = MathHelper.Lerp(0f, 1f, (BaseUtility.RotationTo(start, end) / (float)(Math.PI * 2f)));
            bool horizontal = rotPercent < 0.125f || (rotPercent > 0.375f && rotPercent < 0.625f) || rotPercent > 0.825f;
            Vector2 topEnd = new Vector2(endX, endY);
            int[] clearInt = new int[] { -2 };
            Vector2 wallStart = new Vector2((horizontal ? x : x + (2 * nx)), (horizontal ? y + (2 * ny) : y)), wallEnd = new Vector2((horizontal ? endX : endX + (2 * nx)), (horizontal ? endY + (2 * ny) : endY));
            Vector2 bottomStart = new Vector2((horizontal ? x : x + (((thickness * 2) + height) * nx)), (horizontal ? y + (((thickness * 2) + height) * ny) : y)), bottomEnd = new Vector2((horizontal ? endX : endX + (((thickness * 2) + height) * nx)), (horizontal ? endY + (((thickness * 2) + height) * ny) : endY));
            int[] tiles = gen.tiles, walls = gen.walls;
            gen.tiles = null;
            BaseWorldGen.GenerateLine(gen, (int)wallStart.X, (int)wallStart.Y, (int)wallEnd.X, (int)wallEnd.Y, (thickness * 3) + height - 2, false);
            gen.tiles = tiles;
            gen.walls = null;
            BaseWorldGen.GenerateLine(gen, x, y, (int)topEnd.X, (int)topEnd.Y, thickness, false);
            BaseWorldGen.GenerateLine(gen, (int)bottomStart.X, (int)bottomStart.Y, (int)bottomEnd.X, (int)bottomEnd.Y, thickness, false);
            gen.walls = walls;
        }

        /**
		 *  Generates a hollow trapezoid with (x, y) as the top left. 
		 *  Note that (endX, endY) is NOT the actual end of the room, but the end of the inner wall.
		 *  
		 *  thickness: How thick to make the walls of the trapezoid.
		 *  height: The height of the trapezoid.
         *  sync : If true, will sync the client and server.
		 */
        public static void GenerateTrapezoid(GenConditions gen, int x, int y, int endX, int endY, int thickness, int height, bool sync = true)
        {
            if (gen == null) throw new Exception("GenConditions cannot be null!");
            if (endX < x) { int temp = x; x = endX; endX = temp; }
            //if (endY < y) { int temp = y; y = endY; endY = temp; }
            Vector2 start = new Vector2(x, y), end = new Vector2(endX, endY);
            float rotPercent = MathHelper.Lerp(0f, 1f, (BaseUtility.RotationTo(start, end) / (float)(Math.PI * 2f)));
            bool horizontal = rotPercent < 0.125f || (rotPercent > 0.375f && rotPercent < 0.625f) || rotPercent > 0.825f;
            Vector2 topEnd = new Vector2(endX, endY);
            Vector2 wallStart = new Vector2(x + thickness, y + thickness), wallEnd = new Vector2((horizontal ? endX : endX + thickness), (horizontal ? endY + thickness : endY));
            Vector2 bottomStart = new Vector2((horizontal ? x : x + (thickness * 2) + height), (horizontal ? y + (thickness * 2) + height : y)), bottomEnd = new Vector2((horizontal ? endX : endX + (thickness * 2) + height), (horizontal ? endY + (thickness * 2) + height : endY));
            int[] tiles = gen.tiles, walls = gen.walls;
            gen.tiles = null;
            GenerateLine(gen, (int)wallStart.X, (int)wallStart.Y, (int)wallEnd.X, (int)wallEnd.Y, thickness + height, false);
            gen.tiles = tiles;
            gen.walls = null;
            GenerateLine(gen, x, y, (int)topEnd.X, (int)topEnd.Y, thickness, false);
            GenerateLine(gen, (int)bottomStart.X, (int)bottomStart.Y, (int)bottomEnd.X, (int)bottomEnd.Y, thickness, false);
            GenerateLine(gen, x, y, (int)bottomStart.X, (int)bottomStart.Y, thickness, false);
            GenerateLine(gen, (int)topEnd.X, (int)topEnd.Y, (horizontal ? (int)bottomEnd.X : (int)bottomEnd.X + thickness), (horizontal ? (int)bottomEnd.Y + thickness : (int)bottomEnd.Y), thickness, false);
            gen.walls = walls;
        }

        #endregion

        /**
		 *  Generates a width by height hollow room with x, y being the top-left corner of the room, using wall as the walls in the space in the middle.
		 */
        public static void GenerateRoomOld(int x, int y, int width, int height, int tile, int wall)
        {
            GenerateRoomOld(x, y, width, height, tile, tile, tile, wall);
        }

        /**
         *  Generates a width by height hollow room with x, y being the top-left corner of the room, using wall as the walls in the space in the middle.
         *  Making any of the tile vars -1 will result in that piece of the structure not generating. (ie if tileSides == -1, both sides will be w/e was there
         *  before them.)
         *  wallEnds : true if you want every tile to have walls behind them instead of just the tileless ones.
         */
        public static void GenerateRoomOld(int x, int y, int width, int height, int tileSides, int tileFloor, int tileCeiling, int wall, bool wallEnds = false, int sideThickness = 1, int floorThickness = 1, int ceilingThickness = 1, bool sync = true)
        {
            if (tileSides != -1 && sideThickness > 1) { width += sideThickness; x -= sideThickness / 2; }
            if (tileFloor != -1 && floorThickness > 1) { height += floorThickness; }
            if (tileCeiling != -1 && ceilingThickness > 1) { height += ceilingThickness; y -= ceilingThickness / 2; }
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int y1 = 0; y1 < height; y1++)
                {
                    int x2 = x1 + x;
                    int y2 = y1 + y;
                    if ((wallEnds || tileCeiling != -1) && y1 < ceilingThickness) //ceiling
                    {
                        GenerateTile(x2, y2, tileCeiling, (wallEnds && y1 == 0 ? wall : -1), 0, tileCeiling == -1 ? !wallEnds : true, true, 0, false, false);
                    }
                    else
                    if ((wallEnds || tileFloor != -1) && y1 >= height - floorThickness) //floor
                    {
                        GenerateTile(x2, y2, tileFloor, (wallEnds && y1 >= height - 1 ? wall : -1), 0, tileFloor == -1 ? !wallEnds : true, true, 0, false, false);
                    }
                    else
                    if ((wallEnds || tileSides != -1) && (x1 < sideThickness || x1 >= width - sideThickness)) //sides
                    {
                        GenerateTile(x2, y2, tileSides, (wallEnds && (x1 > 0 && x1 < width - 1) ? wall : -1), 0, tileSides == -1 ? !wallEnds : true, true, 0, false, false);
                    }
                    else
                    if (x1 >= sideThickness && x1 < width - sideThickness && y1 >= ceilingThickness && y1 < height - floorThickness)
                    {
                        GenerateTile(x2, y2, -1, wall, 0, false, true, 0, false, false);
                    }
                }
            }
            int size = (width > height ? width : height);
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x + (int)(width * 0.5F) - 1, y + (int)(height * 0.5F) - 1, size + 4);
            }
        }

        /**
         *  Generates a chest with the given item IDs. 
         *  randomAmounts should be true if the item(s) should have a random stack amount (between 1-5).
         *  randomPrefix should be true if the item(s) should get a random prefix.
         */
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, bool randomAmounts = false, bool randomPrefix = false, bool sync = true)
        {
            int[] amounts = new int[20];
            for (int m = 0; m < amounts.Length; m++)
            {
                if (randomAmounts) { amounts[m] = WorldGen.genRand.Next(1, 6); } else { amounts[m] = 1; }
            }
            GenerateChest(x, y, type, chestStyle, stackIDs, amounts, randomPrefix, sync);
        }

        /**
         *  Generates a chest with the given item IDs and stack amounts. 
         *  randomPrefix should be true if the item should get a random prefix.
         */
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, int[] stackAmounts, bool randomPrefix = false, bool sync = true)
        {
            int[] prefixes = new int[20];
            for (int m = 0; m < prefixes.Length; m++)
            {
                if (randomPrefix) { prefixes[m] = -1; } else { prefixes[m] = -10; }
            }
            GenerateChest(x, y, type, chestStyle, stackIDs, stackAmounts, prefixes, sync);
        }

        /**
         *  Generates a chest with the given item ids, prefixes and stack amounts.
         */
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, int[] stackAmounts, int[] stackPrefixes, bool sync = true)
        {
            int num2 = WorldGen.PlaceChest(x - 1, y, (ushort)type, false, chestStyle);
            if (num2 >= 0)
            {
                for (int m = 0; m < Main.chest[num2].item.Length; m++)
                {
                    if (stackIDs == null || stackIDs.Length <= m) break;
                    Main.chest[num2].item[m].SetDefaults(stackIDs[m], false);
                    Main.chest[num2].item[m].stack = stackAmounts[m];
                    if (stackPrefixes[m] != -10) { Main.chest[num2].item[m].Prefix(stackPrefixes[m]); }
                }
            }
            WorldGen.SquareTileFrame(x + 1, y, true);
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x, y, 2);
            }
        }

        public static void SmoothTiles(int topX, int topY, int bottomX, int bottomY)
        {
            Main.tileSolid[137] = false;
            for (int x = topX; x < bottomX; x++)
            {
                for (int y = topY; y < bottomY; y++)
                {
                    var tile = Main.tile[x, y];
                    if (tile.TileType != 48 && tile.TileType != 137 && tile.TileType != 232 && tile.TileType != 191 && tile.TileType != 151 && tile.TileType != 274)
                    {
                        if (!Main.tile[x, y - 1].HasTile)
                        {
                            if (WorldGen.SolidTile(x, y))
                            {
                                if (!Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == SlopeType.Solid && Main.tile[x + 1, y].Slope == SlopeType.Solid)
                                {
                                    if (WorldGen.SolidTile(x, y + 1))
                                    {
                                        if (!WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y - 1].HasTile)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                WorldGen.SlopeTile(x, y, 2);
                                            }
                                            else
                                            {
                                                WorldGen.PoundTile(x, y);
                                            }
                                        }
                                        else if (!WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y - 1].HasTile)
                                        {
                                            if (WorldGen.genRand.Next(2) == 0)
                                            {
                                                WorldGen.SlopeTile(x, y, 1);
                                            }
                                            else
                                            {
                                                WorldGen.PoundTile(x, y);
                                            }
                                        }
                                        else if (WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y + 1) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y].HasTile)
                                        {
                                            WorldGen.PoundTile(x, y);
                                        }
                                        if (WorldGen.SolidTile(x, y))
                                        {
                                            if (WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y + 2) && !Main.tile[x + 1, y].HasTile && !Main.tile[x + 1, y + 1].HasTile && !Main.tile[x - 1, y - 1].HasTile)
                                            {
                                                WorldGen.KillTile(x, y, false, false, false);
                                            }
                                            else if (WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y + 2) && !Main.tile[x - 1, y].HasTile && !Main.tile[x - 1, y + 1].HasTile && !Main.tile[x + 1, y - 1].HasTile)
                                            {
                                                WorldGen.KillTile(x, y, false, false, false);
                                            }
                                            else if (!Main.tile[x - 1, y + 1].HasTile && !Main.tile[x - 1, y].HasTile && WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x, y + 2))
                                            {
                                                if (WorldGen.genRand.Next(5) == 0) WorldGen.KillTile(x, y, false, false, false);
                                                else if (WorldGen.genRand.Next(5) == 0) WorldGen.PoundTile(x, y);
                                                else WorldGen.SlopeTile(x, y, 2);
                                            }
                                            else if (!Main.tile[x + 1, y + 1].HasTile && !Main.tile[x + 1, y].HasTile && WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x, y + 2))
                                            {
                                                if (WorldGen.genRand.Next(5) == 0)
                                                {
                                                    WorldGen.KillTile(x, y, false, false, false);
                                                }
                                                else if (WorldGen.genRand.Next(5) == 0)
                                                {
                                                    WorldGen.PoundTile(x, y);
                                                }
                                                else
                                                {
                                                    WorldGen.SlopeTile(x, y, 1);
                                                }
                                            }
                                        }
                                    }
                                    if (WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y].HasTile)
                                    {
                                        WorldGen.KillTile(x, y, false, false, false);
                                    }
                                }
                            }
                            else if (!Main.tile[x, y].HasTile && Main.tile[x, y + 1].TileType != 151 && Main.tile[x, y + 1].TileType != 274)
                            {
                                if (Main.tile[x + 1, y].TileType != 190 && Main.tile[x + 1, y].TileType != 48 && Main.tile[x + 1, y].TileType != 232 && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y - 1].HasTile)
                                {
                                    WorldGen.PlaceTile(x, y, (int)Main.tile[x, y + 1].TileType, false, false, -1, 0);
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        WorldGen.SlopeTile(x, y, 2);
                                    }
                                    else
                                    {
                                        WorldGen.PoundTile(x, y);
                                    }
                                }
                                if (Main.tile[x - 1, y].TileType != 190 && Main.tile[x - 1, y].TileType != 48 && Main.tile[x - 1, y].TileType != 232 && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y - 1].HasTile)
                                {
                                    WorldGen.PlaceTile(x, y, (int)Main.tile[x, y + 1].TileType, false, false, -1, 0);
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        WorldGen.SlopeTile(x, y, 1);
                                    }
                                    else
                                    {
                                        WorldGen.PoundTile(x, y);
                                    }
                                }
                            }
                        }
                        else if (!Main.tile[x, y + 1].HasTile && WorldGen.genRand.Next(2) == 0 && WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == 0 && Main.tile[x + 1, y].Slope == 0 && WorldGen.SolidTile(x, y - 1))
                        {
                            if (WorldGen.SolidTile(x - 1, y) && !WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y - 1))
                            {
                                WorldGen.SlopeTile(x, y, 3);
                            }
                            else if (WorldGen.SolidTile(x + 1, y) && !WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y - 1))
                            {
                                WorldGen.SlopeTile(x, y, 4);
                            }
                        }
                    }
                }
            }
            for (int x = topX; x < bottomX; x++)
            {
                for (int y = topY; y < bottomY; y++)
                {
                    if (WorldGen.genRand.Next(2) == 0 && !Main.tile[x, y - 1].HasTile && Main.tile[x, y].TileType != 137 && Main.tile[x, y].TileType != 48 && Main.tile[x, y].TileType != 232 && Main.tile[x, y].TileType != 191 && Main.tile[x, y].TileType != 151 && Main.tile[x, y].TileType != 274 && Main.tile[x, y].TileType != 75 && Main.tile[x, y].TileType != 76 && WorldGen.SolidTile(x, y) && Main.tile[x - 1, y].TileType != 137 && Main.tile[x + 1, y].TileType != 137)
                    {
                        if (WorldGen.SolidTile(x, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile)
                        {
                            WorldGen.SlopeTile(x, y, 2);
                        }
                        if (WorldGen.SolidTile(x, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile)
                        {
                            WorldGen.SlopeTile(x, y, 1);
                        }
                    }
                    if (Main.tile[x, y].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(x - 1, y))
                    {
                        WorldGen.SlopeTile(x, y, 0);
                        WorldGen.PoundTile(x, y);
                    }
                    if (Main.tile[x, y].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(x + 1, y))
                    {
                        WorldGen.SlopeTile(x, y, 0);
                        WorldGen.PoundTile(x, y);
                    }
                }
            }
            Main.tileSolid[137] = true;
        }
    }

    /*
	 * A class used by the newer gen methods to make calling them easier without needing to provide a ton of parameters multiple times.
	 * 
	 *  tiles: An array of tiles to use to generate this hallway.
	 *  orderTiles: If true, gets tile ids in order; otherwise gets them at random.
	 *  walls: An array of walls to use to generate the inside of this hallway. Default is none.
	 *  orderWalls: If true, gets wall ids in order; otherwise gets them at random.
	 *  slope: if true, smoothes the gen with slopes.
	 *  CanPlace(x, y, tileID, wallID): an optional Func that can be used to have custom behavior about placing tiles. 
	 *  CanPlaceWall(x, y, tileID, wallID): an optional Func that can be used to have custom behavior about placing walls. 
	 */
    public class GenConditions
    {
        public int[] tiles = null;
        public int[] walls = null;
        public bool orderTiles = false;
        public bool orderWalls = false;
        public bool slope = false;
        public Func<int, int, int, int, bool> CanPlace = null;
        public Func<int, int, int, int, bool> CanPlaceWall = null;

        public int GetTile(int index)
        {
            return (tiles == null || tiles.Length <= index ? -1 : tiles[index]);
        }

        public int GetWall(int index)
        {
            return (walls == null || walls.Length <= index ? -1 : walls[index]);
        }
    }

    #region Custom GenShapes
    public class ShapeChasmSideways : GenShape
    {
        public int _startheight = 20, _endheight = 5, _length = 60, _variance = 0, _randomHeading = 0;
        public float[] _heightVariance = null;
        public bool _dir = true;

        public ShapeChasmSideways(int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance = null, bool dir = true)
        {
            this._startheight = startheight;
            this._endheight = endheight;
            this._length = length;
            this._variance = variance;
            this._randomHeading = randomHeading;
            this._heightVariance = heightVariance;
            this._dir = dir;
        }

        public void ResetChasmParams(int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance = null, bool dir = true)
        {
            this._startheight = startheight;
            this._endheight = endheight;
            this._length = length;
            this._variance = variance;
            this._randomHeading = randomHeading;
            this._heightVariance = heightVariance;
            this._dir = dir;
        }

        private bool DoChasm(Point origin, GenAction action, int startheight, int endheight, int length, int variance, int randomHeading, float[] heightVariance, bool dir)
        {
            Point trueOrigin = origin;
            for (int m = 0; m < length; m++)
            {
                int height = (int)MathHelper.Lerp((float)startheight, (float)endheight, (float)m / (float)length);
                if (heightVariance != null)
                {
                    height = Math.Max(endheight, (int)((float)startheight * BaseUtility.MultiLerp((float)m / (float)length, heightVariance)));
                }
                int x = trueOrigin.X + (dir ? m : -m);
                int y = trueOrigin.Y + (startheight - height);
                if (variance != 0)
                {
                    y += (Main.rand.Next(2) == 0 ? -Main.rand.Next(variance) : Main.rand.Next(variance));
                }
                if (randomHeading != 0)
                {
                    y += randomHeading * (m / 2);
                }
                int yend = y + height - (startheight - height);
                int difference = yend - y;
                for (int m2 = y; m2 < yend; m2++)
                {
                    int y2 = m2;
                    if (!base.UnitApply(action, trueOrigin, x, y2, new object[0]) && this._quitOnFail)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Perform(Point origin, GenAction action)
        {
            return this.DoChasm(origin, action, this._startheight, this._endheight, this._length, this._variance, this._randomHeading, this._heightVariance, this._dir);
        }
    }

    public class ShapeChasm : GenShape
    {
        public int _startwidth = 20, _endwidth = 5, _depth = 60, _variance = 0, _randomHeading = 0;
        public float[] _widthVariance = null;
        public bool _dir = true;

        public ShapeChasm(int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance = null, bool dir = true)
        {
            this._startwidth = startwidth;
            this._endwidth = endwidth;
            this._depth = depth;
            this._variance = variance;
            this._randomHeading = randomHeading;
            this._widthVariance = widthVariance;
            this._dir = dir;
        }

        public void ResetChasmParams(int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance = null, bool dir = true)
        {
            this._startwidth = startwidth;
            this._endwidth = endwidth;
            this._depth = depth;
            this._variance = variance;
            this._randomHeading = randomHeading;
            this._widthVariance = widthVariance;
            this._dir = dir;
        }

        private bool DoChasm(Point origin, GenAction action, int startwidth, int endwidth, int depth, int variance, int randomHeading, float[] widthVariance, bool dir)
        {
            Point trueOrigin = origin;
            for (int m = 0; m < depth; m++)
            {
                int width = (int)MathHelper.Lerp((float)startwidth, (float)endwidth, (float)m / (float)depth);
                if (widthVariance != null)
                {
                    width = Math.Max(endwidth, (int)((float)startwidth * BaseUtility.MultiLerp((float)m / (float)depth, widthVariance)));
                }
                int x = trueOrigin.X + (startwidth - width);
                int y = trueOrigin.Y + (dir ? m : -m);
                if (variance != 0)
                {
                    x += (Main.rand.Next(2) == 0 ? -Main.rand.Next(variance) : Main.rand.Next(variance));
                }
                if (randomHeading != 0)
                {
                    x += randomHeading * (m / 2);
                }
                int xend = x + width - (startwidth - width);
                for (int m2 = x; m2 < xend; m2++)
                {
                    int x2 = m2;
                    if (!base.UnitApply(action, trueOrigin, x2, y, new object[0]) && this._quitOnFail)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Perform(Point origin, GenAction action)
        {
            return this.DoChasm(origin, action, this._startwidth, this._endwidth, this._depth, this._variance, this._randomHeading, this._widthVariance, this._dir);
        }
    }

    #endregion

    #region Custom GenActions
    public class SetModTile : GenAction
    {
        public ushort _type;
        public bool _doFraming;
        public bool _doNeighborFraming;
        public Func<int, int, Tile, bool> _canReplace;

        public SetModTile(ushort type, bool setSelfFrames = false, bool setNeighborFrames = true)
        {
            this._type = type;
            this._doFraming = setSelfFrames;
            this._doNeighborFraming = setNeighborFrames;
        }

        public SetModTile ExtraParams(Func<int, int, Tile, bool> canReplace)
        {
            _canReplace = canReplace;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            if (_canReplace == null || (_canReplace != null && _canReplace(x, y, GenBase._tiles[x, y])))
            {
                _tiles[x, y].ResetToType(this._type);
                if (this._doFraming)
                {
                    WorldUtils.TileFrame(x, y, this._doNeighborFraming);
                }
            }
            return base.UnitApply(origin, x, y, args);
        }
    }

    public class SetMapBrightness : GenAction
    {
        public byte _brightness;

        public SetMapBrightness(byte brightness)
        {
            this._brightness = brightness;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            Main.Map.UpdateLighting(x, y, (byte)Math.Max(Main.Map[x, y].Light, _brightness));
            return base.UnitApply(origin, x, y, args);
        }
    }


    public class PlaceModWall : GenAction
    {
        public ushort _type;
        public bool _neighbors;
        public Func<int, int, Tile, bool> _canReplace;

        public PlaceModWall(int type, bool neighbors = true)
        {
            this._type = (ushort)type;
            this._neighbors = neighbors;
        }

        public PlaceModWall ExtraParams(Func<int, int, Tile, bool> canReplace)
        {
            _canReplace = canReplace;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            if (_canReplace == null || (_canReplace != null && _canReplace(x, y, GenBase._tiles[x, y])))
            {
                GenBase._tiles[x, y].WallType = this._type;
                WorldGen.SquareWallFrame(x, y, true);
                if (this._neighbors)
                {
                    WorldGen.SquareWallFrame(x + 1, y, true);
                    WorldGen.SquareWallFrame(x - 1, y, true);
                    WorldGen.SquareWallFrame(x, y - 1, true);
                    WorldGen.SquareWallFrame(x, y + 1, true);
                }
            }
            return base.UnitApply(origin, x, y, args);
        }
    }

    public class RadialDitherTopMiddle : GenAction
    {
        private int _width, _height;
        private float _innerRadius, _outerRadius;

        public RadialDitherTopMiddle(int width, int height, float innerRadius, float outerRadius)
        {
            this._width = width;
            this._height = height;
            this._innerRadius = innerRadius;
            this._outerRadius = outerRadius;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            Vector2 value = new Vector2((float)origin.X + (_width / 2), (float)origin.Y);
            Vector2 value2 = new Vector2((float)x, (float)y);
            float num = Vector2.Distance(value2, value);
            float num2 = Math.Max(0f, Math.Min(1f, (num - this._innerRadius) / (this._outerRadius - this._innerRadius)));
            if (GenBase._random.NextDouble() > (double)num2)
            {
                return base.UnitApply(origin, x, y, args);
            }
            return base.Fail();
        }
    }
    #endregion

    #region Custom Conditions
    public class IsNotSloped : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            return _tiles[x, y].HasTile && _tiles[x, y].Slope == SlopeType.Solid && !_tiles[x, y].IsHalfBlock;
        }
    }
    public class IsSloped : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            return _tiles[x, y].HasTile && (_tiles[x, y].Slope > 0 || _tiles[x, y].IsHalfBlock);
        }
    }
    #endregion

    #region Custom Modifiers

    #endregion
}