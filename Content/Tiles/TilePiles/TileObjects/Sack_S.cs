using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Sack_S : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Backpack";
            Width = 18;
            Height = 20;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.DirtBlock;
            Durability = 175;
            MinStack = 3;
            MaxStack = 9;
        }
    }
}