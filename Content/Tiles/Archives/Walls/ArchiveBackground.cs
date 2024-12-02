using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveBackground : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(72, 74, 77));
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.25f;
            b = 0f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name).Value;
            var target = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
            target += new Vector2(TileUtils.TileAdj.X * 16, TileUtils.TileAdj.Y * 16);

            // Texture repeats after exceeding the width and height
            var TILE_WIDTH = 14;
            var TILE_HEIGHT = 25;
            var source = new Rectangle(i % TILE_WIDTH * 16, j % TILE_HEIGHT * 16, 16, 16);

            Tile tile = Framing.GetTileSafely(i, j);

            if (Lighting.NotRetro)
            {
                Lighting.GetCornerColors(i, j, out VertexColors vertices);
                Main.tileBatch.Draw(tex, target - new Vector2(0, 0), source, vertices, Vector2.Zero, 1f, SpriteEffects.None);
            }

            if (TileID.Sets.DrawsWalls[tile.TileType])
                spriteBatch.Draw(tex, target, source, Lighting.GetColor(i, j));
        }
    }
}