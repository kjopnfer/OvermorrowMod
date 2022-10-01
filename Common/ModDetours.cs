using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public static class ModDetours
    {
        public static void Load()
        {
            //On.Terraria.Main.DrawNPCChatButtons += DrawNPCChatButtons;
            On.Terraria.Player.Update_NPCCollision += UpdateNPCCollision;
            On.Terraria.Player.SlopingCollision += PlatformCollision;
            On.Terraria.Main.DrawInterface += DrawParticles;
            On.Terraria.Main.DrawDust += DrawOverlay;
            On.Terraria.Main.GUIChatDrawInner += GUIChatDrawInner;
        }

        public static void Unload()
        {
            //On.Terraria.Main.DrawNPCChatButtons -= DrawNPCChatButtons;
            On.Terraria.Player.Update_NPCCollision -= UpdateNPCCollision;
            On.Terraria.Player.SlopingCollision -= PlatformCollision;
            On.Terraria.Main.DrawInterface -= DrawParticles;
            On.Terraria.Main.DrawDust -= DrawOverlay;
            On.Terraria.Main.GUIChatDrawInner -= GUIChatDrawInner;
        }

        static void GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            if (!player.AddedDialogue && Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Full/Guide/Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                Main.NewText("blocked");

                XmlDocument doc = new XmlDocument();
                OvermorrowModFile.Instance.Logger.Debug("File Names: " + string.Join(", ", OvermorrowModFile.Instance.GetFileNames()));
                string text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes("Common/Cutscenes/Dialogue/test.xml"));
                doc.LoadXml(text);

                var dialogue = doc.GetElementsByTagName("DialogueNode");
                XmlNode starting = null;
                foreach (XmlNode node in dialogue)
                {
                    if (node.Attributes["id"].Value == "start")
                    {
                        starting = node;
                    }
                }

                var children = starting.ChildNodes;
                foreach (XmlNode child in children)
                {
                    if (child.Name == "Message") Main.NewText(child.InnerText);

                    /*if (child.Name == "options")
                    {
                        foreach (XmlNode option in child.ChildNodes)
                        {
                            Console.WriteLine(option.InnerText);
                        }
                    }*/
                    //Console.WriteLine(child.Name);
                    //Console.WriteLine(child.Name + " : " + child.InnerText);
                }

                player.SetDialogue(texture, npc.GetChat(), 20, Color.White);
                player.AddedDialogue = true;
            }

            //orig(self);
        }

        // Test for blurred overlay
        private static void DrawOverlay(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            var effect = OvermorrowModFile.Instance.RadialBlur.Value;
            var texture = OvermorrowModFile.Instance.BlurTestTexture.Value;

            var data = Primitives.PrimitiveHelper.GetRectangleStrip(new Rectangle(Main.spawnTileX * 16 - 200, Main.spawnTileY * 16 - 200, 400, 400));

            effect.SafeSetParameter("WVP", Primitives.PrimitiveHelper.GetMatrix());
            effect.SafeSetParameter("img0", texture);
            effect.SafeSetParameter("blurOrigin", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));
            // Relevant parameters. Radius is radius in pixels of the blur effect, no blur beyond that.
            effect.SafeSetParameter("blurRadius", 100);
            // Intensity is the "hardness" parameter of the blur. Distance to origin is converted to a number between 0 and 1, then
            // this number is applied as an exponent. Higher is harder.
            effect.SafeSetParameter("blurIntensity", 4);
            effect.CurrentTechnique.Passes["Blur"].Apply();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;
            device.Textures[0] = texture;

            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, data, 0, 2);

            orig(self);
        }

        public static void DrawParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            Particle.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self, time);
        }

        public static bool HoverButton = false;
        public static bool NextButton = false;
        public static bool AcceptButton = false;
        public static int DialogueCounter = 0;
        /*private static void DrawNPCChatButtons(On.Terraria.Main.orig_DrawNPCChatButtons orig, int superColor, Color chatColor, int numLines, string focusText, string focusText3)
        {
            if (Main.LocalPlayer.talkNPC != -1)
            {
                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                Player player = Main.LocalPlayer;
                QuestPlayer modPlayer = player.GetModPlayer<QuestPlayer>();
                Quest CurrentQuest = npc.GetGlobalNPC<QuestNPC>().CurrentQuest;

                // Text changes for the Quest button
                string Text = "";
                if (!NextButton)
                {
                    if (CurrentQuest != null)
                    {
                        Text = "Quest";
                    }
                    else
                    {
                        foreach (Quest quest in modPlayer.QuestList)
                        {
                            if (quest.QuestGiver() == npc.type)
                            {
                                if (quest.IsCompleted)
                                {
                                    Text = "Turn In";
                                }
                                else
                                {
                                    Text = "Quest";
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AcceptButton)
                    {
                        Text = "Accept";
                    }
                    else
                    {
                        Text = "Next";
                    }
                }

                #region Text
                DynamicSpriteFont font = Main.fontMouseText;
                Color TextColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
                Vector2 TextScale = new Vector2(0.9f);
                Vector2 StringSize = ChatManager.GetStringSize(font, Text, TextScale);
                Vector2 TextPosition = new Vector2((180 + Main.screenWidth / 2) + StringSize.X - 50f, 130 + numLines * 30);
                Vector2 MouseCursor = new Vector2(Main.mouseX, Main.mouseY);
                #endregion

                // Check for Help button
                if (Main.npcChatFocus2 && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    var HelpDialogue = new List<string>
                        {
                            "helpdialogue1",
                            "helpdialogue2",
                            "helpdialogue3",
                            "helpdialogue4",
                            "helpdialogue5"
                        };

                    Main.npcChatText = Main.rand.Next(HelpDialogue);
                }

                #region Quest Button/Dialogue
                if (MouseCursor.Between(TextPosition, TextPosition + StringSize * TextScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.LocalPlayer.releaseUseItem = true;

                    // Plays the sound once
                    if (!HoverButton)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        HoverButton = true;
                    }

                    TextScale *= 1.1f;

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (CurrentQuest != null)
                        {
                            #region Quest Available
                            // This changes it to the 'Next' button
                            NextButton = true;

                            if (!AcceptButton)
                            {
                                // Loop through the Quest's dialogue options
                                if (DialogueCounter < CurrentQuest.QuestDialogue.Count)
                                {
                                    Main.PlaySound(SoundID.MenuTick);
                                    Main.npcChatText = CurrentQuest.GetDialogue(DialogueCounter++);

                                    if (DialogueCounter == CurrentQuest.QuestDialogue.Count)
                                    {
                                        AcceptButton = true;
                                    }
                                }
                            }
                            else
                            {
                                // Add the thing to the player's list of Quests
                                modPlayer.QuestList.Add(CurrentQuest);
                                Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestAccept"), npc.Center);

                                // Run the Quest Accepted UI
                                Main.NewText("ACCEPTED QUEST: " + CurrentQuest.QuestName(), Color.Yellow);
                                npc.GetGlobalNPC<QuestNPC>().CurrentQuest = null;

                                Main.LocalPlayer.talkNPC = -1;
                            }
                            #endregion
                        }
                        else
                        {
                            #region Quest Accepted
                            // Convert the list to prevent collection modification error
                            foreach (Quest quest in modPlayer.QuestList.ToList())
                            {
                                if (quest.QuestGiver() == npc.type)
                                {
                                    if (quest.IsCompleted) // Completion dialogue
                                    {
                                        Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestTurnIn"), npc.Center);
                                        Main.NewText("COMPLETED QUEST: " + quest.QuestName(), Color.Yellow);

                                        // Do reward shenanigans
                                        quest.GiveRewards(player);
                                        if (quest.QuestID() == (int)Quest.ID.TutorialGuideQuest)
                                        {
                                            foreach (Quest newQuest in OvermorrowModFile.QuestList)
                                            {
                                                if (newQuest.QuestID() == (int)Quest.ID.GuideHouseQuest)
                                                {
                                                    npc.GetGlobalNPC<QuestNPC>().CurrentQuest = newQuest;
                                                }
                                            }
                                        }

                                        OvermorrowModFile.CompletedQuests.Add(quest);
                                        modPlayer.QuestList.Remove(quest);

                                        Main.LocalPlayer.talkNPC = -1;
                                    }
                                    else // Hint dialogue
                                    {
                                        if (DialogueCounter < quest.HintDialogue.Count)
                                        {
                                            Main.PlaySound(SoundID.MenuTick);
                                            Main.npcChatText = quest.GetHint(DialogueCounter++);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    // Plays the sound once
                    if (HoverButton)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        HoverButton = false;
                    }
                }
                #endregion

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, Text, TextPosition + new Vector2(16f, 14f), TextColor, 0f,
                      StringSize * 0.5f, TextScale * new Vector2(1f));
            }

            orig(superColor, chatColor, numLines, focusText, focusText3);
        }
        */
        private static void UpdateNPCCollision(On.Terraria.Player.orig_Update_NPCCollision orig, Player self)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // We don't want to include Moving Platforms for collision because this detour will block the FallThrough needed
                if (npc.ModNPC is CollideableNPC && npc.active && npc.ModNPC != null && !(npc.ModNPC is MovingPlatform))
                {
                    Rectangle PlayerBottom = new Rectangle((int)self.position.X, (int)self.position.Y + self.height, self.width, 1);
                    Rectangle NPCTop = new Rectangle((int)npc.position.X, (int)npc.position.Y - (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));

                    if (PlayerBottom.Intersects(NPCTop))
                    {
                        if (self.position.Y <= npc.position.Y && !self.justJumped && self.velocity.Y >= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y - self.height + 4;
                            self.velocity.Y = 0;

                            self.position += npc.velocity;
                            self.fallStart = (int)(self.position.Y / 16f);

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            if (npc.type == ModContent.NPCType<Pillar>())
                            {
                                self.Hurt(PlayerDeathReason.ByCustomReason(self.name + " had an obelisk stuck up their ass"), 20, -1);
                            }

                            orig(self);
                        }
                    }


                    Rectangle PlayerLeft = new Rectangle((int)self.position.X, (int)self.position.Y, 1, self.height);
                    Rectangle PlayerRight = new Rectangle((int)self.position.X + self.width, (int)self.position.Y, 1, self.height);

                    Rectangle NPCRight = new Rectangle((int)npc.position.X + npc.width, (int)npc.position.Y, 8 + (int)Math.Max(self.velocity.X, 0), npc.height);
                    Rectangle NPCLeft = new Rectangle((int)npc.position.X, (int)npc.position.Y, 10 - (int)Math.Max(self.velocity.X, 0), npc.height);

                    if (PlayerLeft.Intersects(NPCRight))
                    {
                        if (self.position.X >= npc.position.X + npc.width && self.velocity.X <= 0)
                        {
                            int offset = npc.ModNPC is PushableNPC ? 7 : 2;
                            self.velocity.X = 0;
                            self.position.X = npc.position.X + npc.width + offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }

                        if (npc.ModNPC is PushableNPC)
                        {
                            npc.position.X -= 1;
                        }
                    }

                    if (PlayerRight.Intersects(NPCLeft))
                    {
                        if (npc.ModNPC is PushableNPC)
                        {
                            npc.position.X += 1;
                        }

                        if (self.position.X <= npc.position.X && self.velocity.X >= 0)
                        {
                            int offset = npc.ModNPC is PushableNPC ? 2 : 0;

                            self.velocity.X = 0;
                            self.position.X = npc.position.X - self.width - offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }
                    }

                    Rectangle PlayerTop = new Rectangle((int)self.position.X, (int)self.position.Y, self.width, 1);
                    Rectangle NPCBottom = new Rectangle((int)npc.position.X, (int)npc.position.Y + npc.height + (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));
                    if (PlayerTop.Intersects(NPCBottom))
                    {
                        if (self.position.Y <= npc.position.X && self.velocity.Y <= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y + npc.height + 4;
                            self.position += npc.velocity;
                            self.velocity.Y = 0;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);
                        }
                    }

                }
            }

            orig(self);
        }

        // Detour for moving platforms
        private static void PlatformCollision(On.Terraria.Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.GetModPlayer<OvermorrowModPlayer>().PlatformTimer > 0)
            {
                orig(self, fallThrough, ignorePlats);
                return;
            }

            if (self.GoingDownWithGrapple)
            {
                if (self.grapCount == 1)
                {
                    foreach (int PlayerGrappleIndex in self.grappling)
                    {
                        if (PlayerGrappleIndex < 0 || PlayerGrappleIndex > Main.maxProjectiles)
                            continue;

                        Projectile GrappleHook = Main.projectile[PlayerGrappleIndex];

                        foreach (NPC npc in Main.npc)
                        {
                            if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                                continue;

                            if (GrappleHook.active && npc.Hitbox.Intersects(GrappleHook.Hitbox) && self.Hitbox.Intersects(GrappleHook.Hitbox))
                            {
                                self.position = GrappleHook.position + new Vector2(GrappleHook.width / 2 - self.width / 2, GrappleHook.height / 2 - self.height / 2);
                                self.position += npc.velocity;

                                self.velocity.Y = 0;
                                self.jump = 0;

                                self.fallStart = (int)(self.position.Y / 16f);
                            }
                        }
                    }
                }

                orig(self, fallThrough, ignorePlats);
                return;
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                    continue;

                Rectangle PlayerRect = new Rectangle((int)self.position.X, (int)self.position.Y + (self.height), self.width, 1);
                Rectangle NPCRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (self.velocity.Y > 0 ? (int)self.velocity.Y : 0));

                if (self.grapCount == 1 && npc.velocity.Y != 0)
                {
                    //if the player is using a single grappling hook we can check if they are colliding with it and its embedded in the moving platform, while its changing Y position so we can give the player their jump back
                    foreach (int eachGrappleIndex in self.grappling)
                    {
                        if (eachGrappleIndex < 0 || eachGrappleIndex > Main.maxProjectiles)//somehow this can be invalid at this point?
                            continue;

                        Projectile grappleHookProj = Main.projectile[eachGrappleIndex];
                        if (grappleHookProj.active && npc.Hitbox.Intersects(grappleHookProj.Hitbox) && self.Hitbox.Intersects(grappleHookProj.Hitbox))
                        {
                            self.position = grappleHookProj.position + new Vector2(grappleHookProj.width / 2 - self.width / 2, grappleHookProj.height / 2 - self.height / 2);
                            self.position += npc.velocity;

                            self.velocity.Y = 0;
                            self.jump = 0;

                            self.fallStart = (int)(self.position.Y / 16f);
                        }
                    }
                }
                else if (PlayerRect.Intersects(NPCRect) && self.position.Y <= npc.position.Y)
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        if (fallThrough) self.GetModPlayer<OvermorrowModPlayer>().PlatformTimer = 10;

                        self.position.Y = npc.position.Y - self.height + 4;
                        self.position += npc.velocity;

                        self.fallStart = (int)(self.position.Y / 16f);

                        self.velocity.Y = 0;
                        self.jump = 0;
                        self.gfxOffY = npc.gfxOffY;

                    }
                }
            }

            orig(self, fallThrough, ignorePlats);
        }
    }
}
