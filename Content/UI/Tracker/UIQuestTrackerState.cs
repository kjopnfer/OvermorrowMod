using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Common.Configs;
using OvermorrowMod.Common;
using OvermorrowMod.Content.UI.AmmoSwap;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.UI.Tracker
{
    public class UIQuestTrackerState : UIState
    {
        private DragableUIPanel testPanel = new DragableUIPanel();
        private QuestTrackerPanel back = new QuestTrackerPanel();

        public override void OnInitialize()
        {
            //ModUtils.AddElement(testPanel, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 240, 120, this);
            ModUtils.AddElement(back, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 240, 120, this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        bool canDo = true;
        public override void Update(GameTime gameTime)
        {
            /*if (canDo)
            {
                ModUtils.AddElement(testPanel, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 240, 120, this);
                canDo = false;
            }*/

            //this.RemoveAllChildren();
            //ModUtils.AddElement(testPanel, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 120, 120, this);
            back.RemoveAllChildren();
            back.Append(new QuestTrackerEntry());

            base.Update(gameTime);
        }
    }

    public class QuestTrackerPanel : DragableUIPanel
    {
        // Code by Seraph
        public void DrawNineSegmentTexturePanel(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, int cornerWidth, Color drawColor)
        {
            Rectangle cornerFrameTopLeft = new Rectangle(0, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomLeft = new Rectangle(0, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerFrameTopRight = new Rectangle(texture.Width - cornerWidth, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomRight = new Rectangle(texture.Width - cornerWidth, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideFrameTop = new Rectangle(cornerWidth, 0, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameBottom = new Rectangle(cornerWidth, texture.Height - cornerWidth, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameLeft = new Rectangle(0, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle sideFrameRight = new Rectangle(texture.Width - cornerWidth, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle centreFrame = new Rectangle(cornerWidth, cornerWidth, texture.Width - (cornerWidth * 2), texture.Height - (cornerWidth * 2));

            Rectangle cornerRectTopLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerRectTopRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideRectTop = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectBottom = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle sideRectRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle centreRect = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + cornerWidth, (int)dimensions.Width - (cornerWidth * 2), (int)dimensions.Height - (cornerWidth * 2));

            spriteBatch.Draw(texture, cornerRectTopLeft, cornerFrameTopLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectTopRight, cornerFrameTopRight, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomLeft, cornerFrameBottomLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomRight, cornerFrameBottomRight, drawColor);
            spriteBatch.Draw(texture, sideRectTop, sideFrameTop, drawColor);
            spriteBatch.Draw(texture, sideRectBottom, sideFrameBottom, drawColor);
            spriteBatch.Draw(texture, sideRectLeft, sideFrameLeft, drawColor);
            spriteBatch.Draw(texture, sideRectRight, sideFrameRight, drawColor);
            spriteBatch.Draw(texture, centreRect, centreFrame, drawColor);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.25f, 0.25f), 0, 0);
            //spriteBatch.Draw(texture, new Vector2(GetDimensions().X, GetDimensions().Y), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.7f, 0.25f), 0, 0);
            DrawNineSegmentTexturePanel(spriteBatch, texture, new Rectangle((int)(GetDimensions().X), (int)(GetDimensions().Center().Y - 70), 256, 160), 35, Color.White * 0.6f);
        }
    }

    public class QuestTrackerContainer : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
        }
    }

    public class QuestTrackerEntry : UIElement
    {
        public readonly float MAXIMUM_LENGTH = 250;

        public override void Draw(SpriteBatch spriteBatch)
        {
            string text = "Stryke's Stryfe";

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, "Quests", GetDimensions().Center() + new Vector2(80, -5), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, MAXIMUM_LENGTH);

            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.Yellow).ToArray();
            //ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, snippets, GetDimensions().Center(), 0f, Color.White, Color.Black, Vector2.Zero, Vector2.One, out var hoveredSnippet, MAXIMUM_LENGTH);
            //ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) + new Vector2(-150, -200), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAXIMUM_LENGTH);

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, GetDimensions().Center() + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, MAXIMUM_LENGTH);

            TextSnippet[] objectiveSnippets = ChatManager.ParseMessage("- 0/1 Defeat Stryfe of Phantasia", Color.White).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, GetDimensions().Center() + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, MAXIMUM_LENGTH);

            objectiveSnippets = ChatManager.ParseMessage("I am the Grass Man", Color.Yellow).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, GetDimensions().Center() + new Vector2(0, 70), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, MAXIMUM_LENGTH);

            objectiveSnippets = ChatManager.ParseMessage("- 2/4 Destroy Lawnmowers", Color.White).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, GetDimensions().Center() + new Vector2(0, 90), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, MAXIMUM_LENGTH);

            //ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, snippets, GetDimensions().Center(), 0f, Vector2.Zero, Vector2.One, out var hoveredSnippet);
        }
    }
}