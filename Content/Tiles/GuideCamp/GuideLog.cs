using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using static OvermorrowMod.Common.TilePiles.BaseTilePile;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class GuideLog : ModTilePile<GuideLogObjects>
    {
        public override TileStyle GridStyle => TileStyle.Style3x3;
    }

    public class GuideLogObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x3;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[3];
            /*PileContents[0] = new TileInfo(ObjectType<Log>(), position, 20, 70, -1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Cane>(), position, 16, 32, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Lantern>(), position, 50, 46, 1, (int)TileInfo.InteractionType.Click);*/

            PileContents[0] = new TileInfo(ObjectType<Log>(), position, 20 - 16, 70 - 32, -1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Cane>(), position, 16 - 16, 32 - 32, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Lantern>(), position, 50 - 16, 46 - 32, 1, (int)TileInfo.InteractionType.Click);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            GuideLogObjects te = ByID[id] as GuideLogObjects;
            te.SetPosition(new Vector2(i, j));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<GuideLog>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<GuideLog>();
        }
    }
}