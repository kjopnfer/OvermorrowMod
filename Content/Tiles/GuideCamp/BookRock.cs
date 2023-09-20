using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using static OvermorrowMod.Common.TilePiles.BaseTilePile;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class BookRock : ModTilePile<BookRockObjects>
    {
        public override TileStyle GridStyle => TileStyle.Style3x2;
    }

    public class BookRockObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x2;

        public override void CreateTilePile()
        {
            string book;
            switch (Main.rand.Next(4))
            {
                case 0:
                    book = ObjectType<Book1>();
                    break;
                case 1:
                    book = ObjectType<Book2>();
                    break;
                case 2:
                    book = ObjectType<Book3>();
                    break;
                case 3:
                    book = ObjectType<Book4>();
                    break;
                default:
                    book = ObjectType<Book1>();
                    break;
            }

            PileContents = new TileInfo[3];

            /*PileContents[0] = new TileInfo(ObjectType<Rock>(), position, 16, 54, -1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(book, position, 24, 48, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Apple>(), position, 48, 50, 0, (int)TileInfo.InteractionType.Mine);*/

            PileContents[0] = new TileInfo(ObjectType<Rock>(), position, 16 - 16, 54 - 16, -1, (int)TileInfo.InteractionType.Mine);
            PileContents[1] = new TileInfo(book, position, 24 - 16, 48 - 16, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Apple>(), position, 48 - 16, 50 - 16, 0, (int)TileInfo.InteractionType.Click);

            /*PileContents[0] = new TileInfo(ObjectType<Rock2>(), position, 16 - 16, 54 - 16, -1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Bow>(), position, 32 - 16, 46 - 16, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Quiver>(), position, 18 - 16, 46 - 16, 0, (int)TileInfo.InteractionType.Mine);*/
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            BookRockObjects te = ByID[id] as BookRockObjects;
            te.SetPosition(new Vector2(i, j));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<BookRock>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<BookRock>();
        }
    }
}