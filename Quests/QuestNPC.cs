using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private BaseQuest availableQuest;

        private int questCheckTick = 0;

        private BaseQuest GetCurrentAvailableQuest(NPC npc)
        {
            if (availableQuest != null) return availableQuest;
            if (questCheckTick > 0)
            {
                questCheckTick++;
                if (questCheckTick >= 3000) questCheckTick = 0;
                return null;
            }
            questCheckTick++;

            var possibleQuests = Quests.QuestList.Values.Where(q => q.IsValidQuest(npc.type, Main.LocalPlayer)).ToList();
            if (!possibleQuests.Any()) return null;

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

        public void TakeQuest()
        {
            availableQuest = null;
            questCheckTick = 0;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            var quest = GetCurrentQuest(npc, out _);
            if (quest != null)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Quests/QuestAlert");
                Rectangle drawRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                spriteBatch.Draw(
                    texture,
                    new Vector2(npc.Center.X, npc.Center.Y - 80) - Main.screenPosition,
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
