using System;
using System.Collections.Generic;
using System.Xml;

namespace OvermorrowMod.Common.Dialogue
{
    public class DialogueManagers
    {
        public static Dictionary<Type, string> DialogueManagerTypes;
        public static Dictionary<int, DialogueManager> NPCDialogueManagers;
        public static string ObjectType<T>() where T : DialogueManager => DialogueManagerTypes[typeof(T)];
        public static void Load()
        {
            DialogueManagerTypes = new Dictionary<Type, string>();
            NPCDialogueManagers = new Dictionary<int, DialogueManager>();

        }

        public static void Unload()
        {
            DialogueManagerTypes = null;
            NPCDialogueManagers = null;
        }

        public static void RegisterDialogueManagers(Type type)
        {
            Type baseType = typeof(DialogueManager);
            if (type.IsSubclassOf(baseType) && !type.IsAbstract && type != baseType)
            {
                string id = type.Name;
                DialogueManagerTypes.Add(type, id);

                DialogueManager dialogueManager = (DialogueManager)Activator.CreateInstance(type);
                NPCDialogueManagers.Add(dialogueManager.NPC, dialogueManager);
            }
        }
    }

    public abstract class DialogueManager
    {
        public abstract int NPC { get; }
        public abstract XmlDocument GetDialogueWindow();
    }
}