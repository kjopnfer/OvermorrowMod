using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.DrawLayers
{
    public class ShieldDrawLayer : PlayerDrawLayer
    {
        public int frameCounter;
        public int frame = 0;
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Shield);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            DrawData drawData = new DrawData();
            Vector2 Position = drawPlayer.position;

            Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 3224) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D ChargeContainer = ModContent.Request<Texture2D>(AssetDirectory.Textures + "TESTCIRCLE").Value;

            frameCounter++;

            if (frameCounter % 3f == 2f) // Ticks per frame
            {
                frame += 1;
            }

            if (frame >= 26) // 6 is max # of frames
            {
                frame = 0; // Reset back to default
            }

            var drawRectangleMeter = new Rectangle(0, 256 * frame, ChargeContainer.Width, 256);
            drawData = new DrawData(ChargeContainer, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeContainer.Size() / 2f, 1f, SpriteEffects.None, 0);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}
