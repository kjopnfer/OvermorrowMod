using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Tiles.Ambient
{
    public class BlueCrystalUp : ModTile
    {
        public override void SetDefaults()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Blue Crystal");
            Main.tileSolid[Type] = false;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            soundType = 21;
            soundStyle = 1;
            minPick = 65;
            AddMapEntry(new Color(102, 255, 255), name);
			drop = ModContent.ItemType<Items.Misc.BlueCrystal>();
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