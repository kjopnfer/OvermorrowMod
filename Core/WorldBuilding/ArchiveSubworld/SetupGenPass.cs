using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldBuilding.ArchiveSubworld
{
    public class SetupGenPass : GenPass
    {
        public SetupGenPass(string name, double loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating tiles";
            Main.spawnTileX = 387;
            Main.spawnTileY = 136;

            // Move the backgrounds somewhere else
            Main.worldSurface = ArchiveSubworld.GetHeight();
            Main.rockLayer = ArchiveSubworld.GetHeight();

            Texture2D tiles = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value;
            Texture2D walls = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D slopes = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D objects = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveObjects", AssetRequestMode.ImmediateLoad).Value;

            Dictionary<Color, int> tileMapping = new()
            {
                [new Color(105, 106, 106)] = ModContent.TileType<CastleBrick>(),
                [new Color(143, 86, 59)] = TileID.WoodBlock,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> wallMapping = new()
            {
                [Color.Black] = -1,
                //[new Color(113, 193, 107)] = ModContent.WallType<ArchiveBackground>(),
                [new Color(66, 64, 61)] = ModContent.WallType<CastleWall>(),
                [new Color(97, 66, 19)] = ModContent.WallType<ArchiveBookWallFrame>(),
                [new Color(118, 66, 138)] = ModContent.WallType<ArchiveBookWall>()
            };

            Dictionary<Color, (int objectId, int styleRange)> objectMapping = new()
            {
                [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 3),
                [new Color(69, 40, 60)] = (ModContent.TileType<BanquetTable>(), 1),
                [new Color(88, 27, 69)] = (ModContent.TileType<CastleChair>(), 1),
            };


            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = TexGen.GetTexGenerator(tiles, tileMapping, walls, wallMapping, null, null, objects, objectMapping);
                gen.Generate(0, 0, true, true);
            });
        }
    }
}