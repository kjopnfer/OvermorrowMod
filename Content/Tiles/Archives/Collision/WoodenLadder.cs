using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.CustomCollision;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WoodenLadder : CustomTileCollision
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;

            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(
                AnchorType.SolidTile | AnchorType.Table | AnchorType.SolidWithTop,
                TileObjectData.newTile.Width, 0);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(101, 69, 45));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            var topLeft = TileObjectData.TopLeft(i, j);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                SpawnCollisionHandler<WoodenLadderCollision>(topLeft.X, topLeft.Y, 4, 3, ModContent.NPCType<WoodenLadderCollision>());
            }
        }
    }
}
