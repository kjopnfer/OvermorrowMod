using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace OvermorrowMod
{
    public class Desert : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
            if (DesertIndex != -1)
            {
                tasks.Insert(DesertIndex + 1, new PassLegacy("Desert Temple", GenerateTemple));
            }
        }

        private void GenerateTemple(GenerationProgress progress)
        {
            progress.Message = "Generate Desert Temple";


            int x = WorldGen.UndergroundDesertLocation.X + (WorldGen.UndergroundDesertLocation.Width / 2);
            int y = WorldGen.UndergroundDesertLocation.Y + (WorldGen.UndergroundDesertLocation.Height / 2);

            Place(x, y);
        }

        public static void Place(int x, int y)
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

            Texture2D TexMap = ModContent.GetTexture("OvermorrowMod/WorldGeneration/TempleTexture");

            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TexMap, TileMapping, TexMap, WallMapping);
            TileGen.Generate(x - (TileGen.width / 2), y - (TileGen.height / 2), true, true);

            //TexGen WallGen = BaseWorldGenTex.GetTexGenerator(TexMap, WallMapping);
            //WallGen.Generate(x - (TileGen.width / 2), y - (TileGen.height / 2), true, true);
        }
    }
}