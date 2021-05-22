using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;

namespace OvermorrowMod.Tiles
{
    public class ManaStone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            //dustType = DustType<Sparkle>();
            drop = ModContent.ItemType<CrystalMana>();
            AddMapEntry(new Color(51, 204, 255));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.25f;
            b = 0.5f;
        }
    }
}