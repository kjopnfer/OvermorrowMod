using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;

namespace OvermorrowMod.Content.UI.ReadableBook
{
    public class UIReadableBookSystem : ModSystem
    {
        internal UIReadableBookState BookState;
        public UserInterface BookUI;

        public static UIReadableBookSystem Instance { get; set; }
        public UIReadableBookSystem()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BookState = new UIReadableBookState();
                BookUI = new UserInterface();
                BookUI.SetState(BookState);
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
                    "OvermorrowMod: Readable Book",
                    delegate
                    {
                        BookUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            BookUI?.Update(gameTime);
        }
    }
}