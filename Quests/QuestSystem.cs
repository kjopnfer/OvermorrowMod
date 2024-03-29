﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace OvermorrowMod.Quests
{
    public class QuestSystem : ModSystem
    {
        public override void PreUpdateNPCs()
        {
            if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.talkNPC == -1) Quests.ResetUI();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var toSave = Quests.State.GetPerPlayerQuestsForWorld();
            tag["questStatesKeys"] = toSave.Keys.ToList();
            tag["questStatesValues"] = toSave.Values.Select(lst => lst.ToList()).ToList();
            tag["globalCompletedQuests"] = Quests.State.GetWorldQuestsToSave().ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Quests.State.Reset();

            var globalCompleted = tag.GetList<string>("globalCompletedQuests");
            var questStateKeys = tag.GetList<string>("questStatesKeys");
            var questStateValues = tag.GetList<List<TagCompound>>("questStatesValues");

            var questStates = questStateKeys.Zip(questStateValues).ToDictionary(pair => pair.First, pair => (IEnumerable<TagCompound>)pair.Second);

            Quests.State.LoadWorld(questStates, globalCompleted);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            DrawDistanceMarker();
        }

        private void DrawDistanceMarker()
        {
            if (Main.mapFullscreen)
            {
                return;
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            foreach (var (_, req) in Quests.State.GetActiveRequirementsOfType<TravelRequirementState>(modPlayer))
            {
                if (modPlayer.SelectedLocation != req.Requirement.ID || req.IsCompleted) continue;

                var requirement = req.Requirement as TravelRequirement;

                PlayerInput.SetZoom_World();
                var screenWidth = Main.screenWidth;
                var screenHeight = Main.screenHeight;
                var screenPosition = Main.screenPosition;
                PlayerInput.SetZoom_UI();
                var uiScale = Main.UIScale;

                var labelPosition = FontAssets.MouseText.Value.MeasureString(requirement.displayName);
                var labelPositionYNegative = 0f;
                if (Main.LocalPlayer.chatOverhead.timeLeft > 0)
                {
                    labelPositionYNegative = -labelPosition.Y;
                }

                var screenCenter = new Vector2(screenWidth / 2 + screenPosition.X, screenHeight / 2 + screenPosition.Y);
                var travelPosition = requirement.Location;
                travelPosition += (travelPosition - screenCenter) * (Main.GameViewMatrix.Zoom - Vector2.One);

                var distance2 = 0f;
                var color = Color.White;
                var distanceX = travelPosition.X - screenCenter.X;
                var distanceY = travelPosition.Y - labelPosition.Y - 2f + labelPositionYNegative - screenCenter.Y;
                var distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                var screenHeight2 = screenHeight;
                if (screenHeight > screenWidth)
                {
                    screenHeight2 = screenWidth;
                }

                screenHeight2 = screenHeight2 / 2 - 30;
                if (screenHeight2 < 100)
                {
                    screenHeight2 = 100;
                }

                if (distance < screenHeight2)
                {
                    labelPosition.X = travelPosition.X - labelPosition.X / 2f - screenPosition.X;
                    labelPosition.Y = travelPosition.Y - labelPosition.Y - 2f + labelPositionYNegative - screenPosition.Y;
                }
                else
                {
                    distance2 = distance;
                    distance = screenHeight2 / distance;
                    labelPosition.X = screenWidth / 2 + distanceX * distance - labelPosition.X / 2f;
                    labelPosition.Y = screenHeight / 2 + distanceY * distance;
                }

                if (Main.LocalPlayer.gravDir == -1f)
                {
                    labelPosition.Y = screenHeight - labelPosition.Y;
                }

                labelPosition *= 1f / uiScale;
                var LabelPosition2 = FontAssets.MouseText.Value.MeasureString(requirement.displayName);
                labelPosition += LabelPosition2 * (1f - uiScale) / 4f;
                if (distance2 > 0f)
                {
                    var distanceText = Language.GetTextValue("GameUI.PlayerDistance", (int)(distance2 / 16f * 2f));
                    var distanceTextPosition = FontAssets.MouseText.Value.MeasureString(distanceText);
                    distanceTextPosition.X = labelPosition.X + LabelPosition2.X / 2f - distanceTextPosition.X / 2f;
                    distanceTextPosition.Y = labelPosition.Y + LabelPosition2.Y / 2f - distanceTextPosition.Y / 2f - 20f;

                    // Draws the black outline around the distance value from the marker
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, distanceText, new Vector2(distanceTextPosition.X - 2f, distanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, distanceText, new Vector2(distanceTextPosition.X + 2f, distanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, distanceText, new Vector2(distanceTextPosition.X, distanceTextPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, distanceText, new Vector2(distanceTextPosition.X, distanceTextPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                    // Draw the actual distance value from the marker
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, distanceText, distanceTextPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                }

                // Draw the black outline around the text for the location
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, requirement.displayName, new Vector2(labelPosition.X - 2f, labelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, requirement.displayName, new Vector2(labelPosition.X + 2f, labelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, requirement.displayName, new Vector2(labelPosition.X, labelPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, requirement.displayName, new Vector2(labelPosition.X, labelPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                // Draw the actual text for the location
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, requirement.displayName, labelPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                Texture2D arrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "DirectionArrow").Value;
                float rotation = modPlayer.Player.DirectionTo(travelPosition).ToRotation() + MathHelper.PiOver2;
                Main.spriteBatch.Draw(arrow, labelPosition + new Vector2(arrow.Width + 6, 32), null, Color.White, rotation, arrow.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
