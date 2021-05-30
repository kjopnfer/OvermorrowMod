using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using System.Drawing.Drawing2D;
using Terraria.UI.Chat;

namespace OvermorrowMod.UI
{
    class AltarUI : UIState
    {
        private VanillaItemSlotWrapper _vanillaItemSlot;
        private UIText header;
        private bool cool { get; set; }
        private string text = "test";
        public override void OnInitialize()
        {
            /*UIPanel panel = new UIPanel();
            panel.Width.Set((Main.screenWidth / 3) * 2, 0);
            panel.Height.Set((Main.screenHeight / 3) * 2, 0);
            panel.BackgroundColor = Color.Black;
            panel.BorderColor = new Color(91, 0, 0, 255);
            panel.HAlign = panel.VAlign = 0.5f;
            panel.SetPadding(0);*/

            UIElement panel = new UIElement();
            panel.Width.Set(Main.screenWidth / 2, 0);
            panel.Height.Set(Main.screenHeight / 2, 0);
            panel.MaxWidth.Set(400, 0);
            panel.MaxHeight.Set(400, 0);
            panel.HAlign = panel.VAlign = 0.5f;
            Append(panel);

            header = new UIText(text);
            header.HAlign = VAlign = 0.5f;
            panel.Append(header);

            _vanillaItemSlot = new VanillaItemSlotWrapper(ItemSlot.Context.BankItem, 0.85f)
            {
                Left = { Pixels = 180 },
                Top = { Pixels = 250 },
                ValidItemFunc = item => item.IsAir || !item.IsAir
            };
            panel.Append(_vanillaItemSlot);

            if (cool)
            {
                UIText newtext = new UIText("you know dont say swears");
                newtext.HAlign = VAlign = 0.5f;
                newtext.Top.Set(50, 0);
                panel.Append(header);
            }

            /*UIPanel button2 = new UIPanel();
            button2.Width.Set(100, 0);
            button2.Height.Set(50, 0);
            button2.HAlign = 0.75f;
            button2.Top.Set(50, 0);
            button2.BackgroundColor = Color.Green;
            button2.OnClick += OnButtonClick;
            panel.Append(button2);*/

            if (cool)
            {
                Texture2D plusButtonTexture = ModContent.GetTexture("OvermorrowMod/UI/PlusButton");
                HoverImageButton button = new HoverImageButton(plusButtonTexture, "hi");
                button.Top.Set(164, 0);
                button.HAlign = 0.5f;
                button.Width.Set(18, 0f);
                button.Height.Set(18, 0f);
                button.OnClick += OnButtonClick;
                panel.Append(button);
            }

            base.Append(panel);
        }


        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.NewText("hi");
        }

        public override void OnDeactivate()
        {
            if (!_vanillaItemSlot.Item.IsAir)
            {
                // QuickSpawnClonedItem will preserve mod data of the item. QuickSpawnItem will just spawn a fresh version of the item, losing the prefix.
                Main.LocalPlayer.QuickSpawnClonedItem(_vanillaItemSlot.Item, _vanillaItemSlot.Item.stack);
                // Now that we've spawned the item back onto the player, we reset the item by turning it into air.
                _vanillaItemSlot.Item.TurnToAir();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const int slotX = 50;
            const int slotY = 270;
            if (!_vanillaItemSlot.Item.IsAir)
            {
                string name = _vanillaItemSlot.Item.Name;
                header.SetText("choose buf");
                cool = true;
            }
            else
            {
                header.SetText("put crystal lol");
                cool = false;
                //string message = "Place an item here to Awesomeify";
                //ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, message, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            // This will hide the crafting menu similar to the reforge menu.
            Main.HidePlayerCraftingMenu = true;

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface) // Prevent player from using items
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            
        }
    }
}