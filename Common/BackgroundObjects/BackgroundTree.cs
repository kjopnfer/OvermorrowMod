using Terraria;

namespace OvermorrowMod.Common.BackgroundObjects
{
    public class BackgroundTree : BaseBackgroundObject
    {
        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(420, 420);
    }
}