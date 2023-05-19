using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Apple : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Apple";
            Width = 10;
            Height = 16;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Apple;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}