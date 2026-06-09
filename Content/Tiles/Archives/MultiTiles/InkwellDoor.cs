using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using SubworldLibrary;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class InkwellDoor : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 12;
            TileObjectData.newTile.Height = 15;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 14);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(24, 21, 18), name);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ArchiveKey>();
            base.MouseOver(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<InkwellDoor_TE>(bottomLeft.X, bottomLeft.Y, out InkwellDoor_TE door);
            door?.Interact();
            return base.RightClick(i, j);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<InkwellDoor_TE>(bottomLeft.X, bottomLeft.Y, out InkwellDoor_TE door);

            var tileSize = 18;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
            var offset = 270 * (door.DoorFrame - 1);
            for (int xFrame = 0; xFrame <= 198; xFrame += tileSize)
            {
                for (int yFrame = 0; yFrame <= 258; yFrame += tileSize)
                {
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        Rectangle drawRectangle = new Rectangle(xFrame, offset + yFrame, 16, 16);
                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                        spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            return false;
        }
    }

    public class InkwellDoor_TE : ModTileEntity
    {
        public string TargetSubworld = "";

        private int FrameCounter = 0;
        public int DoorFrame = 1;
        public Vector2 DoorPosition => Position.ToWorldCoordinates(16, 16);

        public override void SaveData(TagCompound tag)
        {
            tag["TargetSubworld"] = TargetSubworld;
        }

        public override void LoadData(TagCompound tag)
        {
            TargetSubworld = tag.GetString("TargetSubworld");
        }

        public void Interact()
        {
            if (!string.IsNullOrEmpty(TargetSubworld))
                SubworldSystem.Enter(TargetSubworld);
        }

        public override void Update()
        {
            Vector2 playerPosition = Main.LocalPlayer.Center;
            float distance = Vector2.Distance(playerPosition, DoorPosition + new Vector2(75, 0));

            if (distance <= ModUtils.TilesToPixels(6))
            {
                if (DoorFrame < 7 && ++FrameCounter >= 3) { DoorFrame++; FrameCounter = 0; }
            }
            else if (DoorFrame > 1 && ++FrameCounter >= 3) { DoorFrame--; FrameCounter = 0; }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<InkwellDoor>())
                Kill(Position.X, Position.Y);
            return tile.HasTile && tile.TileType == ModContent.TileType<InkwellDoor>();
        }
    }
}
