using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Core;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class AxeStump : ModTilePile<AxeStumpObjects>
    {
        public override BaseTilePile.TileStyle GridStyle => BaseTilePile.TileStyle.Style2x2;
    }

    public class AxeStumpObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style2x2;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[2];
            PileContents[0] = new TileInfo(ObjectType<Axe>(), position, 10, 22, 1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Stump>(), position, 12, 38, -1, (int)TileInfo.InteractionType.Chop);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            AxeStumpObjects te = ByID[id] as AxeStumpObjects;
            te.SetPosition(new Vector2(i, j));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<AxeStump>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<AxeStump>();
        }
    }
}