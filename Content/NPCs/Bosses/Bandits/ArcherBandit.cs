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
            NPC.width = 22;
            NPC.height = 44;
            NPC.aiStyle = -1;
            NPC.damage = 21;
            NPC.defense = 12;
            NPC.lifeMax = 340;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0.5f;
            NPC.boss = true;
            NPC.npcSlots = 10f;
        }

        private ref float AIState => ref NPC.ai[0];
        private ref float AICounter => ref NPC.ai[1];

        private enum AIStates
        {
            Walk = 0,
            LongShot = 1
        }

        public void Move(Vector2 targetPosition, float moveSpeed, float maxSpeed, float jumpSpeed)
        {
            if (NPC.Center.X < targetPosition.X)
            {
                NPC.velocity.X += moveSpeed;

                if (NPC.velocity.X > maxSpeed) NPC.velocity.X = maxSpeed;
            }
            else if (NPC.Center.X > targetPosition.X)
            {
                NPC.velocity.X -= moveSpeed;

                if (NPC.velocity.X < -maxSpeed) NPC.velocity.X = -maxSpeed;
            }

            if (NPC.collideY && NPC.velocity.Y == 0)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

                #region Jump Handling
                if (NPC.collideX || CheckGap())
                {
                    NPC.velocity.Y += jumpSpeed;
                }

                #endregion
            }
        }

        private bool CheckGap()
        {
            Rectangle npcHitbox = NPC.getRect();

            Vector2 checkLeft = new Vector2(npcHitbox.BottomLeft().X, npcHitbox.BottomLeft().Y);
            Vector2 checkRight = new Vector2(npcHitbox.BottomRight().X, npcHitbox.BottomRight().Y);
            Vector2 hitboxDetection = (NPC.velocity.X < 0 ? checkLeft : checkRight) / 16;

            int directionOffset = NPC.direction;

            Tile tile = Framing.GetTileSafely((int)hitboxDetection.X + directionOffset, (int)hitboxDetection.Y);

            return !tile.HasTile;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];

            switch (AIState)
            {
                case (int)AIStates.Walk:
                    FrameUpdate(FrameType.Walk);

                    float xDistance = Math.Abs(NPC.Center.X - target.Center.X);
                    if (xDistance < 12 * 16) // Try to stay within 12 tiles away from the player
                    {
                        AICounter = 0;

                        if (NPC.Center.X > target.Center.X) // The NPC is to the right of the player, therefore move to the right 
                        {  
                            Move(target.Center + new Vector2(12 * 16), 0.35f, 1.2f, 2f);
                        }
                        else
                        {
                            Move(target.Center - new Vector2(12 * 16), 0.35f, 1.2f, 2f);
                        }
                    }
                    else
                    {
                        if (AICounter++ == 60)
                        {
                            AIState = (int)AIStates.LongShot;
                            AICounter = 0;
                        }
                    }       

                    break;
                case (int)AIStates.LongShot:
                    NPC.velocity = Vector2.Zero;
                    NPC.aiStyle = -1;

                    if (FrameUpdate(FrameType.LongShot))
                    {
                        // Handle shooting here
                    }
                    else
                    {
                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                    }

                    break;
            }
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
            LongShot
        }

        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                case FrameType.Walk:
                    if (NPC.velocity.X != 0)
                    {
                        NPC.direction = (int)Vector2.Normalize(new Vector2(NPC.velocity.X, NPC.velocity.Y)).X;
                    }
                    
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
                        if (tempCounter > 5)
                        {
                            yFrame++;
                            tempCounter = 0;
                        }
                    }
                    break;
                case FrameType.LongShot:
                    xFrame = 1;

                    if (tempCounter++ < 60) // NPC stands in the ready position prior to drawing back the bow
                    {
                        if (yFrame > 0) yFrame = 0;
                    }
                    else
                    {
                        if (tempCounter > 68)
                        {
                            if (yFrame == 10)
                            {
                                yFrame = 0;
                                tempCounter = 0;

                                return false;
                            }

                            yFrame++;
                            tempCounter = 60;
                        }
                    }

                    break;
            }

            return true;
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
