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
    public class ArchiveBookWallFrame : ModWall
    {
        //public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override string Texture => AssetDirectory.ArchiveTiles + "ArchiveBookWallFrame";

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(72, 74, 77));
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        void DrawVerticalFrameSegment(Texture2D texture, int i, int j)
        {
            Lighting.GetCornerColors(i, j, out VertexColors vertices);

            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8);
            Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 6, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
        }

        void DrawMiddleFrameSegment(Texture2D texture, int i, int j)
        {
            Lighting.GetCornerColors(i, j, out VertexColors vertices);

            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8);
            Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 2, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
        }


        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];
            Tile tileAbove = Main.tile[i, j - 1];
            Tile tileBelow = Main.tile[i, j + 1];

            Lighting.GetCornerColors(i, j, out VertexColors vertices);

            var frame = new Rectangle(i % 16 * 16, j % 28 * 16, 16, 16);
            var frame2 = new Rectangle(tile.WallFrameX, tile.WallFrameY, 32, 32);
            //if (!(frame2.Intersects(new Rectangle(36, 36, 36 * 3, 36)) || frame2.Intersects(new Rectangle(36 * 6, 36, 36 * 3, 36 * 2)) || frame2.Intersects(new Rectangle(36 * 10, 0, 36 * 2, 36 * 3))))
            //    Main.tileBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "ArchiveBookWallFrame").Value, new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8), frame2, vertices, Vector2.Zero, 1f, SpriteEffects.None);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "ArchiveBookWallFrame").Value;
            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8);


            // Bottom Frame Sections
            if (tileBelow.HasTile)
            {
                // End Sections
                if (tileLeft.WallType == ModContent.WallType<CastleWall>() || tileRight.WallType == ModContent.WallType<CastleWall>())
                {
                    Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                    return false;
                }

                // Middle Frame Sections
                if (tileLeft.WallType == ModContent.WallType<ArchiveBookWallFrame>() || tileRight.WallType == ModContent.WallType<ArchiveBookWallFrame>())
                    DrawMiddleFrameSegment(texture, i, j);
                return false;
            }


            if (tileAbove.WallType == ModContent.WallType<ArchiveBookWall>() && tileAbove.WallType == ModContent.WallType<ArchiveBookWall>())
            {
                DrawMiddleFrameSegment(texture, i, j);
                return false;
            }

            if (tileLeft.WallType == ModContent.WallType<ArchiveBookWall>() && tileRight.WallType == ModContent.WallType<ArchiveBookWall>())
            {
                DrawVerticalFrameSegment(texture, i, j);
                return false;
            }

            // Left and Right Frame Sections
            if ((tileLeft.WallType == ModContent.WallType<ArchiveBookWall>() && tileRight.WallType == ModContent.WallType<CastleWall>()) || 
                (tileRight.WallType == ModContent.WallType<ArchiveBookWall>() && tileLeft.WallType == ModContent.WallType<CastleWall>()))
            {
                DrawVerticalFrameSegment(texture, i, j);
                return false;
            }

            // Middle Left/Right End Sections
            if (tileLeft.WallType == ModContent.WallType<CastleWall>() || tileRight.WallType == ModContent.WallType<CastleWall>())
            {
                Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                return false;
            }

            // Intersection Segment
            if (tileLeft.WallType == ModContent.WallType<ArchiveBookWallFrame>() || tileRight.WallType == ModContent.WallType<CastleWall>())
            {
                Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                return false;
            }


            // Middle Frame Sections
            if (tileLeft.WallType == ModContent.WallType<ArchiveBookWallFrame>() && tileRight.WallType == ModContent.WallType<ArchiveBookWallFrame>() &&
                tileBelow.WallType == ModContent.WallType<ArchiveBookWall>())
            {
                Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 2, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                return false;
            }





            if (frame2.Intersects(new Rectangle(36, 36, 36 * 3, 36)))
            {
                //Main.tileBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "ArchiveBookWall").Value, new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8), frame2, vertices, Vector2.Zero, 1f, SpriteEffects.None);
            }
            //Main.tileBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "ArchiveBookWall").Value, new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange), frame, vertices, Vector2.Zero, 1f, SpriteEffects.None);
            Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {/*
            Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "ArchiveBookWall").Value;
            var target = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
            target += new Vector2(TileUtils.TileAdj.X * 16, TileUtils.TileAdj.Y * 16);

            // Texture repeats after exceeding the width and height
            var TILE_WIDTH = 16;
            var TILE_HEIGHT = 28;
            var source = new Rectangle(i % TILE_WIDTH * 16, j % TILE_HEIGHT * 16, 16, 16);

            Tile tile = Framing.GetTileSafely(i, j);

            if (Lighting.NotRetro)
            {
                Lighting.GetCornerColors(i, j, out VertexColors vertices);
                Main.tileBatch.Draw(tex, target - new Vector2(0, 0), source, vertices, Vector2.Zero, 1f, SpriteEffects.None);
            }

            if (TileID.Sets.DrawsWalls[tile.TileType])
                spriteBatch.Draw(tex, target, source, Lighting.GetColor(i, j));*/
        }
    }
}