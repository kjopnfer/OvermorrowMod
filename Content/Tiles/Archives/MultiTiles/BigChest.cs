using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
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
    public class BigChest : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

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
            BigChest_TE chest;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<BigChest_TE>(bottomLeft.X, bottomLeft.Y, out chest);
            if (chest != null)
            {
                chest.Interact();
            }

            return base.RightClick(i, j);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            BigChest_TE chest;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<BigChest_TE>(bottomLeft.X, bottomLeft.Y, out chest);

            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name).Value;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestContainer").Value;
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestGlow").Value;

            var frameWidth = 72;
            var frameHeight = 90;

            var tileSize = 18;
            var numTilesX = 4;
            var numTilesY = 3;

            var framePixelsX = (numTilesX - 1) * tileSize;
            var framePixelsY = (numTilesY - 1) * tileSize;

            float glowProgress;
            if (chest.AnimationCounter <= 120)
            {
                glowProgress = MathHelper.Lerp(0f, 1f, EasingUtils.EaseOutBack(chest.AnimationCounter / 120f));
            }
            else
            {
                float fadeProgress = (chest.AnimationCounter - 120) / 120f;
                glowProgress = MathHelper.Lerp(1f, 0f, fadeProgress);
            }

            //var offset = 270 * (chest.AnimationFrame - 1);
            for (int xFrame = 0; xFrame <= framePixelsX; xFrame += tileSize)
            {
                // Loop through all possible frame positions for y (0 to 240) in increments of 18
                for (int yFrame = 0; yFrame <= framePixelsY; yFrame += tileSize)
                {
                    // Only draw frames that match the current TileFrameX and TileFrameY
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        // Calculate the rectangle for the current tile frame
                        Rectangle drawRectangle = new Rectangle(xFrame, 0 + (yFrame /* door.DoorFrame*/) + 2, 16, 16);

                        // Off-screen range for drawing optimization
                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                        spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        spriteBatch.Draw(glow, drawPos, drawRectangle, Color.White * glowProgress, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                        if (xFrame == 0 && yFrame == 0)
                        {
                            Texture2D lid = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestLid").Value;
                            spriteBatch.Draw(lid, drawPos + new Vector2(0, 2), null, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        }
                    }
                }
            }



            return false;
        }
    }

    public class BigChest_TE : ModTileEntity
    {
        public int ChestItem;

        public bool HasOpened = false;
        private int FrameCounter = 0;
        public int AnimationCounter = 1; // Goes from frame 0 to 7
        public bool AnimationStarted = false;
        public override void SaveData(TagCompound tag)
        {
            tag["ChestItem"] = ChestItem;
            tag["HasOpened"] = HasOpened;
        }

        public override void LoadData(TagCompound tag)
        {
            HasOpened = tag.Get<bool>("HasOpened");
            ChestItem = tag.Get<int>("ChestItem");
        }

        public void Interact()
        {
            AnimationStarted = true;
            //Item.NewItem(null, Position.ToWorldCoordinates(), ChestItem);
            Main.NewText("execute opening with item id" + ChestItem);
        }

        public override void Update()
        {
            if (AnimationStarted)
            {
                AnimationCounter++;
                if (AnimationCounter > 240) // Extended to allow for fade out
                {
                    AnimationCounter = 0;
                    AnimationStarted = false;
                }

                float glowProgress;
                if (AnimationCounter <= 120)
                {
                    // Fade in
                    glowProgress = MathHelper.Lerp(0f, 1f, EasingUtils.EaseOutBack(AnimationCounter / 120f));
                }
                else
                {
                    // Fade out
                    float fadeProgress = (AnimationCounter - 120) / 120f;
                    glowProgress = MathHelper.Lerp(1f, 0f, fadeProgress);
                }

                Lighting.AddLight(Position.ToWorldCoordinates() + new Vector2(2 * 16 - 12, 0), new Vector3(0f, 1f, 0.5f) * glowProgress);
            }
            //Dust.NewDust(Position.ToWorldCoordinates(), 1, 1, DustID.Torch);

            Main.NewText(AnimationCounter);
            /*FrameCounter++;
            if (FrameCounter > 10)
            {
                FrameCounter = 0;
                AnimationFrame++;
                if (AnimationFrame > 7)
                {
                    AnimationFrame = 0;
                }
            }*/


            //AnimationFrame = 1;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<BigChest>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<BigChest>();
        }
    }
}