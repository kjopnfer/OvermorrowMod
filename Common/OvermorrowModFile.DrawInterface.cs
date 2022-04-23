using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.Requirements;
using Terraria.GameInput;
using System;
using Terraria.Localization;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModFile : Mod
    {
        // UI
        internal UserInterface MyInterface;
        internal UserInterface AltarUI;

        internal AltarUI Altar;
        private GameTime _lastUpdateUiGameTime;


        Texture2D QuestMarker;
        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            if (QuestMarker == null || QuestMarker.IsDisposed)
            {
                QuestMarker = ModContent.GetTexture(AssetDirectory.Textures + "QuestMarker");
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && !QuestWorld.PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        Vector2 MousePosition = new Vector2(Main.mouseX, Main.mouseY);
                        Vector2 ScreenPosition = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

                        MousePosition -= ScreenPosition;
                        MousePosition *= Main.UIScale;
                        MousePosition += ScreenPosition;

                        Vector2 MapPosition = Main.mapFullscreenPos * Main.mapFullscreenScale;
                        Vector2 ScreenOrigin = ScreenPosition - MapPosition;

                        ScreenOrigin += new Vector2(Main.mapFullscreenScale, Main.mapFullscreenScale) * 10;

                        Vector2 MouseTile = (MousePosition - ScreenOrigin) / Main.mapFullscreenScale;
                        MouseTile += Vector2.One * 10;

                        float MapScale = Main.mapFullscreenScale / Main.UIScale;
                        Vector2 MapCoordinates = Main.mapFullscreenPos * -MapScale;
                        MapCoordinates += ScreenPosition;

                        // Convert the world coordinates into map coordinates
                        Vector2 DrawCoordinates = travelRequirement.location / 16;
                        DrawCoordinates *= MapScale;
                        DrawCoordinates += MapCoordinates;

                        // Check the player's cursor
                        float HoverRange = 32 / Main.mapFullscreenScale;
                        if ((MouseTile - (travelRequirement.location / 16)).Length() <= HoverRange)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 OffsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 3;
                                //Main.spriteBatch.Draw(QuestMarker, DrawCoordinates + OffsetPositon, null, Color.Red, 0, QuestMarker.Size() / 2, 1.07f, SpriteEffects.None, 1);
                            }

                            Main.spriteBatch.Draw(QuestMarker, DrawCoordinates, null, Color.White, 0, QuestMarker.Size() / 2, 1.08f, SpriteEffects.None, 1);

                            if (Main.mouseLeft)
                            {
                                Main.PlaySound(SoundID.Item20, Main.LocalPlayer.position);
                                modPlayer.SelectedLocation = travelRequirement.ID;
                                Main.mapFullscreen = false;
                                Main.PlaySound(SoundID.Item20, travelRequirement.location * 16);
                            }
                        }
                        else
                        {
                            Main.spriteBatch.Draw(QuestMarker, DrawCoordinates, null, Color.White, 0, QuestMarker.Size() / 2, 1f, SpriteEffects.None, 1);
                        }
                    }
                }
            }

            base.PostDrawFullscreenMap(ref mouseText);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            // They tell me this hook should not be used but if I put into ModifyInterfaceLayers it doesn't work, lol
            DrawMiniMap();
            DrawDistanceMarker();
        }

        private void DrawDistanceMarker()
        {
            if (Main.mapFullscreen)
            {
                return;
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && modPlayer.SelectedLocation == travelRequirement.ID
                        && !QuestWorld.PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        PlayerInput.SetZoom_World();
                        var ScreenWidth = Main.screenWidth;
                        var ScreenHeight = Main.screenHeight;
                        var ScreenPosition = Main.screenPosition;
                        PlayerInput.SetZoom_UI();
                        var UIScale = Main.UIScale;

                        var LabelPosition = Main.fontMouseText.MeasureString(travelRequirement.ID);
                        var LabelPositionYNegative = 0f;
                        if (Main.LocalPlayer.chatOverhead.timeLeft > 0)
                        {
                            LabelPositionYNegative = -LabelPosition.Y;
                        }

                        var ScreenCenter = new Vector2(ScreenWidth / 2 + ScreenPosition.X, ScreenHeight / 2 + ScreenPosition.Y);
                        var TravelPosition = travelRequirement.location;
                        TravelPosition += (TravelPosition - ScreenCenter) * (Main.GameViewMatrix.Zoom - Vector2.One);

                        var distance2 = 0f;
                        var color = Color.White;
                        var DistanceX = TravelPosition.X - ScreenCenter.X;
                        var DistanceY = TravelPosition.Y - LabelPosition.Y - 2f + LabelPositionYNegative - ScreenCenter.Y;
                        var distance = (float)Math.Sqrt(DistanceX * DistanceX + DistanceY * DistanceY);

                        var ScreenHeight2 = ScreenHeight;
                        if (ScreenHeight > ScreenWidth)
                        {
                            ScreenHeight2 = ScreenWidth;
                        }

                        ScreenHeight2 = ScreenHeight2 / 2 - 30;
                        if (ScreenHeight2 < 100)
                        {
                            ScreenHeight2 = 100;
                        }

                        if (distance < ScreenHeight2)
                        {
                            LabelPosition.X = TravelPosition.X - LabelPosition.X / 2f - ScreenPosition.X;
                            LabelPosition.Y = TravelPosition.Y - LabelPosition.Y - 2f + LabelPositionYNegative - ScreenPosition.Y;
                        }
                        else
                        {
                            distance2 = distance;
                            distance = ScreenHeight2 / distance;
                            LabelPosition.X = ScreenWidth / 2 + DistanceX * distance - LabelPosition.X / 2f;
                            LabelPosition.Y = ScreenHeight / 2 + DistanceY * distance;
                        }

                        if (Main.LocalPlayer.gravDir == -1f)
                        {
                            LabelPosition.Y = ScreenHeight - LabelPosition.Y;
                        }

                        LabelPosition *= 1f / UIScale;
                        var LabelPosition2 = Main.fontMouseText.MeasureString(travelRequirement.ID);
                        LabelPosition += LabelPosition2 * (1f - UIScale) / 4f;
                        if (distance2 > 0f)
                        {
                            var DistanceText = Language.GetTextValue("GameUI.PlayerDistance", (int)(distance2 / 16f * 2f));
                            var DistanceTextPosition = Main.fontMouseText.MeasureString(DistanceText);
                            DistanceTextPosition.X = LabelPosition.X + LabelPosition2.X / 2f - DistanceTextPosition.X / 2f;
                            DistanceTextPosition.Y = LabelPosition.Y + LabelPosition2.Y / 2f - DistanceTextPosition.Y / 2f - 20f;

                            // Draws the black outline around the distance value from the marker
                            Main.spriteBatch.DrawString(Main.fontMouseText, DistanceText, new Vector2(DistanceTextPosition.X - 2f, DistanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(Main.fontMouseText, DistanceText, new Vector2(DistanceTextPosition.X + 2f, DistanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(Main.fontMouseText, DistanceText, new Vector2(DistanceTextPosition.X, DistanceTextPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(Main.fontMouseText, DistanceText, new Vector2(DistanceTextPosition.X, DistanceTextPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                            // Draw the actual distance value from the marker
                            Main.spriteBatch.DrawString(Main.fontMouseText, DistanceText, DistanceTextPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        }

                        // Draw the black outline around the text for the location
                        Main.spriteBatch.DrawString(Main.fontMouseText, travelRequirement.ID, new Vector2(LabelPosition.X - 2f, LabelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(Main.fontMouseText, travelRequirement.ID, new Vector2(LabelPosition.X + 2f, LabelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(Main.fontMouseText, travelRequirement.ID, new Vector2(LabelPosition.X, LabelPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(Main.fontMouseText, travelRequirement.ID, new Vector2(LabelPosition.X, LabelPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                        // Draw the actual text for the location
                        Main.spriteBatch.DrawString(Main.fontMouseText, travelRequirement.ID, LabelPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        private void DrawMiniMap()
        {
            if (Main.mapFullscreen || Main.mapStyle != 1) return;

            QuestMarker = ModContent.GetTexture(AssetDirectory.Textures + "QuestMarker");

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && !QuestWorld.PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        // Slightly rewritten gross vanilla code, I don't wanna bother with the variable names
                        var num145 = (Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f;
                        var num28 = (Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f;
                        var num14 = Main.miniMapWidth / Main.mapMinimapScale;
                        var num15 = Main.miniMapHeight / Main.mapMinimapScale;
                        var num12 = (int)num145 - num14 / 2f;
                        var num60 = ((travelRequirement.location.X / 16f) - num12) * Main.mapMinimapScale;
                        var num13 = (int)num28 - num15 / 2f;
                        var num61 = ((travelRequirement.location.Y / 16f) - num13) * Main.mapMinimapScale;
                        var num3 = (float)Main.miniMapX;
                        var num4 = (float)Main.miniMapY;
                        num60 += num3;
                        num61 += num4;
                        num61 -= 2f * Main.mapMinimapScale / 5f;
                        if (num60 > Main.miniMapX + 12 && num60 < Main.miniMapX + Main.miniMapWidth - 16 &&
                            num61 > Main.miniMapY + 10 && num61 < Main.miniMapY + Main.miniMapHeight - 14)
                        {
                            var num10 = -(num145 - (int)((Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f)) * Main.mapMinimapScale;
                            var num11 = -(num28 - (int)((Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f)) * Main.mapMinimapScale;
                            var scale = (Main.mapMinimapScale * 0.25f * 2f + 1f) / 3f;
                            Main.spriteBatch.Draw(QuestMarker, new Vector2(num60 + num10, num61 + num11), null, Color.White, 0, QuestMarker.Size() / 2, scale, SpriteEffects.None, 1f);
                            var num62 = num60 - QuestMarker.Width / 2 * scale;
                            var num63 = num61 - QuestMarker.Height / 2 * scale;
                            var num64 = num62 + QuestMarker.Width * scale;
                            var num65 = num63 + QuestMarker.Height * scale;

                            if (Main.mouseX >= num62 && Main.mouseX <= num64 && Main.mouseY >= num63 && Main.mouseY <= num65)
                            {
                                Main.instance.MouseText(travelRequirement.ID);
                            }
                        }
                    }
                }
            }
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
            Vector2 textSize = Main.fontDeathText.MeasureString(BossName);
            Vector2 textSize2 = Main.fontDeathText.MeasureString(BossTitle) * 0.5f;
            float textPositionLeft = (Main.screenWidth / 2) - textSize.X / 2f;
            float text2PositionLeft = (Main.screenWidth / 2) - textSize2.X / 2f;
            /*float alpha = 255;
			float alpha2 = 255;*/

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossTitle, new Vector2(text2PositionLeft, (Main.screenHeight / 2 - 250)), titleColor, 0f, Vector2.Zero, 0.6f, 0, 0f);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossName, new Vector2(textPositionLeft, (Main.screenHeight / 2 - 300)), nameColor, 0f, Vector2.Zero, 1f, 0, 0f);
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
    }
}