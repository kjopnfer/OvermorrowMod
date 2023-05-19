using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Quiver : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Quiver";
            Width = 18;
            Height = 20;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.WoodenArrow;
            Durability = 160;
            MinStack = 20;
            MaxStack = 30;
        }
    }
}