using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveBookWall : ModWall
    {
        //public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override string Texture => AssetDirectory.ArchiveTiles + "ArchiveBookWall";

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(72, 74, 77));
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];
            Tile tileBelow = Main.tile[i, j + 1];

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name).Value;
            var frame = new Rectangle(tile.WallFrameX, tile.WallFrameY, 32, 32);
            Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + Main.offScreenRange - 8, j * 16 - (int)Main.screenPosition.Y + Main.offScreenRange - 8);
            Lighting.GetCornerColors(i, j, out VertexColors vertices);

            int seed = (int)i * 10007 + (int)j * 5003; // Unique seed per position
            Random rng = new Random(seed);

            int index = ModContent.GetInstance<NPCSpawnPoint>().Find(i, j);
            if (index != -1)
            {
                switch (rng.Next(3))
                {
                    case 0:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 2, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 1:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 7, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 2:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 11, 0, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                }

                return false;
            }

            /*if (frame.Intersects(new Rectangle(36 * 2, 36, 36, 36)))
            {
                if (rng.Next(2) == 0)
                    Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 3, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                else
                    Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);

                return false;
            }*/

            if (frame.Intersects(new Rectangle(36 * 2, 36, 36, 36)) ||
                frame.Intersects(new Rectangle(36 * 7, 36, 36, 36)) || 
                frame.Intersects(new Rectangle(36 * 11, 0, 36, 36)))
            {
                switch (rng.Next(12))
                {
                    case 0:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 6, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 1:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 8, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 2:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 6, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 3:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 7, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 4:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 8, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 5:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 0, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 6:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 7:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 8:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 11, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 9:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 11, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 10:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 3, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 11:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 1, 36 * 1, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                }

                return false;
            }

            if (frame.Intersects(new Rectangle(36 * 11, 0, 36, 36)))
            {
                /*switch (rng.Next(5))
                {
                    case 0:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 0, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 1:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 2:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 10, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 3:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 11, 36, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                    case 4:
                        Main.tileBatch.Draw(texture, drawPosition, new Rectangle(36 * 11, 36 * 2, 32, 32), vertices, Vector2.Zero, 1f, SpriteEffects.None);
                        break;
                }*/

                return false;
            }

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