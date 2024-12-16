using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static OvermorrowMod.Core.WorldGeneration.ArchiveSubworld.SetupGenPass;

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

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TowerKey>();

            base.MouseOver(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);


            SanctumGate_TE door;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<SanctumGate_TE>(bottomLeft.X, bottomLeft.Y, out door);
            if (door != null)
            {
                door.Interact();
            }

            return base.RightClick(i, j);
        }
    }

    public class SanctumGate_TE : ModTileEntity
    {
        public void Interact()
        {
            Main.NewText("The DOOR.", Color.OrangeRed);
        }

        public override void Update()
        {

        }
        public override void SaveData(TagCompound tag)
        {
        }


        public override void LoadData(TagCompound tag)
        {     
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<SanctumGate>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<SanctumGate>();
        }
    }
}