using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using ReLogic.Graphics;
using ReLogic.Content;
using OvermorrowMod.Core;
using Terraria.UI.Chat;
using OvermorrowMod.Content.UI.AmmoSwap;
using OvermorrowMod.Common.Cutscenes;
using Terraria.Audio;

namespace OvermorrowMod.Content.UI.JobBoard
{
    public class UIJobBoardState : UIState
    {
        // has 3 states:
        // 1. opening animation
        // 2. displaying jobs
        // 3. closing animation

        // there will be unique job boards tied to various towns
        // therefore should load and save jobs tied to the job board
        private UIJobBoardPanel drawSpace = new UIJobBoardPanel();
        internal UIJobBoardCloseButton closeButton = new UIJobBoardCloseButton();

        public bool showBoard = false;
        public void OpenJobBoard()
        {
            showBoard = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!showBoard) return;

            this.RemoveAllChildren();

            Main.LocalPlayer.mouseInterface = true;
            ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);
            ModUtils.AddElement(closeButton, 700, 0, 22, 22, drawSpace);
        }

        /// <summary>
        /// Handles the opening animation of the board and loading all the jobs
        /// </summary>
        private void OnBoardOpen() { }

        /// <summary>
        /// Handles drawing of the board UI
        /// </summary>
        private void DisplayBoard() { }

        /// <summary>
        /// Handles the closing animation of the board
        /// </summary>
        private void OnBoardClose() { }
    }

    public class UIJobBoardPanel : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BookBack").Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1f, 0, 0);
        }
    }

    public class UIJobBoardCloseButton : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (Parent.Parent is UIJobBoardState parent)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BookClose").Value;
                Color color = isHovering ? Color.White * 0.5f : Color.White;
                spriteBatch.Draw(texture, pos + new Vector2(texture.Width / 2f, texture.Height / 2f), null, color, 0f, texture.Size() / 2f, 1f, 0, 0);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            if (Parent.Parent is UIJobBoardState parent)
            {
                parent.RemoveAllChildren();
                parent.showBoard = false;
            }
        }
    }

}