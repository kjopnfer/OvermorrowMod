using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveDoor : ModTile
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
            player.cursorItemIconID = ModContent.ItemType<TowerKey>();

            base.MouseOver(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);


            ArchiveDoor_TE door;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<ArchiveDoor_TE>(bottomLeft.X, bottomLeft.Y, out door);
            if (door != null)
            {
                door.Interact();
            }

            return base.RightClick(i, j);
        }
    }

    public class ArchiveDoor_TE : ModTileEntity
    {
        public int DoorID;
        public int PairedDoor;
        public Vector2 DoorPosition => Position.ToWorldCoordinates(16, 16);
        public override void SaveData(TagCompound tag)
        {
            tag["DoorID"] = DoorID;
            tag["PairedDoor"] = PairedDoor;
        }

        public void Interact()
        {
            // Try to find the matching paired door by DoorID and PairedDoor
            var matchingDoor = ByID.Values
                                    .OfType<ArchiveDoor_TE>()
                                    .FirstOrDefault(door => door.PairedDoor == DoorID && PairedDoor == door.DoorID);

            // If a matching door is found, teleport the player to it
            if (matchingDoor != null)
            {
                Vector2 teleportOffset = new Vector2(64, -32);
                Main.LocalPlayer.Teleport(matchingDoor.Position.ToWorldCoordinates(16, 16) + teleportOffset, -1);
            }
        }

        public override void Update()
        {

        }

        public override void LoadData(TagCompound tag)
        {
            DoorID = tag.Get<int>("DoorID");
            PairedDoor = tag.Get<int>("PairedDoor");
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<ArchiveDoor>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<ArchiveDoor>();
        }
    }
}