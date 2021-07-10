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