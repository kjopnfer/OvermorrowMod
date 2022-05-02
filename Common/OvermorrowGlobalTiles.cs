using OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalTiles : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (Main.rand.NextBool(6))
                {
                    // Normal Trees
                    if (type == TileID.Trees && Main.tile[i, j + 1].TileType == TileID.Grass)
                    {
                        Item.NewItem(new EntitySource_TileBreak(i * 16, (j - 5) * 16), i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<BrambleCannon>());
                    }

                    // Tundra
                    if (type == TileID.Trees && (Main.tile[i, j + 1].TileType == TileID.SnowBlock))
                    {
                        Item.NewItem(new EntitySource_TileBreak(i * 16, (j - 5) * 16), i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<SpruceSprayer>());
                    }

                    // Corruption
                    if (type == TileID.Trees && (Main.tile[i, j + 1].TileType == TileID.CorruptGrass))
                    {
                        Item.NewItem(new EntitySource_TileBreak(i * 16, (j - 5) * 16), i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<RotRocket>());
                    }

                    // Crimson
                    if (type == TileID.Trees && (Main.tile[i, j + 1].TileType == TileID.CrimsonGrass))
                    {
                        Item.NewItem(new EntitySource_TileBreak(i * 16, (j - 5) * 16), i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<MeatMissile>());
                    }

                    // Palm Trees
                    if (type == TileID.PalmTree && Main.tile[i, j + 1].TileType == TileID.Sand)
                    {
                        Item.NewItem(new EntitySource_TileBreak(i * 16, (j - 5) * 16), i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<CoconutBuster>());
                    }
                }
            }

            return base.Drop(i, j, type);
        }
    }
}