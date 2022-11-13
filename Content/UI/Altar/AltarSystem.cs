using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.Altar
{
    public class AltarSystem : ModSystem
    {
        UserInterface AltarUI;
        AltarState AltarState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                AltarState = new AltarState();
                AltarUI = new UserInterface();
                AltarUI.SetState(AltarState);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int DialogueBox = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (DialogueBox != -1)
            {
                layers.Insert(DialogueBox, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: AltarUI",
                    delegate
                    {
                        AltarUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            AltarUI?.Update(gameTime);
        }
    }
}