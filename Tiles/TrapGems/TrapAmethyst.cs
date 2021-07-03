using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles.TrapGems
{
    public class TrapAmethyst : ModTile
    {
        public override void SetDefaults()
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
            name.SetDefault("Fake Diamond");
            AddMapEntry(new Color(200, 30, 0), name);
            //dustType = 9;
            soundStyle = 1;
            mineResist = 2f;
            minPick = 1;
        }
		public override bool Drop(int i, int j)
		{
			Tile t = Main.tile[i, j];
			int style = 0;
			if (style == 0) // It can be useful to share a single tile with multiple styles. This code will let you drop the appropriate bar if you had multiple.
			{
				Item.NewItem(i * 16, j * 16, 16, 16, 3);
                NPC.NewNPC(i * 16, j * 16, mod.NPCType("AngryStone"));
		    }
		    return base.Drop(i, j);
        }
	}
}