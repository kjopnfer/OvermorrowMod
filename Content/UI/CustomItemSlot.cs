using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    // This code is currently incompatible with Modder's Toolkit due to the Click override and Modder's Toolkit detouring that method
    /*internal class CustomItemSlot : UIElement
    {
        private Item item = new Item();

        public CustomItemSlot() : base()
        {
            Width.Set(52, 0);
            Height.Set(52, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (item.type == ItemID.None || item.stack <= 0) item.TurnToAir();

            base.Update(gameTime);
        }

        /// <summary>
        /// Used to draw the container of the item slot and to add any additional effects
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawTexture(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), new Rectangle(0, 0, 52, 52), Color.White, 0, texture.Size() / 2f, 1, 0, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Main.LocalPlayer.mouseInterface = true; // This blocks the player from using any items

            DrawTexture(spriteBatch);

            if (!item.IsAir)
            {
                Texture2D itemTexture = item.type > ItemID.Count ? ModContent.Request<Texture2D>(item.ModItem.Texture).Value : TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(itemTexture, new Rectangle((int)GetDimensions().X + 30, (int)GetDimensions().Y + 30, (int)MathHelper.Min(itemTexture.Width, 28), (int)MathHelper.Min(itemTexture.Height, 28)), itemTexture.Frame(), Color.White, 0, itemTexture.Size() / 2, 0, 0);

                if (item.stack > 1)
                    Utils.DrawBorderString(spriteBatch, item.stack.ToString(), GetDimensions().Position() + Vector2.One * 32, Color.White, 0.75f);

                if (IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.HoverItem = item.Clone();
                    Main.hoverItemName = " ";
                }
            }

            base.Draw(spriteBatch);
        }

        public virtual bool CheckValid(Item item)
        {
            return true;
        }

        public override void Click(UIMouseEvent evt)
        {
            if (item.IsAir && !Main.mouseItem.IsAir && CheckValid(Main.mouseItem)) // Places item within the slot
            {
                item = Main.mouseItem.Clone();
                Main.mouseItem.TurnToAir();
            }
            else if (!item.IsAir && Main.mouseItem.IsAir) // Removes the item from the slot
            {
                Main.mouseItem = item.Clone();
                item.TurnToAir();
            }
        }
    }*/

    public class CustomItemSlot : UIElement
    {
        internal Item Item;
        private readonly int _context;
        private readonly float _scale;

        protected bool canDraw = true;
        protected float itemOpacity = 1;
        public CustomItemSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            Item = new Item();
            Item.SetDefaults(0);

            Width.Set(TextureAssets.InventoryBack9.Value.Width * scale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * scale, 0f);
        }

        public virtual bool CheckValid(Item item)
        {
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            var oldTexture = TextureAssets.InventoryBack;

            Main.inventoryScale = _scale;
            //TextureAssets.InventoryBack = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer", AssetRequestMode.ImmediateLoad);
            //TextureAssets.InventoryBack = ModContent.Request<Texture2D>(AssetDirectory.Empty);
            TextureAssets.InventoryBack2 = ModContent.Request<Texture2D>(AssetDirectory.UI + "InventoryBack_Empty");

            Rectangle rectangle = GetDimensions().ToRectangle();

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (CheckValid(Main.mouseItem) || Main.mouseItem.IsAir)
                {
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref Item, _context);
                }
            }

            // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
            if (canDraw)
                ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft(), Color.White * itemOpacity);

            Main.inventoryScale = oldScale;
            TextureAssets.InventoryBack2 = oldTexture;
        }
    }
}