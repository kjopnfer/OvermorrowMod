using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Overmorrow.Common;
using OvermorrowMod.Common;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.UI
{
    //needs cleanup
    public class bowChargeUI : UIElement
    {
        public const int chargeUiWidth = 56;
        public const int chargeUiHeight = 14; //54
        Player player = Main.LocalPlayer;
        public static bool mouseHoverCharge;
        public static bool dragging;

        public override void OnInitialize()
        {
            Width.Set(chargeUiWidth, 0);
            Height.Set(chargeUiHeight, 0);
        }


        public override void MouseDown(UIMouseEvent evt)
        {
            if (IsMouseHovering && !Config.bowChargeLockSend)
            {
                dragging = true;
                player.mouseInterface = true;
                if (!player.channel && player.itemTime != 0)
                {
                    player.itemTime = 0;
                    player.channel = false;
                }
            }

            base.MouseDown(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            if (!Config.bowChargeLockSend)
            {
                Width.Set(chargeUiWidth, 0);
                Height.Set(chargeUiHeight, 0);
                dragging = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();
            if (trajectoryPlayer.drawChargeBar && !Main.LocalPlayer.dead)
            {
                float barWidth = (51 / (float)trajectoryPlayer.bowTimingMax) * (float)trajectoryPlayer.bowTiming;
                int barFrame = 0;
                trajectoryPlayer.chargeVelocityDivide = 4;
                if (barWidth > 18)
                {
                    barFrame = 8;
                    trajectoryPlayer.chargeVelocityDivide -= 2;
                }
                if (barWidth > 38)
                {
                    barFrame = 16;
                    trajectoryPlayer.chargeVelocityDivide--;
                }

                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/bowCharge").Value;
                if (trajectoryPlayer.bowTimingMax == trajectoryPlayer.bowTiming)
                    spriteBatch.Draw(texture, new Vector2(Main.LocalPlayer.Bottom.X - texture.Width / 2, Main.LocalPlayer.Bottom.Y + 6) - Main.screenPosition, new Rectangle(0, 40, 55, 14), Color.White);
                else
                {
                    spriteBatch.Draw(texture, new Vector2(Main.LocalPlayer.Bottom.X - texture.Width / 2, Main.LocalPlayer.Bottom.Y + 6) - Main.screenPosition, new Rectangle(0, 24, 55, 14), Color.White);
                    spriteBatch.Draw(texture, new Vector2(Main.LocalPlayer.Bottom.X - texture.Width / 2, Main.LocalPlayer.Bottom.Y + 10) - Main.screenPosition, new Rectangle(0, barFrame, (int)barWidth, 6), Color.White);
                }
            }

            if (Main.LocalPlayer.dead)
                trajectoryPlayer.bowTiming = 0;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

    public class bowChargeDraw : UIState
    {
        public bowChargeUI bowCharge;
        public override void OnInitialize()
        {
            bowCharge = new bowChargeUI();
            Append(bowCharge);
        }
    }
}