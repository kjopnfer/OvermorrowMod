using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.UI.SpeechBubble;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles
{
    public class FeydenCaveHandler : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 420;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        List<int> ActiveSlimes = new List<int>();
        bool firstWave = true;
        bool secondWave = false;
        bool thirdWave = false;
        bool finalWave = false;
        public override void AI()
        {
            DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            Projectile.timeLeft = 60;

            if (Projectile.ai[0]++ == 0)
            {
                dialoguePlayer.AddNPCPopup(ModContent.NPCType<Feyden>(), ModUtils.GetXML(AssetDirectory.Popups + "FeydenCave.xml"), "INTRO");
            }

            SpawnSlimes(finalWave);
            UpdateSlimes(ref ActiveSlimes);

            if (ActiveSlimes.Count <= 0)
            {
                if (firstWave)
                {
                    firstWave = false;
                    secondWave = true;

                    Main.NewText("second wave");
                    dialoguePlayer.AddNPCPopup(ModContent.NPCType<Feyden>(), ModUtils.GetXML(AssetDirectory.Popups + "FeydenCave.xml"), "SECOND_WAVE");
                }
                else if (secondWave)
                {
                    secondWave = false;
                    thirdWave = true;

                    Main.NewText("third wave");
                    dialoguePlayer.AddNPCPopup(ModContent.NPCType<Feyden>(), ModUtils.GetXML(AssetDirectory.Popups + "FeydenCave.xml"), "MID_WAVE");
                }
                else if (thirdWave)
                {
                    thirdWave = false;
                    finalWave = true;
                    Main.NewText("final wave");

                    NPC npc = ModUtils.FindFirstNPC(ModContent.NPCType<Feyden>());
                    if (npc != null)
                    {
                        BaseSpeechBubble speechBubble = new BaseSpeechBubble();
                        speechBubble.Add(new Text("Look who's trying to be the king of slimes!", 45, 75));
                        speechBubble.Add(new Text("Not on my watch!", 30, 60));

                        UISpeechBubbleSystem.Instance.SpeechBubbleState.AddSpeechBubble(npc, speechBubble, true);
                    }
                }
                else if (finalWave)
                {
                    Projectile.Kill();
                }

                iterations = 0;
                Projectile.ai[1] = 0;
            }
        }

        // Check all currently active slimes and return a new list
        private void UpdateSlimes(ref List<int> activeSlimes)
        {
            List<int> removeSlimes = new List<int>();
            foreach (int slimeID in activeSlimes)
            {
                if (!Main.npc[slimeID].active) removeSlimes.Add(slimeID);
            }

            foreach (int slimeID in removeSlimes)
            {
                activeSlimes.Remove(slimeID);
            }
        }

        int iterations = 0;
        float previousOffset = 999;
        private void SpawnSlimes(bool bigSlime = false)
        {
            int maxIterations = bigSlime ? 1 : 5;

            // Pick random spots to spawn the slimes, staggered by a counter
            if (Projectile.ai[1]++ % 25 == 0 && iterations < maxIterations)
            {
                float randomOffset = Main.rand.NextFloat(-7, 7);

                // Make it harder for the slimes to randomly spawn next to each other sequentially
                while (Math.Abs(randomOffset - previousOffset) <= 3) randomOffset = Main.rand.NextFloat(-7, 7);

                Vector2 position = new Vector2(Projectile.Center.X + randomOffset * 64, (int)Projectile.Center.Y);
                Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                int slime;
                if (bigSlime) slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.BlueSlime, 0, 0, ItemID.IronPickaxe);
                else slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.BlueSlime);

                Player nearestPlayer = ModUtils.GetNearestPlayer(Main.npc[slime]);

                if (nearestPlayer != null && nearestPlayer.active)
                {
                    Main.npc[slime].GetGlobalNPC<SlimeOverrides>().SetTarget(nearestPlayer);
                }

                ActiveSlimes.Add(slime);
                iterations++;
                previousOffset = randomOffset;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (finalWave && ActiveSlimes.Count <= 0)
            {
                OvermorrowWorld.savedFeyden = true;
                Main.NewText("Monster Den Cleared!", Color.Yellow);
            }
        }
    }
}