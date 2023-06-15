using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.JobBoard
{
    public class UIJobBoardSystem : ModSystem
    {
        internal UIJobBoardState BoardState;
        public UserInterface BoardUI;

        public static UIJobBoardSystem Instance { get; set; }
        public UIJobBoardSystem()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BoardState = new UIJobBoardState();
                BoardUI = new UserInterface();
                BoardUI.SetState(BoardState);
            }
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int ResourceBars = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (ResourceBars != -1)
            {
                layers.Insert(ResourceBars, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Job Board",
                    delegate
                    {
                        BoardUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            BoardUI?.Update(gameTime);
        }
    }
}