using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Tiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Effects
{
    public class DarknessTileRenderer : ILoadable
    {
        public void Load(Mod mod)
        {
            Terraria.On_Main.DrawDust += DrawOverlay;
        }

        public void Unload()
        {
            Terraria.On_Main.DrawDust -= DrawOverlay;
        }

        private static void DrawOverlay(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            if (Main.spriteBatch == null) return;
            if (Main.gameMenu || Main.instance.tileTarget == null || Main.instance.tileTarget.IsDisposed) return;

            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.Begin();
            Player player = Main.LocalPlayer;
            //var position = player.TopLeft.ToTileCoordinates();
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            for (int i = -2 + (int)Main.screenPosition.X / 16; i <= 2 + (int)(Main.screenPosition.X + Main.screenWidth) / 16; i++)
            {
                for (int j = -2 + (int)Main.screenPosition.Y / 16; j <= 2 + (int)(Main.screenPosition.Y + Main.screenHeight) / 16; j++)
                {
                    if (WorldGen.InWorld(i, j))
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.TileType == ModContent.TileType<DarknessBlock>())
                        {
                            var target = new Rectangle((int)(i * 16 - Main.screenPosition.X) / 2, (int)(j * 16 - Main.screenPosition.Y) / 2, 8, 8);

                            Vector2 drawPos = new Vector2(i, j) * 16;
                            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos - Main.screenPosition, new Rectangle(0, 0, 16, 16), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            spriteBatch.Draw(OvermorrowModFile.Instance.GradientRectangle.Value, drawPos - Main.screenPosition, new Rectangle(0, OvermorrowModFile.Instance.GradientRectangle.Value.Height / 3, 16, OvermorrowModFile.Instance.GradientRectangle.Value.Height), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                            //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, target, new Rectangle(0 * 18, 0 * 18, 16, 16), Color.Black);
                        }
                    }
                }
            }

            /*for (int i = (int)Main.screenPosition.X; i < (int)Main.screenPosition.X + Main.screenWidth; i++)
            {
                for (int j = (int)Main.screenPosition.Y; i < (int)Main.screenPosition.Y + Main.screenHeight; j++)
                {
                    var position = new Vector2(i, j).ToTileCoordinates();
                    //var tile = Framing.GetTileSafely(position.X, position.Y);
                    //if (tile.TileType == ModContent.TileType<DarknessBlock>())
                    //{
                    //    Main.NewText("intersection");
                    //    Vector2 drawPos = new Vector2(i, j);
                    //    spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPos - new Vector2(8, 8) - Main.screenPosition, new Rectangle(0, 0, 16, 16), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    //    //Dust.NewDust(new Vector2(position.X + i, position.Y + j).ToWorldCoordinates(), 16, 16, DustID.Torch);
                    //}
                }
            }*/

            spriteBatch.End();
        }
    }
}