using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles.GuideCamp;
using OvermorrowMod.Content.Tiles.TilePiles;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class GuideCamp : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));
            if (GuideIndex != -1)
            {
                tasks.Insert(GuideIndex + 1, new PassLegacy("Spawn Camp", GenerateCamp));
            }
        }

        private void GenerateCamp(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Setting up camp";

            //int startX = Main.spawnTileX;
            //int startY = Main.spawnTileY;
            int startX = Main.maxTilesX / 2;
            int startY = 0;

            bool validArea = false;

            int x = startX;
            int y = startY/* - 15*/;

            while (!validArea)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                while (!tile.HasTile)
                {
                    y++;
                    tile = Framing.GetTileSafely(x, y);
                }

                Tile aboveTile = Framing.GetTileSafely(x, y - 1);

                // We have the tile but we want to check if its a grass block, if it isn't restart the process
                if (/*!aboveTile.HasTile*/Main.tileSolid[tile.TileType])
                {
                    validArea = true;
                }
            }

            WorldGen.PlaceTile(x, y, TileID.Adamantite, false, true);

            for (int i = 0; i < 2; i++)
            {
                PlaceCamp(x + 3, y + 8);
            }
        }

        public static void PlaceCamp(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(42, 100, 46)] = TileID.Grass,
                [new Color(71, 38, 28)] = TileID.Dirt,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            //WorldGen.PlaceTile(x - (TileGen.width / 2), y - TileGen.height, TileID.Adamantite, false, true);

            Vector2 origin = new Vector2(x - (TileGen.width / 2), y - TileGen.height);

            ModUtils.PlaceObject((int)(origin.X + 20), (int)(origin.Y + 5), ModContent.TileType<GuideCampfire>());
            ModContent.GetInstance<GuideCampfire_TE>().Place((int)(origin.X + 19), (int)(origin.Y + 4));

            ModUtils.PlaceTilePile<BowRock, BowRockObjects>((int)(origin.X + 12), (int)(origin.Y + 4));
            //WorldGen.PlaceTile((int)(origin.X + 12), (int)(origin.Y + 4), TileID.Adamantite, false, true);
            ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>((int)(origin.X + 16), (int)(origin.Y + 5));
            //WorldGen.PlaceTile((int)(origin.X + 26), (int)(origin.Y + 5), TileID.Adamantite, false, true);

            ModUtils.PlaceTilePile<GuideLog, GuideLogObjects>((int)(origin.X + 26), (int)(origin.Y + 4));
            ModUtils.PlaceTilePile<GuideTent, GuideTentObjects>((int)(origin.X + 34), (int)(origin.Y + 3));
            ModUtils.PlaceTilePile<BookRock, BookRockObjects>((int)(origin.X + 39), (int)(origin.Y + 3));
            ModUtils.PlaceTilePile<AxeStump, AxeStumpObjects>((int)(origin.X + 42), (int)(origin.Y + 3));
        }
    }
}