using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Materials;
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
            DisplayName.SetDefault("Rock Crawler");
            Main.npcFrameCount[npc.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 102;
            npc.damage = 20;
            npc.defense = 6;
            npc.lifeMax = 780;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;

            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public enum AIStates
        {
            Idle = 0,
            Swipe = 1
        }

        public ref float AICase => ref npc.ai[0];
        public ref float AICounter => ref npc.ai[1];

        public override void AI()
        {
            switch (AICase)
            {
                case (int)AIStates.Idle:
                    npc.velocity.X = 0;
                    npc.aiStyle = -1;

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
                    npc.velocity = Vector2.Zero;
                    npc.aiStyle = -1;

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
                npc.spriteDirection = npc.direction;
            }

            npc.frame.Y = frameHeight * frame;
        }

        private const int TEXTURE_HEIGHT = 102;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.NPC + "RockCrawler/CrawlerShell");
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color color = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));
            Rectangle drawRectangle = new Rectangle(0, TEXTURE_HEIGHT * frame, texture.Width, TEXTURE_HEIGHT);

            Main.spriteBatch.Draw(texture, npc.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) + 4 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition, drawRectangle, color, npc.rotation, origin, 1f, SpriteEffects.None, 0f);

            Texture2D glow = ModContent.GetTexture(AssetDirectory.NPC + "RockCrawler/RockCrawler_Glow");
            spriteBatch.Draw(glow, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(glow, npc.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) + 4 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition, drawRectangle, color, npc.rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}