using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles.DesertTemple;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class Desert : ModSystem
    {
        public static Vector2 DesertArenaCenter;
        public override void SaveWorldData(TagCompound tag)
        {
            tag["DesertArenaCenter"] = DesertArenaCenter;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            DesertArenaCenter = tag.Get<Vector2>("DesertArenaCenter");
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (DesertIndex != -1)
            {
                tasks.Insert(DesertIndex + 1, new PassLegacy("Desert Temple", GenerateTemple));
            }
        }

        private void GenerateTemple(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Generate Desert Temple";


            int x = WorldGen.UndergroundDesertLocation.X + (WorldGen.UndergroundDesertLocation.Width / 2);
            int y = WorldGen.UndergroundDesertLocation.Y + (WorldGen.UndergroundDesertLocation.Height / 2);

            //Place_LowerTemple(x, y);


            // Position the temple's upper area to the left side of the desert, start the structure 450 pixels up into the air before dropping
            //x = WorldGen.UndergroundDesertLocation.X + (WorldGen.UndergroundDesertLocation.Width / 6);
            //y = WorldGen.UndergroundDesertLocation.Y - 200;
            x = WorldGen.UndergroundDesertLocation.X + (WorldGen.UndergroundDesertLocation.Width / 2);
            y = (int)(Main.worldSurface * 0.35f) + 50;
            //y = WorldGen.UndergroundDesertLocation.Y + (WorldGen.UndergroundDesertLocation.Height / 2);

            // Check if the ground is solid before creating the temple
            Tile tile = Framing.GetTileSafely(x, y);
            while (!tile.HasTile && tile.WallType != WallID.Sandstone)
            {
                y++;

                tile = Framing.GetTileSafely(x, y);
            }


            // Offset the spawn to swallow less of the chasm
            Place_UpperTemple(x, y - 10);
        }

        public static void Place_UpperTemple(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(143, 86, 59)] = ModContent.TileType<SandBrick>(),
                [new Color(238, 195, 154)] = TileID.Sand,
                [new Color(123, 85, 49)] = TileID.HardenedSand,
                [new Color(224, 220, 128)] = TileID.Gold,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(102, 57, 49)] = WallID.SandstoneBrick,
                [new Color(69, 40, 60)] = WallID.DemoniteBrick
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            #region Temple Generation
            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/SurfaceTemple_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height / 2), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/SurfaceTemple").Value;
            Texture2D WallMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/SurfaceTemple_Walls").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, WallMap, WallMapping);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height / 2), true, true);
            #endregion

            #region Object Placement
            DesertArenaCenter = new Vector2(x, y) * 16;
            //WorldGen.PlaceTile(x, y, ModContent.TileType<DharuudArena>());
            //DesertArenaCenter = new Vector2(x - (TileGen.width / 2) + 68, y - (TileGen.width / 2) + 31) * 16;
            //StructureHelper.Generator.GenerateStructure("Content/WorldGeneration/Structures/test2", new Point16(x - (TileGen.width / 2) + 68, y - (TileGen.width / 2) + 31), OvermorrowModFile.Instance);

            #endregion
        }

        public static void Place_LowerTemple(int x, int y)
        {
            // black is removed, white is ignored

            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(140, 131, 103)] = TileID.SandstoneBrick,
                [new Color(113, 100, 62)] = TileID.WoodBlock,
                [new Color(128, 37, 98)] = TileID.HellstoneBrick,
                [new Color(209, 178, 86)] = TileID.Gold,
                [new Color(150, 80, 53)] = TileID.Spikes,
                [new Color(101, 252, 241)] = TileID.LivingDemonFire,
                [new Color(34, 127, 77)] = TileID.LeafBlock,
                [new Color(212, 192, 106)] = TileID.PlatinumBrick,
                [new Color(93, 87, 68)] = -2,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(93, 87, 68)] = WallID.SandstoneBrick,
            };

            Texture2D TexMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/TempleTexture").Value;

            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TexMap, TileMapping, TexMap, WallMapping);
            TileGen.Generate(x - (TileGen.width / 2), y - (TileGen.height / 2), true, true);

            //TexGen WallGen = BaseWorldGenTex.GetTexGenerator(TexMap, WallMapping);
            //WallGen.Generate(x - (TileGen.width / 2), y - (TileGen.height / 2), true, true);
        }
    }
}