using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Quests.State;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Quests
{
    public static class Quests
    {
        public static Dictionary<string, BaseQuest> QuestList = new Dictionary<string, BaseQuest>();

        private static Dictionary<Type, BaseQuest> QuestTypes = new Dictionary<Type, BaseQuest>();

        public static QuestsState State { get; } = new QuestsState();


        private static bool hoverButton = false;
        private static bool nextButton = false;
        private static BaseQuest endDialogueQuest = null;
        private static int dialogueCounter = 0;

        public static void Load(OvermorrowModFile mod)
        {
            foreach (Type type in mod.Code.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BaseQuest)) && !type.IsAbstract && type != typeof(BaseQuest))
                {
                    BaseQuest quest = (BaseQuest)Activator.CreateInstance(type);
                    quest.SetDefaults();
                    QuestList.Add(quest.QuestID, quest);
                    QuestTypes.Add(type, quest);
                }
            }
            On.Terraria.Main.DrawNPCChatButtons += Main_DrawNPCChatButtons;
        }

        public static void ClearAllCompletedQuests()
        {
            State.Reset();
        }

        public static void ResetUI()
        {
            hoverButton = false;
            nextButton = false;
            endDialogueQuest = null;
            dialogueCounter = 0;
        }

        private static string GetQuestButtonText(BaseQuest quest, bool isDoing, Player player)
        {
            if (nextButton && dialogueCounter >= quest.DialogueCount) return "Accept";
            else if (nextButton) return "Next";

            if (isDoing)
            {
                return quest.CheckRequirements(player) ? "Turn In" : "Quest";
            }
            return "Quest";
        }

        private static void HandleButtonClick(
            bool isDoing,
            BaseQuest quest,
            NPC npc,
            Player player,
            QuestPlayer questPlayer,
            QuestNPC questNpc)
        {
            if (isDoing)
            {
                if (quest.CheckRequirements(player))
                {
                    quest.CompleteQuest(player, true);
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("OvermorrowMod/Sounds/QuestTurnIn"), npc.Center);

                    if (quest.EndDialogueCount > 0)
                    {
                        endDialogueQuest = quest;
                        dialogueCounter = 0;
                        nextButton = true;
                        Main.npcChatText = endDialogueQuest.GetEndDialogue(dialogueCounter++);
                    }
                    else
                    {
                        player.SetTalkNPC(-1);
                        ResetUI();
                    }
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatText = quest.GetHint(Main.rand.Next(0, quest.HintCount - 1));
                }
                return;
            }

            if (endDialogueQuest != null)
            {
                if (dialogueCounter >= endDialogueQuest.EndDialogueCount)
                {
                    player.SetTalkNPC(-1);
                    ResetUI();
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatText = endDialogueQuest.GetEndDialogue(dialogueCounter++);
                }
                return;
            }


            nextButton = true;
            if (dialogueCounter >= quest.DialogueCount)
            {
                // Accept button pressed
                questPlayer.AddQuest(quest);
                questNpc.TakeQuest();

                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("OvermorrowMod/Sounds/QuestAccept"), npc.Center);

                // Run the Quest Accepted UI
                Main.NewText("ACCEPTED QUEST: " + quest.QuestName, Color.Yellow);

                player.SetTalkNPC(-1);
                ResetUI();
            }
            else
            {
                // Next button pressed
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.npcChatText = quest.GetDialogue(dialogueCounter++);
            }
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
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var questNpc = npc.GetGlobalNPC<QuestNPC>();
            bool isDoing = false;
            var quest = endDialogueQuest ?? questNpc.GetCurrentQuest(npc, out isDoing);
            if (quest == null)
            {
                orig(superColor, chatColor, numLines, focusText, focusText3);
                return;
            }

            var text = GetQuestButtonText(quest, isDoing, player);
            // In this case there is no quest available or active, so we don't need to draw any buttons at all.

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Color textColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
            Vector2 textScale = new Vector2(0.9f);
            Vector2 stringSize = ChatManager.GetStringSize(font, text, textScale);
            Vector2 textPosition = new Vector2((180 + Main.screenWidth / 2) + stringSize.X - 50f, 130 + numLines * 30);
            Vector2 mouseCursor = new Vector2(Main.mouseX, Main.mouseY);

            if (mouseCursor.Between(textPosition, textPosition + stringSize * textScale) && !PlayerInput.IgnoreMouseInterface)
            {
                player.mouseInterface = true;
                player.releaseUseItem = true;

                if (!hoverButton)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    hoverButton = true;
                }

                textScale *= 1.1f;

                // Button pushed
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    HandleButtonClick(isDoing, quest, npc, player, questPlayer, questNpc);
                }
            }
            else
            {
                // Plays the sound once
                if (hoverButton)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    hoverButton = false;
                }
            }
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, textPosition + new Vector2(16f, 14f), textColor, 0f,
                      stringSize * 0.5f, textScale);

            orig(superColor, chatColor, numLines, focusText, focusText3);
        }

        public static void Unload()
        {
            QuestList.Clear();
            QuestTypes.Clear();
        }

        // utils

        public static BaseQuest GetQuest<T>() where T : BaseQuest
        {
            return QuestTypes[typeof(T)];
        }

        public static bool HasCompletedQuest<T>(Player player) where T : BaseQuest
        {
            var modPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = GetQuest<T>();
            return quest.Repeatability == QuestRepeatability.Repeatable
                || State.HasCompletedQuest(modPlayer, quest);
        }
    }
}
