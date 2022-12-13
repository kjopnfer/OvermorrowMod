using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class BookStack_S2 : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Books";
            Width = 20;
            Height = 10;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Book;
            Durability = 175;
            MinStack = 2;
            MaxStack = 4;
        }
    }
}