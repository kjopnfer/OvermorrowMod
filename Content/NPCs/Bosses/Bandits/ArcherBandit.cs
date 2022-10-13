using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace OvermorrowMod.Content.NPCs.Bosses.Bandits
{
    public class ArcherBandit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archer Bandit");
            Main.npcFrameCount[NPC.type] = 15;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;

            NPC.width = 22;
            NPC.height = 44;

            NPC.damage = 21;
            NPC.defense = 12;
            NPC.lifeMax = 340;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.npcSlots = 10f;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];

            FrameUpdate(FrameType.Walk);
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
        }

        int xFrame = 1;
        int yFrame = 0;

        const int MAX_COLUMNS = 3;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / MAX_COLUMNS;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = frameHeight * yFrame;
        }

        private enum FrameType
        {
            Walk,
            Jump, 
            DrawBow
        }

        float tempCounter = 0;
        private void FrameUpdate(FrameType type)
        {
            switch (type)
            {
                case FrameType.Walk:
                    xFrame = 2;

                    if (NPC.velocity.X == 0 && NPC.velocity.Y == 0) // Frame for when the NPC is standing still
                    {
                        yFrame = 0;
                        tempCounter = 0;
                    }
                    else if (NPC.velocity.Y != 0) // Frame for when the NPC is jumping or falling
                    {
                        yFrame = 1;
                        tempCounter = 0;
                    }
                    else // Frames for when the NPC is walking
                    {
                        if (yFrame == 14) yFrame = 0;

                        // Change the walking frame at a speed depending on the velocity
                        int walkRate = (int)Math.Round(Math.Abs(NPC.velocity.X));
                        tempCounter += walkRate;
                        if (tempCounter > 8)
                        {
                            yFrame++;
                            tempCounter = 0;
                        }
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 11 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -4);
            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}
