using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Ambient
{
    public class BlueCrystalUp : ModTile
    {
        public override void SetStaticDefaults()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Blue Crystal");
            Main.tileSolid[Type] = false;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            SoundType = SoundID.Tink;
            SoundStyle = 1;
            MinPick = 65;
            AddMapEntry(new Color(102, 255, 255), name);
            ItemDrop = ModContent.ItemType<Content.Items.Misc.BlueCrystal>();
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 1f;
            b = 1f;
        }
    }
}