using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Journal : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Journal";
            Width = 20;
            Height = 8;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            Durability = 160;
            MinStack = 20;
            MaxStack = 30;
        }
    }
}