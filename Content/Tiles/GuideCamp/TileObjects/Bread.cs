using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Bread : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Bread";
            Width = 18;
            Height = 10;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = ModContent.ItemType<StaleBread>();
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}