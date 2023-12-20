using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
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
                XmlDocument doc = new XmlDocument();

                string text;
                #region Dialogue Overrides

                // TODO: This is dogshit someone fix this
                if (npc.type == NPCID.Guide)
                {
                    if (questPlayer.HasCompletedQuest<GuideCampfire>())
                    {
                        orig(self);
                        return;
                    }

                    //text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Common/Cutscenes/Dialogue/GuideIntro.xml"));
                    text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(AssetDirectory.Popups + "GuideCamp.xml"));
                    doc.LoadXml(text);

                    player.SetDialogue(npc.GetChat(), 20, doc);
                }
                /*else if (npc.type == ModContent.NPCType<Feyden_Bound>())
                {
                    // Check if the handler is active to prevent the player from interacting with this NPC
                    bool handlerActive = false;
                    foreach (Projectile projectile in Main.projectile)
                    {
                        if (projectile.active && projectile.type == ModContent.ProjectileType<FeydenCaveHandler>()) handlerActive = true;
                    }

                    if (handlerActive) return;
                    text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/FeydenBound.xml"));

                    doc.LoadXml(text);
                    player.SetDialogue(npc.GetChat(), 20, doc);
                }*/
                else if (npc.type == ModContent.NPCType<Feyden>())
                {
                    if (questPlayer.FindActiveQuest("FeydenEscort"))
                    {
                        text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(AssetDirectory.DialogWindow + "FeydenEscort.xml"));
                        doc.LoadXml(text);
                        player.SetDialogue(npc.GetChat(), 20, doc);

                        return;
                    }
                    
                    text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(AssetDirectory.DialogWindow + "FeydenFree.xml"));
                    doc.LoadXml(text);
                    player.SetDialogue(npc.GetChat(), 20, doc);
                }
                /*else if (npc.type == NPCID.Merchant)
                {
                    text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Common/Cutscenes/Dialogue/MerchantTest.xml"));
                    doc.LoadXml(text);

                    player.SetDialogue(texture, npc.GetChat(), 20, doc);
                }*/
                        /*else if (npc.type == ModContent.NPCType<TownKid>())
                        {
                            text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Common/Cutscenes/Dialogue/TownKid.xml"));
                            doc.LoadXml(text);

                            texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Full/dog", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            player.SetDialogue(texture, npc.GetChat(), 20, doc);
                        }
                        else if (npc.type == ModContent.NPCType<SojournGuard>())
                        {
                            text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/SojournGuard.xml"));
                            doc.LoadXml(text);

                            texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Full/dog", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            player.SetDialogue(texture, npc.GetChat(), 20, doc);
                        }
                        else if (npc.type == ModContent.NPCType<SojournGuard2>())
                        {
                            text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/SojournGuard2.xml"));
                            doc.LoadXml(text);

                            texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Full/dog", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            player.SetDialogue(texture, npc.GetChat(), 20, doc);
                        }
                        else if (npc.type == ModContent.NPCType<SojournGuard3>())
                        {
                            text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/SojournGuard3.xml"));

                            // If the player has interacted with Moxley and has spoken to this NPC the first time
                            if (player.DialogueFlags.Contains("SojournGuard_4") && player.kittFirst)
                            {
                                text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/SojournGuard3_3.xml"));
                            }
                            else
                            {
                                // If you finish 'Anything to Do?' and 'Visitors' AND talk to Moxley, load the second version
                                if (player.DialogueFlags.Contains("SojournGuard_4") && player.DialogueFlags.Contains("SojournGuard3_4") && player.DialogueFlags.Contains("SojournGuard3_5"))
                                {
                                    text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Content/UI/Dialogue/SojournGuard3_2.xml"));
                                }
                            }

                            // The player has spoken to this NPC once already
                            if (player.kittFirst) player.kittFirst = false;

                            doc.LoadXml(text);

                            texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Full/dog", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            player.SetDialogue(texture, npc.GetChat(), 20, doc);
                        }*/
                else
                {
                    orig(self);
                }

                #endregion
            }

            //orig(self);
        }
    }
}