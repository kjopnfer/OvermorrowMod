using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueSystem : ModSystem
    {
        UserInterface PopupUI;
        internal PopupState PopupState;

        //UserInterface DialogueUI;
        //internal DialogueState DialogueState;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                PopupState = new PopupState();
                PopupUI = new UserInterface();
                PopupUI.SetState(PopupState);

                /*DialogueState = new DialogueState();
                DialogueUI = new UserInterface();
                DialogueUI.SetState(DialogueState);*/
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int DialogueBox = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (DialogueBox != -1)
            {
                layers.Insert(DialogueBox, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Dialogue",
                    delegate
                    {
                        PopupUI.Draw(Main.spriteBatch, new GameTime());
                        //DialogueUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            PopupUI?.Update(gameTime);
            //DialogueUI?.Update(gameTime);
        }
    }
}