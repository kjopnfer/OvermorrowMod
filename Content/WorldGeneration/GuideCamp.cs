using OvermorrowMod.Common;
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
            var logger = OvermorrowModFile.Instance.Logger;

            progress.Message = "Setting Up Camp";

            int startX = Main.spawnTileX;
            int startY = Main.spawnTileY;

            #region Campfire
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
            #endregion
        }
    }
}