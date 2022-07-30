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
using OvermorrowMod.Common.Bow;

namespace OvermorrowMod.Content.UI
{
    //needs cleanup
    public class bowChargeUI : UIElement
    {
        public static Vector2 position;
        public const int chargeUiWidth = 56;
        public const int chargeUiHeight = 14; //54
        const int frames = 5;
        float alpha;
        Player player = Main.LocalPlayer;
        public static bool mouseHoverCharge;
        public static bool dragging;

        public override void OnInitialize()
        {
            position = new Vector2(Main.MouseWorld.X- (chargeUiWidth / 2), Main.MouseWorld.Y - (chargeUiHeight / 2));
            Left.Set(position.X, 0);
            Top.Set(position.Y, 0);
            Width.Set(chargeUiWidth, 0);
            Height.Set(chargeUiHeight, 0);
        }

        public static void updatePos()
        {
            position = new Vector2(OvermorrowModPlayer.bowChargeMeterPosX, OvermorrowModPlayer.bowChargeMeterPosY);
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
                position = new Vector2(Main.MouseWorld.X - (chargeUiWidth / 2), Main.MouseWorld.Y - (chargeUiHeight / 2));
                Left.Set(position.X, 0);
                Top.Set(position.Y, 0);
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
                /*if (alpha < 255)
                    alpha += 8.5f;
                Color cringeColor = new Color(255, 255, 255, alphs);*/
                if (IsMouseHovering && !Config.bowChargeLockSend)
                    Main.hoverItemName = "Bow Charge Meter (You can adjust/lock my position by dragging me/toggling a config option)";
                mouseHoverCharge = (IsMouseHovering && !Config.bowChargeLockSend);
                if (dragging && !Config.bowChargeLockSend)
                {
                    position = new Vector2(Main.MouseWorld.X - (chargeUiWidth / 2), Main.MouseWorld.Y - (chargeUiHeight / 2));
                }
                Left.Set(position.X, 0);
                Width.Set(chargeUiWidth, 0);
                Top.Set(position.Y, 0);
                Height.Set(chargeUiHeight, 0);
                float barWidth;
                barWidth = (51 / (float)trajectoryPlayer.bowTimingMax) * (float)trajectoryPlayer.bowTiming;
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
                /*Main.NewText(barWidth);
                Main.NewText(trajectoryPlayer.bowTiming);
                Main.NewText(trajectoryPlayer.bowTimingMax);*/
                if (trajectoryPlayer.bowTimingMax == trajectoryPlayer.bowTiming)
                    spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/bowCharge").Value, position, new Rectangle(0, 40, 55, 14), Color.White);
                else
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/bowCharge").Value, position, new Rectangle(0, 24, 55, 14), Color.White);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/bowCharge").Value, new Vector2(position.X, position.Y + 4), new Rectangle(0, barFrame, (int)barWidth, 6), Color.White);
                }
            }
            else
            {
                //alpha = 0;
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