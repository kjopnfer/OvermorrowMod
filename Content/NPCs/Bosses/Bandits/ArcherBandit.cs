using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
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
        private int DodgeCooldown = 0;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
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
        private ref float DodgeCounter => ref NPC.ai[2];

        private enum AIStates
        {
            Walk = 0,
            Jump = 1,
            LongShot = 2,
            AngleShot = 3
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

                        // The player is too close, set a timer to determine if they should perform a jump or roll
                        if (xDistance < 4 * 16)
                        {
                            if (DodgeCounter++ >= 60 && DodgeCooldown-- <= 0)
                            {
                                NPC.velocity = Vector2.Zero;

                                AIState = (int)AIStates.Jump;
                                AICounter = 0;
                                DodgeCounter = 0;
                            }
                        }
                    }
                    else
                    {
                        if (AICounter++ == 60)
                        {
                            //AIState = Main.rand.NextBool() ? (int)AIStates.LongShot : (int)AIStates.AngleShot;
                            AIState = (int)AIStates.AngleShot;
                            AICounter = 0;
                        }
                    }

                    break;
                case (int)AIStates.Jump:
                    FrameUpdate(FrameType.Jump);

                    if (AICounter++ == 0)
                    {
                        //JumpDirection = NPC.Center.X > target.Center.X ? 1 : -1;
                        int jumpDirection = NPC.Center.X > target.Center.X ? 5 : -5;
                        NPC.velocity = new Vector2(jumpDirection, -6);
                    }

                    if (NPC.collideY && NPC.velocity.Y == 0)
                    {
                        NPC.velocity.X = 0;

                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                        DodgeCooldown = 30;
                    }

                    break;
                case (int)AIStates.LongShot:
                    NPC.velocity = Vector2.Zero;
                    NPC.aiStyle = -1;

                    if (FrameUpdate(FrameType.LongShot))
                    {
                        if (yFrame == 6 && tempCounter == 62)
                        {
                            for (int i = 0; i < Main.rand.Next(3, 6); i++)
                            {
                                float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                                Vector2 RandomVelocity = -Vector2.UnitX.RotatedBy(randomAngle) * Main.rand.Next(4, 7);
                                Color color = Color.Orange;

                                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), NPC.Center, RandomVelocity, color, 1, randomScale);
                            }

                            float particleScale = Main.rand.NextFloat(0.4f, 0.5f);
                            float particleRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                            float particleTime = 90;

                            Particle.CreateParticle(Particle.ParticleType<RingSolid>(), NPC.Center, Vector2.Zero, Color.Orange, 1, particleScale, particleRotation, particleTime);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 12 * NPC.direction, ModContent.ProjectileType<FlameArrow>(), NPC.damage, 2f, Main.myPlayer);
                        }
                    }
                    else
                    {
                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                    }

                    break;
                case (int)AIStates.AngleShot:
                    NPC.velocity = Vector2.Zero;
                    NPC.aiStyle = -1;

                    if (FrameUpdate(FrameType.AngleShot))
                    {
                        if (yFrame == 6 && tempCounter == 62)
                        {
                            for (int i = 0; i < Main.rand.Next(3, 6); i++)
                            {
                                float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                                Vector2 RandomVelocity = new Vector2(-1, 1).RotatedBy(randomAngle) * Main.rand.Next(4, 7);
                                Color color = Color.Purple;

                                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), NPC.Center, RandomVelocity, color, 1, randomScale);
                            }

                            float particleScale = 0.1f;
                            float particleRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                            float particleTime = 90;

                            Particle.CreateParticle(Particle.ParticleType<RingSolid>(), NPC.Center + new Vector2(26, -28), Vector2.Zero, Color.Purple, 1, particleScale, particleRotation, particleTime);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7 * NPC.direction, -7) , ModContent.ProjectileType<SplitArrow>(), NPC.damage, 2f, Main.myPlayer, 0, 0);
                        }
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

        const int MAX_COLUMNS = 4;
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
            LongShot,
            AngleShot
        }

        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                case FrameType.Walk:
                    xFrame = 3;

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
                case FrameType.Jump:
                    if (NPC.velocity.X != 0)
                    {
                        NPC.direction = -Math.Sign(NPC.velocity.X);
                    }

                    xFrame = 0;
                    yFrame = 1;
                    break;
                case FrameType.LongShot:
                    xFrame = 1;

                    if (tempCounter++ < 60) // NPC stands in the ready position prior to drawing back the bow
                    {
                        if (yFrame > 0) yFrame = 0;
                    }
                    else
                    {
                        if (yFrame == 5) // Holds the frame for half a second longer 
                        {
                            if (tempCounter > 98)
                            {
                                yFrame++;
                                tempCounter = 60;
                            }
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
                    }

                    break;
                case FrameType.AngleShot:
                    xFrame = 2;

                    if (tempCounter++ < 60) // NPC stands in the ready position prior to drawing back the bow
                    {
                        if (yFrame > 0) yFrame = 0;
                    }
                    else
                    {
                        if (yFrame == 5)
                        {
                            if (tempCounter > 98)
                            {
                                yFrame++;
                                tempCounter = 60;
                            }
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

            if ((AIState == (int)AIStates.LongShot || AIState == (int)AIStates.AngleShot) && yFrame == 5)
            {
                spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

                int frameOffset = xFrame == 1 ? 0 : 60;
                Rectangle drawRectangle = new Rectangle(frameOffset, 0, 60, 60);
                Vector2 bowOffset = new Vector2(NPC.direction == 1 ? 11 : -10, -4);

                float progress = Utils.Clamp(tempCounter - 68, 0, 30) / 30f;

                Main.spriteBatch.Reload(BlendState.Additive);

                Color frameColor = xFrame == 1 ? Color.Orange : Color.Purple;

                Texture2D flare = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flare_01").Value;
                float scale = MathHelper.Lerp(0, 1, progress);
                spriteBatch.Draw(flare, NPC.Center - screenPos + new Vector2(26, -28), null, frameColor, NPC.rotation, flare.Size() / 2, scale, spriteEffects, 0);

                float colorStrength = MathHelper.Lerp(0, 1f, progress);
                Lighting.AddLight(NPC.Center + new Vector2(26, -28), colorStrength, 0, colorStrength);

                Texture2D glowFrame = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Bandits/ArcherBandit_ArrowGlow").Value;
                spriteBatch.Draw(glowFrame, NPC.Center - screenPos + bowOffset, drawRectangle, frameColor * progress, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
            else
            {
                spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if ((AIState == (int)AIStates.LongShot || AIState == (int)AIStates.AngleShot) && yFrame == 5)
            {
                int frameOffset = xFrame == 1 ? 0 : 60;
                Rectangle drawRectangle = new Rectangle(frameOffset, 0, 60, 60);
                Vector2 bowOffset = new Vector2(NPC.direction == 1 ? 11 : -10, -4);

                Texture2D bowFrame = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Bandits/ArcherBandit_Bow").Value;
                spriteBatch.Draw(bowFrame, NPC.Center - screenPos + bowOffset, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

                Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                float progress = Utils.Clamp(tempCounter - 68, 0, 30) / 30f;

                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                Texture2D arrowFrame = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Bandits/ArcherBandit_Arrow").Value;
                spriteBatch.Draw(arrowFrame, NPC.Center - screenPos + bowOffset, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
            }
        }
    }
}
