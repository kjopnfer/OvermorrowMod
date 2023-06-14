using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class SojournFlag : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 4);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(128, 136, 148));
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            animationFrame = frame;

            if (++frameCounter >= 9)
            {
                frameCounter = 0;
                frame = ++frame % 6;
            }
        }

        private int animationFrame = 0;
        private readonly int animationFrameHeight = 36;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Tiles/Town/SojournFlag_Pole").Value;
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Vector2 yOffset = Vector2.UnitY * 4;
                Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + yOffset, null, Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

                Texture2D flag = ModContent.Request<Texture2D>("OvermorrowMod/Content/Tiles/Town/SojournFlag_Flag").Value;
                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
                Vector2 flagOffset = new Vector2(4, 12);
                Rectangle drawRectangle = new Rectangle(0, animationFrameHeight * animationFrame, 42, animationFrameHeight);
                Main.spriteBatch.Draw(flag, position + yOffset + flagOffset, drawRectangle, Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f); ;
            }

            return false;
        }
    }
}