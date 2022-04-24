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

        private Texture2D questMarker;
        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            if (questMarker == null || questMarker.IsDisposed)
            {
                questMarker = ModContent.Request<Texture2D>(AssetDirectory.Textures + "QuestMarker").Value;
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && !PlayerTraveled.Contains(travelRequirement.ID))
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

                            Main.spriteBatch.Draw(questMarker, DrawCoordinates, null, Color.White, 0, questMarker.Size() / 2, 1.08f, SpriteEffects.None, 1);

                            if (Main.mouseLeft)
                            {
                                SoundEngine.PlaySound(SoundID.Item20, Main.LocalPlayer.position);
                                modPlayer.SelectedLocation = travelRequirement.ID;
                                Main.mapFullscreen = false;
                                SoundEngine.PlaySound(SoundID.Item20, travelRequirement.location * 16);
                            }
                        }
                        else
                        {
                            Main.spriteBatch.Draw(questMarker, DrawCoordinates, null, Color.White, 0, questMarker.Size() / 2, 1f, SpriteEffects.None, 1);
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
                        && !PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        PlayerInput.SetZoom_World();
                        var ScreenWidth = Main.screenWidth;
                        var ScreenHeight = Main.screenHeight;
                        var ScreenPosition = Main.screenPosition;
                        PlayerInput.SetZoom_UI();
                        var UIScale = Main.UIScale;
                        
                        var LabelPosition = FontAssets.MouseText.Value.MeasureString(travelRequirement.ID);
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
                        var LabelPosition2 = FontAssets.MouseText.Value.MeasureString(travelRequirement.ID);
                        LabelPosition += LabelPosition2 * (1f - UIScale) / 4f;
                        if (distance2 > 0f)
                        {
                            var DistanceText = Language.GetTextValue("GameUI.PlayerDistance", (int)(distance2 / 16f * 2f));
                            var DistanceTextPosition = FontAssets.MouseText.Value.MeasureString(DistanceText);
                            DistanceTextPosition.X = LabelPosition.X + LabelPosition2.X / 2f - DistanceTextPosition.X / 2f;
                            DistanceTextPosition.Y = LabelPosition.Y + LabelPosition2.Y / 2f - DistanceTextPosition.Y / 2f - 20f;

                            // Draws the black outline around the distance value from the marker
                            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, DistanceText, new Vector2(DistanceTextPosition.X - 2f, DistanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, DistanceText, new Vector2(DistanceTextPosition.X + 2f, DistanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, DistanceText, new Vector2(DistanceTextPosition.X, DistanceTextPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, DistanceText, new Vector2(DistanceTextPosition.X, DistanceTextPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                            // Draw the actual distance value from the marker
                            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, DistanceText, DistanceTextPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        }

                        // Draw the black outline around the text for the location
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(LabelPosition.X - 2f, LabelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(LabelPosition.X + 2f, LabelPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(LabelPosition.X, LabelPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, new Vector2(LabelPosition.X, LabelPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

                        // Draw the actual text for the location
                        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, travelRequirement.ID, LabelPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        private void DrawMiniMap()
        {
            if (Main.mapFullscreen || Main.mapStyle != 1) return;

            if (questMarker == null || questMarker.IsDisposed) questMarker = ModContent.Request<Texture2D>(AssetDirectory.Textures + "QuestMarker").Value;

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    if (requirement is TravelRequirement travelRequirement && !PlayerTraveled.Contains(travelRequirement.ID))
                    {



                        // Slightly rewritten gross vanilla code, I don't wanna bother with the variable names
                        var centerScreenX = (Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f;
                        var centerScreenY = (Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f;
                        var mapRealSizeX = Main.miniMapWidth / Main.mapMinimapScale;
                        var mapRealSizeY = Main.miniMapHeight / Main.mapMinimapScale;
                        var num12 = (int)centerScreenX - mapRealSizeX / 2f;
                        var num60 = ((travelRequirement.location.X / 16f) - num12) * Main.mapMinimapScale;
                        var num13 = (int)centerScreenY - mapRealSizeY / 2f;
                        var num61 = ((travelRequirement.location.Y / 16f) - num13) * Main.mapMinimapScale;
                        var num3 = (float)Main.miniMapX;
                        var num4 = (float)Main.miniMapY;
                        num60 += num3;
                        num61 += num4;
                        num61 -= 2f * Main.mapMinimapScale / 5f;
                        if (num60 > Main.miniMapX + 12 && num60 < Main.miniMapX + Main.miniMapWidth - 16 &&
                            num61 > Main.miniMapY + 10 && num61 < Main.miniMapY + Main.miniMapHeight - 14)
                        {
                            var num10 = -(centerScreenX - (int)((Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f)) * Main.mapMinimapScale;
                            var num11 = -(centerScreenY - (int)((Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f)) * Main.mapMinimapScale;
                            var scale = (Main.mapMinimapScale * 0.25f * 2f + 1f) / 3f;
                            Main.spriteBatch.Draw(questMarker, new Vector2(num60 + num10, num61 + num11), null, Color.White, 0, questMarker.Size() / 2, scale, SpriteEffects.None, 1f);
                            var num62 = num60 - questMarker.Width / 2 * scale;
                            var num63 = num61 - questMarker.Height / 2 * scale;
                            var num64 = num62 + questMarker.Width * scale;
                            var num65 = num63 + questMarker.Height * scale;

                            if (Main.mouseX >= num62 && Main.mouseX <= num64 && Main.mouseY >= num63 && Main.mouseY <= num65)
                            {
                                Main.instance.MouseText(travelRequirement.ID);
                            }
                        }
                    }
                }
            }
        }
    }
}
