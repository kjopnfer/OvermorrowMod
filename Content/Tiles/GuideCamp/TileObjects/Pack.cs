using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Consumable;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Pack : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Pack";
            Width = 14;
            Height = 22;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = ModContent.ItemType<GuideBag>();
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}