/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.RockCrawler
{
    public class RockCrawler : ModNPC
    {
        private bool isWalking = false;
        private int frame = 0;
        private int frameTimer = 0;

        public const int MAX_FRAMES = 5;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rock Crawler");
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 102;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 780;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;

            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
        }

        public enum AIStates
        {
            Idle = 0,
            Swipe = 1
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        public override void AI()
        {
            switch (AICase)
            {
                case (int)AIStates.Idle:
                    NPC.velocity.X = 0;
                    NPC.aiStyle = -1;

                    #region Frame Animation
                    // Idle animation
                    // Frames 0 - 2
                    if (frame < 2)
                    {
                        frameTimer++;
                        if (frameTimer % 8 == 0)
                        {
                            frame++;
                        }
                    }
                    else
                    {
                        frameTimer++;
                        if (frameTimer % 8 == 0)
                        {
                            frame = 0;
                            frameTimer = 0;
                        }
                    }
                    #endregion

                    if (AICounter++ == 240)
                    {
                        AICase = (int)AIStates.Swipe;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Swipe:
                    NPC.velocity = Vector2.Zero;
                    NPC.aiStyle = -1;

                    #region Frame Animation
                    // Frames 3 - 4
                    if (frame >= 3 && frame < 4)
                    {
                        frameTimer++;
                        if (frameTimer % 6 == 0)
                        {
                            frame++;
                        }
                    }
                    else
                    {
                        frameTimer++;
                        if (frameTimer % 6 == 0)
                        {
                            frame = 3;
                            frameTimer = 0;
                        }
                    }
                    #endregion

                    if (AICounter++ == 30)
                    {
                        AICase = (int)AIStates.Idle;
                        AICounter = 0;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (isWalking)
            {
                NPC.spriteDirection = NPC.direction;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        private const int TEXTURE_HEIGHT = 102;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "RockCrawler/CrawlerShell").Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color color = Lighting.GetColor((int)NPC.Center.X / 16, (int)(NPC.Center.Y / 16f));
            Rectangle drawRectangle = new Rectangle(0, TEXTURE_HEIGHT * frame, texture.Width, TEXTURE_HEIGHT);

            Main.spriteBatch.Draw(texture, NPC.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) + 4 - (TEXTURE_HEIGHT / 2)) - screenPos, drawRectangle, color, NPC.rotation, origin, 1f, SpriteEffects.None, 0f);

            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.NPC + "RockCrawler/RockCrawler_Glow").Value;
            spriteBatch.Draw(glow, new Vector2(NPC.Center.X - screenPos.X, NPC.Center.Y - screenPos.Y + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(glow, npc.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) + 4 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition, drawRectangle, color, npc.rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}*/