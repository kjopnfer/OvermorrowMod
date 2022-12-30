using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.AmmoSwap
{
    public class UIAmmoSwapSystem : ModSystem
    {
        UserInterface AmmoUI;
        internal UIAmmoSwapState AmmoState;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                AmmoState = new UIAmmoSwapState();
                AmmoUI = new UserInterface();
                AmmoUI.SetState(AmmoState);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int DialogueBox = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (DialogueBox != -1)
            {
                layers.Insert(DialogueBox, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Ammo",
                    delegate
                    {
                        AmmoUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            AmmoUI?.Update(gameTime);
        }
    }
}