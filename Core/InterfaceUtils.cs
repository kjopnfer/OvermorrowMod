using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Input;

namespace OvermorrowMod.Core
{
    public static partial class ModUtils
    {
        // Code by Seraph
        public static void DrawNineSegmentTexturePanel(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, int cornerWidth, Color drawColor)
        {
            Rectangle cornerFrameTopLeft = new Rectangle(0, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomLeft = new Rectangle(0, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerFrameTopRight = new Rectangle(texture.Width - cornerWidth, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomRight = new Rectangle(texture.Width - cornerWidth, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideFrameTop = new Rectangle(cornerWidth, 0, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameBottom = new Rectangle(cornerWidth, texture.Height - cornerWidth, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameLeft = new Rectangle(0, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle sideFrameRight = new Rectangle(texture.Width - cornerWidth, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle centreFrame = new Rectangle(cornerWidth, cornerWidth, texture.Width - (cornerWidth * 2), texture.Height - (cornerWidth * 2));

            Rectangle cornerRectTopLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerRectTopRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideRectTop = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectBottom = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle sideRectRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle centreRect = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + cornerWidth, (int)dimensions.Width - (cornerWidth * 2), (int)dimensions.Height - (cornerWidth * 2));

            spriteBatch.Draw(texture, cornerRectTopLeft, cornerFrameTopLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectTopRight, cornerFrameTopRight, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomLeft, cornerFrameBottomLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomRight, cornerFrameBottomRight, drawColor);
            spriteBatch.Draw(texture, sideRectTop, sideFrameTop, drawColor);
            spriteBatch.Draw(texture, sideRectBottom, sideFrameBottom, drawColor);
            spriteBatch.Draw(texture, sideRectLeft, sideFrameLeft, drawColor);
            spriteBatch.Draw(texture, sideRectRight, sideFrameRight, drawColor);
            spriteBatch.Draw(texture, centreRect, centreFrame, drawColor);
        }
    }
}