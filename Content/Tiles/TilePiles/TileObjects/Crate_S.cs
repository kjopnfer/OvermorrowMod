using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Crate_S : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Crate";
            Width = 28;
            Height = 18;
            HitSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.WoodenCrate;
            Durability = 125;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}