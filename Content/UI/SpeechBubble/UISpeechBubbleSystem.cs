using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.SpeechBubble
{
    public class UISpeechBubbleSystem : ModSystem
    {
        UserInterface SpeechBubbleUI;
        internal UISpeechBubbleState SpeechBubbleState;

        public static UISpeechBubbleSystem Instance { get; set; }
        public UISpeechBubbleSystem()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SpeechBubbleState = new UISpeechBubbleState();
                SpeechBubbleUI = new UserInterface();
                SpeechBubbleUI.SetState(SpeechBubbleState);
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
                    "OvermorrowMod: SpeechBubble UI",
                    delegate
                    {
                        SpeechBubbleUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            SpeechBubbleUI?.Update(gameTime);
        }
    }
}