using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Town
{
    public class TownKid : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Town Kid");
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetDefaults()
        {
            NPC.width = 12;
            NPC.height = 38;
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.defense = 30;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.4f;
        }

        private ref float AIState => ref NPC.ai[0];
        private ref float AICounter => ref NPC.ai[1];

        private enum AIStates
        {
            Idle = 0,
            Run = 1
        }

        private Vector2 movePosition;

        public override void AI()
        {
            foreach (Player player in Main.player)
            {
                if (!player.active) continue;

                if (player.talkNPC == NPC.whoAmI) return;
            }

            switch (AIState)
            {
                case (int)AIStates.Idle:
                    FrameUpdate(FrameType.Idle);

                    NPC.TargetClosest();

                    if (AICounter++ == 180)
                    {
                        AIState = (int)AIStates.Run;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Run:
                    if (AICounter++ == 0)
                    {
                        Vector2 anchorPoint = new Vector2(Main.spawnTileX, Main.spawnTileY) * 16; // TODO: Replace with where the town spawns

                        int direction = Main.rand.NextBool() ? 1 : -1;

                        // Randomly choose an x position within 30 tiles of the anchor point, but 8 to 12 tiles away from this NPC
                        float xPosition = Main.rand.Next(19, 26) * 16;
                        movePosition = new Vector2(NPC.Center.X + (xPosition * direction), anchorPoint.Y + 256);

                        // Check if this position chosen doesn't go out of bounds
                        float checkDistance = Math.Abs(anchorPoint.X - movePosition.X);

                        if (checkDistance > 45 * 16) // Go the opposite direction otherwise
                        {
                            Main.NewText("out of bounds");
                            movePosition = new Vector2(NPC.Center.X + (xPosition * -direction), anchorPoint.Y);
                        }
                    }

                    if (NPC.collideX) NPC.velocity.Y -= 12;

                    FrameUpdate(FrameType.Run);

                    float xDistance = Math.Abs(NPC.Center.X - movePosition.X);

                    float moveSpeed = xDistance <= 3 * 16 ? 3f : 7f;

                    NPC.Move(movePosition, moveSpeed);
                    Dust.NewDust(movePosition, 16, 16, DustID.AmberBolt);

                    if (AICounter == 120 || xDistance < 1)
                    {
                        NPC.velocity.X = 0;

                        AIState = (int)AIStates.Idle;
                        AICounter = 0;
                    }
                    break;
            }
        }

        private enum FrameType
        {
            Idle,
            Run
        }

        int xFrame = 0;
        int yFrame = 0;

        const int MAX_COLUMNS = 1;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / MAX_COLUMNS;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = frameHeight * yFrame;
        }

        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                #region Idle
                case FrameType.Idle:
                    xFrame = 0;
                    yFrame = 0;
                    break;
                #endregion
                #region Walk
                case FrameType.Run:
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
                    else // Frames for when the NPC is walking
                    {
                        if (yFrame == 8) yFrame = 2;

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
            }

            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 3 : -3;
            Vector2 drawOffset = new Vector2(xOffset, -6);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}