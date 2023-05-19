using OvermorrowMod.Common.TilePiles;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Axe : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Axe";
            Width = 26;
            Height = 18;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.CopperAxe;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}