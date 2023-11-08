using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Xml;
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
                SpawnSlimes();
            }

            UpdateSlimes(ref ActiveSlimes);

            if (firstWave && ActiveSlimes.Count <= 0)
            {
                firstWave = false;
                secondWave = true;
                Main.NewText("second wave");

                SpawnSlimes();
            }

            if (secondWave && ActiveSlimes.Count <= 0)
            {
                secondWave = false;
                thirdWave = true;
                Main.NewText("third wave");

                SpawnSlimes();
            }

            if (thirdWave && ActiveSlimes.Count <= 0)
            {
                Main.NewText("final wave");
                dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "FeydenCaveBoss.xml"));

                thirdWave = false;
                finalWave = true;

                Vector2 position = new Vector2(Projectile.Center.X + Main.rand.Next(-7, 7) * 32, (int)Projectile.Center.Y);
                Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                int slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.YellowSlime, 0);
                ActiveSlimes.Add(slime);
            }

            if (finalWave && ActiveSlimes.Count <= 0)
            {              
                Projectile.Kill();
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
        
        private void SpawnSlimes()
        {
            // Pick three random spots to spawn the slimes
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(Projectile.Center.X + Main.rand.NextFloat(-7, 7) * 64, (int)Projectile.Center.Y);
                Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                int slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.BlueSlime, 0);
                ActiveSlimes.Add(slime);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (finalWave && ActiveSlimes.Count <= 0) Main.NewText("Monster Den Cleared!", Color.Yellow);
        }
    }
}