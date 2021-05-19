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
using Terraria.GameInput;
using System.Text;

namespace OvermorrowMod
{
	public class OvermorrowModFile : Mod
	{
        // UI
        internal UserInterface MyInterface;
        internal SoulUI Souls;
        private GameTime _lastUpdateUiGameTime;

        // Hotkeys
        public static ModHotKey SandModeKey;

        public static OvermorrowModFile Mod { get; set; }

        public OvermorrowModFile()
        {
            Mod = this;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
            switch (msg)
            {
                case Message.AddSoul:
                    XPPacket.Read(reader);
                    break;
            }
        }

        public override void Load()
        {
            SandModeKey = RegisterHotKey("Swap Sand Mode", "Q");
            if (!Main.dedServ)
            {
                MyInterface = new UserInterface();

                Souls = new SoulUI();
                Souls.Activate();
            }
        }

        internal void BossTitle(int BossID)
		{
			string BossName = "";
			string BossTitle = "";
			Color titleColor = Color.White;
			switch (BossID)
			{
				case 1:
					BossName = "snoop dogg";
					BossTitle = "high king";
					titleColor = Color.Green;
					break;
			}
			Vector2 textSize = Main.fontDeathText.MeasureString(BossName);
			Vector2 textSize2 = Main.fontDeathText.MeasureString(BossTitle) * 0.5f;
			float textPositionLeft = (Main.screenWidth / 2) - textSize.X / 2f;
			float text2PositionLeft = (Main.screenWidth / 2) - textSize2.X / 2f;
			float alpha = 255;
			float alpha2 = 255;

			DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossTitle, new Vector2(text2PositionLeft, (Main.screenHeight / 2 - 250)), titleColor, 0f, Vector2.Zero, 0.6f, 0, 0f);
			DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossName, new Vector2(textPositionLeft, (Main.screenHeight / 2 - 300)), Color.LimeGreen, 0f, Vector2.Zero, 1f, 0, 0f);
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

                OvermorrowModPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<OvermorrowModPlayer>();
                if (modPlayer.ShowText)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Overmorrow: Title",
                    delegate
                    {
                        BossTitle(1);
                        return true;
                    },
                    InterfaceScaleType.UI));
                }
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

            SandModeKey = null;
        }
    }
}