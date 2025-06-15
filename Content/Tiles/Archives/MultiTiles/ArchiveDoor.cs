using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            ArchiveDoor_TE door;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<ArchiveDoor_TE>(bottomLeft.X, bottomLeft.Y, out door);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;

            /*if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
                var textureHeight = texture.Height / 7;
                //Rectangle drawRectangle = new Rectangle(0, textureHeight * door.DoorFrame, texture.Width, texture.Height / 7);
                Rectangle drawRectangle = new Rectangle(0, 0 * door.DoorFrame, 16, 16);
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }

            if (tile.TileFrameX == 0 && tile.TileFrameY == 18)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
                var textureHeight = texture.Height / 7;
                //Rectangle drawRectangle = new Rectangle(0, textureHeight * door.DoorFrame, texture.Width, texture.Height / 7);
                Rectangle drawRectangle = new Rectangle(0, 18 * door.DoorFrame, 16, 16);
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }

            if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
                var textureHeight = texture.Height / 7;
                //Rectangle drawRectangle = new Rectangle(0, textureHeight * door.DoorFrame, texture.Width, texture.Height / 7);
                Rectangle drawRectangle = new Rectangle(18, 18 * door.DoorFrame, 16, 16);
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }

            if (tile.TileFrameX == 198 && tile.TileFrameY == 18)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
                var textureHeight = texture.Height / 7;
                //Rectangle drawRectangle = new Rectangle(0, textureHeight * door.DoorFrame, texture.Width, texture.Height / 7);
                Rectangle drawRectangle = new Rectangle(198, 18 * door.DoorFrame, 16, 16);
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }*/
            var offset = 270 * (door.DoorFrame - 1);
            for (int xFrame = 0; xFrame <= 198; xFrame += 18)
            {
                // Loop through all possible frame positions for y (0 to 240) in increments of 18
                for (int yFrame = 0; yFrame <= 258; yFrame += 18)
                {
                    // Only draw frames that match the current TileFrameX and TileFrameY
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        // Calculate the rectangle for the current tile frame
                        Rectangle drawRectangle = new Rectangle(xFrame, offset + (yFrame /* door.DoorFrame*/), 16, 16);

                        // Off-screen range for drawing optimization
                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                        // Draw the tile using spriteBatch
                        spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }

            return false;
        }
    }

    public class ArchiveDoor_TE : ModTileEntity
    {
        public int DoorID;
        public int PairedDoor;

        // Face sprite
        private int FrameCounter = 0;
        public int DoorFrame = 1; // Goes from frame 0 to 6
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
                matchingDoor.DoorFrame = 6;
            }
        }

        public override void Update()
        {
            Vector2 playerPosition = Main.LocalPlayer.Center;
            float distance = Vector2.Distance(playerPosition, DoorPosition + new Vector2(75, 0));

            if (distance <= ModUtils.TilesToPixels(6))
            {
                if (DoorFrame < 7)
                {
                    FrameCounter++;
                    if (FrameCounter >= 3)
                    {
                        DoorFrame++;
                        FrameCounter = 0;
                    }
                }
            }
            else
            {
                if (DoorFrame > 1)
                {
                    FrameCounter++;
                    if (FrameCounter >= 3)
                    {
                        DoorFrame--;
                        FrameCounter = 0;
                    }
                }
            }
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