using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveBridge : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 17;
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 4);

            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 5);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 5);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(115, 72, 34));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {

                Vector2 pos = new Vector2(i * 16, j * 16);
                if (!Main.npc.Any(NPC => NPC.type == ModContent.NPCType<BridgeCollision>() && (NPC.ModNPC as BridgeCollision).parentTile == Main.tile[i, j] && NPC.active))
                {
                    int collider = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X + 32, (int)pos.Y + 76, ModContent.NPCType<BridgeCollision>());
                    if (Main.npc[collider].ModNPC is BridgeCollision) (Main.npc[collider].ModNPC as BridgeCollision).parentTile = Main.tile[i, j];
                }
            }
        }
    }
}