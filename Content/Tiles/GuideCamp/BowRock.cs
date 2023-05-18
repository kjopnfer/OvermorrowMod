using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using static OvermorrowMod.Common.TilePiles.BaseTilePile;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class BowRock : ModTilePile<BowRockObjects>
    {
        public override TileStyle GridStyle => TileStyle.Style3x2;
    }

    public class BowRockObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x2;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[3];

            /*PileContents[0] = new TileInfo(ObjectType<Rock2>(), position, 16, 54, -1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Bow>(), position, 32, 46, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Quiver>(), position, 18, 46, 0, (int)TileInfo.InteractionType.Mine);*/

            PileContents[0] = new TileInfo(ObjectType<Rock2>(), position, 16 - 16, 54 - 16, -1, (int)TileInfo.InteractionType.Mine);
            PileContents[1] = new TileInfo(ObjectType<Bow>(), position, 32 - 16, 46 - 16, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Quiver>(), position, 18 - 16, 46 - 16, 0, (int)TileInfo.InteractionType.Click);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            BowRockObjects te = ByID[id] as BowRockObjects;
            te.SetPosition(new Vector2(i, j));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<BowRock>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<BowRock>();
        }
    }
}