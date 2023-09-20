using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Bow : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Bow";
            Width = 22;
            Height = 16;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.WoodenBow;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}