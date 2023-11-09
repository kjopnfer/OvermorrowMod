using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.NPCs;
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
                dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "FeydenCaveIntro.xml"));
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
                }
                else if (secondWave)
                {
                    secondWave = false;
                    thirdWave = true;

                    Main.NewText("third wave");
                }
                else if (thirdWave)
                {
                    thirdWave = false;
                    finalWave = true;
                    Main.NewText("final wave");

                    dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "FeydenCaveBoss.xml"));
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
            int maxIterations = bigSlime ? 1 : 3;
            int type = bigSlime ? NPCID.YellowSlime : NPCID.BlueSlime;

            // Pick random spots to spawn the slimes, staggered by a counter
            if (Projectile.ai[1]++ % 25 == 0 && iterations < maxIterations)
            {
                float randomOffset = Main.rand.NextFloat(-7, 7);

                // Make it harder for the slimes to randomly spawn next to each other sequentially
                while (Math.Abs(randomOffset - previousOffset) <= 3) randomOffset = Main.rand.NextFloat(-7, 7);

                Vector2 position = new Vector2(Projectile.Center.X + randomOffset * 64, (int)Projectile.Center.Y);
                Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                int slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, type);
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