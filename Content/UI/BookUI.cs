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
        public BaseQuest FocusQuest = null;

        private BookPanel BackPanel = new BookPanel();
        private UIElement LeftPanel = new UIElement();
        private UIElement RightPanel = new UIElement();

        private UIPanel LeftPage = new UIPanel();
        private UIPanel RightPage = new UIPanel();

        private UIList QuestList = new UIList();
        private UIScrollbar ScrollBar = new UIScrollbar();

        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));

        private UIText QuestTitle = new UIText("");
        private UIText QuestGiver = new UIText("");
        private UIText QuestType = new UIText("");
        private TextWrapper QuestDescription = new TextWrapper("", 346);


        public override void OnInitialize()
        {
            ModUtils.AddElement(BackPanel, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);

            ModUtils.AddElement(LeftPanel, 0, 0, 346, 500, BackPanel);
            LeftPanel.SetPadding(0);

            ModUtils.AddElement(RightPanel, 380, 0, 346, 500, BackPanel);
            RightPanel.SetPadding(0);

            //ModUtils.AddElement(LeftPage, 0, 0, 346, 500, LeftPanel);
            //ModUtils.AddElement(RightPage, 0, 0, 346, 500, RightPanel);

            ModUtils.AddElement(QuestList, 0, 20, 150, 410, LeftPanel);
            QuestList.ListPadding = 2;

            ModUtils.AddElement(ScrollBar, 0, 20, 18, 410, LeftPanel);
            ScrollBar.SetView(0, 410);
            QuestList.SetScrollbar(ScrollBar);

            ModUtils.AddElement(ExitButton, 188, 4, 32, 32, RightPanel);


            #region Right Page
            ModUtils.AddElement(QuestTitle, 173, 50, 32, 32, RightPanel);
            ModUtils.AddElement(QuestGiver, 173, 65, 32, 32, RightPanel);
            ModUtils.AddElement(QuestType, 173, 80, 32, 32, RightPanel);
            ModUtils.AddElement(QuestDescription, 0, 95, 346, 96, RightPanel);

            #endregion

            UpdateList();

            ExitButton.OnClick += Exit;
        }

        private void AddEntry(UIElement element, float offY)
        {
            element.Left.Set(40, 0);
            element.Top.Set(offY, 0);
            element.Width.Set(150, 0);
            element.Height.Set(28, 0);
            QuestList.Add(element);
        }

        private void Exit(UIMouseEvent evt, UIElement listeningElement)
        {
            var state = ModContent.GetInstance<OvermorrowModSystem>().BookUI;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);

            FocusQuest = null;
        }

        private void UpdateList()
        {
            QuestList.Clear();

            for (int i = 0; i < 5; i++)
            {
                foreach (var Quest in Quests.Quests.QuestList.Values)
                {
                    //QuestEntry entry = new QuestEntry(Quest.QuestName);
                    QuestEntry entry = new QuestEntry(Quest);
                    AddEntry(entry, 0);
                }

            }
        }

        private void UpdatePage()
        {
            if (FocusQuest != null)
            {
                QuestTitle.SetText(FocusQuest.QuestName);
                QuestGiver.SetText(Lang.GetNPCNameValue(FocusQuest.QuestGiver));
                QuestType.SetText(FocusQuest.Type.ToString());
                QuestDescription.text = FocusQuest.QuestDescription;
                //QuestDescription.SetText(FocusQuest.QuestDescription);
            }
        }

        public override void Update(GameTime gameTime)  
        {
            base.Update(gameTime);

            UpdateList();
            UpdatePage();

            // You're not allowed to open the inventory if the book is OPEN
            Main.playerInventory = false;
        }
    }

    internal class QuestEntry : UIElement
    {
        private BaseQuest questEntry;

        public QuestEntry(BaseQuest quest)
        {
            questEntry = quest;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            Utils.DrawBorderString(spriteBatch, questEntry.QuestName, pos, IsMouseHovering ? Color.Blue : Color.Red);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

            Main.NewText("pass back " + questEntry.QuestName);
            ModContent.GetInstance<OvermorrowModSystem>().QuestLog.FocusQuest = questEntry;
        }
    }
}