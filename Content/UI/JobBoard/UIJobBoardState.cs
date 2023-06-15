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

        private bool showBoard = false;
        public void OpenJobBoard()
        {
            showBoard = true;
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
}