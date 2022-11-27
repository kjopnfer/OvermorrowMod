using Terraria.ID;

namespace OvermorrowMod.Common.TilePiles
{
    public class Crate_M : TileObject
    {
        public override void SetDefaults()
        {
            name = "Crate";
            width = 32;
            height = 22;
            itemID = ItemID.WoodenCrate;
            minStack = 1;
            maxStack = 1;
        }
    }
}