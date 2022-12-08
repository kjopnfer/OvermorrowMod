using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Stump : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Stump";
            Width = 26;
            Height = 12;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Wood;
            Durability = 140;
            MinStack = 1;
            MaxStack = 1;
            CanWiggle = false;
        }
    }
}