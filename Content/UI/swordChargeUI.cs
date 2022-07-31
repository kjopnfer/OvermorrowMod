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
using OvermorrowMod.Common.VanillaOverrides;

namespace OvermorrowMod.Content.UI
{
    //needs cleanup
    public class swordChargeUI : UIElement
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
            bool should = GlobalSword.ShouldOverrideSword(player.HeldItem.type);
            Main.NewText($"Should override: {should}");
            if (should && !Main.LocalPlayer.dead)
            {
                SwordOverride so = Main.LocalPlayer.HeldItem.GetGlobalItem<GlobalSword>().SwordOverride;
                float progress = so.ChargeAmount / so.MaxCharge;
  
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/swordCharge").Value;
      
                // first draw the first sprite fully
                spriteBatch.Draw(texture, new Vector2(Main.LocalPlayer.Bottom.X - texture.Width / 2, Main.LocalPlayer.Bottom.Y + 6) - Main.screenPosition, new Rectangle(0, 0, 64, 15), Color.White);
                // then draw sec sprite above it starting on 18 x because the actual bar starts there
                spriteBatch.Draw(texture, new Vector2(Main.LocalPlayer.Bottom.X - texture.Width / 2, Main.LocalPlayer.Bottom.Y + 6) - Main.screenPosition, new Rectangle(18, 15, (int)(46 * progress), 15), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

    public class swordChargeDraw : UIState
    {
        public swordChargeUI swordCharge;
        public override void OnInitialize()
        {
            swordCharge = new swordChargeUI();
            Append(swordCharge);
        }
    }
}