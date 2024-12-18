using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using System;
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
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 14;
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 108 && tile.TileFrameY == 126)
            {
                r = 0f; // Increase red for a more vibrant orange
                g = 1f; // Slightly lower green for a deeper orange
                b = 0; // Keep blue at 0 for a pure orange hue
            }
            /*float pulse = Main.rand.Next(28, 42) * 0.005f;
            pulse += (270 - Main.mouseTextColor) / 700f;

            r = 0f + pulse; // Increase red for a more vibrant orange
            g = 0.55f + pulse; // Slightly lower green for a deeper orange
            b = 0; // Keep blue at 0 for a pure orange hue*/

            base.ModifyLight(i, j, ref r, ref g, ref b);
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

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            /*Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "SanctumGateEye").Value;

                Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(0, 1f, 0));

                spriteBatch.Draw(texture, new Vector2(i, j) * 16, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }*/

            return base.PreDraw(i, j, spriteBatch);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "SanctumGateEye", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                // Positions for the eye lights and textures
                Vector2[] eyePositions = new Vector2[]
                {
                    new Vector2(i + 3.75f, j + 7.65f),
                    new Vector2(i + 6.1f, j + 7.2f),
                    new Vector2(i + 8.5f, j + 7.65f)
                };

                // Iterate through the positions and draw each one
                foreach (var pos in eyePositions)
                {
                    DrawEye(spriteBatch, texture, pos, i, j);
                }
            }
        }

        private void DrawEye(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, int i, int j)
        {
            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = position * 16 - Main.screenPosition + offScreenRange;

            spriteBatch.Draw(texture, drawPos, new Rectangle(0, 0, texture.Width, texture.Height / 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            Lighting.AddLight(new Vector2(position.X, position.Y) * 16, new Vector3(0, 1f, 0));
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

        public override void PostGlobalUpdate()
        {
            base.PostGlobalUpdate();
        }

        public override void SaveData(TagCompound tag)
        {
        }


        public override void LoadData(TagCompound tag)
        {
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<SanctumGate>();
        }
    }
}