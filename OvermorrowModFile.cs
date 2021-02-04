using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Steamworks;
using OvermorrowMod.UI;

namespace OvermorrowMod
{
	public class OvermorrowModFile : Mod
	{
        // UI
        internal UserInterface MyInterface;
        internal SoulUI Souls;
        private GameTime _lastUpdateUiGameTime;

        public static OvermorrowModFile Mod { get; set; }

        public OvermorrowModFile()
        {
            Mod = this;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                MyInterface = new UserInterface();

                Souls = new SoulUI();
                Souls.Activate();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if(MyInterface?.CurrentState != null && !Main.gameMenu)
            {
                MyInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "MyMod: MyInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && MyInterface?.CurrentState != null)
                        {
                            MyInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));
            }
        }

        internal void ShowMyUI()
        {
            MyInterface?.SetState(Souls);
        }

        internal void HideMyUI()
        {
            MyInterface?.SetState(null);
        }

        public override void Unload()
        {
            Souls = null;
        }
    }
}