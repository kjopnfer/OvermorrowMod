using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public class Cane : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Cane";
            Width = 20;
            Height = 46;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }
}