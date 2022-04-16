using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private BaseQuest availableQuest;

        private int questCheckTick = 300;

        private BaseQuest GetCurrentAvailableQuest(NPC npc)
        {
            if (availableQuest != null) return availableQuest;
            if (questCheckTick > 0)
            {
                questCheckTick--;
                if (questCheckTick <= 0) questCheckTick = 0;
                return null;
            }
            //questCheckTick++;

            var possibleQuests = Quests.QuestList.Values
                .Where(q => q.IsValidQuest(npc.type, Main.LocalPlayer))
                .GroupBy(q => q.Priority)
                .Max()
                ?.ToList();
            if (possibleQuests == null || !possibleQuests.Any()) return null;

            availableQuest = possibleQuests[Main.rand.Next(0, possibleQuests.Count - 1)];

            return availableQuest;
        }

        /// <summary>
        /// Get quest the active player is pursuing, invoked on the client.
        /// If the player is not pursuing any from this NPC, lock one in at random.
        /// </summary>
        public BaseQuest GetCurrentQuest(NPC npc, out bool isDoing)
        {
            if (Main.netMode == NetmodeID.Server) throw new ArgumentException("GetCurrentQuest invoked on the server is invalid");
            isDoing = true;
            var currentModPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            var pursuedQuest = currentModPlayer.QuestByNpc(npc.type);
            if (pursuedQuest != null) return pursuedQuest;
            isDoing = false;
            return GetCurrentAvailableQuest(npc);
        }

        // Sets the delay between Quests after the player turns them in
        public void SetDelay()
        {

        }

        public void TakeQuest()
        {
            // Set the delay between Quests based on the Quest
            switch (availableQuest.QuestName)
            {
                case "Tutorial 2":
                    questCheckTick = 1200;
                    break;
                default:
                    questCheckTick = 600;
                    break;
            }

            availableQuest = null;
        }

        private int frame = 0;
        private int frameCounter = 0;
        private int lerpTimer = 0;
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            bool isDoing = false;
            var quest = GetCurrentQuest(npc, out isDoing);

            // Show the alert icon if the player is not doing a Quest from the NPC and the NPC has a Quest
            if (quest != null && !isDoing)
            {
                #region Quest Alert Frames
                if (!Main.gamePaused)
                {
                    if (frameCounter++ >= 30)
                    {
                        frame++;
                        frameCounter = 0;
                    }

                    switch (quest.Type)
                    {
                        case QuestType.Fetch:
                            if (frame >= 6 || frame <= 4)
                            {
                                frame = 4;
                            }
                            break;
                        case QuestType.Housing:
                            if (frame >= 2)
                            {
                                frame = 0;
                            }
                            break;
                    }

                    lerpTimer++;
                }
                #endregion

                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Quests/QuestAlert");
                Rectangle drawRectangle = new Rectangle(0, texture.Height / 6 * frame, texture.Width, texture.Height / 6);
                float yOffset = MathHelper.Lerp(48, 52, (float)Math.Sin(lerpTimer / 15f));

                spriteBatch.Draw(
                    texture,
                    new Vector2(npc.Center.X, npc.Center.Y - yOffset) - Main.screenPosition,
                    drawRectangle,
                    Color.White,
                    npc.rotation,
                    new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
            }

            base.PostDraw(npc, spriteBatch, drawColor);
        }
    }
}
