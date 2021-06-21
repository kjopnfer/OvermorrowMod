using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Tiles
{
    // This class shows off many things common to Lamp tiles in Terraria. The process for creating this example is detailed in: https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#examplelamp-tile
    // If you can't figure out how to recreate a vanilla tile, see that guide for instructions on how to figure it out yourself.
    internal class HerosAltar : ModTile
	{
		public override void SetDefaults() {
			// Main.tileFlame[Type] = true; This breaks it.
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileWaterDeath[Type] = false;
			Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Hero's Altar");
			AddMapEntry(new Color(253, 221, 3), name);
		}
		public override void HitWire(int i, int j) {
			Tile tile = Main.tile[i, j];
			int topY = j - tile.frameY / 18 % 3;
			short frameAdjustment = (short)(tile.frameX > 0 ? -18 : 18);
			Main.tile[i, topY].frameX += frameAdjustment;
			Main.tile[i, topY + 1].frameX += frameAdjustment;
			Main.tile[i, topY + 2].frameX += frameAdjustment;
			Wiring.SkipWire(i, topY);
			Wiring.SkipWire(i, topY + 1);
			Wiring.SkipWire(i, topY + 2);
			NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
		}
		
		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Weapons.PreHardmode.Melee.HerosBlade>());
		}
	}
}
