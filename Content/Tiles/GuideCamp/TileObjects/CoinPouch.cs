using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class CoinPouch : TileObject
    {
        public override void SetDefaults()
        {
            Name = "CoinPouch";
            Width = 10;
            Height = 10;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.SilverCoin;
            Durability = 160;
            MinStack = 40;
            MaxStack = 60;
        }
    }
}