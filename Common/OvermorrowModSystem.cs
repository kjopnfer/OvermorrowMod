using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.VanillaOverrides;
using OvermorrowMod.Content.UI;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common
{
    public class OvermorrowModSystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        internal UserInterface MyInterface;
        internal UserInterface AltarUI;

        internal AltarUI Altar;

        internal TrajectoryDraw trajectoryDraw;
        private UserInterface trajDraw;

        internal bowChargeDraw BowChargeDraw;
        private UserInterface bowCargDraw;

        public static bool shid;
        public static int[] bow2Send;
        public string faef = "foof";

        public override void PostUpdateEverything()
        {
            Trail.UpdateTrails();
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                AltarUI = new UserInterface();

                MyInterface = new UserInterface();

                Altar = new AltarUI();
                Altar.Activate();

                trajectoryDraw = new TrajectoryDraw();
                trajectoryDraw.Activate();
                trajDraw = new UserInterface();
                trajDraw.SetState(trajectoryDraw);

                BowChargeDraw = new bowChargeDraw();
                BowChargeDraw.Activate();
                bowCargDraw = new UserInterface();
                bowCargDraw.SetState(BowChargeDraw);
            }
        }

        public override void Unload()
        {
            shid = false;
            bow2Send = new int[] { };
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (MyInterface?.CurrentState != null && !Main.gameMenu)
            {
                MyInterface.Update(gameTime);
            }

            if (AltarUI?.CurrentState != null && !Main.gameMenu)
            {
                AltarUI.Update(gameTime);
            }

            trajDraw?.Update(gameTime);

            bowCargDraw?.Update(gameTime);
        }

        internal void BossTitle(int BossID)
        {
            string BossName = "";
            string BossTitle = "";
            Color titleColor = Color.White;
            Color nameColor = Color.White;
            switch (BossID)
            {
                case 1:
                    BossName = "Dharuud";
                    BossTitle = "The Sandstorm";
                    nameColor = Color.LightGoldenrodYellow;
                    titleColor = Color.Yellow;
                    break;
                case 2:
                    BossName = "The Storm Drake";
                    BossTitle = "Apex Predator";
                    nameColor = Color.Cyan;
                    titleColor = Color.DarkCyan;
                    break;
                case 3:
                    BossName = "Dripplord";
                    BossTitle = "Bloody Assimilator";
                    nameColor = Color.Red;
                    titleColor = Color.DarkRed;
                    break;
                case 4:
                    BossName = "Iorich";
                    BossTitle = "The Guardian";
                    nameColor = Color.LimeGreen;
                    titleColor = Color.Green;
                    break;
                case 5:
                    BossName = "Gra-knight and Lady Apollo";//"Gra-knight and Apollus";
                    BossTitle = "The Super Stoner Buds";//"The Super Stoner Bros"; /*The Super Biome Brothers*/
                    nameColor = new Color(230, 228, 216);
                    titleColor = new Color(64, 80, 89);
                    break;
                default:
                    BossName = "snoop dogg";
                    BossTitle = "high king";
                    nameColor = Color.LimeGreen;
                    titleColor = Color.Green;
                    break;

            }
            Vector2 textSize = FontAssets.DeathText.Value.MeasureString(BossName);
            Vector2 textSize2 = FontAssets.DeathText.Value.MeasureString(BossTitle) * 0.5f;
            float textPositionLeft = (Main.screenWidth / 2) - textSize.X / 2f;
            float text2PositionLeft = (Main.screenWidth / 2) - textSize2.X / 2f;
            /*float alpha = 255;
			float alpha2 = 255;*/

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, BossTitle, new Vector2(text2PositionLeft, (Main.screenHeight / 2 - 250)), titleColor, 0f, Vector2.Zero, 0.6f, 0, 0f);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, BossName, new Vector2(textPositionLeft, (Main.screenHeight / 2 - 300)), nameColor, 0f, Vector2.Zero, 1f, 0, 0f);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                       "OvermorrowMod: AltarUI",
                       delegate
                       {
                           if (_lastUpdateUiGameTime != null && AltarUI?.CurrentState != null)
                           {
                               AltarUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                           }
                           return true;
                       },
                          InterfaceScaleType.UI));

                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: MyInterface",
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
                    "OvermorrowMod: Title",
                    delegate
                    {
                        BossTitle(modPlayer.TitleID);
                        return true;
                    },
                    InterfaceScaleType.UI));
                }

                mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "OvermorrowMod: Bow Trajectory",
                        delegate
                        {
                            trajDraw.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.Game)
                    );
                }
                mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "OvermorrowMod: Bow Charge",
                        delegate
                        {
                            bowCargDraw.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.Game)
                    );
                }
            }
        }


        internal void ShowAltar()
        {
            AltarUI?.SetState(Altar);
        }

        internal void HideAltar()
        {
            AltarUI?.SetState(null);
        }

        internal void HideMyUI()
        {
            MyInterface?.SetState(null);
        }


        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                Particle.UpdateParticles();
            }
        }
    }
}
