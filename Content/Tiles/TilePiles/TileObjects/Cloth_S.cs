using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.TilePiles.TileObjects
{
    public class Cloth_S : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Silk";
            Width = 26;
            Height = 10;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Silk;
            Durability = 175;
            MinStack = 2;
            MaxStack = 5;
        }
    }
}