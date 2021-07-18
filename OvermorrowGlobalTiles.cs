using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using OvermorrowMod.Projectiles.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Items.Other;

namespace OvermorrowMod
{
    public class OvermorrowGlobalTiles : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (Main.rand.Next(6) == 0)
                {
                    // Normal Trees
                    if (type == TileID.Trees && Main.tile[i, j + 1].type == TileID.Grass)
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<BrambleCannon>());
                    }

                    // Tundra
                    if (type == TileID.Trees && (Main.tile[i, j + 1].type == TileID.SnowBlock))
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<SpruceSprayer>());
                    }

                    // Corruption
                    if (type == TileID.Trees && (Main.tile[i, j + 1].type == TileID.CorruptGrass))
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<RotRocket>());
                    }

                    // Crimson
                    if (type == TileID.Trees && (Main.tile[i, j + 1].type == TileID.FleshGrass))
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<MeatMissile>());
                    }

                    // Jungle Trees
                    if (type == TileID.Trees && (Main.tile[i, j + 1].type == TileID.Mud || Main.tile[i, j + 1].type == TileID.JungleGrass))
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<JungleSapper>());
                    }

                    // Palm Trees
                    if (type == TileID.PalmTree && Main.tile[i, j + 1].type == TileID.Sand)
                    {
                        Item.NewItem(i * 16, (j - 5) * 16, 32, 32, ModContent.ItemType<CoconutBuster>());
                    } 
                }
            }

            return base.Drop(i, j, type);
        }
    }
}