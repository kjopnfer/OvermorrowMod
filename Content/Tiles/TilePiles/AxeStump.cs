using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Tiles.TilePiles.TileObjects;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Core;

namespace OvermorrowMod.Content.Tiles.TilePiles
{
    public class AxeStump : ModTilePile<AxeLoot>
    {
        public override string Texture => AssetDirectory.TilePiles + "Grid_3x3";
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); // Probably should be changeable within the child
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<AxeLoot>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55; // debugging
            TileObjectData.addTile(Type);
        }
    }

    public class AxeLoot : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x3;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[2];
            PileContents[0] = new TileInfo(ObjectType<Axe>(), position, 14, 22, 1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Stump>(), position, 16, 38, -1, (int)TileInfo.InteractionType.Chop);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            AxeLoot te = ByID[id] as AxeLoot;
            te.SetPosition(new Vector2(i + 1, j + 2));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<AxeStump>())
            {
                Main.NewText("death");
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<AxeStump>();
        }
    }
}