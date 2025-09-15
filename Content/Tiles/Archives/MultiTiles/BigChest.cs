using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestContainer").Value;
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestGlow").Value;

            var tileSize = 18;
            var numTilesX = 4;
            var numTilesY = 3;

            var framePixelsX = (numTilesX - 1) * tileSize;
            var framePixelsY = (numTilesY - 1) * tileSize;

            float glowProgress = 0f;
            float lidOffset = 0f;

            int phase1End = 60;
            int phase2End = phase1End + chest.WaitDuration;
            int phase3End = phase2End + 60;
            int phase4End = phase3End + 30;

            if (chest.AnimationCounter <= phase1End) // Wake up glow
            {
                glowProgress = MathHelper.Lerp(0f, 1f, EasingUtils.EaseOutBack(chest.AnimationCounter / (float)phase1End));
                lidOffset = 0f;
            }
            else if (chest.AnimationCounter <= phase2End)
            {
                glowProgress = 1f;
                lidOffset = 0f;
            }
            else if (chest.AnimationCounter <= phase3End) // Lift up lid
            {
                glowProgress = 1f;
                float liftProgress = (chest.AnimationCounter - phase2End) / 60f; // Match the 60 from Update
                lidOffset = MathHelper.Lerp(0f, -ModUtils.TilesToPixels(4), EasingUtils.EaseOutQuart(liftProgress));
            }
            else if (chest.WaitingForItemPickup) // Keep lid open while waiting for item pickup
            {
                glowProgress = 1f;
                lidOffset = -ModUtils.TilesToPixels(4);
            }
            else if (chest.AnimationCounter <= phase4End + 30) // Drop lid after item pickup
            {
                glowProgress = 1f;
                float dropProgress = (chest.AnimationCounter - phase4End) / 30f;
                lidOffset = MathHelper.Lerp(-ModUtils.TilesToPixels(4), 0f, EasingUtils.EaseInBack(dropProgress));
            }
            else
            {
                float fadeProgress = (chest.AnimationCounter - phase4End - 30) / 60f;
                glowProgress = MathHelper.Lerp(1f, 0f, fadeProgress);
                lidOffset = 0f;
            }

            for (int xFrame = 0; xFrame <= framePixelsX; xFrame += tileSize)
            {
                for (int yFrame = 0; yFrame <= framePixelsY; yFrame += tileSize)
                {
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        Rectangle drawRectangle = new Rectangle(xFrame, 0 + (yFrame) + 2, 16, 16);

                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                        spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        spriteBatch.Draw(glow, drawPos, drawRectangle, Color.White * glowProgress, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                        if (xFrame == 0 && yFrame == 0)
                        {
                            Texture2D lid = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "BigChestLid").Value;
                            spriteBatch.Draw(lid, drawPos + new Vector2(0, 2 + lidOffset), null, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
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
        public int AnimationCounter = 1;
        public bool AnimationStarted = false;

        public int WaitDuration = 60;
        public int SpawnedItemID = -1;
        public bool WaitingForItemPickup = false;
        public bool ItemSpawned = false;

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
            if (!HasOpened && !AnimationStarted)
            {
                AnimationStarted = true;
                HasOpened = true;
                AnimationCounter = 1;
            }
            else
            {
            }
        }

        public override void Update()
        {
            if (AnimationStarted)
            {
                AnimationCounter++;

                int phase1End = 60;
                int phase2End = phase1End + WaitDuration;
                int phase3End = phase2End + 60;
                int phase4End = phase3End + 30;

                if (AnimationCounter <= phase1End)
                {
                }
                else if (AnimationCounter <= phase2End)
                {
                }
                else if (AnimationCounter <= phase3End)
                {
                    if (!ItemSpawned)
                    {
                        Vector2 spawnPos = Position.ToWorldCoordinates() + new Vector2(32, -16);
                        int itemID = Item.NewItem(null, spawnPos, ChestItem, 1);
                        SpawnedItemID = itemID;
                        ItemSpawned = true;
                        WaitingForItemPickup = true;
                    }
                }
                else
                {
                    // Check if item still exists
                    if (WaitingForItemPickup && SpawnedItemID >= 0 && SpawnedItemID < Main.item.Length)
                    {
                        Item spawnedItem = Main.item[SpawnedItemID];
                        if (!spawnedItem.active)
                        {
                            WaitingForItemPickup = false;
                            AnimationCounter = phase4End + 1; // Start closing animation
                        }
                        else
                        {
                            AnimationCounter = phase4End; // Stay in waiting phase
                        }
                    }
                    else if (WaitingForItemPickup)
                    {
                        WaitingForItemPickup = false;
                        AnimationCounter = phase4End + 1;
                    }

                    // End animation check
                    if (!WaitingForItemPickup && AnimationCounter > phase4End + 90) // 30 for close + 60 for fade
                    {
                        AnimationCounter = 1;
                        AnimationStarted = false;
                        ItemSpawned = false;
                        SpawnedItemID = -1;
                        WaitingForItemPickup = false;
                        HasOpened = false;
                    }
                }

                float glowProgress = 0f;

                if (AnimationCounter <= phase1End)
                {
                    glowProgress = MathHelper.Lerp(0f, 1f, EasingUtils.EaseOutBack(AnimationCounter / (float)phase1End));
                }
                else if (AnimationCounter <= phase2End)
                {
                    glowProgress = 1f;
                }
                else if (AnimationCounter <= phase3End)
                {
                    glowProgress = 1f;
                }
                else if (WaitingForItemPickup)
                {
                    glowProgress = 1f;
                }
                else if (AnimationCounter <= phase4End + 30)
                {
                    glowProgress = 1f;
                }
                else
                {
                    float fadeProgress = (AnimationCounter - phase4End - 30) / 60f;
                    glowProgress = MathHelper.Lerp(1f, 0f, fadeProgress);
                }

                Lighting.AddLight(Position.ToWorldCoordinates() + new Vector2(2 * 16 - 12, 0), new Vector3(0f, 1f, 0.5f) * glowProgress);
            }
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