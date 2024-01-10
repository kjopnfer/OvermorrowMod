using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Content.Projectiles;
using OvermorrowMod.Content.UI.SpeechBubble;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public partial class Feyden : ModNPC
    {
        private void ReassignToHost()
        {
            followPlayer = Main.player[0];
        }

        private void SpawnIdleSlimes()
        {
            if (!CheckSlimeHandler()) return;

            if (targetNPC == null)
            {
                float randomOffset = Main.rand.NextFloat(-7, 7);

                Vector2 position = new Vector2(GuideCamp.FeydenCavePosition.X + randomOffset * 64, (int)GuideCamp.FeydenCavePosition.Y);
                Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                int slime = NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.BlueSlime);
                Main.npc[slime].GetGlobalNPC<SlimeOverrides>().SetTarget(NPC);
            }
        }

        private void TrySpawnSlimeCaveHandler()
        {
            if (OvermorrowWorld.savedFeyden) return;

            Player nearbyPlayer = NPC.FindClosestPlayer(24 * 16);
            if (nearbyPlayer != null)
            {
                SpawnSlimeHandler();
            }
        }

        private bool CheckSlimeHandler()
        {
            bool spawnHandler = true;
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.type == ModContent.ProjectileType<FeydenCaveHandler>()) spawnHandler = false;
            }

            return spawnHandler;
        }

        private void SpawnSlimeHandler()
        {
            if (CheckSlimeHandler()) Projectile.NewProjectile(null, GuideCamp.FeydenCavePosition + new Vector2(16 * 16, 0), Vector2.Zero, ModContent.ProjectileType<FeydenCaveHandler>(), 0, 0f, -1);
        }

        int frameCounter = 0;
        int yFrame = 0;
        private void NPCTextureHandler(out Texture2D texture, out int yFrameCount)
        {
            //Main.NewText("AISTATE: " + AIState + " YFRAME: " + yFrame);
            switch (AIState)
            {
                case (int)AICase.Approach:
                    texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Town/Sojourn/Feyden_Run").Value;
                    yFrameCount = 6;

                    if (yFrame >= 5) yFrame = 0;

                    int frameRate = (int)Math.Round(Math.Abs(NPC.velocity.X));
                    if (!Main.gamePaused) frameCounter += frameRate;

                    if (frameCounter >= 5)
                    {
                        yFrame++;
                        frameCounter = 0;
                    }
                    break;
                case (int)AICase.Fighting:
                    texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Town/Sojourn/Feyden_Attack").Value;
                    yFrameCount = 6;

                    if (yFrame >= 5) yFrame = 0;

                    if (!Main.gamePaused) frameCounter++;
                    if (frameCounter >= 5)
                    {
                        yFrame++;
                        frameCounter = 0;
                    }
                    break;
                case (int)AICase.Dodge:
                    texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Town/Sojourn/Feyden_Roll").Value;
                    yFrameCount = 5;

                    if (yFrame >= 5) yFrame = 0;

                    if (!Main.gamePaused) frameCounter++;
                    if (frameCounter >= 4)
                    {
                        yFrame++;
                        frameCounter = 0;
                    }
                    break;
                default:
                    texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Town/Sojourn/Feyden").Value;
                    yFrameCount = 1;

                    yFrame = 0;
                    break;
            }
        }
    }
}