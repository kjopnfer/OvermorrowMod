using OvermorrowMod.Content.Tiles.WaterCave;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class TEShenanigans : ModTileEntity
    {
        public override void Update()
        {
            int i = Position.X;
            int j = Position.Y;
            WorldGen.Convert(i, j, 0, 0);

            Tile tile = Framing.GetTileSafely(i, j - 1);
            tile.LiquidType = LiquidID.Water;
            tile.LiquidAmount = byte.MaxValue;
            WorldGen.SquareTileFrame(i, j - 1, true);
        }

        public override bool IsTileValidForEntity(int i, int j)
        {
            // valid tile does not get called for some reason
            Tile tile = Main.tile[i, j];
            return tile.HasTile && tile.TileType == ModContent.TileType<GlowBlock>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
         
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