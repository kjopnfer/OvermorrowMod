using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Crate_S : TileObject
    {
        public override void SetDefaults()
        {
            name = "Crate";
            width = 28;
            height = 18;
            itemID = ItemID.WoodenCrate;
            minStack = 1;
            maxStack = 1;
        }
    }
}