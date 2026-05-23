using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using ReLogic.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.LoadoutSelection
{
    public class LoadoutSelection : UIState
    {
        public static bool visible = false;

        private const int PanelWidth = 440;
        private const int PanelHeight = 240;
        private const int SlotSize = 52;
        private const int ButtonWidth = 120;
        private const int ButtonHeight = 32;
        private const int SlotSpacing = 100;
        private const int ButtonGap = 24;

        private LoadoutSlot weaponSlot;
        private LoadoutSlot miscSlot;
        private LoadoutActionButton actionButton;
        private LoadoutActionButton closeButton;

        public override void OnInitialize()
        {
            weaponSlot = new LoadoutSlot { kind = LoadoutSlotKind.Weapon };
            weaponSlot.Width.Set(SlotSize, 0);
            weaponSlot.Height.Set(SlotSize, 0);
            Append(weaponSlot);

            miscSlot = new LoadoutSlot { kind = LoadoutSlotKind.Misc };
            miscSlot.Width.Set(SlotSize, 0);
            miscSlot.Height.Set(SlotSize, 0);
            Append(miscSlot);

            actionButton = new LoadoutActionButton();
            actionButton.Width.Set(ButtonWidth, 0);
            actionButton.Height.Set(ButtonHeight, 0);
            actionButton.onClick = OnActionClicked;
            Append(actionButton);

            closeButton = new LoadoutActionButton { kind = LoadoutButtonKind.Close };
            closeButton.Width.Set(ButtonWidth, 0);
            closeButton.Height.Set(ButtonHeight, 0);
            closeButton.onClick = Hide;
            Append(closeButton);
        }

        public void Show()
        {
            visible = true;
            actionButton.kind = SubworldSystem.IsActive<TestSubworld>() ? LoadoutButtonKind.Regenerate : LoadoutButtonKind.Enter;
        }

        public void Hide()
        {
            visible = false;
        }

        private void OnActionClicked()
        {
            var sp = Main.LocalPlayer.GetModPlayer<SubworldPlayer>();
            sp.pendingLoadout = weaponSlot.storedItem.Clone();
            sp.pendingMisc = miscSlot.storedItem.Clone();

            bool regenerating = SubworldSystem.IsActive<TestSubworld>();
            if (regenerating)
            {
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; i++) Main.LocalPlayer.inventory[i] = new Item();
            }

            Hide();
            SubworldSystem.Enter<TestSubworld>();
        }

        public override void Update(GameTime gameTime)
        {
            if (!visible) return;

            float panelX = (Main.screenWidth - PanelWidth) / 2f;
            float panelY = (Main.screenHeight - PanelHeight) / 2f;

            float slotsTotalWidth = SlotSize * 2 + SlotSpacing;
            float slotsStartX = panelX + (PanelWidth - slotsTotalWidth) / 2f;
            float slotsY = panelY + 80f;

            weaponSlot.Left.Set(slotsStartX, 0);
            weaponSlot.Top.Set(slotsY, 0);

            miscSlot.Left.Set(slotsStartX + SlotSize + SlotSpacing, 0);
            miscSlot.Top.Set(slotsY, 0);

            float buttonsY = panelY + PanelHeight - ButtonHeight - 16f;
            float buttonsTotal = ButtonWidth * 2 + ButtonGap;
            float buttonsStartX = panelX + (PanelWidth - buttonsTotal) / 2f;

            actionButton.Left.Set(buttonsStartX, 0);
            actionButton.Top.Set(buttonsY, 0);

            closeButton.Left.Set(buttonsStartX + ButtonWidth + ButtonGap, 0);
            closeButton.Top.Set(buttonsY, 0);

            Recalculate();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            float panelX = (Main.screenWidth - PanelWidth) / 2f;
            float panelY = (Main.screenHeight - PanelHeight) / 2f;
            Rectangle panelRect = new Rectangle((int)panelX, (int)panelY, PanelWidth, PanelHeight);

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, panelRect, new Color(20, 20, 30, 240));
            DrawBorder(spriteBatch, panelRect, new Color(160, 140, 90), 2);

            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            string titleText = "Loadout";
            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePos = new Vector2(panelX + PanelWidth / 2f, panelY + 12f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, titleText, titlePos, new Color(220, 200, 130), 0f, new Vector2(titleSize.X / 2f, 0f), new Vector2(0.6f));

            DynamicSpriteFont labelFont = FontAssets.MouseText.Value;
            DrawLabelAboveSlot(spriteBatch, labelFont, weaponSlot, "Weapon");
            DrawLabelAboveSlot(spriteBatch, labelFont, miscSlot, "Misc");

            if (panelRect.Contains(Main.MouseScreen.ToPoint()))
                Main.LocalPlayer.mouseInterface = true;

            base.Draw(spriteBatch);
        }

        private static void DrawLabelAboveSlot(SpriteBatch sb, DynamicSpriteFont font, LoadoutSlot slot, string label)
        {
            Vector2 size = ChatManager.GetStringSize(font, label, Vector2.One);
            Vector2 pos = new Vector2(slot.Left.Pixels + SlotSize / 2f - size.X / 2f, slot.Top.Pixels - size.Y - 2f);
            ChatManager.DrawColorCodedStringWithShadow(sb, font, label, pos, Color.White, 0f, Vector2.Zero, Vector2.One);
        }

        private static void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int t)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            sb.Draw(px, new Rectangle(r.X, r.Y, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Y, t, r.Height), c);
            sb.Draw(px, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }
    }
}
