using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ArchiveRat : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPC.width = 46;
            NPC.height = 44;
            NPC.lifeMax = 100;
            NPC.defense = 8;
            NPC.damage = 23;
        }

        private int frame = 0;
        private int frameTimer = 0;
        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        public enum AICase
        {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Stealth = 3
        }

        public override void AI()
        {
            NPC.TargetClosest();

            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    xFrame = 0;
                    yFrame = 2;

                    if (AICounter++ == 120)
                    {
                        AIState = (int)AICase.Walk;
                        AICounter = 0;
                    }
                    break;
                case AICase.Walk:
                    if (AICounter++ % 6 == 0)
                    {
                        yFrame++;
                        if (yFrame >= 9) yFrame = 0;
                    }

                    if (AICounter >= 54 * 5)
                    {
                        AIState = (int)AICase.Idle;
                        AICounter = 0;
                    }
                    break;
                case AICase.Attack:
                    break;
                case AICase.Stealth:
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        int FRAME_WIDTH = 56;
        int FRAME_HEIGHT = 48;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 10;
            NPC.frame.X = FRAME_WIDTH * xFrame;
            NPC.frame.Y = FRAME_HEIGHT * yFrame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOffset = new Vector2(0, 2);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}