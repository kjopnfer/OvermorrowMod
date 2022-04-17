using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles
{
    public class OrbAltar : ModTile
    {
        int glowFrame;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[4] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Reaper Buff Station Thing");
            AddMapEntry(new Color(33, 18, 36), name);

            AnimationFrameHeight = 72;
        }


        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            glowFrame = frame;

            frameCounter++;
            if (frameCounter > 10)
            {
                frameCounter = 0;
                frame++;
                if (frame > 6)
                {
                    frame = 0;
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            /*
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "OrbAltar_Glow");
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(texture, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);*/
            /*
            if (tile.frameX == 0 && tile.frameY == 0)
            {
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "OrbAltar_Glow");
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Rectangle drawRectangle = new Rectangle(0, texture.Height / 7 * tile.frameY, texture.Width, texture.Height / 7);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;

                spriteBatch.Draw(texture, drawPos, drawRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }*/


            /*if (tile.frameX == 0 && tile.frameY == 0)
            {
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "OrbAltar");
                Texture2D glow = ModContent.GetTexture(AssetDirectory.Tiles + "OrbAltar_Glow");
                
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                Rectangle drawRectangle = new Rectangle(0, texture.Height / 7 * tile.frameY, texture.Width, texture.Height / 7);

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(glow, drawPos, drawRectangle, Color.White * 0.25f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }*/

            return true;
        }
    }
}
