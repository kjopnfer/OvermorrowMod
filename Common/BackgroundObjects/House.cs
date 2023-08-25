using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Common.BackgroundObjects
{
    public class House : BaseBackgroundObject
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(420, 420);
    }
}