using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.TilePiles
{
    public class Crate_S : TileObject
    {
        public override void SetDefaults()
        {
            width = 28;
            height = 18;
            itemID = ItemID.WoodenCrate;
            minStack = 1;
            maxStack = 1;
        }
    }
}