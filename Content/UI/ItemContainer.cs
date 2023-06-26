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
    public class DisplayItemSlot : UIElement
    {
        private Item _item;
        public Item Item { get => _item; }

        public DisplayItemSlot(int itemID, int stack) : base()
        {
            Item item = new Item();
            item.SetDefaults(itemID);
            item.stack = stack;
            _item = item;
        }

        public DisplayItemSlot(Item item) : base()
        {
            _item = item;
        }

        public override void OnInitialize()
        {
            int size = 42;

            Width.Set(size, 0);
            Height.Set(size, 0);

            base.OnInitialize();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float prevScale = Main.inventoryScale;

            Main.inventoryScale = 0.8f;

            CalculatedStyle style = GetDimensions();

            ItemSlot.Draw(spriteBatch, ref _item, 1, new Vector2(style.X, style.Y));
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.hoverItemName = _item.Name;
                Main.HoverItem = _item.Clone();
            }

            Main.inventoryScale = prevScale;
        }
    }

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
            //TextureAssets.InventoryBack2 = ModContent.Request<Texture2D>(AssetDirectory.UI + "InventoryBack_Empty");

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
            //TextureAssets.InventoryBack2 = oldTexture;
        }
    }
}