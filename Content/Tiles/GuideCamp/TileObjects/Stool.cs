using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Stool : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Stool";
            Width = 28;
            Height = 14;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.PortableStool;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
            CanWiggle = false;
        }
    }
}