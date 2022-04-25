using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Quests.Requirements;
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
        public static List<string> PlayerTraveled = new List<string>();

        public override void PreUpdateNPCs()
        {
            if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.talkNPC == -1) Quests.ResetUI();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["globalCompletedQuests"] = Quests.GlobalCompletedQuests.ToList();
            tag["perPlayerCompletedQuestsKeys"] = Quests.PerPlayerCompletedQuests.Keys.ToList();
            tag["perPlayerCompletedQuestsValues"] = Quests.PerPlayerCompletedQuests.Values.Select(v => v.ToList()).ToList();
            tag["perPlayerActiveQuestsKeys"] = Quests.PerPlayerActiveQuests.Keys.ToList();
            tag["perPlayerActiveQuestsValues"] = Quests.PerPlayerActiveQuests.Values
                    .Select(v => v.Select(q => q.QuestID).ToList()).ToList();
            tag["PlayerTraveled"] = PlayerTraveled;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Quests.GlobalCompletedQuests.Clear();
            var quests = tag.GetList<string>("globalCompletedQuests");

            var validGlobals = quests.Where(qid => Quests.QuestList.TryGetValue(qid, out var quest)
                && quest.Repeatability == QuestRepeatability.OncePerWorld);
            foreach (var q in validGlobals) Quests.GlobalCompletedQuests.Add(q);

            foreach (var kvp in Quests.PerPlayerCompletedQuests) kvp.Value.Clear();

            var keys = tag.GetList<string>("perPlayerCompletedQuestsKeys");
            var values = tag.GetList<List<string>>("perPlayerCompletedQuestsValues");
            foreach (var pair in keys.Zip(values, (k, v) => (k, v)))
            {
                var valid = pair.v.Where(qid => Quests.QuestList.TryGetValue(qid, out var quest)
                    && quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer);

                Quests.PerPlayerCompletedQuests[pair.k] = new HashSet<string>(valid);
            }

            foreach (var kvp in Quests.PerPlayerActiveQuests) kvp.Value.Clear();

            var activeKeys = tag.GetList<string>("perPlayerActiveQuestsKeys");
            var activeValues = tag.GetList<List<string>>("perPlayerActiveQuestsValues");
            foreach (var pair in activeKeys.Zip(activeValues, (k, v) => (k, v)))
            {
                var qlist = pair.v.Select(qid => Quests.QuestList.TryGetValue(qid, out var quest) ? quest : null)
                    .Where(q => q != null
                        && (q.Repeatability == QuestRepeatability.OncePerWorld
                        || q.Repeatability == QuestRepeatability.OncePerWorldPerPlayer))
                    .ToList();
                Quests.PerPlayerActiveQuests[pair.k] = qlist;
            }

            // Stores the places the player has traveled in order to persist the Travel quest data
            var TraveledList = tag.GetList<string>("PlayerTraveled");
            foreach (var Location in TraveledList)
            {
                PlayerTraveled.Add(Location);
            }
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
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && modPlayer.SelectedLocation == travelRequirement.ID
                        && !PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        PlayerInput.SetZoom_World();
                        var screenWidth = Main.screenWidth;
                        var screenHeight = Main.screenHeight;
                        var screenPosition = Main.screenPosition;
                        PlayerInput.SetZoom_UI();
                        var uiScale = Main.UIScale;
                        
                        var labelPosition = FontAssets.MouseText.Value.MeasureString(travelRequirement.ID);
                        var labelPositionYNegative = 0f;
                        if (Main.LocalPlayer.chatOverhead.timeLeft > 0)
                        {
                            labelPositionYNegative = -labelPosition.Y;
                        }

                        var screenCenter = new Vector2(screenWidth / 2 + screenPosition.X, screenHeight / 2 + screenPosition.Y);
                        var travelPosition = travelRequirement.Location * 16f;
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
                        var LabelPosition2 = FontAssets.MouseText.Value.MeasureString(travelRequirement.ID);
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
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(labelPosition.X - 2f, labelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(labelPosition.X + 2f, labelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(labelPosition.X, labelPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(labelPosition.X, labelPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                        // Draw the actual text for the location
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, labelPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
