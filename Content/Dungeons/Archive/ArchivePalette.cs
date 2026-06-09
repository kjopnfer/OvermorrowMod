using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.Tiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive
{
    public class ArchivePalette : DungeonPalette
    {
        public ArchivePalette() : base(BuildWalls(), BuildTiles(), BuildObjects()) { }

        private static Dictionary<(byte R, byte G, byte B), TexPlaceFunction> BuildWalls() => new()
        {
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(66, 64, 61)] = TexPlaceAction.PlaceWall(ModContent.WallType<CastleWall>()),
            [(54, 36, 11)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWallFrame>()),
            [(118, 66, 138)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveBookWall>()),
            [(86, 0, 251)] = TexPlaceAction.PlaceWall(ModContent.WallType<InvisibleWall>()),
            [(70, 67, 117)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlue>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte R, byte G, byte B), TexPlaceFunction> BuildTiles() => new()
        {
            [(105, 106, 106)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastleBrick>()),
            [(89, 86, 82)] = TexPlaceAction.PlaceTile(ModContent.TileType<DarkCastleBrick>()),
            [(138, 111, 48)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastlePlatform>()),
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte R, byte G, byte B), TexPlaceFunction> BuildObjects() => new()
        {
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
            [(74, 15, 56)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar>()),
            [(148, 109, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandleholder>()),
            [(135, 28, 66)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArch>()),
            [(199, 158, 59)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchiveBanner>()),
            [(69, 40, 60)] = TexPlaceAction.PlaceObject(ModContent.TileType<BanquetTable>()),
            [(159, 131, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandelabra>()),
            [(75, 105, 47)] = TexPlaceAction.PlaceObject(ModContent.TileType<BookPileTable>()),
            [(159, 183, 204)] = TexPlaceAction.PlaceObject(ModContent.TileType<Bismarck>()),
            [(99, 49, 110)] = TexPlaceAction.PlaceObject(ModContent.TileType<FireplacePillar>()),
            [(180, 58, 0)] = TexPlaceAction.PlaceObject(ModContent.TileType<Fireplace>()),
            [(208, 61, 125)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>()),
            [(237, 86, 227)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>(), direction: 1),
            [(153, 229, 80)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxChandelier>()),
            [(237, 157, 102)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxSconceEven>()),
            [(237, 152, 93)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxSconce>()),
            [(0, 255, 255)] = TexPlaceAction.PlaceObject(ModContent.TileType<TallWindow>()),
            [(251, 242, 54)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchivePotSmall>()),
            [(215, 186, 87)] = TexPlaceAction.PlaceObject(ModContent.TileType<FatVase>()),
            [(171, 107, 152)] = TexPlaceAction.PlaceObject(ModContent.TileType<HallwayPillar>()),
            [(114, 70, 123)] = TexPlaceAction.PlaceObject(ModContent.TileType<Moose>()),
            [(246, 178, 185)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchiveBridge>()),
            [(198, 183, 242)] = TexPlaceAction.PlaceObject(ModContent.TileType<NormalWizardStatue>()),

            [(19, 215, 73)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width4;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),
            [(101, 224, 135)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width8;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),

            [(91, 110, 225)] = TexPlaceAction.CustomPlaceObject((x, y) => PlaceBookshelfArch(x, y)),
        };

        /// <summary>
        /// 14-tile-wide wooden arch with a 7-tile gap in the middle for shelf objects underneath.
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
    }
}
