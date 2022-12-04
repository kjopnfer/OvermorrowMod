using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Crate_M : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Crate";
            Width = 32;
            Height = 22;
            ItemID = Terraria.ID.ItemID.WoodenCrate;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}