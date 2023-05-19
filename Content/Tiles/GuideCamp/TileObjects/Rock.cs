using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Rock : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Rock";
            Width = 48;
            Height = 12;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.StoneBlock;
            Durability = 160;
            MinStack = 3;
            MaxStack = 6;
            CanWiggle = false;
            CanHighlight = false;
        }
    }
}