using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Tiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class SnapDragon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Secluded Snapdragon");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.ManEater);
            npc.width = 58;
            npc.height = 64;
            aiType = NPCID.ManEater;
            npc.aiStyle = 13;
            npc.lifeMax = 140;
            npc.damage = 30;
            npc.defense = 13;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 2) // 2 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            npc.netAlways = true;
        }

        public override void NPCLoot()
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 vector16 = new Vector2(npc.position.X + (float)(npc.width / 2), npc.position.Y + (float)(npc.height / 2));

            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + vector16 + new Vector2(0f, npc.gfxOffY);
                Color color = npc.GetAlpha(drawColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, vector16, npc.scale, SpriteEffects.None, 0f);
            }

            float num109 = npc.ai[0] * 16f + 8f - vector16.X;
            float num110 = npc.ai[1] * 16f + 8f - vector16.Y;
            float rotation11 = (float)Math.Atan2(num110, num109) - 1.57f;
            bool flag15 = true;
            while (flag15)
            {
                int num274 = 28;
                int num299 = 40;
                float num102 = (float)Math.Sqrt(num109 * num109 + num110 * num110);
                if (num102 < (float)num299)
                {
                    num274 = (int)num102 - num299 + num274;
                    flag15 = false;
                }
                num102 = (float)num274 / num102;
                num109 *= num102;
                num110 *= num102;
                vector16.X += num109;
                vector16.Y += num110;
                num109 = npc.ai[0] * 16f + 8f - vector16.X;
                num110 = npc.ai[1] * 16f + 8f - vector16.Y;
                Color color95 = Lighting.GetColor((int)vector16.X / 16, (int)(vector16.Y / 16f));
                Texture2D chain5Texture = mod.GetTexture("Content/NPCs/SnapDragon_Chain");
                spriteBatch.Draw(chain5Texture, new Vector2(vector16.X - Main.screenPosition.X, vector16.Y - Main.screenPosition.Y), new Rectangle(0, 0, chain5Texture.Width, num274), color95, rotation11, new Vector2((float)chain5Texture.Width * 0.5f, (float)chain5Texture.Height * 0.5f), 1f, SpriteEffects.None, 0f);
            }
            return base.PreDraw(spriteBatch, drawColor);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave && Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == ModContent.TileType<GlowBlock>() ? 0.02f : 0f;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(tileX * 16 + 8, tileY * 16, npc.type, 0, tileX, tileY);
        }
    }
}