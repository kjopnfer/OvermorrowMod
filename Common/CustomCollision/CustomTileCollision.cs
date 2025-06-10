using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.CustomCollision
{
    public abstract class CustomTileCollision : ModTile
    {
        protected void SpawnCollisionHandler<T>(int i, int j, int npcType) where T : TileCollisionNPC
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Vector2 pos = new Vector2(i * 16, j * 16);
                if (!Main.npc.Any(NPC =>
                    NPC.type == npcType &&
                    NPC.ModNPC is T collision &&
                    collision.parentTile == Main.tile[i, j] &&
                    NPC.active))
                {
                    int collider = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X + 32, (int)pos.Y + 76, npcType);
                    if (Main.npc[collider].ModNPC is T collision)
                    {
                        collision.parentTile = Main.tile[i, j];
                        Main.npc[collider].width = 1;
                        Main.npc[collider].height = 1;
                    }
                }
            }
        }

        protected void SpawnCollisionHandler<T>(int i, int j, int width, int height, int npcType) where T : TileCollisionNPC
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Vector2 pos = new Vector2(i * 16, j * 16);
                if (!Main.npc.Any(NPC =>
                    NPC.type == npcType &&
                    NPC.ModNPC is T collision &&
                    collision.parentTile == Main.tile[i, j] &&
                    NPC.active))
                {
                    int collider = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X, (int)pos.Y, npcType);
                    if (Main.npc[collider].ModNPC is T collision)
                    {
                        collision.parentTile = Main.tile[i, j];
                        Main.npc[collider].width = ModUtils.TilesToPixels(width);
                        Main.npc[collider].height = ModUtils.TilesToPixels(height);
                    }
                }
            }
        }
    }
}