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
            Main.spawnTileX = 345;
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
                [new Color(89, 86, 82)] = ModContent.TileType<DarkCastleBrick>(),
                [new Color(138, 111, 48)] = ModContent.TileType<CastlePlatform>(),
                [new Color(143, 86, 59)] = TileID.WoodBlock,
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> wallMapping = new()
            {
                [Color.Black] = -1,
                [new Color(113, 193, 107)] = ModContent.WallType<ArchiveBackground>(),
                [new Color(66, 64, 61)] = ModContent.WallType<CastleWall>(),
                [new Color(97, 66, 19)] = ModContent.WallType<ArchiveBookWallFrame>(),
                [new Color(118, 66, 138)] = ModContent.WallType<ArchiveBookWall>(),
                [new Color(66, 57, 46)] = ModContent.WallType<CastleWall>(),
                [new Color(101, 66, 14)] = WallID.BorealWood,
            };

            Dictionary<Color, (int objectId, int styleRange)> objectMapping = new()
            {
                [new Color(75, 105, 47)] = (ModContent.TileType<BookPile>(), 1),
                [new Color(69, 40, 60)] = (ModContent.TileType<BanquetTable>(), 1),
                [new Color(88, 27, 69)] = (ModContent.TileType<CastleChair>(), 1),
                [new Color(208, 61, 125)] = (ModContent.TileType<CozyChair>(), 1),
                [new Color(180, 58, 0)] = (ModContent.TileType<Fireplace>(), 1),
                [new Color(99, 49, 110)] = (ModContent.TileType<FireplacePillar>(), 1),
                [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 3),
                [new Color(114, 70, 123)] = (ModContent.TileType<Moose>(), 1),
                [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
                [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
                [new Color(198, 74, 118)] = (ModContent.TileType<WoodenArch>(), 1),
                [new Color(135, 28, 66)] = (ModContent.TileType<WoodenArchL1>(), 1),
                [new Color(176, 16, 73)] = (ModContent.TileType<WoodenArchL2>(), 1),
                [new Color(189, 44, 95)] = (ModContent.TileType<WoodenArchL3>(), 1),
                [new Color(88, 68, 75)] = (ModContent.TileType<WoodenArchR1>(), 1),
                [new Color(79, 38, 52)] = (ModContent.TileType<WoodenArchR2>(), 1),
                [new Color(88, 13, 39)] = (ModContent.TileType<WoodenArchR3>(), 1),
                [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),

            };

            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = TexGen.GetTexGenerator(tiles, tileMapping, walls, wallMapping, null, null, objects, objectMapping);
                gen.Generate(0, 0, true, true);
            });

            // Run TexGen a second time for just the objects.
            // Object anchors do not behave properly for whatever reason if done within the first pass.
            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen objectGen = TexGen.GetTexGenerator(tiles, tileMapping, null, null, null, null, objects, objectMapping);
                objectGen.Generate(0, 0, true, true);
            });

            // ...yet I have to do it manually for these fucking things anyways because they don't work??
            WorldGen.PlaceObject(307, 111, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(363, 111, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(404, 111, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(337, 131, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(337, 127, ModContent.TileType<Moose>());
            WorldGen.PlaceObject(337, 113, ModContent.TileType<WoodenArchSmall>());
        }
    }
}