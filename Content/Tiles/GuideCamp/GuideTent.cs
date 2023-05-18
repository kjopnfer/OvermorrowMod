using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using static OvermorrowMod.Common.TilePiles.BaseTilePile;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class GuideTent : ModTilePile<GuideTentObjects>
    {
        public override TileStyle GridStyle => TileStyle.Style6x3;
    }

    public class GuideTentObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style6x3;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[4];

            /*PileContents[0] = new TileInfo(ObjectType<Tent>(), position, 20, 36, -1, (int)TileInfo.InteractionType.Mine);
            PileContents[1] = new TileInfo(ObjectType<Pack>(), position, 78, 44, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Journal>(), position, 86, 74, 0, (int)TileInfo.InteractionType.Click);
            PileContents[3] = new TileInfo(ObjectType<CoinPouch>(), position, 92, 64, 2, (int)TileInfo.InteractionType.Click);
            */

            PileContents[0] = new TileInfo(ObjectType<Tent>(), position, 20 - 48, 36 - 32, -1, (int)TileInfo.InteractionType.Mine);
            PileContents[1] = new TileInfo(ObjectType<Pack>(), position, 78 - 48, 44 - 32, 0, (int)TileInfo.InteractionType.Click);
            PileContents[2] = new TileInfo(ObjectType<Journal>(), position, 86 - 48, 74 - 32, 0, (int)TileInfo.InteractionType.Click);
            PileContents[3] = new TileInfo(ObjectType<CoinPouch>(), position, 92 - 48, 64 - 32, 2, (int)TileInfo.InteractionType.Click);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            GuideTentObjects te = ByID[id] as GuideTentObjects;
            te.SetPosition(new Vector2(i/* + 1*/, j /*+ 2*/));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<GuideTent>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<GuideTent>();
        }
    }
}