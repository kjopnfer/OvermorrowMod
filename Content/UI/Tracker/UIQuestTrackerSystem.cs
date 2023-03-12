using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.Tracker
{
    public class UIQuestTrackerSystem : ModSystem
    {
        UserInterface TrackerUI;
        internal UIQuestTrackerState TrackerState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TrackerState = new UIQuestTrackerState();
                TrackerUI = new UserInterface();
                TrackerUI.SetState(TrackerState);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int DialogueBox = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (DialogueBox != -1)
            {
                layers.Insert(DialogueBox, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Quest Tracker",
                    delegate
                    {
                        TrackerUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            TrackerUI?.Update(gameTime);
        }
    }
}