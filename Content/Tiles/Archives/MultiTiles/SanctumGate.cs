using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class SanctumGate : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 12;
            TileObjectData.newTile.Height = 19;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 18);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(178, 149, 52));
        }
    }
}