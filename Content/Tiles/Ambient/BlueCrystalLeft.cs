using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Ambient
{
    public class BlueCrystalLeft : ModTile
    {
        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Blue Crystal");
            Main.tileSolid[Type] = false;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            HitSound = SoundID.Tink;
            MinPick = 65;
            AddMapEntry(new Color(102, 255, 255), name);
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