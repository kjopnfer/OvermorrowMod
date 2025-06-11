using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.CustomCollision;
using OvermorrowMod.Content.NPCs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WoodenStairs : CustomTileCollision
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        protected virtual int Height => 12;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 14;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(101, 69, 45), name);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            var topLeft = TileObjectData.TopLeft(i, j);
           
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                SpawnCollisionHandler<StairCollision>(topLeft.X, topLeft.Y, 14, Height, ModContent.NPCType<StairCollision>());
            }
        }
    }
}