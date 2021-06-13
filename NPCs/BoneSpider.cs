using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs
{
    public class BoneSpider : ModNPC
    {
        private bool isWalking = true;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulfire Creeper");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 76;
            npc.height = 52;
            npc.damage = 20;
            npc.defense = 6;
            npc.lifeMax = 200;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 60f;
            npc.knockBackResist = 0.25f;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public override void AI()
        {
            npc.ai[0]++;

            if (npc.ai[0] == 240)
            {
                if (!isWalking)
                {
                    isWalking = true;
                    npc.ai[0] = 0;
                }
                else
                {
                    isWalking = false;
                    npc.ai[0] = 0;
                }
            }

            if (isWalking)
            {
                // I was dumb and didn't realize the frame size multiplier works from 0 to (frame - 1)
                // Walking animation
                if (frame >= 4 && frame < 7)
                {
                    frameTimer++;
                    if (frameTimer % 7 == 0)
                    {
                        frame++;
                    }
                }
                else
                {
                    frameTimer++;
                    if (frameTimer % 7 == 0)
                    {
                        frame = 4;
                        frameTimer = 0;
                    }
                }
            }
            else
            {
                // Idle animation

                if (frame < 3)
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
                        frame = 0;
                        frameTimer = 0;
                    }
                }
                npc.velocity = Vector2.Zero;
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
    }
}