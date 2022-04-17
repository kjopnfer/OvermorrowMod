using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.TrapOre
{
    public class FakeiteGold : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = false;
            Main.tileSpelunker[Type] = false;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;



            ModTranslation name = CreateMapEntryName();
            name.SetDefault("\"Gold\"");
            ItemDrop = ModContent.ItemType<FakeGem>();
            AddMapEntry(new Color(100, 0, 0), name);
            DustType = DustID.Copper;
            SoundType = SoundID.Tink;
            SoundStyle = 1;
            MineResist = 2f;
            MinPick = 1;
        }
        public override bool Drop(int i, int j)
        {
            Tile t = Main.tile[i, j];
            int style = 0;
            if (style == 0) // It can be useful to share a single tile with multiple styles. This code will let you drop the appropriate bar if you had multiple.
            {
                Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16, j * 16, 0, 0, Mod.Find<ModProjectile>("FakeGold").Type, 0, 0f, Main.myPlayer);
            }
            return base.Drop(i, j);
        }
    }
}