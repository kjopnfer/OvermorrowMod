using Microsoft.Xna.Framework;
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
        private int RepeatShots = 0;

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
            LongShot = 2,
            AngleShot = 3,
            JumpShot = 4,
            Stun = 5
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
                #region Walk
                case (int)AIStates.Walk:
                    FrameUpdate(FrameType.Walk);

                    float xDistance = Math.Abs(NPC.Center.X - target.Center.X);
                    if (xDistance < 10 * 16) // Try to stay within 10 tiles away from the player
                    {
                        AICounter = 0;

                        if (NPC.Center.X > target.Center.X) // The NPC is to the right of the player, therefore move to the right 
                        {
                            Move(target.Center + new Vector2(12 * 16), 0.4f, 1.2f, 2f);
                        }
                        else
                        {
                            Dust.NewDust(new Vector2((int)NPC.BottomLeft.X / 16, (int)NPC.BottomLeft.Y / 16), 1, 1, DustID.Torch);

                            // Check if the tile below the NPC is empty
                            if (Framing.GetTileSafely((int)NPC.BottomLeft.X / 16, (int)NPC.BottomLeft.Y / 16).HasTile)
                            {
                                Move(target.Center - new Vector2(12 * 16), 0.35f, 1.2f, 2f);
                            }
                            else
                            {
                                NPC.velocity = Vector2.Zero;
                            }
                        }

                        // The player is too close, set a timer to determine if they should perform a jump or roll
                        if (xDistance < 6 * 16)
                        {
                            if (DodgeCounter++ >= 10 && DodgeCooldown-- <= 0)
                            {
                                NPC.velocity = Vector2.Zero;

                                AIState = Main.rand.NextBool() ? (int)AIStates.JumpShot : (int)AIStates.Jump;
                                AICounter = 0;
                                DodgeCounter = 0;
                            }
                        }
                    }
                    else
                    {
                        if (AICounter++ == 60)
                        {
                            AIState = Main.rand.NextBool() ? (int)AIStates.AngleShot : (int)AIStates.LongShot;
                            //AIState = (int)AIStates.AngleShot;
                            AICounter = 0;
                        }
                    }
                    break;
                #endregion
                #region Jump
                case (int)AIStates.Jump:
                    FrameUpdate(FrameType.Jump);

                    if (AICounter++ == 0)
                    {
                        int jumpDirection = NPC.Center.X > target.Center.X ? 8 : -8;

                        if (Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) - 15, (int)NPC.BottomLeft.Y / 16).HasTile)
                        {
                            // Check if the right side of the NPC is near a solid block
                            if ((!Framing.GetTileSafely((int)(NPC.Center.X / 16) + 1, (int)NPC.Center.Y / 16).HasTile && Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 1, (int)NPC.BottomLeft.Y / 16).TileType != TileID.WoodenBeam) &&
                                (!Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 15, (int)NPC.BottomLeft.Y / 16).HasTile && Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 15, (int)NPC.BottomLeft.Y / 16).TileType != TileID.WoodenBeam))
                            {
                                NPC.velocity = new Vector2(jumpDirection, -4);
                            }
                            else
                            {
                                NPC.velocity = new Vector2(-jumpDirection, -4);
                                Main.NewText("tile behind me");
                            }

                        }
                        else // The NPC will jump off the ledge if they jump, therefore launch in the other direction
                        {
                            NPC.velocity = new Vector2(-jumpDirection, -4);
                        }
                    }

                    if (NPC.collideY && NPC.velocity.Y == 0)
                    {
                        NPC.velocity.X = 0;

                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                        DodgeCooldown = 0;
                    }

                    break;
                #endregion
                #region LongShot
                case (int)AIStates.LongShot:
                    NPC.velocity = Vector2.Zero;

                    if (FrameUpdate(FrameType.LongShot))
                    {
                        if (yFrame == 6)
                        {
                            if (tempCounter == 61)
                            {
                                float particleScale = Main.rand.NextFloat(0.2f, 0.3f);
                                float particleRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                                float particleTime = 90;

                                Particle.CreateParticle(Particle.ParticleType<RingSolid>(), NPC.Center + new Vector2(26 * NPC.direction, 0), Vector2.Zero, Color.Orange, 1, particleScale, particleRotation, particleTime);
                            }

                            if (tempCounter == 62)
                            {
                                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                                {
                                    float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                                    float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                                    Vector2 RandomVelocity = -Vector2.UnitX.RotatedBy(randomAngle) * Main.rand.Next(4, 7) * NPC.direction;
                                    Color color = Color.Orange;

                                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), NPC.Center, RandomVelocity, color, 1, randomScale);
                                }

                                Lighting.AddLight(NPC.Center + new Vector2(26, -28), 2f, 2f * 0.65f, 0);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 12 * NPC.direction, ModContent.ProjectileType<FlameArrow>(), NPC.damage * 2, 2f, Main.myPlayer);
                            }
                        }
                    }
                    else
                    {
                        if (RepeatShots++ < 0)
                        {
                            AIState = (int)AIStates.LongShot;
                            AICounter = 0;
                        }
                        else
                        {
                            AIState = (int)AIStates.Walk;
                            AICounter = 0;
                            RepeatShots = 0;
                        }
                    }

                    break;
                #endregion
                #region AngleShot
                case (int)AIStates.AngleShot:
                    NPC.velocity = Vector2.Zero;

                    if (FrameUpdate(FrameType.AngleShot))
                    {
                        if (yFrame == 6)
                        {
                            if (tempCounter == 61)
                            {
                                float particleScale = Main.rand.NextFloat(0.2f, 0.3f);
                                float particleRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                                float particleTime = 90;

                                Particle.CreateParticle(Particle.ParticleType<RingSolid>(), NPC.Center + new Vector2(26 * NPC.direction, -28), Vector2.Zero, Color.Purple, 1, particleScale, particleRotation, particleTime);
                            }

                            if (tempCounter == 62)
                            {
                                /*DialoguePlayer dialoguePlayer = target.GetModPlayer<DialoguePlayer>();
                                XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "Archer.xml");
                                dialoguePlayer.AddPopup(doc);*/

                                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                                {
                                    float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                                    float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                                    Vector2 RandomVelocity = new Vector2(-1 * NPC.direction, 1).RotatedBy(randomAngle) * Main.rand.Next(4, 7);
                                    Color color = Color.Purple;

                                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), NPC.Center, RandomVelocity, color, 1, randomScale);
                                }

                                Lighting.AddLight(NPC.Center + new Vector2(26, -28), 2f, 0, 2f);

                                //Vector2 initialVelocity = 7 * new Vector2((float)Math.Cos(45), (float)Math.Sin(45));

                                // Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7 * NPC.direction, -12), ModContent.ProjectileType<SplitArrow>(), NPC.damage, 2f, Main.myPlayer, 0, 0);
                                //for (int i = 0; i < 4; i++)
                                //{
                                //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(6, 10) * NPC.direction, -Main.rand.NextFloat(12, 17)), ModContent.ProjectileType<SplitArrow>(), NPC.damage, 2f, Main.myPlayer, 0, 0);
                                //}
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(6, 10) * NPC.direction, -Main.rand.NextFloat(12, 16.5f)), ModContent.ProjectileType<SplitArrow>(), NPC.damage, 2f, Main.myPlayer, 0, 0);


                                /*for (int i = -1; i <= 1; i += 2)
                                {
                                    SplitArrow arrow = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7 * NPC.direction, -12), ModContent.ProjectileType<SplitArrow>(), NPC.damage, 2f, Main.myPlayer, NPC.whoAmI, target.whoAmI).ModProjectile as SplitArrow;
                                    arrow.ShootPosition = (target.Center + Vector2.UnitX * 32 * i) + new Vector2(target.velocity.X * 4, 0);
                                }*/
                            }
                        }
                    }
                    else
                    {
                        if (RepeatShots++ < 0)
                        {
                            AIState = (int)AIStates.AngleShot;
                            AICounter = 0;
                        }
                        else
                        {
                            AIState = (int)AIStates.Walk;
                            AICounter = 0;
                            RepeatShots = 0;
                        }
                    }

                    break;
                #endregion
                #region JumpShot
                case (int)AIStates.JumpShot:
                    const int JUMP_HEIGHT = -11;

                    FrameUpdate(FrameType.JumpShot);

                    if (AICounter++ == 0)
                    {
                        int jumpDirection = NPC.Center.X > target.Center.X ? 7 : -7;

                        // Check if the right side of the NPC is near a solid block
                        if ((!Framing.GetTileSafely((int)(NPC.Center.X / 16) + 1, (int)NPC.Center.Y / 16).HasTile && Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 1, (int)NPC.BottomLeft.Y / 16).TileType != TileID.WoodenBeam) &&
                            (!Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 15, (int)NPC.BottomLeft.Y / 16).HasTile && Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) + 15, (int)NPC.BottomLeft.Y / 16).TileType != TileID.WoodenBeam))
                        {
                            NPC.velocity = new Vector2(jumpDirection, -4);
                        }
                        else
                        {
                            NPC.velocity = new Vector2(-jumpDirection, -4);
                            Main.NewText("tile behind me");
                        }

                        Projectile.NewProjectile(null, NPC.Center, new Vector2(-1, -3), ModContent.ProjectileType<SlimeGrenade>(), 0, 0f, Main.myPlayer);
                    }

                    if (NPC.collideY && NPC.velocity.Y == 0 && MiscCounter == 0)
                    {
                        NPC.velocity.X = 0;
                        MiscCounter = 1;
                    }

                    if (MiscCounter == 1)
                    {
                        int jumpDirection = NPC.Center.X > target.Center.X ? 3 : -3;

                        if (Framing.GetTileSafely((int)(NPC.BottomLeft.X / 16) - 15, (int)NPC.BottomLeft.Y / 16).HasTile)
                        {
                            NPC.velocity = new Vector2(jumpDirection, JUMP_HEIGHT);
                        }
                        else // The NPC will jump off the ledge if they jump, therefore launch in the other direction
                        {
                            NPC.velocity = new Vector2(-jumpDirection, JUMP_HEIGHT);
                        }
                    }

                    if (MiscCounter >= 1)
                    {
                        if (MiscCounter++ == 30)
                        {
                            Projectile.NewProjectile(null, NPC.Center, new Vector2(-6 * -NPC.direction, 6), ModContent.ProjectileType<FlameArrow>(), 26, 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.collideY && NPC.velocity.Y == 0 && MiscCounter > 1)
                    {
                        NPC.velocity.X = 0;
                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                        MiscCounter = 0;
                        DodgeCooldown = 15;
                    }

                    break;
                #endregion
                #region Stun
                case (int)AIStates.Stun:
                    FrameUpdate(FrameType.Stun);

                    if (NPC.collideY && NPC.velocity.Y == 0)
                    {
                        NPC.velocity.X = 0;
                    }

                    if (AICounter++ == 120)
                    {
                        AIState = (int)AIStates.Walk;
                        AICounter = 0;
                        MiscCounter = 0;
                    }
                    break;

                    #endregion
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

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (AIState == (int)AIStates.AngleShot || (AIState == (int)AIStates.LongShot))
            {
                Main.NewText("stun");

                AIState = (int)AIStates.Stun;
                AICounter = 0;
                NPC.velocity = new Vector2(2 * -NPC.direction, -2);
            }
        }

        private enum FrameType
        {
            Walk,
            Jump,
            LongShot,
            AngleShot,
            JumpShot,
            Stun
        }

        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                #region Walk
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
                #endregion
                #region Jump
                case FrameType.Jump:
                    if (NPC.velocity.X != 0)
                    {
                        NPC.direction = -Math.Sign(NPC.velocity.X);
                    }

                    xFrame = 0;
                    yFrame = 1;
                    break;
                #endregion
                #region LongShot
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
                #endregion
                #region AngleShot
                case FrameType.AngleShot:
                    xFrame = 2;

                    //if (RepeatShots > 0 && tempCounter < 55) tempCounter = 65; // If the NPC has fired the first angled shot, make it take less time

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
                #endregion
                #region JumpShot
                case FrameType.JumpShot:
                    xFrame = 0;

                    //Main.NewText("tempcounter " + tempCounter);
                    if (MiscCounter == 0)
                    {
                        yFrame = 1;
                    }
                    else
                    {
                        if (tempCounter++ == 0)
                        {
                            yFrame = 2;
                        }

                        if (tempCounter == 18)
                        {
                            yFrame = 3;
                        }

                        if (tempCounter == 26)
                        {
                            yFrame = 4;
                        }

                        if (tempCounter == 32)
                        {
                            yFrame = 1;
                        }
                    }

                    break;
                #endregion
                #region Stun
                case FrameType.Stun:
                    xFrame = 0;

                    if (NPC.velocity.Y != 0)
                    {
                        yFrame = 1;
                    }
                    else if (NPC.collideY)
                    {
                        yFrame = 0;
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


            if ((AIState == (int)AIStates.LongShot || AIState == (int)AIStates.AngleShot) && yFrame == 5)
            {
                PreDrawLongShot(spriteBatch, screenPos, drawColor);
            }
            /*else if (AIState == (int)AIStates.JumpShot && yFrame == 3)
            {

            }*/
            else
            {
                spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if ((AIState == (int)AIStates.LongShot || AIState == (int)AIStates.AngleShot) && yFrame == 5)
            {
                PostDrawLongShot(spriteBatch, screenPos, drawColor);
            }
        }

        private void PreDrawJumpShot(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 11 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -4);
        }

        private void PreDrawLongShot(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 11 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -4);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            int frameOffset = xFrame == 1 ? 0 : 60;
            Rectangle drawRectangle = new Rectangle(frameOffset, 0, 60, 60);
            Vector2 bowOffset = new Vector2(NPC.direction == 1 ? 11 : -10, -4);

            float progress = Utils.Clamp(tempCounter - 68, 0, 30) / 30f;

            Main.spriteBatch.Reload(BlendState.Additive);

            float colorStrength = MathHelper.Lerp(0, 1f, progress);
            Color frameColor = xFrame == 1 ? Color.Orange : Color.Purple;

            if (xFrame == 1)
            {
                Lighting.AddLight(NPC.Center + new Vector2(26, -28), colorStrength, colorStrength * 0.65f, 0);
            }
            else
            {
                Lighting.AddLight(NPC.Center + new Vector2(26, -28), colorStrength, 0, colorStrength);
            }

            Texture2D glowFrame = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Bandits/ArcherBandit_ArrowGlow").Value;
            spriteBatch.Draw(glowFrame, NPC.Center - screenPos + bowOffset, drawRectangle, frameColor * progress, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }

        private void PostDrawLongShot(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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
            Main.spriteBatch.Reload(BlendState.Additive);

            Color frameColor = xFrame == 1 ? Color.Orange : Color.Purple;
            Texture2D flare = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flare_01").Value;
            float scale = MathHelper.Lerp(0, 1, progress);

            Vector2 flareOffset = xFrame == 1 ? new Vector2(34 * NPC.direction, -3) : new Vector2(26 * NPC.direction, -28);
            spriteBatch.Draw(flare, NPC.Center - screenPos + flareOffset, null, frameColor, NPC.rotation, flare.Size() / 2, scale, spriteEffects, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}
