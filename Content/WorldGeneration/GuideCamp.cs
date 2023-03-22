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
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Guide"));
            if (GuideIndex != -1)
            {
                tasks.Insert(GuideIndex + 1, new PassLegacy("Spawn Camp", GenerateCamp));
            }
        }

        private int GetRandomTent()
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    return ModContent.TileType<GreenTent>();
                case 2:
                    return ModContent.TileType<BlueTent>();
                case 3:
                    return ModContent.TileType<BlueTent>();
            }

            return 0;
        }

        private void GenerateCamp(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Setting up camp";

            int startX = Main.spawnTileX;
            int startY = Main.spawnTileY;

            bool validArea = false;

            int x = startX;
            int y = startY - 15;

            while (!validArea)
            {
                // Loop downwards until we reach a solid tile
                Tile tile = Framing.GetTileSafely(x, y);
                while (!tile.HasTile)
                {
                    y++;
                    tile = Framing.GetTileSafely(x, y);
                }

                Tile aboveTile = Framing.GetTileSafely(x, y - 1);

                // We have the tile but we want to check if its a grass block, if it isn't restart the process
                if ((tile.TileType == TileID.Grass || tile.TileType == TileID.Dirt) && tile.WallType == WallID.None && !aboveTile.HasTile && Main.tileSolid[tile.TileType])
                {
                    validArea = true;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                PlaceCamp(x + 3, y + 8);
            }

            /*#region Campfire
            int x = startX;
            int y = startY - 15;
            Tile tile = Framing.GetTileSafely(x, y);

            bool validArea = false;
            while (!validArea)
            {
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    validArea = true;

                    //WorldGen.PlaceTile(x + 1, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x - 1, y, TileID.Adamantite, true, true);

                    WorldGen.KillTile(x - 1, y - 1);
                    WorldGen.KillTile(x, y - 1);
                    WorldGen.KillTile(x + 1, y - 1);

                    ModUtils.PlaceObject(x, y - 1, ModContent.TileType<GuideCampfire>());
                    ModContent.GetInstance<GuideCampfire_TE>().Place(x - 1, y - 2);
                }
                else
                {
                    y += 1;
                    tile = Framing.GetTileSafely(x, y);
                }
            }
            #endregion

            #region Right Tent
            x = startX + 6;
            y = startY - 15;
            tile = Framing.GetTileSafely(x, y);

            validArea = false;
            while (!validArea)
            {
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    validArea = true;

                    //WorldGen.PlaceTile(x - 2, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x - 1, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 1, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 2, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 3, y, TileID.Adamantite, true, true);

                    WorldGen.KillTile(x - 2, y - 1);
                    WorldGen.KillTile(x - 1, y - 1);
                    WorldGen.KillTile(x, y - 1);
                    WorldGen.KillTile(x + 1, y - 1);
                    WorldGen.KillTile(x + 2, y - 1);
                    WorldGen.KillTile(x + 3, y - 1);

                    ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>(x, y - 1);
                }
                else
                {
                    y += 1;
                    tile = Framing.GetTileSafely(x, y);
                }
            }
            #endregion

            #region Left Tent
            x = startX - 6;
            y = startY - 15;
            tile = Framing.GetTileSafely(x, y);

            validArea = false;
            while (!validArea)
            {
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    validArea = true;

                    //WorldGen.PlaceTile(x - 2, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x - 1, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 1, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 2, y, TileID.Adamantite, true, true);
                    //WorldGen.PlaceTile(x + 3, y, TileID.Adamantite, true, true);

                    WorldGen.KillTile(x - 2, y - 1);
                    WorldGen.KillTile(x - 1, y - 1);
                    WorldGen.KillTile(x, y - 1);
                    WorldGen.KillTile(x + 1, y - 1);
                    WorldGen.KillTile(x + 2, y - 1);
                    WorldGen.KillTile(x + 3, y - 1);

                    ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>(x, y - 1);
                }
                else
                {
                    y += 1;
                    tile = Framing.GetTileSafely(x, y);
                }
            }
            #endregion*/
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

            WorldGen.PlaceTile(x - (TileGen.width / 2), y - TileGen.height, TileID.Adamantite, false, true);

            Vector2 origin = new Vector2(x - (TileGen.width / 2), y - TileGen.height);

            ModUtils.PlaceObject((int)(origin.X + 10), (int)(origin.Y + 5), ModContent.TileType<GuideCampfire>());
            ModContent.GetInstance<GuideCampfire_TE>().Place(x + 9, y + 4);

            ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>((int)(origin.X + 6), (int)(origin.Y + 5));
            ModUtils.PlaceTilePile<BowRock, BowRockObjects>((int)(origin.X + 2), (int)(origin.Y + 4));
        }
    }
}