using System;
using System.Collections.Generic;

namespace OvermorrowMod.Quests
{
    public static class QuestLoader
    {
        public static Dictionary<Type, int> QuestTypes;
        //public static Dictionary<int, Type> Quests;
        public static Dictionary<int, Quest> QuestList = new Dictionary<int, Quest>();
        /*public static void TryRegisteringQuest(Type type)
        {
            Type baseType = typeof(Quest);
            if (!type.IsAbstract && type.IsSubclassOf(baseType) && type != baseType)
            {
                int id = QuestTypes.Count;
                QuestTypes.Add(type, id);
                Quests.Add(id, type);
            }
        }*/

        public static void TryRegisteringQuest(Type type)
        {
            Type baseType = typeof(Quest);
            if (type.IsSubclassOf(typeof(Quest)) && !type.IsAbstract && type != baseType)
            {
                int id = QuestList.Count;
                Quest quest = (Quest)Activator.CreateInstance(type);

                QuestList.Add(id, quest);
            }
        }


        /*public static Quest CreateQuest(int type)
        {
            Quest quest = (Quest)Activator.CreateInstance(Quests[type]);
            quest.SetDefaults();
            return quest;
        }*/

        public static int QuestType<T>() where T : Quest
        {
            return QuestTypes[typeof(T)];
        }

        public static void Load() {}
        public static void Unload() {}
    }
}