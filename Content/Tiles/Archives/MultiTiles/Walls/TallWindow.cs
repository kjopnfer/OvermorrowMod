using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class TallWindow : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.AnchorWall = true;

            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 17;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 0);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 180, 220));
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
                return;

            Texture2D inner = ModContent.Request<Texture2D>(Texture + "_Inner").Value;

            var tileSize = 18;
            var numTilesX = 8;
            var numTilesY = 17;

            var framePixelsX = (numTilesX - 1) * tileSize;
            var framePixelsY = (numTilesY - 1) * tileSize;

            for (int xFrame = 0; xFrame <= framePixelsX; xFrame += tileSize)
            {
                for (int yFrame = 0; yFrame <= framePixelsY; yFrame += tileSize)
                {
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        Rectangle drawRectangle = new Rectangle(xFrame, yFrame, 16, 16);

                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                        spriteBatch.Draw(inner, drawPos, drawRectangle, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }
        }
    }
}
