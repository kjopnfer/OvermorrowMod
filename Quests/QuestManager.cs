using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    // Couldn't extend Mod, can only have one class extending mod :pepehands:
    public static class QuestManager
    {
        public static List<Quest> ActiveQuests = new List<Quest>();
        public static List<Quest> QuestList = new List<Quest>();
        public static void Load()
        {
            foreach (Type type in OvermorrowModFile.Mod.Code.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Quest)) && !type.IsAbstract && type != typeof(Quest))
                {
                    Quest quest = (Quest)Activator.CreateInstance(type);
                    QuestList.Add(quest);
                }
            }

        }

        public static void Unload()
        {
            ActiveQuests = null;
            QuestList = null;
        }
    }
}