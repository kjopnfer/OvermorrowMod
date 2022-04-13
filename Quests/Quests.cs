using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public static class Quests
    {
        public static List<BaseQuest> QuestList = new List<BaseQuest>();
        public static List<int> CompletedQuests = new List<int>();

        public static void Load(OvermorrowModFile mod)
        {
            foreach (Type type in mod.Code.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BaseQuest)) && !type.IsAbstract && type != typeof(BaseQuest))
                {
                    BaseQuest quest = (BaseQuest)Activator.CreateInstance(type);
                    quest.SetDefaults();
                    QuestList.Add(quest);
                }
            }
            On.Terraria.Main.DrawNPCChatButtons += Main_DrawNPCChatButtons;
        }

        private static void Main_DrawNPCChatButtons(
            On.Terraria.Main.orig_DrawNPCChatButtons orig,
            int superColor,
            Microsoft.Xna.Framework.Color chatColor,
            int numLines,
            string focusText,
            string focusText3)
        {
            if (Main.LocalPlayer.talkNPC == -1) return;

            NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
            Player player = Main.LocalPlayer;
            QuestPlayer modPlayer = player.GetModPlayer<QuestPlayer>();
            BaseQuest CurrentQuest = npc.GetGlobalNPC<QuestNPC>().CurrentQuest;
        }

        public static void Unload()
        {
            QuestList.Clear();
        }
    }
}
