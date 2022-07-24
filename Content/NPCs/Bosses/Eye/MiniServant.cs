using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.Dusts;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class MiniServant : ModNPC, ITrailEntity
    {
        private float randomAmplitude;
        private float rotateDirection = 1;
        private float moveSpeed;
        private float turnResistance;
        private Vector2 positionOffset;

        public Player latchPlayer;
        private bool hasLatched = false;
        private Vector2 latchPoint;
        private int latchCounter;

        private bool isLeft = false;
        private bool isRight = false;

        public Color TrailColor(float progress) => AICase == (int)AIStates.Latch ? Color.Transparent : Color.Black;
        public float TrailSize(float progress) => 16;
        public Type TrailType() => typeof(LightningTrail);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drainer of Cthulhu");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 16;
            NPC.lifeMax = 10;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
        }

        public enum AIStates
        {
            Fly = 0,
            Latch = 1,
            ShakeOff = 2
        }
        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public override void OnSpawn(IEntitySource source)
        {
            moveSpeed = Main.rand.Next(12, 15) * 2;
            turnResistance = Main.rand.Next(16, 19) * 5;
            positionOffset = new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)) * 10;
            rotateDirection = Main.rand.NextBool() ? 1 : -1;
            randomAmplitude = Main.rand.NextFloat(0.1f, 0.25f);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            switch (AICase)
            {
                case (int)AIStates.Fly:
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                    if (++AICounter < 150)
                    {
                        NPC.Move(player.Center, moveSpeed, turnResistance);
                    }
                    else if (AICounter == 150)
                    {
                        rotateDirection = Main.rand.NextBool() ? 1 : -1;
                        NPC.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4) * (moveSpeed / 2);
                    }

                    NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

                    if (AICounter == 210) AICounter = 0;

                    foreach (Player latchTarget in Main.player)
                    {
                        if (!latchTarget.active) continue;

                        if (NPC.Hitbox.Intersects(latchTarget.getRect()))
                        {
                            //hasLatched = true;
                            latchPoint = NPC.Center - player.Center;
                            latchPlayer = player;

                            AICase = (int)AIStates.Latch;
                            AICounter = 0;
                        }
                    }
                    break;
                case (int)AIStates.Latch:
                    NPC.Center = latchPlayer.Center + latchPoint;

                    #region Shake Off Detection
                    // The reason that we don't use the control keys for left/right is to account for bottle-type accessories
                    if (player.direction == 1)
                    {
                        if (!isRight)
                        {
                            //Main.NewText("facing right");
                            isRight = true;
                            isLeft = false;

                            latchCounter += Main.rand.Next(3, 6);
                        }
                    }

                    if (player.direction == -1)
                    {
                        if (!isLeft)
                        {
                            //Main.NewText("facing left");
                            isLeft = true;
                            isRight = false;

                            latchCounter += Main.rand.Next(3, 6);
                        }
                    }

                    if (latchPlayer.justJumped)
                    {
                        //Main.NewText("jump");
                        latchCounter += Main.rand.Next(3, 6);
                    }

                    latchCounter -= Main.rand.Next(1, 3);
                    if (latchCounter < 0) latchCounter = 0;

                    if (latchCounter >= 60)
                    {
                        NPC.velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 8;
                        latchCounter = 0;

                        AICase = (int)AIStates.ShakeOff;
                        AICounter = 0;
                    }
                    #endregion
                    break;
                case (int)AIStates.ShakeOff:
                    NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

                    if (AICounter++ == 90)
                    {
                        AICase = (int)AIStates.Fly;
                        AICounter = 0;
                    }
                    break;
            }

            /*if (!hasLatched)
            {
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                if (++AICounter < 150)
                {
                    NPC.Move(player.Center, moveSpeed, turnResistance);
                }
                else if (AICounter == 150)
                {
                    rotateDirection = Main.rand.NextBool() ? 1 : -1;
                    NPC.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4) * (moveSpeed / 2);
                }

                NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

                if (AICounter == 210) AICounter = 0;

                foreach (Player latchTarget in Main.player)
                {
                    if (!latchTarget.active) continue;

                    if (NPC.Hitbox.Intersects(latchTarget.getRect()))
                    {
                        hasLatched = true;
                        latchPoint = NPC.Center - player.Center;
                        latchPlayer = player;
                    }
                }
            }
            else
            {
                NPC.Center = latchPlayer.Center + latchPoint;

                #region Shake Off Detection

                //if(latchPlayer.releaseLeft && latchPlayer.oldDirection == 1) Main.NewText("left");
                //if (latchPlayer.releaseRight && latchPlayer.oldDirection == -1) Main.NewText("right");

                //if (latchPlayer.oldDirection == 1 && latchPlayer.direction == -1) Main.NewText("right");

                // The reason that we don't use the control keys for left/right is to account for bottle-type accessories
                if (player.direction == 1)
                {
                    if (!isRight)
                    {
                        Main.NewText("facing right");
                        isRight = true;
                        isLeft = false;

                        latchCounter += Main.rand.Next(3, 6);
                    }
                }

                if (player.direction == -1)
                {
                    if (!isLeft)
                    {
                        Main.NewText("facing left");

                        isLeft = true;
                        isRight = false;

                        latchCounter += Main.rand.Next(3, 6);
                    }
                }

                if (latchPlayer.justJumped)
                {
                    Main.NewText("jump");
                    latchCounter += Main.rand.Next(3, 6);
                }

                if (latchCounter >= 40)
                {
                    latchCounter = 0;
                    hasLatched = false;
                }
                #endregion
            }*/
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0) NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= frameHeight * 2) NPC.frame.Y = 0;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant_Glow").Value;
            Color color = Color.Lerp(Color.White, Color.Transparent, NPC.alpha / 255f);

            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        }
    }
}