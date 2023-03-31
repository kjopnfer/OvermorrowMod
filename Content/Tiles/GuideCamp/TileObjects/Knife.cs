using OvermorrowMod.Common.TilePiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Knife : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Knife";
            Width = 10;
            Height = 20;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = ModContent.ItemType<Items.Weapons.Melee.Knife>();
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}