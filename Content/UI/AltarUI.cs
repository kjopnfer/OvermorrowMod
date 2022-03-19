using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    class AltarUI : UIState
    {
        private VanillaItemSlotWrapper _vanillaItemSlot;
        private UIText header;
        UIElement panel;
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

            panel = new UIElement();
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

                if (Main.netMode != NetmodeID.Server)
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

            if (!_vanillaItemSlot.Item.IsAir)
            {
                // string name = _vanillaItemSlot.Item.Name;
                header.SetText("choose buf");
                cool = true;
            }
            else
            {
                header.SetText("put crystal lol");
                cool = false;
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