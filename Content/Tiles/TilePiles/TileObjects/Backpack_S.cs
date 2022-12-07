using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Backpack_S : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Backpack";
            Width = 20;
            Height = 16;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.StoneBlock;
            Durability = 175;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}