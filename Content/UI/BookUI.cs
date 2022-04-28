using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    public class BookUI : UIState
    {
        public static bool Open = false;
        public static bool Dragging = false;
        public static bool Visible => Main.playerInventory && Main.LocalPlayer.chest == -1 && Main.npcShop == 0;

        private QuestLog Back = new QuestLog();
        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));
        private UIImageButton BookButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));
        public override void OnInitialize()
        {
            ModUtils.AddElement(BookButton, 570, 274, 34, 38, this);
            BookButton.OnClick += BookClicked;
            BookButton.SetVisibility(1, 1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                BookButton.Draw(spriteBatch);
                BookButton.SetVisibility(1, 1);

                if (BookButton.IsMouseHovering)
                {
                    Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Outline").Value;
                    spriteBatch.Draw(outline, BookButton.GetDimensions().Position() + new Vector2(-2, -2), Color.White);

                    Utils.DrawBorderString(spriteBatch, "Journal", Main.MouseScreen + Vector2.One * 16, Main.MouseTextColorReal, 1f);
                }
            }

            if (Open) base.Draw(spriteBatch);
        }

        private void BookClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            var state = ModContent.GetInstance<OvermorrowModSystem>().QuestLog;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);

            Main.playerInventory = false;
            //Main.LocalPlayer.ToggleInv();
            //Open = true;
        }

        public override void Update(GameTime gameTime)
        {
            BookButton.SetImage(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));

            if (!Main.mouseLeft) Dragging = false;

            Recalculate();
            base.Update(gameTime);
        }
    }

    internal class QuestLog : UIState
    {
        private BookPanel BackPanel = new BookPanel();
        private UIElement LeftPanel = new UIElement();
        private UIElement RightPanel = new UIElement();

        private UIPanel LeftPage = new UIPanel();
        private UIPanel RightPage = new UIPanel();

        private UIList QuestList = new UIList();
        private UIScrollbar ScrollBar = new UIScrollbar();

        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));

        public override void OnInitialize()
        {
            ModUtils.AddElement(BackPanel, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);

            ModUtils.AddElement(LeftPanel, 0, 0, 346, 500, BackPanel);
            LeftPanel.SetPadding(0);

            ModUtils.AddElement(RightPanel, 380, 0, 346, 500, BackPanel);
            RightPanel.SetPadding(0);

            ModUtils.AddElement(LeftPage, 0, 0, 346, 500, LeftPanel);
            ModUtils.AddElement(RightPage, 0, 0, 346, 500, RightPanel);

            ModUtils.AddElement(QuestList, 0, 0, 120, 390, LeftPanel);
            QuestList.ListPadding = 2;

            ModUtils.AddElement(ScrollBar, 0, 0, 18, 390, LeftPanel);
            ScrollBar.SetView(0, 410);
            QuestList.SetScrollbar(ScrollBar);

            ModUtils.AddElement(ExitButton, 188, 4, 32, 32, RightPanel);

            UpdateList();         

            ExitButton.OnClick += Exit;
        }

        private void AddEntry(UIElement element, float offY)
        {
            element.Left.Set(0, 0);
            element.Top.Set(offY, 0);
            element.Width.Set(120, 0);
            element.Height.Set(28, 0);
            QuestList.Add(element);
        }

        private void Exit(UIMouseEvent evt, UIElement listeningElement)
        {
            var state = ModContent.GetInstance<OvermorrowModSystem>().BookUI;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);
        }

        private void UpdateList()
        {
            QuestList.Clear();

            for (int i = 0; i < 25; i++)
            {
                QuestList.Add(new UIText("TEST1"));
                QuestList.Add(new UIText("AFHAF"));
            }

            if (!Main.gameMenu)
            {
                AddEntry(new UIText("TEST1"), 0);
                AddEntry(new UIText("TEST2"), 0);

                var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                foreach (var quest in modPlayer.CompletedQuests)
                {
                    UIText text = new UIText(quest.ToString());
                    AddEntry(text, 0);
                }

                foreach (var quest in Quests.Quests.GlobalCompletedQuests)
                {
                    UIText text = new UIText("TEST");
                    AddEntry(text, 0);
                }

                foreach (var quest in Quests.Quests.PerPlayerCompletedQuests)
                {
                    UIText text = new UIText("Test");
                    AddEntry(text, 0);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // You're not allowed to open the inventory if the book is OPEN
            Main.playerInventory = false;
        }
    }
}