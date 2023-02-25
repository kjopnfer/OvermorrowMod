using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Ambient;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.WaterCave
{
    public class CaveMud : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            //ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.CaveMud>();
            MineResist = 2f;
            AddMapEntry(new Color(79, 86, 97));
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            // Grow vines
            if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<GlowWorms>();
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            // Grow plants
            if (WorldGen.genRand.NextBool(2) && !tileAbove.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<GlowPlants>();
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(7) * 18); // 7 is the amount of plants in the sprite
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
        }
    }
}