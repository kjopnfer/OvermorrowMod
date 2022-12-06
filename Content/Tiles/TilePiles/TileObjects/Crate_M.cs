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
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.WoodenCrate;
            Durability = 175;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}