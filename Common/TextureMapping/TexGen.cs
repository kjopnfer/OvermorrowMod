using AsepriteDotNet.Aseprite;
using AsepriteDotNet.Aseprite.Types;
using AsepriteDotNet.Common;
using AsepriteDotNet.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.TextureMapping
{
    public enum SheetLayer { Walls, Tiles, Objects }

    // Code by GroxTheGreat, made functional on servers by Feldy
    public class TexGen
    {
        private class TileInfo
        {
            public int tileID = -1, tileStyle, wallID = -1;
            public (int, int) objectID;
            public int liquidType = -1, liquidAmt; //liquidType can be 0 (water), 1 (lava), 2 (honey)
            public int slope = -2, wire = -1;

            public TileInfo(int id, int style, int wallId = -1, int liquidType = -1, int liquidAmount = 0, int slope = -2, (int, int) objectId = default, int wire = -1)
            {
                tileID = id; 
                tileStyle = style; 
                wallID = wallId; 
                this.liquidType = liquidType; 
                liquidAmt = liquidAmount; 
                this.slope = slope; 
                objectID = objectId; 
                this.wire = wire;
            }
        }

        public static Dictionary<Color, int> colorToLiquid;
        public static Dictionary<Color, int> colorToSlope;

        public int width { get; private set; }
        public int height { get; private set; }

        private TileInfo[,] tileGen;
        private int torchStyle = 0, platformStyle = 0;

        public TexGen(int w, int h)
        {
            width = w; height = h;
            tileGen = new TileInfo[width, height];
        }

        /// <summary>
        /// Generates the TexGen. (x,y) are the top-left coordinates of the generation.
        /// </summary>
        /// <param name="silent">Play sound on tile placement.</param>
        /// <param name="sync">Sync the placement across clients.</param>
        /// <param name="progress">An optional progress indicator (if provided).</param>
        public void Generate(int x, int y, bool silent, bool sync, GenerationProgress progress = null)
        {
            for (int x1 = 0; x1 < width; x1++)
            {
                progress?.Set(x1 / (float)width);

                for (int y1 = 0; y1 < height; y1++)
                {
                    int x2 = x + x1;
                    int y2 = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    // Skip if no valid tile, wall, liquid, or wire data
                    if (info.tileID == -1 && info.wallID == -1 && info.liquidType == -1 && info.wire == -1)
                        continue;

                    // Generate tile, wall, or wire
                    if (info.tileID != -1 || info.wallID > -1 || info.wire > -1)
                    {
                        WorldGenUtils.GenerateTile(
                            x2, y2, info.tileID, info.wallID,
                            info.tileStyle != 0 ? info.tileStyle :
                                info.tileID == TileID.Torches ? torchStyle :
                                info.tileID == TileID.Platforms ? platformStyle : 0,
                            info.tileID > -1,
                            info.liquidAmt == 0,
                            info.slope,
                            silent: silent,
                            sync: sync
                        );
                    }

                    // Generate liquid if applicable
                    if (info.liquidType != -1)
                    {
                        WorldGenUtils.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmt, sync);
                    }

                    // Place object if applicable
                    if (info.objectID != (0, 0))
                    {
                        //Core.OvermorrowMod.Instance.Logger.Debug("Generating Object " + info.objectID.ToString());

                        int objectId = info.objectID.Item1;
                        int styleRange = info.objectID.Item2;

                        string name = TileLoader.GetTile(objectId).GetLocalizedValue("en");

                        bool status = WorldGen.PlaceObject(x2, y2, objectId, true, Main.rand.Next(0, styleRange));
                        if (!status)
                        {
                            //OvermorrowModFile.Instance.Logger.Debug("Generating Object" + name + " Failed!");
                        }

                        NetMessage.SendObjectPlacement(-1, x2, y2, objectId, Main.rand.Next(0, styleRange), 0, -1, -1);
                    }
                }
            }
        }


        /// <summary> Reads texture data for world generation. Works on dedicated servers. </summary>
        /// <param name="path"> Path to the texture. </param>
        /// <param name="useAsset"> Whether to load the Texture2D data from the asset repository, or directly from IO. Always false on dedicated servers. </param>
        /// <returns></returns>
        public static TexGenData GetTextureForGen(string path, bool useAsset = false)
        {
            if (Main.dedServ)
                useAsset = false;

            if (useAsset)
            {
                var texture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
                return TexGenData.FromTexture2D(texture);
            }
            else
            {
                path = path.Replace(nameof(OvermorrowMod) + "/", "") + ".rawimg";
                return TexGenData.FromStream(OvermorrowModFile.Instance.GetFileStream(path));
            }
        }

        /// <summary> 
        /// Creates a <see cref="TexGen"/> from <see cref="TexGenData"/>s. 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        public static TexGen GetTexGenerator(TexGenData tileData, Dictionary<Color, int> colorToTile, TexGenData wallData = null, Dictionary<Color, int> colorToWall = null, TexGenData liquidData = null, TexGenData slopeData = null, TexGenData objectData = null, Dictionary<Color, (int, int)> colorToObject = null)
        {
            if (colorToLiquid == null)
            {
                colorToLiquid = new Dictionary<Color, int>
                {
                    [new Color(0, 0, 255)] = LiquidID.Water,
                    [new Color(255, 0, 0)] = LiquidID.Lava,
                    [new Color(255, 255, 0)] = LiquidID.Honey,
                    [new Color(255, 0, 255)] = LiquidID.Shimmer
                };

                colorToSlope = new Dictionary<Color, int>
                {
                    [new Color(255, 0, 0)] = 1,   // |\    Red
                    [new Color(0, 255, 0)] = 2,   // /|    Green
                    [new Color(0, 0, 255)] = 3,   // |/    Blue
                    [new Color(255, 255, 0)] = 4, // \|    Yellow
                    [new Color(255, 255, 255)] = -1, // HALFBRICK // White
                    [new Color(0, 0, 0)] = -2 // FULLBLOCK //
                };
            }

            int x = 0, y = 0;
            TexGen gen = new(tileData.Width, tileData.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m];
                Color wallColor = wallData != null ? wallData[m] : Color.Black;
                Color liquidColor = liquidData != null ? liquidData[m] : Color.Black;
                Color slopeColor = slopeData != null ? slopeData[m] : Color.Black;
                Color objectColor = objectData != null ? objectData[m] : Color.Black;
                int tileID = colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1; //if no key assume no action
                int wallID = colorToWall != null && colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1;
                int liquidID = colorToLiquid != null && colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1;
                int slopeID = colorToSlope != null && colorToSlope.ContainsKey(slopeColor) ? colorToSlope[slopeColor] : -1;
                (int, int) objectID = colorToObject != null && colorToObject.ContainsKey(objectColor) ? colorToObject[objectColor] : (0, 0);
                gen.tileGen[x, y] = new TileInfo(tileID, 0, wallID, liquidID, liquidID == -1 ? 0 : 255, slopeID, objectID);
                x++;
                if (x >= tileData.Width) { x = 0; y++; }
                if (y >= tileData.Height) break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }
            return gen;
        }

        /// <summary> 
        /// Creates a <see cref="TexGen"/> directly from <see cref="Texture2D"/>s. 
        /// Old implementation, does not work on dedicated servers, prefer to use <see cref="GetTexGenerator(TexGenData, Dictionary{Color, int}, TexGenData?, Dictionary{Color, int}, TexGenData?, TexGenData?, TexGenData?, Dictionary{Color, int})"> GetTexGenerator(TexGenData...) </see> instead 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        /// <remarks>
        /// NOTE: tileTex CANNOT be null.
        /// </remarks>
        public static TexGen GetTexGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile, Texture2D wallTex = null, Dictionary<Color, int> colorToWall = null, Texture2D liquidTex = null, Texture2D slopeTex = null, Texture2D objectTex = null, Dictionary<Color, (int objectId, int styleRange)> colorToObject = null)
        {
            if (colorToLiquid == null)
            {
                colorToLiquid = new Dictionary<Color, int>
                {
                    [new Color(0, 0, 255)] = LiquidID.Water,
                    [new Color(255, 0, 0)] = LiquidID.Lava,
                    [new Color(255, 255, 0)] = LiquidID.Honey,
                    [new Color(255, 0, 255)] = LiquidID.Shimmer
                };

                colorToSlope = new Dictionary<Color, int>
                {
                    [new Color(255, 0, 0)] = 1,   // |\    Red
                    [new Color(0, 255, 0)] = 2,   // /|    Green
                    [new Color(0, 0, 255)] = 3,   // |/    Blue
                    [new Color(255, 255, 0)] = 4, // \|    Yellow
                    [new Color(255, 255, 255)] = -1, // HALFBRICK // White
                    [new Color(0, 0, 0)] = -2 // FULLBLOCK //
                };
            }
            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);

            Color[] wallData = wallTex != null ? new Color[wallTex.Width * wallTex.Height] : null;
            if (wallData != null)
                wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);

            Color[] liquidData = liquidTex != null ? new Color[liquidTex.Width * liquidTex.Height] : null;
            if (liquidData != null)
                liquidTex.GetData(0, liquidTex.Bounds, liquidData, 0, liquidTex.Width * liquidTex.Height);

            Color[] slopeData = slopeTex != null ? new Color[slopeTex.Width * slopeTex.Height] : null;
            if (slopeData != null)
                slopeTex.GetData(0, slopeTex.Bounds, slopeData, 0, slopeTex.Width * slopeTex.Height);

            Color[] objectData = objectTex != null ? new Color[objectTex.Width * objectTex.Height] : null;
            if (objectData != null)
                objectTex.GetData(0, objectTex.Bounds, objectData, 0, objectTex.Width * objectTex.Height);

            int x = 0, y = 0;
            TexGen gen = new(tileTex.Width, tileTex.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m], wallColor = wallTex == null ? Color.Black : wallData[m], liquidColor = liquidTex == null ? Color.Black : liquidData[m], slopeColor = slopeTex == null ? Color.Black : slopeData[m], objectColor = objectTex == null ? Color.Black : objectData[m];
                int tileID = colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1; //if no key assume no action
                int wallID = colorToWall != null && colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1;
                int liquidID = colorToLiquid != null && colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1;
                int slopeID = colorToSlope != null && colorToSlope.ContainsKey(slopeColor) ? colorToSlope[slopeColor] : -1;
                (int, int) objectID = colorToObject != null && colorToObject.ContainsKey(objectColor) ? colorToObject[objectColor] : (0, 0);
                gen.tileGen[x, y] = new TileInfo(tileID, 0, wallID, liquidID, liquidID == -1 ? 0 : 255, slopeID, objectID);
                x++;
                if (x >= tileTex.Width) { x = 0; y++; }
                if (y >= tileTex.Height) break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }
            return gen;
        }

        public static void PaintAsepriteLayer(
            SheetLayer sheet,
            string modPath, int worldX, int worldY,
            Dictionary<(byte R, byte G, byte B), (int id, int styleRange)> colorMap,
            int srcX = 0, int srcY = 0, int srcW = -1, int srcH = -1)
        {
            string layerName = sheet.ToString();
            try
            {
                Rgba32[] canvas = LoadLayerPixels(modPath, layerName, out int cw, out int ch);
                if (canvas == null) return;

                int w = srcW > 0 ? srcW : cw;
                int h = srcH > 0 ? srcH : ch;
                var unknown = new HashSet<(byte, byte, byte)>();

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        Rgba32 c = canvas[(srcY + y) * cw + (srcX + x)];
                        if (c.A == 0) continue;
                        var key = (c.R, c.G, c.B);
                        if (!colorMap.TryGetValue(key, out var entry))
                        {
                            unknown.Add(key);
                            continue;
                        }
                        int wx = worldX + x;
                        int wy = worldY + y;
                        // id < 0 = clear sentinel.
                        switch (sheet)
                        {
                            case SheetLayer.Walls:
                                if (entry.id < 0) WorldGenUtils.ClearWall(wx, wy);
                                else WorldGenUtils.SetWall(wx, wy, (ushort)entry.id);
                                break;
                            case SheetLayer.Tiles:
                                if (entry.id < 0) WorldGenUtils.ClearTile(wx, wy);
                                else WorldGenUtils.PlaceTile(wx, wy, (ushort)entry.id);
                                break;
                            case SheetLayer.Objects:
                                if (entry.id < 0) break;
                                int style = entry.styleRange > 1 ? Main.rand.Next(0, entry.styleRange) : 0;
                                WorldGen.PlaceObject(wx, wy, entry.id, true, style);
                                break;
                        }
                    }

                LogUnmapped(modPath, layerName, unknown);
            }
            catch (Exception ex)
            {
                OvermorrowModFile.Instance.Logger.Error($"Aseprite paint failed for {modPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the tile and wall at that world position using the spritesheet's Clear layer.
        /// </summary>
        public static void PaintClearLayer(
            string modPath, int worldX, int worldY,
            int srcX = 0, int srcY = 0, int srcW = -1, int srcH = -1)
        {
            try
            {
                Rgba32[] canvas = LoadLayerPixels(modPath, "Clear", out int cw, out int ch, optional: true);
                if (canvas == null) return;

                int w = srcW > 0 ? srcW : cw;
                int h = srcH > 0 ? srcH : ch;

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        Rgba32 c = canvas[(srcY + y) * cw + (srcX + x)];
                        if (c.A == 0) continue;
                        int wx = worldX + x;
                        int wy = worldY + y;
                        WorldGenUtils.ClearTile(wx, wy);
                        WorldGenUtils.ClearWall(wx, wy);
                    }
            }
            catch (Exception ex)
            {
                OvermorrowModFile.Instance.Logger.Error($"Aseprite clear pass failed for {modPath}: {ex.Message}");
            }
        }

        private static Rgba32[] LoadLayerPixels(string modPath, string layerName, out int canvasWidth, out int canvasHeight, bool optional = false)
        {
            canvasWidth = 0;
            canvasHeight = 0;

            string strippedPath = modPath.Replace(nameof(OvermorrowMod) + "/", "");
            byte[] bytes = OvermorrowModFile.Instance.GetFileBytes(strippedPath);
            using var ms = new MemoryStream(bytes);
            AsepriteFile file = AsepriteFileLoader.FromStream(strippedPath, ms);

            if (!file.TryGetLayer(layerName, out AsepriteLayer? _))
            {
                if (!optional)
                    OvermorrowModFile.Instance.Logger.Warn($"Aseprite layer '{layerName}' not found in {modPath}");
                return null;
            }

            canvasWidth = file.CanvasWidth;
            canvasHeight = file.CanvasHeight;

            Rgba32[] canvas = new Rgba32[canvasWidth * canvasHeight];
            AsepriteFrame frame = file.Frames[0];
            foreach (var cel in frame.Cels)
            {
                if (cel.Layer.Name != layerName) continue;
                if (cel is not AsepriteImageCel imageCel) continue;

                int celW = imageCel.Size.Width;
                int celH = imageCel.Size.Height;
                int celX = imageCel.Location.X;
                int celY = imageCel.Location.Y;
                ReadOnlySpan<Rgba32> celPixels = imageCel.Pixels;
                for (int cy = 0; cy < celH; cy++)
                    for (int cx = 0; cx < celW; cx++)
                    {
                        int canvasX = celX + cx;
                        int canvasY = celY + cy;
                        if (canvasX < 0 || canvasX >= canvasWidth) continue;
                        if (canvasY < 0 || canvasY >= canvasHeight) continue;
                        canvas[canvasY * canvasWidth + canvasX] = celPixels[cy * celW + cx];
                    }
            }
            return canvas;
        }

        private static void LogUnmapped(string modPath, string layerName, HashSet<(byte, byte, byte)> unknown)
        {
            if (unknown.Count == 0) return;
            string list = string.Join(", ", unknown.Select(t => $"({t.Item1},{t.Item2},{t.Item3})"));
            OvermorrowModFile.Instance.Logger.Info($"Aseprite '{modPath}' layer '{layerName}' unmapped colors: {list}");
        }
    }
}