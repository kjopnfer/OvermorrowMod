/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.Linq;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace OvermorrowMod.Content.NPCs.Bandits
{
    public class JackalBandit : ModNPC
    {
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jackal Bandit");
            Main.npcFrameCount[NPC.type] = 16;
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
            NPC.knockBackResist = 0;
            NPC.boss = true;
            NPC.npcSlots = 10f;
        }

        private ref float AIState => ref NPC.ai[0];
        private ref float AICounter => ref NPC.ai[1];
        private ref float MiscCounter => ref NPC.ai[2];
        private ref float DodgeCounter => ref NPC.ai[3];

        private enum AIStates
        {
            Walk = 0,
            Jump = 1,
            Leap = 2,
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
                    NPC.velocity.Y -= jumpSpeed;
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
            Player player = Main.player[NPC.target];

            switch (AIState)
            {
                #region Walk
                case (int)AIStates.Walk:
                    FrameUpdate(FrameType.Walk);

                    Move(player.Center, 0.6f, 1.8f, 8f);


                    if (AICounter++ == 180)
                    {
                        AIState = Main.rand.NextBool() ? (int)AIStates.Jump : (int)AIStates.Jump;
                        AICounter = 0;
                        tempCounter = 0;
                    }
                    break;
                #endregion
                #region Jump
                case (int)AIStates.Jump:
                    if (AICounter++ == 24)
                    {
                        tempCounter = 0;

                        int jumpDirection = NPC.Center.X > player.Center.X ? -8 : 8;
                        NPC.velocity = new Vector2(jumpDirection, -4);

                        FrameUpdate(FrameType.Jump);
                    }
                    else if (AICounter < 24)
                    {
                        FrameUpdate(FrameType.PrepareLeap);
                        NPC.velocity = Vector2.Zero;
                    }
                    else if (AICounter > 24)
                    {
                        FrameUpdate(FrameType.Jump);

                        Rectangle hitbox = new Rectangle((int)NPC.Center.X + (14 * NPC.direction), (int)NPC.Center.Y, 28, 28);
                        foreach (Player target in Main.player)
                        {
                            if (!target.active) continue;
                            if (target.Hitbox.Intersects(hitbox))
                            {
                                target.Hurt(PlayerDeathReason.LegacyDefault(), 25, 0, false, false);
                                //target.velocity = new Vector2(26 * NPC.direction, -4);
                                target.velocity = new Vector2(78 * NPC.direction, -48);

                                Main.NewText("holy SHIT");
                            }
                        }
                    }

                    if (((NPC.collideY && NPC.velocity.Y == 0) || (NPC.collideX && NPC.velocity.X == 0)) && AICounter > 24)
                    {
                        NPC.velocity.X = 0;

                        AIState = (int)AIStates.Walk;
                        AICounter = 0;

                        tempCounter = 0;
                    }

                    break;
                    #endregion
            }
        }


        int xFrame = 0;
        int yFrame = 0;

        const int MAX_COLUMNS = 2;
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
            PrepareLeap,
            Leap
        }

        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                #region Walk
                case FrameType.Walk:
                    xFrame = 0;

                    if (NPC.velocity.X != 0)
                    {
                        NPC.direction = Math.Sign(NPC.velocity.X);
                    }

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
                        if (yFrame == 15) yFrame = 2;

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
                #endregion
                #region Prepare Leap
                case FrameType.PrepareLeap:
                    xFrame = 1;

                    if (tempCounter++ == 0)
                    {
                        yFrame = 0;
                    }
                    else if (tempCounter % 4 == 0)
                    {
                        if (tempCounter < 12) yFrame++;
                        else if (yFrame > 1) yFrame--;
                    }

                    break;
                #endregion
                #region Jump
                case FrameType.Jump:
                    xFrame = 1;
                    if (NPC.velocity.X != 0) NPC.direction = Math.Sign(NPC.velocity.X);

                    if (tempCounter++ == 0)
                    {
                        yFrame = 4;
                    }
                    else if (tempCounter % 4 == 0)
                    {
                        if (yFrame == 4 || yFrame == 5) yFrame++;
                        else if (yFrame == 6) yFrame = 5;
                    }

                    break;
                    #endregion
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        }
    }
}*/