using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Core;
using Terraria.Audio;
using OvermorrowMod.Content.Tiles.Town;

namespace OvermorrowMod.Content.UI.JobBoard
{
    public class JobEntry
    {
        public int jobRank;
        public string jobName;

        public JobEntry(int rank, string name)
        {
            jobRank = rank;
            jobName = name;
        }
    }

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
        private JobBoard_TE boardTileEntity;

        // TODO: make a job entry ui object that gets created when running through hashset
        public void OpenJobBoard(JobBoard_TE entity)
        {
            showBoard = true;
            boardTileEntity = entity;
        }

        public override void Update(GameTime gameTime)
        {
            if (!showBoard) return;

            this.RemoveAllChildren();

            Main.LocalPlayer.mouseInterface = true;
            ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);
            ModUtils.AddElement(closeButton, 700, 0, 22, 22, drawSpace);

            DisplayBoard();
        }

        /// <summary>
        /// Handles the opening animation of the board and loading all the jobs
        /// </summary>
        private void OnBoardOpen() { }

        /// <summary>
        /// Handles drawing of the board UI
        /// </summary>
        private void DisplayBoard()
        {
            if (boardTileEntity.JobQuests.Count > 0)
            {
                Main.NewText(boardTileEntity.JobQuests.Count);
            }
            else
            {
                Main.NewText("no");
            }
        }

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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardBack").Value;
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