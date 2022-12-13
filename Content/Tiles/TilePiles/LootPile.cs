using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Content.Tiles.TilePiles.TileObjects;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Common;

namespace OvermorrowMod.Content.Tiles.TilePiles
{
    public class LootPile : ModTilePile<BasicLoot>
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); // Probably should be changeable within the child
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BasicLoot>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55; // debugging
            TileObjectData.addTile(Type);
        }
    }

    // The Tile Pile type that is tied to the in-world Tile
    public class BasicLoot : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x3;

        public override void CreateTilePile()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    PileContents = new TileInfo[2];
                    PileContents[0] = new TileInfo(ObjectType<BookStack_S3>(), position, 16, 38, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo(ObjectType<BookStack_S2>(), position, 18, 28, 0, (int)TileInfo.InteractionType.Click);
                    break;
                case 1:
                    PileContents = new TileInfo[4];
                    PileContents[0] = new TileInfo(ObjectType<Crate_S>(), position, 4, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo(ObjectType<Crate_M>(), position, 18, 28, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[2] = new TileInfo(ObjectType<Cloth_L>(), position, 18, 26, 1, (int)TileInfo.InteractionType.Click);
                    PileContents[3] = new TileInfo(ObjectType<BookStack_S3>(), position, 24, 14, 2, (int)TileInfo.InteractionType.Click);
                    break;
                case 2:
                    PileContents = new TileInfo[4];
                    PileContents[0] = new TileInfo(ObjectType<Crate_S>(), position, 18, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo(ObjectType<Cloth_S>(), position, 18, 30, 0, (int)TileInfo.InteractionType.Click);
                    PileContents[2] = new TileInfo(ObjectType<BookStack_S3>(), position, 22, 18, 1, (int)TileInfo.InteractionType.Click);
                    PileContents[3] = new TileInfo(ObjectType<Sack_S>(), position, 4, 30, -1, (int)TileInfo.InteractionType.Click);
                    break;
                case 3:
                    PileContents = new TileInfo[3];
                    PileContents[0] = new TileInfo(ObjectType<Crate_S>(), position, 6, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo(ObjectType<Crate_S>(), position, 8, 14, 0, (int)TileInfo.InteractionType.Click);
                    PileContents[2] = new TileInfo(ObjectType<Backpack_Sr>(), position, 26, 34, -1, (int)TileInfo.InteractionType.Click);
                    break;
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            BasicLoot te = ByID[id] as BasicLoot;
            te.SetPosition(new Vector2(i + 1, j + 2)); // TODO: The origin is in the top left corner, change to scale based on Style
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<LootPile>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<LootPile>();
        }
    }
}