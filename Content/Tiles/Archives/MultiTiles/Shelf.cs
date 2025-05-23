using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.CustomCollision;
using OvermorrowMod.Content.NPCs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class LeftShelf : CustomTileCollision
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        protected virtual int Height => 13;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 10;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(100, 61, 41), name);
        }
    
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                SpawnCollisionHandler<LeftShelfCollision>(i, j, ModContent.NPCType<LeftShelfCollision>());
                /*Vector2 pos = new Vector2(i * 16, j * 16);
                if (!Main.npc.Any(NPC => NPC.type == ModContent.NPCType<LeftShelfCollision>() && (NPC.ModNPC as LeftShelfCollision).parentTile == Main.tile[i, j] && NPC.active))
                {
                    int collider = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X + 32, (int)pos.Y + 76, ModContent.NPCType<LeftShelfCollision>());
                    if (Main.npc[collider].ModNPC is LeftShelfCollision) (Main.npc[collider].ModNPC as LeftShelfCollision).parentTile = Main.tile[i, j];
                }*/
            }

            base.NearbyEffects(i, j, closer);
        }
    }

    public class RightShelf : LeftShelf
    {
        protected override int Height => 10;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                SpawnCollisionHandler<RightShelfCollision>(i, j, ModContent.NPCType<RightShelfCollision>());
            }
        }
    }
}