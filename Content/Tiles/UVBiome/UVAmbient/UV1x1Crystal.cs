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
    public class UV1x1CrystalItem : ModItem
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/UVBiome/UVAmbient/UltravioletCrystalItem";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultraviolet Crystal (1x1)");
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
            Item.createTile = TileType<UV1x1Crystal>();
        }
    }
    public class UV1x1Crystal : ModTile
    {
        public override void SetStaticDefaults()
        {
            int[] UVMultiTiles = { Type, 1, 18 /*16 but padding*/ };
            if (!UVMultiGlobalTile.UVMultiTiles.Contains(UVMultiTiles))
                UVMultiGlobalTile.UVMultiTiles.Add(UVMultiTiles);
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Tyfloite Crystal");
            AddMapEntry(new Color(255, 255, 255), name);
            TileObjectData.addTile(Type);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 pos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + zero;
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            spriteBatch.Draw(Request<Texture2D>(Texture).Value, pos, frame, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}