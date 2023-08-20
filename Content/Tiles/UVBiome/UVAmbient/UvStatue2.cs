using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModTile;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Tiles.UVBiome.UVAmbient
{
    public class uvStatue2Item : ModItem
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/UVBiome/UVAmbient/UltravioletCrystalItem";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UV Snake statue thing idk");
            Tooltip.SetDefault("visible while the ultraviolet goggles are equipped");
        }

        public override void SetDefaults()
        {
            Item.width = 49;
            Item.height = 48;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = TileType<uvStatue2>();
        }
    }
    public class uvStatue2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            int[] UVMultiTiles = { Type, 4, 18 /*16 but padding*/ };
            if (!UVMultiGlobalTile.UVMultiTiles.Contains(UVMultiTiles))
                UVMultiGlobalTile.UVMultiTiles.Add(UVMultiTiles);
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Statue");
            AddMapEntry(new Color(50, 168, 82), name);
            TileObjectData.addTile(Type);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //drop statue item
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 pos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition - (Vector2.One * 8);
            spriteBatch.Draw(Request<Texture2D>("OvermorrowMod/Content/Tiles/UVBiome/UVAmbient/uvStatue2Glow").Value, pos + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}