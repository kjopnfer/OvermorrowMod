using Microsoft.Xna.Framework;
using OvermorrowMod.Tiles.Ambient;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles
{
    public class CaveMud : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            drop = ModContent.ItemType<OvermorrowMod.Items.Placeable.Tiles.CaveMud>();
            mineResist = 2f;
            AddMapEntry(new Color(79, 86, 97));
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            // Grow vines
            if (WorldGen.genRand.NextBool(2) && !tileBelow.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope())
                {
                    tileBelow.type = (ushort)ModContent.TileType<GlowWorms>();
                    tileBelow.active(true);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            // Grow plants
            if (WorldGen.genRand.NextBool(2) && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick())
                {
                    tileAbove.type = (ushort)ModContent.TileType<GlowPlants>();
                    tileAbove.active(true);
                    tileAbove.frameY = 0;
                    tileAbove.frameX = (short)(WorldGen.genRand.Next(7) * 18); // 7 is the amount of plants in the sprite
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