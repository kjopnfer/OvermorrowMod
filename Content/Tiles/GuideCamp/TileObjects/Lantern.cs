using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Lantern : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Lantern";
            Width = 10;
            Height = 16;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = ModContent.ItemType<Items.Accessories.GuideLantern.GuideLantern>();
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}