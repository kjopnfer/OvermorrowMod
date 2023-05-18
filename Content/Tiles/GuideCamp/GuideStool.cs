using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using static OvermorrowMod.Common.TilePiles.BaseTilePile;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class GuideStool : ModTilePile<GuideStoolObjects>
    {
        public override TileStyle GridStyle => TileStyle.Style2x2;
    }

    public class GuideStoolObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style2x2;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[3];
            /*PileContents[0] = new TileInfo(ObjectType<Bread>(), position, 10, 10, 2, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Knife>(), position, 2, 0, 2, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Stool>(), position, 2, 20, -1, (int)TileInfo.InteractionType.Mine);*/

            PileContents[0] = new TileInfo(ObjectType<Bread>(), position, 10 + 16, 10 + 16, 2, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Knife>(), position, 2 + 16, 0 + 16, 2, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Stool>(), position, 2 + 16, 20 + 16, -1, (int)TileInfo.InteractionType.Mine);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            GuideStoolObjects te = ByID[id] as GuideStoolObjects;
            te.SetPosition(new Vector2(i/* + 1*/, j /*+ 2*/));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<GuideStool>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<GuideStool>();
        }
    }
}