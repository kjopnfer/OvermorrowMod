using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Common.Base
{
    public class BaseWorldGenTex
    {
        public static Dictionary<Color, int> colorToLiquid = null;

        //NOTE: all textures MUST be the same size or horrible things happen!
        public static TexGen GetTexGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile, Texture2D wallTex = null, Dictionary<Color, int> colorToWall = null, Texture2D liquidTex = null, Texture2D wireTex = null, Texture2D slopeTex = null, Dictionary<Color, int> colorToSlopes = null, int platformStyle = 0)
        {
            if (colorToLiquid == null)
            {
                colorToLiquid = new Dictionary<Color, int>();
                colorToLiquid[new Color(0, 0, 255)] = 0;
                colorToLiquid[new Color(255, 0, 0)] = 1;
                colorToLiquid[new Color(255, 255, 0)] = 2;
                colorToLiquid[new Color(150, 150, 150)] = -2;
            }


            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            Color[] wallData = (wallTex != null ? new Color[wallTex.Width * wallTex.Height] : null);
            Color[] liquidData = (liquidTex != null ? new Color[liquidTex.Width * liquidTex.Height] : null);
            Color[] slopeData = (slopeTex != null ? new Color[slopeTex.Width * slopeTex.Height] : null);
            var evt = new ManualResetEvent(false);
            Main.QueueMainThreadAction(() =>
            {
                tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);
                if (wallData != null) wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);
                if (liquidData != null) liquidTex.GetData(0, liquidTex.Bounds, liquidData, 0, liquidTex.Width * liquidTex.Height);
                if (slopeData != null) slopeTex.GetData(0, slopeTex.Bounds, slopeData, 0, slopeTex.Width * slopeTex.Height);
                evt.Set();
            });

            evt.WaitOne();

            int x = 0, y = 0;
            TexGen gen = new TexGen(tileTex.Width, tileTex.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m], wallColor = (wallTex == null ? Color.Black : wallData[m]), liquidColor = (liquidTex == null ? Color.Black : liquidData[m]), slopeColor = (slopeTex == null ? Color.Black : slopeData[m]);
                int tileID = (colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1); //if no key assume no action
                int wallID = (colorToWall != null && colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1);
                int liquidID = (colorToLiquid != null && colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1);
                int slopeID = (colorToSlopes != null && colorToSlopes.ContainsKey(slopeColor) ? colorToSlopes[slopeColor] : -2);
                gen.tileGen[x, y] = new TileInfo(tileID, 0, wallID, liquidID, liquidID == -1 ? 0 : 255, slopeID);
                gen.platformStyle = platformStyle;
                x++;
                if (x >= tileTex.Width) { x = 0; y++; }
                if (y >= tileTex.Height) break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }

            return gen;
        }
        public static TexGen GetPlatformTexGenerator(Texture2D platformTex, Dictionary<Color, int> colorToPlatform) // platforms wont generate using above for some reason
        {
            Color[] platformData = new Color[platformTex.Width * platformTex.Height];
            var evt = new ManualResetEvent(false);

            Main.QueueMainThreadAction(() =>
            {
                platformTex.GetData(0, platformTex.Bounds, platformData, 0, platformTex.Width * platformTex.Height);
                evt.Set();
            });

            evt.WaitOne();

            int x = 0, y = 0;
            TexGen gen = new TexGen(platformTex.Width, platformTex.Height);
            for (int m = 0; m < platformData.Length; m++)
            {
                Color tileColor = platformData[m];
                int platformStyle = (colorToPlatform.ContainsKey(tileColor) ? colorToPlatform[tileColor] : 0);
                int tileID = platformData[m] == Color.Black ? -1 : TileID.Platforms;
                gen.tileGen[x, y] = new TileInfo(tileID, 0);
                gen.platformStyle = 13;
                x++;
                if (x >= platformTex.Width)
                {
                    x = 0; y++;
                }
                if (y >= platformTex.Height)
                {
                    break; //you've somehow reached the end of the texture! (this shouldn't happen!)
                }
            }

            return gen;
        }
    }

    public class TexGen
    {
        public int width, height;
        public TileInfo[,] tileGen;
        public int torchStyle = 0, platformStyle = 0;

        public TexGen(int w, int h)
        {
            width = w; height = h;
            tileGen = new TileInfo[width, height];
        }

        public void Generate(int x, int y, bool silent, bool sync, bool removeSlopes = false, bool flip = false)
        {
            if (!flip)
            {
                for (int x1 = 0; x1 < width; x1++)
                {
                    for (int y1 = 0; y1 < height; y1++)
                    {
                        int x2 = x + x1, y2 = y + y1;
                        TileInfo info = tileGen[x1, y1];
                        if (removeSlopes) info.slope = 0;
                        if (info.tileID == -1 && info.wallID == -1 && info.liquidType == -1 && info.wire == -1) continue;
                        if (info.tileID != -1 || info.wallID > -1 || info.wire > -1) BaseWorldGen.GenerateTile(x2, y2, info.tileID, info.wallID, (info.tileStyle != 0 ? info.tileStyle : info.tileID == TileID.Torches ? torchStyle : info.tileID == TileID.Platforms ? platformStyle : 0), info.tileID > -1, info.liquidAmt == 0, info.slope == 99 ? info.slope : info.slope, false, sync);
                        if (info.slope == 99)
                        {
                            var tile = Main.tile[x2, y2];
                            tile.IsHalfBlock = true;
                        }
                        if (info.liquidType > -1)
                        {
                            BaseWorldGen.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmt, sync);
                        }
                        else if (info.liquidType == -2)
                        {
                            var tile = Main.tile[x1, y1];
                            tile.LiquidType = LiquidID.Water;
                        }
                    }
                }
            }
            else
            {
                // added by ea
                for (int x1 = 0; x1 < width; x1++)
                {
                    for (int y1 = 0; y1 < height; y1++)
                    {
                        int x2 = x + width - x1, y2 = y + y1;
                        TileInfo info = tileGen[x1, y1];
                        if (removeSlopes) info.slope = 0;
                        if (info.tileID == -1 && info.wallID == -1 && info.liquidType == -1 && info.wire == -1) continue;
                        if (info.tileID != -1 || info.wallID > -1 || info.wire > -1) BaseWorldGen.GenerateTile(x2, y2, info.tileID, info.wallID, (info.tileStyle != 0 ? info.tileStyle : info.tileID == TileID.Torches ? torchStyle : info.tileID == TileID.Platforms ? platformStyle : 0), info.tileID > -1, info.liquidAmt == 0, info.slope == 99 ? info.slope : info.slope, false, sync);
                        if (info.slope == 99)
                        {
                            var tile = Main.tile[x2, y2];
                            tile.IsHalfBlock = true;
                        }
                        if (info.liquidType > -1)
                        {
                            BaseWorldGen.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmt, sync);
                        }
                        else if (info.liquidType == -2)
                        {
                            var tile = Main.tile[x1, y1];
                            tile.LiquidType = LiquidID.Water;
                        }
                    }
                }
            }
        }
    }

    public class TileInfo
    {
        public int tileID = -1, tileStyle = 0, wallID = -1;
        public int liquidType = -1, liquidAmt = 0; //liquidType can be 0 (water), 1 (lava), 2 (honey)
        public int slope = -2, wire = -1;

        public TileInfo(int id, int style, int wid = -1, int lType = -1, int lAmt = 0, int sl = -2, int w = -1)
        {
            tileID = id; tileStyle = style; wallID = wid; liquidType = lType; liquidAmt = lAmt; slope = sl; wire = w;
        }
    }
}