using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Log : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Log";
            Width = 46;
            Height = 12;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            ItemID = Terraria.ID.ItemID.Wood;
            Durability = 160;
            MinStack = 4;
            MaxStack = 8;
            CanWiggle = false;
            CanHighlight = false;
        }
    }
}