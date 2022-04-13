using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public BaseQuest GetCurrentQuest(NPC npc)
        {
            // Active quests are per-player, not really per NPC.
            var currentModPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            return currentModPlayer.QuestByNpc(npc.type);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            var quest = GetCurrentQuest(npc);
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
