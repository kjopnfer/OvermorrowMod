using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Placeable.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Tent : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Tent";
            Width = 82;
            Height = 46;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = ModContent.ItemType<BlueTent>();
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}