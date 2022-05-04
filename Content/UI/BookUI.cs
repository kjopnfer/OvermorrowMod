using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.Rewards;
using OvermorrowMod.Quests.State;
using ReLogic.Graphics;
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

        private UIList TaskList = new UIList();

        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));

        private LargeText QuestTitle = new LargeText("", Color.White);
        //private UIText QuestTitle = new UIText("");
        private UIText QuestGiver = new UIText("");
        private HeadDisplay NPCHead = new HeadDisplay(-1);

        private UIText QuestType = new UIText("");
        private UITextWrapper QuestDescription = new UITextWrapper("", 346);
        private RewardList RewardList = new RewardList();

        public override void OnInitialize()
        {
            ModUtils.AddElement(BackPanel, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);

            ModUtils.AddElement(LeftPanel, 0, 0, 346, 500, BackPanel);
            LeftPanel.SetPadding(0);

            ModUtils.AddElement(RightPanel, 380, 0, 346, 500, BackPanel);
            RightPanel.SetPadding(0);

            //ModUtils.AddElement(LeftPage, 0, 0, 346, 500, LeftPanel);
            //ModUtils.AddElement(RightPage, 0, 0, 346, 500, RightPanel);

            #region Left Page
            ModUtils.AddElement(QuestList, 0, 20, 350, 410, LeftPanel);
            QuestList.ListPadding = 2;

            ModUtils.AddElement(ScrollBar, 0, 20, 18, 410, LeftPanel);
            ScrollBar.SetView(0, 410);
            QuestList.SetScrollbar(ScrollBar);
            #endregion

            #region Right Page
            ModUtils.AddElement(QuestTitle, 0 /*173*/, 50, 32, 32, RightPanel);
            QuestTitle.HAlign = 0.5f;

            ModUtils.AddElement(QuestGiver, 0 /*173*/, 90, 32, 32, RightPanel);

            ModUtils.AddElement(QuestType, 0 /*173*/, 115, 32, 32, RightPanel);
            ModUtils.AddElement(NPCHead, 46, 84, 32, 32, RightPanel);

            ModUtils.AddElement(QuestDescription, 0, 135, 346, 96, RightPanel);

            ModUtils.AddElement(TaskList, 0, 225, 346, 96, RightPanel);

            //ModUtils.AddElement(RewardList, 173, 155, 32, 32, RightPanel);
            ModUtils.AddElement(RewardList, 0, 360, 0, 1f, 44, 0f, RightPanel);
            RewardList.ListPadding = 4f;
            RewardList.ItemSize = new Vector2(42);

            ModUtils.AddElement(ExitButton, 188, 4, 32, 32, RightPanel);

            //ModUtils.AddElement(test, 173, 140, 32, 32, RightPanel);

            #endregion

            UpdateList();

            ExitButton.OnClick += Exit;
        }

        private void AddEntry(UIElement element, float offY)
        {
            element.Left.Set(40, 0);
            element.Top.Set(offY, 0);
            element.Width.Set(350, 0);
            element.Height.Set(24, 0);
            QuestList.Add(element);
        }

        private void AddTask(UIElement element, float offY)
        {
            element.Left.Set(0, 0);
            element.Top.Set(offY, 0);
            element.Width.Set(TaskList.Width.Pixels, 0);
            element.Height.Set(28, 0);
            TaskList.Add(element);
        }

        private void Exit(UIMouseEvent evt, UIElement listeningElement)
        {
            FocusQuest = null;

            var state = ModContent.GetInstance<OvermorrowModSystem>().BookUI;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);
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
                //QuestGiver.SetText("NPC: " + Lang.GetNPCNameValue(FocusQuest.QuestGiver));
                QuestGiver.SetText("NPC: ");
                NPCHead.SetHead(FocusQuest.QuestGiver);

                QuestType.SetText("Type: " + FocusQuest.Type.ToString());
                QuestDescription.text = "Description:\n" + FocusQuest.QuestDescription;

                TaskList.Clear();
                foreach (var task in FocusQuest.Requirements)
                {
                    AddTask(new UITextWrapper(task.Description, (int)TaskList.Width.Pixels), 0);
                    //AddTask(new TaskEntry(task.Description), 0);
                }

                RewardList.Clear();
                foreach (var reward in FocusQuest.Rewards)
                {
                    if (!(reward is ItemReward item)) continue;

                    RewardList.Add(new UIItemContainer(item.type, item.stack));
                }
                //QuestDescription.SetText(FocusQuest.QuestDescription);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateList();
            UpdatePage();

            // You're not allowed to open the inventory if the book is OPEN
            //Main.playerInventory = false;
        }
    }

    internal class LargeText : UIElement
    {
        private string text;
        private Color color;

        public LargeText(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            Vector2 pos = GetDimensions().ToRectangle().TopLeft() - new Vector2(GetDimensions().ToRectangle().Width, 0);
            DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.DeathText.Value, text, pos, color, 0f, Vector2.Zero, 0.6f, 0, 0f);
        }
    }

    internal class HeadDisplay : UIElement
    {
        private int NPC;

        public HeadDisplay(int NPC)
        {
            this.NPC = NPC;
        }

        public void SetHead(int ID)
        {
            NPC = ID;
        }

        /// <summary>
        /// Converts the NPC ID into the corresponding ID for the NPC head texture, hardcoded cause fuck vanilla
        /// </summary>
        /// <param name="NPC"></param>
        /// <returns></returns>
        private int TextureID(int NPC)
        {
            switch (NPC)
            {
                case NPCID.Guide: return 1;
                case NPCID.Merchant: return 2;
                case NPCID.Nurse: return 3;
                case NPCID.Demolitionist: return 4;
                case NPCID.Dryad: return 5;
                case NPCID.ArmsDealer: return 6;
                case NPCID.Clothier: return 7;
                case NPCID.Mechanic: return 8;
                case NPCID.GoblinTinkerer: return 9;
                case NPCID.Wizard: return 10;
                case NPCID.Truffle: return 12;
                case NPCID.Steampunker: return 13;
                case NPCID.DyeTrader: return 14;
                case NPCID.PartyGirl: return 15;
                case NPCID.Cyborg: return 16;
                case NPCID.Painter: return 17;
                case NPCID.WitchDoctor: return 18;
                case NPCID.Pirate: return 19;
                case NPCID.Stylist: return 20;
                case NPCID.TravellingMerchant: return 21;
                case NPCID.TaxCollector: return 23;
                case NPCID.Golfer: return 25;
                case NPCID.BestiaryGirl: return 26;
                default: return 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (NPC != -1)
            {
                Vector2 pos = GetDimensions().ToRectangle().TopLeft();
                spriteBatch.Draw(TextureAssets.NpcHead[TextureID(NPC)].Value, pos, Color.White);

                if (ContainsPoint(Main.MouseScreen))
                {
                    Utils.DrawBorderString(spriteBatch, Lang.GetNPCNameValue(NPC), Main.MouseScreen + Vector2.One * 16, Main.MouseTextColorReal, 1f);
                }
            }
        }
    }

    internal class TaskEntry : UIElement
    {
        private string text;
        public TaskEntry(string text)
        {
            this.text = text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            Utils.DrawBorderString(spriteBatch, text, pos, Color.White);
        }
    }

    internal class QuestEntry : UIElement
    {
        private BaseQuest questEntry;
        //public static QuestsState State { get; } = new QuestsState();

        public QuestEntry(BaseQuest quest)
        {
            questEntry = quest;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            // If the Quest isn't valid for a player (it is not unlocked yet), mark it as black. Otherwise,
            // if the player has completed a Quest, mark it as green. Otherwise, mark it as red.
            var stringColor = questEntry.IsValidFor(Main.LocalPlayer) ? (Quests.Quests.State.HasCompletedQuest(modPlayer, questEntry) ? (isHovering ? Color.LimeGreen : Color.Green) : (isHovering ? Color.Pink : Color.Red)) : Color.Black;
            if (modPlayer.IsDoingQuest(questEntry.QuestID))
            {
                // If the player is currently doing this Quest, mark it as yellow.
                stringColor = Color.Yellow;
            }

            Utils.DrawBorderString(spriteBatch, questEntry.QuestName, pos, stringColor);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

            ModContent.GetInstance<OvermorrowModSystem>().QuestLog.FocusQuest = questEntry;
        }
    }
}