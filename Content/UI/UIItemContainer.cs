using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.UI
{
    internal class UIItemContainer : UIElement
    {
        private Item displayItem;

        public UIItemContainer(int item, int stack, float scale = 1f)
        {
            Item displayItem = new Item();
            displayItem.SetDefaults(item);
            displayItem.stack = stack;
            this.displayItem = displayItem;

            Width.Set(TextureAssets.InventoryBack9.Value.Width * scale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * scale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float prevScale = Main.inventoryScale;

            Rectangle rectangle = GetDimensions().ToRectangle();
            ItemSlot.Draw(spriteBatch, ref displayItem, 1, rectangle.TopLeft());

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.hoverItemName = displayItem.Name;
                Main.HoverItem = displayItem.Clone();
            }
        }
    }
}