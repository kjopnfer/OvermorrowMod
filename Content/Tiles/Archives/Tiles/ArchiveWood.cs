using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveWood : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(74, 47, 33));
        }

        bool isUpPanel(Tile tile)
        {
            const int FRAME_SIZE = 18;
            return tile.TileFrameX == FRAME_SIZE * 1 && tile.TileFrameY == FRAME_SIZE * 1;
        }

        bool isDownPanel(Tile tile)
        {
            const int FRAME_SIZE = 18;
            return tile.TileFrameX == FRAME_SIZE * 2 && tile.TileFrameY == FRAME_SIZE * 1;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            const int FRAME_SIZE = 18;
            
            Tile tile = Main.tile[i, j];
            Tile tileAbove = Main.tile[i, j - 1];
            Tile tileBelow = Main.tile[i, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange);
            Lighting.GetCornerColors(i, j, out VertexColors vertices);
            
            var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            var aboveFrame = new Rectangle(tileAbove.TileFrameX, tileAbove.TileFrameY, 16, 16);
            var belowFrame = new Rectangle(tileBelow.TileFrameX, tileBelow.TileFrameY, 16, 16);
            var leftFrame = new Rectangle(tileLeft.TileFrameX, tileLeft.TileFrameY, 16, 16);
            var rightFrame = new Rectangle(tileRight.TileFrameX, tileRight.TileFrameY, 16, 16);

            Rectangle tilePanelUp = new Rectangle(FRAME_SIZE * 1, FRAME_SIZE * 1, 16, 16);
            Rectangle tilePanelDown = new Rectangle(FRAME_SIZE * 2, FRAME_SIZE * 1, 16, 16);

            // Frames
            #region Outer Frames
            // Top Frame
            if (!tileAbove.HasTile || tileAbove.TileType != ModContent.TileType<ArchiveWood>())
            {
                if (tileLeft.TileType != ModContent.TileType<ArchiveWood>() && tileRight.TileType == ModContent.TileType<ArchiveWood>())
                {
                    Main.tile[i, j].TileFrameX = FRAME_SIZE * 0;
                    Main.tile[i, j].TileFrameY = FRAME_SIZE * 3;

                    return true;
                }

                if (tileRight.TileType != ModContent.TileType<ArchiveWood>() && tileLeft.TileType == ModContent.TileType<ArchiveWood>())
                {
                    Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                    Main.tile[i, j].TileFrameY = FRAME_SIZE * 3;

                    return true;
                }

                Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 0;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelDown, Color.Cyan, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }

            // Bottom Frame
            if (!tileBelow.HasTile || tileBelow.TileType != ModContent.TileType<ArchiveWood>())
            {
                if (tileLeft.TileType != ModContent.TileType<ArchiveWood>() && tileRight.TileType == ModContent.TileType<ArchiveWood>())
                {
                    Main.tile[i, j].TileFrameX = FRAME_SIZE * 0;
                    Main.tile[i, j].TileFrameY = FRAME_SIZE * 4;

                    return true;
                }

                if (tileRight.TileType != ModContent.TileType<ArchiveWood>() && tileLeft.TileType == ModContent.TileType<ArchiveWood>())
                {
                    Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                    Main.tile[i, j].TileFrameY = FRAME_SIZE * 4;

                    return true;
                }

                Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 2;

                return true;
            }

            // Left Frame
            if (!tileLeft.HasTile || tileLeft.TileType != ModContent.TileType<ArchiveWood>())
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 0;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                return true;
            }

            // Right Frame
            if (!tileRight.HasTile || tileRight.TileType != ModContent.TileType<ArchiveWood>())
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 4;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                return true;
            }
            #endregion

            // Tiles are calculated from top left, going row by row until the bottom-right.
            #region Topmost Row
            // Top-Left Corner
            // This should decide how the rest of the tiles are interpolated
            if (!aboveFrame.Intersects(tilePanelUp) && !aboveFrame.Intersects(tilePanelDown) &&
                !leftFrame.Intersects(tilePanelUp) && !leftFrame.Intersects(tilePanelDown) &&
                tileRight.TileType == ModContent.TileType<ArchiveWood>() && 
                tileLeft.TileType == ModContent.TileType<ArchiveWood>())
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelUp, Color.Red, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }

            // If the top tile is not pointing up or down (frame), and the left tile is pointing up,
            // then draw the tile pointing down.
            if (!aboveFrame.Intersects(tilePanelUp) && !aboveFrame.Intersects(tilePanelDown) &&
                isUpPanel(tileLeft))
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 2;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelDown, Color.Cyan, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }

            if (!aboveFrame.Intersects(tilePanelUp) && !aboveFrame.Intersects(tilePanelDown) &&
                isDownPanel(tileLeft))
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelUp, Color.White, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }
            #endregion


            #region Main Body
            if (isUpPanel(tileAbove))
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 2;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelDown, Color.MediumPurple, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }

            if (isDownPanel(tileAbove))
            {
                Main.tile[i, j].TileFrameX = FRAME_SIZE * 1;
                Main.tile[i, j].TileFrameY = FRAME_SIZE * 1;

                //Main.tileBatch.Draw(texture, drawPosition, tilePanelUp, Color.LightYellow, Vector2.Zero, 1f, SpriteEffects.None);
                return true;
            }
            #endregion

            return false;
        }
    }
}