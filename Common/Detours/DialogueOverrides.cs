using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Dialogue;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.Projectiles;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.ModQuests;
using System.Linq;
using System.Xml;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Detours
{
    public class DialogueOverrides
    {
        public static void GUIChatDrawInner(Terraria.On_Main.orig_GUIChatDrawInner orig, Main self)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            if (player.GetDialogue() == null && Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];

                if (DialogueManagers.NPCDialogueManagers.ContainsKey(npc.type))
                {
                    DialogueManager manager = DialogueManagers.NPCDialogueManagers[npc.type];
                    player.SetDialogue(npc.GetChat(), 20, manager.GetDialogueWindow());
                }
                else
                {
                    orig(self);
                }
            }
        }
    }
}