using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Cloth_L : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Silk";
            Width = 30;
            Height = 12;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Silk;
            Durability = 175;
            MinStack = 3;
            MaxStack = 8;
        }
    }
}