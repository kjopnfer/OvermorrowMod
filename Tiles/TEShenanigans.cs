using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles
{
    public class TEShenanigans : ModTileEntity
    {
        public override void Update()
        {
            int i = Position.X;
            int j = Position.Y;
            WorldGen.Convert(i, j, 0, 0);

            Tile tile = Framing.GetTileSafely(i, j - 1);
            tile.liquidType(Tile.Liquid_Water);
            tile.liquid = byte.MaxValue;
            WorldGen.SquareTileFrame(i, j - 1, true);
        }

        public override bool ValidTile(int i, int j)
        {
            // valid tile does not get called for some reason
            Main.NewText("valid tile");
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<GlowBlock>() && tile.frameX == 0 && tile.frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            // this only gets called when this is placed directly on top of a solid block for some reason
            Main.NewText("i " + i + " j " + j + " t " + type + " s " + style + " d " + direction);

            // i - 1 and j - 2 come from the fact that the origin of the tile is "new Point16(1, 2);", so we need to pass the coordinates back to the top left tile. If using a vanilla TileObjectData.Style, make sure you know the origin value.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1); // this is -1, -1, however, because -1, -1 places the 3 diameter square over all the tiles, which are sent to other clients as an update.
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
    }
}