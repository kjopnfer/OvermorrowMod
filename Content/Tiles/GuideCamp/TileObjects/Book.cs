using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.GuideCamp.TileObjects
{
    public abstract class Book : TileObject
    {
        public override void SetDefaults()
        {
            Name = "Book";
            Width = 18;
            Height = 12;
            HitSound = SoundID.Dig;
            DeathSound = SoundID.Dig;
            GrabSound = SoundID.Dig;
            Durability = 160;
            MinStack = 1;
            MaxStack = 1;
        }
    }

    public class Book1 : Book { }
    public class Book2 : Book { }
    public class Book3 : Book { }
    public class Book4 : Book { }
}