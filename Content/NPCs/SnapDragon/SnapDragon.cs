using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Tiles.WaterCave;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.SnapDragon
{
    public class SnapDragon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Secluded Snapdragon");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.ManEater);
            NPC.width = 58;
            NPC.height = 64;
            AIType = NPCID.ManEater;
            NPC.aiStyle = 13;
            NPC.lifeMax = 140;
            NPC.damage = 30;
            NPC.defense = 13;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 12f == 11f) // Ticks per frame
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 2) // 2 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            NPC.netAlways = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 vector16 = new Vector2(NPC.position.X + (float)(NPC.width / 2), NPC.position.Y + (float)(NPC.height / 2));

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - screenPos + vector16 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(drawColor) * ((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, vector16, NPC.scale, SpriteEffects.None, 0f);
            }

            float num109 = NPC.ai[0] * 16f + 8f - vector16.X;
            float num110 = NPC.ai[1] * 16f + 8f - vector16.Y;
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
                num109 = NPC.ai[0] * 16f + 8f - vector16.X;
                num110 = NPC.ai[1] * 16f + 8f - vector16.Y;
                Color color95 = Lighting.GetColor((int)vector16.X / 16, (int)(vector16.Y / 16f));
                Texture2D chain5Texture = Mod.Assets.Request<Texture2D>("Content/NPCs/SnapDragon_Chain").Value;
                spriteBatch.Draw(chain5Texture, new Vector2(vector16.X - Main.screenPosition.X, vector16.Y - Main.screenPosition.Y), new Rectangle(0, 0, chain5Texture.Width, num274), color95, rotation11, new Vector2((float)chain5Texture.Width * 0.5f, (float)chain5Texture.Height * 0.5f), 1f, SpriteEffects.None, 0f);
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // TODO: Figure out biomes
            // return spawnInfo.player.GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave && Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].TileType == ModContent.TileType<GlowBlock>() ? 0.02f : 0f;
            return 0.0f;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), tileX * 16 + 8, tileY * 16, NPC.type, 0, tileX, tileY);
        }
    }
}