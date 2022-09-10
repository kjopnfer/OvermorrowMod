using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public partial class Paladin : Mercenary
    {
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // test
        }

        // The position where the paladin should aim attacks
        public Vector2 targetPosition;

        // Used as a stun delay when the paladin has collision while running in the same direction
        public int stunDuration;

        #region Leap
        // If a leap has started (catching up)
        private bool isLeaping;
        // The x value in the parabolic function for the catch up mechanic
        private float leapDist;
        // How long the paladin must stay still before leaping
        private int leapDelay;
        #endregion

        // Used to make ProjectileReact() a one-time action for a projectile
        private bool projReaction;

        #region Draw Variables
        // Used as an alternate direction lock for attacks
        public int hammerDirection;

        // The frame that is used while drawing the NPC
        private Point moveFrame;

        #region Hammer
        // Used for attack animation cycles
        public int hammerDelay;
        public int slamTimer;
        public int throwStyle;
        #endregion

        #region Catch Up
        // Used for hammer slam animation cycles (used in CatchUp())
        private bool catchUpLanded;
        private bool catchUpFinished;
        #endregion

        #region Close Attack
        // Used for differentiating between hammer spin and hammer slam for close attacks
        private bool CAStyleDecided;
        private bool doHammerSpin;
        #endregion

        #endregion

        // Used for the CatchUp() method
        private MathFunctions.Parabola parabola;

        // Determines if the afterimage effect is drawn
        bool drawAfterimage;

        #region Afterimage variables
        //Past position
        Vector2[] imgPos = new Vector2[6] { new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2() };
        //Past frame
        Point[] imgFrame = new Point[6];
        #endregion

        public override List<MercenaryDialogue> Dialogue
        {
            get => PlaceholderDialogue();
        }
        public override string MercenaryName => "Rookie Paladin";
        public override int AttackDelay => 60;
        public override float DetectRadius => 220;
        public override int MaxHealth => Main.expertMode ? 1000 : 500;
        public override int Defense => Main.expertMode ? 60 : 40;
        public override float KnockbackResist => 0f;
        public override int HealDelay => 5;
        public override int MaxFrames() => 15;

        // TODO: Refactor all code for readability
        public override void AI()
        {
            if (stunDuration > 0)
            {
                Main.NewText("stunned: " + stunDuration);
                NPC.velocity.X = 0;

                HealCooldown = 60;
                drawAfterimage = false;
                targetNPC = null;

                stunDuration--;
                FrameUpdate(FrameType.WalkBattle);

                if (stunDuration == 0)
                {
                    // I will explain this since it was confusing me for like a solid half an hour.
                    // The NPC has two velocity values, the base velocity all NPCs have and the unique velocity used to scale speed
                    // Within MovementAI is where this second value is applied. Therefore setting it to zero means it doesn't move at all.
                    // When the NPC is stunned, their second velocity is set to zero and this cannot be called.
                    acceleration = 0.125f;
                    canCheckTiles = true;
                }

                return;
            }

            //Main.NewText("continue attack? " + continueAttack + " / healDelay: " + HealCooldown + " / hammer delay: " + hammerDelay);

            drawAfterimage = false;
            BaseAI();

            // Stun and attackDelay cooldown update
            if (attackDelay > 0) attackDelay--;

            // Allow another projectile reaction to happen if not on a solid tile
            if (projReaction && OnSolidTile() != null) projReaction = false;

            // The paladin will speed up if it goes in the same direction
            if (SameDirection() && !DangerThreshold())
            {
                acceleration += 0.001f * (Spinning() != null ? 50 : 1);
                acceleration = MathHelper.Clamp(acceleration, 0, 0.5f);
            }

            // The paladin moves faster than usual while in danger, but is more careful (the above code does not run)
            // Therefore, the paladin will have a higher base movement speed but with no acceleration
            if (DangerThreshold()) acceleration = 0.2f;

            // The paladin has completely stopped, so reset their acceleration to their base
            if (NPC.velocity.X == 0)
            {
                if (DangerThreshold()) acceleration = 0.2f;
                else acceleration = 0.125f;
            }

            // The paladin is catching up to the player and is not in danger of dying
            if (catchingUp && !DangerThreshold())
            {
                drawAfterimage = true;
                if (catchUpLanded && FrameUpdate(FrameType.CatchUp)) // If the leap has started
                    catchUpFinished = true; // Declare that the paladin has caught up when the animation is finished
            }
            else
            {
                catchUpLanded = false;
                catchUpFinished = false;

                catchingUp = false;
            }

            if (acceleration > 0.4f)
            {
                // Draws an afterimage after the NPC reaches a certain velocity. If the NPC is not in danger of dying, start ramming.
                drawAfterimage = true;

                if (!DangerThreshold())
                {
                    // The paladin will not perform wall or pit checks, and if it runs into a tile (if the projectile collides), the paladin is stunned for about 1 second
                    canCheckTiles = false;
                    if (!IsRamming() && Spinning() == null && HammerAlive() == null)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            PaladinRamHitbox ram = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinRamHitbox>(), 50, 0, hiredBy).ModProjectile as PaladinRamHitbox;
                            ram.owner = this;
                        }
                    }
                }
            }

            if (targetNPC == null || acceleration > 0.4f)
            {
                if ((HammerAlive() == null && !catchingUp) || closeAttackStyle) // Resets attack variables
                {
                    //hammerDelay = 0;
                    //slamTimer = 0;
                    throwStyle = 0;
                    targetPosition = Vector2.Zero;
                }
                else if (HammerAlive() != null)
                    NPC.velocity.X = 0;

                // Starts the "walk" animation cycle
                if (targetNPC == null && !CanHeal && !catchingUp && !CAStyleDecided) FrameUpdate(FrameType.Walk);
            }

            // Starts the "walkBattle" animation cycle if the paladin finds an enemy
            if (targetNPC != null)
            {
                if (!CAStyleDecided)
                {
                    if (!CanHeal && !catchingUp || (HammerAlive() == null && (!closeAttackStyle || DangerThreshold())))
                        FrameUpdate(FrameType.WalkBattle);
                }
            }

            if (drawAfterimage)
            {
                imgFrame[5] = new Point(xFrame, yFrame);
                imgPos[5] = NPC.Center;

                for (int i = 1; i < 6; i++)
                {
                    imgFrame[i - 1] = imgFrame[i];
                    imgPos[i - 1] = imgPos[i];
                }
            }
        }

        public override bool RestoreHealth()
        {
            bool finished = false;
            if (OnSolidTile() != null)
            {
                FrameUpdate(FrameType.Heal);
                // If the NPC is on a solid tile, slow the NPC to a halt, and when the NPC is not moving, wait two seconds, then heal
                HealTimer++;
                Main.NewText("heal timer: " + HealTimer);


                if (targetNPC == null && incomingProjectile == null) NPC.velocity.X *= 0.125f;

                if (HealTimer++ >= 60 && Spinning() == null && HammerAlive() == null)
                {
                    NPC.velocity.X = 0;

                    if (HealTimer % 15 == 0)
                    {
                        Vector2 randomOffset = new Vector2(Main.rand.Next(-16, 16) * 2, Main.rand.Next(-16, 0) * 2);
                        Particle.CreateParticle(Particle.ParticleType<Ember>(), NPC.Bottom + randomOffset, -Vector2.UnitY, new Color(240, 221, 137));
                    }

                    if (HealTimer % 30 == 0 && HealTimer < 180) Heal();

                    // When healing is done, return true (and reset variables)
                    if (HealTimer >= 300)
                    {
                        CanHeal = false;
                        HealCooldown = HealDelay * 60;
                        HealTimer = 0;

                        finished = true;
                    }
                }
            }

            return finished;
        }

        public override void Heal()
        {
            int heal = 25;

            if (NPC.life + heal < NPC.lifeMax)
            {
                NPC.life += heal;
                CombatText.NewText(NPC.getRect(), Color.SpringGreen, $"{heal}", false, false);
            }
            else
            {
                if (NPC.life < NPC.lifeMax)
                {
                    int overflow = NPC.lifeMax - NPC.life;
                    NPC.life += overflow;
                    CombatText.NewText(NPC.getRect(), Color.SpringGreen, $"{overflow}", false, false);
                }
            }
        }

        public override void SafetyBehaviour()
        {
            if (targetNPC != null)
            {
                //Create an NPC decoy of the detected threat to pass in StandardAI(), then walk towards **the decoy's defined position
                //**A far distance away from the detected threat based on the threat's width
                NPC threat = targetNPC;
                if (threat != null)
                {
                    Vector2 location = threat.Center + new Vector2(30 * (NPC.Center.X < threat.Center.X ? -1 : 1) * (NPC.width / 2), 0);
                    NPC decoy = new NPC();
                    decoy.width = threat.width;
                    decoy.height = threat.height;
                    decoy.Center = location;

                    if (DangerThreshold())
                    {
                        hammerDirection = NPC.velocity.X < 0 ? -1 : 1;
                        NPC.direction = hammerDirection;
                        MovementAI(Rect(decoy));
                    }
                }
            }
            //else
            //StandardAI(Rect(FollowPlayer()));
        }

        public override void ProjectileReact()
        {
            // This is supposed to send the NPC in a direction perpendicular to the projectile, but it needs to be reworked, along with the projectile check itself
            if (HammerAlive() == null && CanHeal && HealCooldown > 0)
            {
                Vector2 velocity = incomingProjectile.velocity + new Vector2(10 * (incomingProjectile.velocity.X < 0 ? -1 : 1), 10 * (incomingProjectile.velocity.Y < 0 ? -1 : 1));
                if (!projReaction)
                {
                    projReaction = true;
                    float inverse = (-velocity.X / velocity.Y);
                    float x = (inverse * velocity.X) * (velocity.Y / velocity.X);
                    NPC.velocity += new Vector2(x, x * inverse);

                    if (NPC.velocity.Y > 0) NPC.velocity.Y *= -1;

                    NPC.velocity = new Vector2(MathHelper.Clamp(NPC.velocity.X, -5, 5), MathHelper.Clamp(NPC.velocity.Y, -5, 0));
                    NPC.position += NPC.velocity;
                }
            }
        }

        public override bool CatchUp(Vector2 location = new Vector2())
        {
            Player player = Main.player[hiredBy];
            if (!catchUpLanded)
            {
                if (HammerAlive() == null && Spinning() == null)
                {
                    if (leapDelay >= 40) // Occurs after 2/3rds of a second
                    {
                        if (!isLeaping)
                        {
                            if (OnSolidTile() != null || NPC.velocity.Y == 0) // If on a solid tile
                            {
                                // Create a parabolic function that is expected to be done in at least 1/2 a second
                                // The parabolic function is from the NPC center, to the expected player's position plus an additive to prevent extremely small distanced leaps
                                Vector2 place = location != new Vector2() ? location : player.MountedCenter;
                                Vector2 additive = Vector2.Zero;

                                if (Math.Abs(NPC.Center.X - place.X) < 75) additive.X = place.X < NPC.Center.X ? -125 : 125;

                                if (Math.Abs(NPC.Center.Y - place.Y) < 50) additive.Y = -35;

                                parabola = MathFunctions.Parabola.ParabolaFromCoords(NPC.Center, place + additive);
                                parabola.SetIncrement(30);
                                isLeaping = true;
                                moveFrame = new Point(0, 3);
                            }
                        }
                        else
                        {
                            // Updates the parabola, and updates the velocity based on the y value from the parabola
                            drawAfterimage = true;
                            NPC.noTileCollide = true;
                            leapDist += parabola.increment[0];
                            NPC.velocity.Y = parabola.GetY(leapDist - parabola.increment[0]) - parabola.GetY(leapDist);
                            NPC.velocity.X = parabola.increment[0] * (parabola.backwards ? -1 : 1);
                            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                        }

                        if (parabola != null && leapDist >= ((parabola.z1 + parabola.z2) * 0.5f))
                        {
                            // If the NPC is on a solid tile, invert velocity to prevent falling through platforms, and set the position directly above the tile
                            // Create a screenshake event and reset leap variables
                            // This will only be called once, and will set a variable for the hammer slam animation to play;
                            // This method will be called until that animation is finished
                            Tile tile = OnSolidTile();
                            if (tile != new Tile())
                            {
                                NPC.velocity.Y *= -0.5f;
                                NPC.position.Y = MathFunctions.AGF.Round(NPC.Center.Y - 8) - 24;
                                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0, -4);
                                NPC.noTileCollide = false;
                                ScreenShake.ScreenShakeEvent(NPC.Center, 15, 4.5f, 400);
                                isLeaping = false;
                                leapDist = 0;
                                leapDelay = 0;
                                catchUpLanded = true;
                            }
                        }
                    }
                    else
                    {
                        // While waiting to leap, set the frame to a prepared stance 
                        moveFrame = new Point(3, 4);
                        leapDelay++;
                    }
                }
            }

            return catchUpFinished;
        }

        /// <summary>
        /// Determines whether the Paladin is not spinning, the hammer has not been thrown and there is no attack delay
        /// </summary>
        /// <returns></returns>
        bool CanAttack() => HammerAlive() == null && attackDelay < 1;

        public override bool FarAttack()
        {
            Vector2 scout;
            if (targetNPC != null) // Sets the target, and makes the NPC face towards it
            {
                scout = targetNPC.Center;
                targetPosition = scout;
                NPC.direction = NPC.Center.X < scout.X ? 1 : -1;
                hammerDirection = NPC.direction;
            }

            if (CanAttack())
            {
                // Perform three different kinds of hammer throws; each of the motions are solely performed by the projectile

                Main.NewText("throw hamber");

                throwStyle++;
                if (throwStyle > 3) throwStyle = 1;

                hammerDelay = 0;

                PaladinHammer hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammer>(), 20, 1f, hiredBy).ModProjectile as PaladinHammer;
                hammer.owner = this;
                hammer.direction = targetPosition.X < NPC.Center.X ? -1 : 1;
                hammer.targetPosition = targetPosition;

                switch (throwStyle)
                {
                    case 2:
                        hammer.startValue = 3;
                        hammer.endValue = 4.9f;
                        break;
                    case 3:
                        hammer.startValue = 5;
                        hammer.endValue = 5;
                        break;
                    default:
                        hammer.startValue = 1;
                        hammer.endValue = 3;
                        break;
                }
            }

            return HammerAlive() != null && targetNPC != null;
        }

        public override bool CloseAttack()
        {
            HealCooldown = 60;

            if (!CAStyleDecided)
            {
                // Check the distance between the enemy and the NPC; if *really* close, perform a hammer spin, otherwise perform a hammer slam
                if (targetNPC != null)
                {
                    Vector2 scout = targetNPC.Center;
                    float general = Vector2.Distance(new Vector2(scout.X, 0), new Vector2(NPC.Center.X, 0));
                    float height = Vector2.Distance(new Vector2(scout.Y, 0), new Vector2(NPC.Center.Y, 0));

                    if (!doHammerSpin) doHammerSpin = height > 50 || OnSolidTile() == null || general < 75;

                    // Turn off default town NPC shit
                    AIType = -1;

                    // We don't want any of the attacks to be interrupted after they've started
                    continueClose = true;
                    hammerDirection = targetNPC.Center.X > NPC.Center.X ? 1 : -1;
                    acceleration = 0.0625f;

                    hammerDelay = 60;
                    CAStyleDecided = true;
                }
            }

            if (doHammerSpin)
            {
                if (spinCounter++ == 300)
                {
                    if (Spinning() != null) Spinning().Projectile.Kill();

                    spinCounter = 0;
                    stunDuration = 90;

                    CAStyleDecided = false;
                    doHammerSpin = false;
                    continueClose = false;

                    AIType = 0;
                    attackDelay = AttackDelay;

                    Main.NewText("stun");
                    return false;
                }

                FrameUpdate(FrameType.HammerSpin);

                // Go towards the target
                if (!DangerThreshold() && targetNPC != null && spinCounter < 210) MovementAI(Rect(targetNPC));

                // Create a projectile that moves forth and back from the paladin
                int spinDamage = 5;
                float knockBack = 1f;

                // Basically decelerates near the end of the maximum spin time
                if (spinCounter > 280)
                {
                    spinDamage = 0;
                    knockBack = 0f;
                }
                else if (spinCounter > 250)
                {
                    spinDamage = 5;
                    knockBack = 1f;
                }
                else if (spinCounter > 210)
                {
                    spinDamage = 15;
                    knockBack = 0.5f;
                }
                else if (spinCounter > 150)
                {
                    spinDamage = 20;
                    knockBack = 0.2f;
                }
                else if (spinCounter > 90)
                {
                    spinDamage = 15;
                    knockBack = 0.5f;
                }
                else if (spinCounter > 30)
                {
                    spinDamage = 10;
                    knockBack = 1f;
                }

                if (spinCounter > 90 && spinCounter < 260) drawAfterimage = true;

                PaladinHammerSpin hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammerSpin>(), spinDamage, knockBack, hiredBy).ModProjectile as PaladinHammerSpin;
                hammer.owner = this;

                // Keep the spinning projectile alive while he is spinning
                Spinning().Projectile.timeLeft = 2;

                // Always returns true, the NPC will only exit out of this when he has spun for 5 seconds.
                return true;
            }
            else
            {
                FrameUpdate(FrameType.HammerSlam);

                if (hammerDelay > 0)
                {
                    yFrame = 2;
                    hammerDelay--;
                }
                else
                {
                    slamTimer++;
                    if (slamTimer >= 32)
                    {
                        yFrame = 6;
                        if (slamTimer == 32)
                        {
                            for (int i = 0; i < Main.rand.Next(16, 24); i++)
                            {
                                float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                                //float randomTime = Main.rand.Next(5, 7) * 10;
                                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(80), MathHelper.ToRadians(80));
                                Vector2 RandomVelocity = -Vector2.UnitY.RotatedBy(randomAngle) * Main.rand.Next(9, 15);
                                Color color = Color.Orange;

                                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), NPC.Center + new Vector2(32 * hammerDirection, 24), RandomVelocity, color, 1, randomScale);
                            }

                            ScreenShake.ScreenShakeEvent(NPC.Center, 15, 2, 250);
                        }
                    }
                    else if (slamTimer > 28) yFrame = 5;
                    else if (slamTimer > 24)
                    {
                        yFrame = 4;

                    }
                    else if (slamTimer > 20)
                    {
                        yFrame = 3;
                        if (slamTimer == 21)
                        {
                            Projectile.NewProjectileDirect(Source(), NPC.Center + new Vector2(32 * hammerDirection, -16), Vector2.Zero, ModContent.ProjectileType<LightExplosion>(), 65, 12f, hiredBy);

                            for (int i = 0; i < Main.rand.Next(4, 7); i++)
                            {
                                float randomScale = Main.rand.NextFloat(1f, 2f);
                                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(80), MathHelper.ToRadians(80));
                                Vector2 RandomVelocity = -Vector2.UnitY.RotatedBy(randomAngle) * Main.rand.Next(4, 8);
                                Color color = Color.Orange;

                                Particle.CreateParticle(Particle.ParticleType<Flash>(), NPC.Center + new Vector2(32 * hammerDirection, 24), RandomVelocity, color, 1, randomScale);
                                Particle.CreateParticle(Particle.ParticleType<Bubble>(), NPC.Center + new Vector2(32 * hammerDirection, 24), RandomVelocity, color, 1, randomScale);
                            }
                        }
                    }

                    if (slamTimer == 152)
                    {
                        continueClose = false;
                        CAStyleDecided = false;

                        slamTimer = 0;
                        hammerDelay = 60;
                        yFrame = 0;

                        AIType = 0;

                        return false;
                    }
                }

                //Main.NewText("decide on slam, delay: " + hammerDelay + " / slamTimer: " + slamTimer);
                return true;
            }
        }

        // Doing this in order to make the NPC slide around when doing the hammer spin
        public override void MovementAI(RBC target)
        {
            Vector2 targetPosition = target.position;

            // When safe: When the player is not too far
            // Otherwise: When in danger and the target (generally scoutNPC) is not null
            if (!TooFar() || (DangerThreshold() && targetPosition != Vector2.Zero))
            {
                // Checks the vertical distance to the target; some randomization is added with runCond[0]
                bool NextToTarget() => Vector2.Distance(new Vector2(NPC.Center.X), new Vector2(targetPosition.X)) < runCond[0];

                if (hiredBy != -1)
                {
                    if (!NextToTarget() || targetPosition != Vector2.Zero)
                    {
                        // Add randomization for NextToTarget()
                        if (runCond[1] == runCond[0]) runCond[0] = Main.rand.Next(150, 400);

                        // Determines which direction the NPC should go
                        bool moveCondition = direction != 0 && NPC.velocity.Y != 0 && groundDetectPos != Vector2.Zero ? (direction > 0) : (NPC.Center.X < targetPosition.X);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                        NPC.velocity.X += moveCondition ? acceleration : -acceleration;
                        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -7, 7);

                        // The mercenary cannot check tiles if it is in the air
                        if (NPC.velocity.Y == 0)
                        {
                            if (groundDetectPos == Vector2.Zero) direction = 0;
                            if (canCheckTiles) CheckTiles();
                        }
                    }
                    else
                        runCond[1] = runCond[0];

                    FallThrough(target, targetPosition);
                }
            }
        }

        // Doing this because the NPC keeps jumping when doing the spin attack and messing with afterimages
        public override void CheckTiles()
        {
            // Changes the position of the wall detection scanner
            wallDetectPos = new Vector2(NPC.Center.X + (NPC.width * (NPC.direction == 1 ? 0.25f : -0.25f)), (NPC.Center.Y + 8) - (detectTimer[0] * 16));

            #region Wall check
            // If there is no obstruction between the scanned position, and the wall detection scanner, there is no tile there
            bool noWall = Collision.CanHitLine(wallDetectPos, 12, 12, new Vector2(wallDetectPos.X + (96 * (NPC.direction == 1 ? 1 : -1)), wallDetectPos.Y - 8), 12, 12);
            if (!noWall) detectValue[0]++;

            // A minimum amount of time must pass until a leap can be performed (>= num represents how many tiles can be accounted upwards)
            if (detectTimer[0]++ >= 5)
            {
                // Make the mercenary jump a height based on how many obstructions were found, and reset the values
                #region Wall check jump
                if (detectValue[0] > 0 && !doHammerSpin) NPC.velocity.Y -= (detectValue[0] * 6f) * (1.5f / detectValue[0]);

                #endregion
                detectValue[0] = 0;
                detectTimer[0] = 0;
            }
            #endregion

            #region Horizontal check
            if (groundDetectPos != Vector2.Zero)
            {
                // Like wall detection, but it checks downwards and also checks for a **gap quota
                bool pit = Pit(0) && Pit(1);
                if (pit)
                {
                    //**The gap must be at least two tiles wide in order for the mercenary to jump; to prevent unnecessary leaps
                    if (++checkGap == 2)
                    {
                        // Sets a place for the mercenary to go towards to perform the leap
                        pitJumpRelative = groundDetectPos;
                        direction = NPC.Center.X - groundDetectPos.X < 0 ? 1 : -1;
                    }

                    detectValue[1]++;
                }
                else
                    checkGap = 0;

                // Same story with wall detection; can check a maximum of around 17 tiles before checking if a leap is needed
                if (detectTimer[1] < 17) detectTimer[1]++;

                if (detectTimer[1] >= 15)
                {
                    // If a leap is needed (and the quota is met i.e. "checkGap == 2") 
                    if (pitJumpRelative != Vector2.Zero)
                    {
                        //perform a leap if the NPC is close enough to where pitJumpRelative was set (where the leap should be performed)
                        float dist = Vector2.Distance(new Vector2(NPC.Center.X, 0), new Vector2(pitJumpRelative.X, 0)) * (NPC.direction == -1 ? 0.5f : 1);
                        float condition = (16 * 4) / (NPC.direction == -1 ? 2 : 1);
                        // Erases the expected leap position after a brief period (brief because the NPC would be near it anyways)
                        if (++pitJumpExpire >= 15) pitJumpRelative = Vector2.Zero;

                        //if close enough to the expected position
                        if (dist < condition && condition < 75)
                        {
                            // Perform a leap based on how many tiles were detected, and reset the values
                            #region Wall check jump
                            if (detectValue[1] > 0)
                                NPC.velocity.Y -= (detectValue[1] * 6.75f) * (1.45f / detectValue[1]);
                            #endregion

                            pitJumpExpire = 0;
                            detectValue[1] = 0;
                            detectTimer[1] = 0;
                            pitJumpRelative = Vector2.Zero;
                        }
                    }
                    else // If a leap was not expected, reset values
                    {
                        pitJumpExpire = 0;
                        detectValue[1] = 0;
                        detectTimer[1] = 0;
                    }
                }
            }

            // Update the pit detection position
            if (detectValue[0] < 1)
                groundDetectPos = new Vector2(NPC.Center.X + (detectTimer[1] * (NPC.direction == 1 ? 16f : -16f)), NPC.Center.Y + (NPC.height / 2.33f));
            else
                groundDetectPos = Vector2.Zero;
            #endregion
        }

        int xFrame = 1;
        int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 4;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = frameHeight * yFrame;
        }

        public enum FrameType
        {
            Walk,
            WalkBattle,
            HammerSpin,
            HammerSlam,
            Heal,
            HammerChuck,
            HammerChuckAwait,
            CatchUp
        }

        public int spinCounter = 0;
        float tempCounter = 0;
        private bool FrameUpdate(FrameType type)
        {
            switch (type)
            {
                #region Walk
                case FrameType.Walk:
                    {
                        xFrame = 1;

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
                            if (yFrame < 2 || yFrame == 13) yFrame = 2;

                            // Change the walking frame at a speed depending on the velocity
                            int walkRate = (int)Math.Round(Math.Abs(NPC.velocity.X));
                            tempCounter += walkRate;
                            if (tempCounter > 8)
                            {
                                yFrame++;
                                tempCounter = 0;
                            }
                        }

                        return true;
                    }
                #endregion
                #region WalkBattle
                case FrameType.WalkBattle:
                    {
                        // Just like "walk", but at different Y frames
                        xFrame = 0;

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
                            if (yFrame < 2 || yFrame == 13) yFrame = 2;

                            // Change the walking frame at a speed depending on the velocity
                            int walkRate = (int)Math.Round(Math.Abs(NPC.velocity.X));
                            tempCounter += walkRate;
                            if (tempCounter > 8)
                            {
                                yFrame++;
                                tempCounter = 0;
                            }
                        }

                        return true;
                    }
                #endregion
                #region HammerSpin
                case FrameType.HammerSpin:
                    {
                        xFrame = 3;
                        NPC.direction = hammerDirection;

                        if (yFrame < 7 || yFrame == 14) yFrame = 7;

                        // The longer the paladin is spinning, the faster they go. Slows down near the end.
                        float spinSpeed = 1;
                        if (spinCounter > 280) spinSpeed = 0f;
                        else if (spinCounter > 250) spinSpeed = 0.5f;
                        else if (spinCounter > 240) spinSpeed = 1;
                        else if (spinCounter > 210) spinSpeed = 3;
                        else if (spinCounter > 150) spinSpeed = 6;
                        else if (spinCounter > 90) spinSpeed = 3;
                        else if (spinCounter > 30) spinSpeed = 2;

                        tempCounter += spinSpeed;
                        if (tempCounter >= 6)
                        {
                            tempCounter = 0;
                            yFrame++;
                        }

                        return true;
                    }
                #endregion
                #region HammerSlam
                case FrameType.HammerSlam: // The frames are handled in CloseAttack()
                    {
                        xFrame = 3;
                        NPC.direction = hammerDirection;

                        return true;
                    }
                #endregion
                #region Heal
                case FrameType.Heal:
                    xFrame = 2;
                    yFrame = 1;

                    return true;
                #endregion
                #region HammerChuck
                case FrameType.HammerChuck:
                    xFrame = 3;
                    yFrame = 2;

                    return true;
                #endregion
                case FrameType.CatchUp:
                    {
                        xFrame = 3;

                        // Or "hammer slam"; default to "prepareSwing" (0, 4), then move the X frame every 10 ticks
                        slamTimer++;
                        moveFrame.Y = 4;
                        switch (slamTimer)
                        {
                            case 10:
                                moveFrame.X = 1;
                                break;
                            case 20:
                                moveFrame.X = 2;
                                break;
                            case 30:
                                moveFrame.X = 3;
                                break;
                        }

                        return slamTimer >= 40;
                    }
            }

            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Each frame on the spritesheet is 116 x 72
            const int frameWidth = 116;
            const int frameHeight = 72;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (CAStyleDecided) spriteEffects = hammerDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (drawAfterimage)
            {
                for (int i = 0; i < imgPos.Length; i++)
                {
                    if (imgPos[i] == new Vector2()) continue;

                    Rectangle drawRectangle = new Rectangle(frameWidth * imgFrame[i].X, frameHeight * imgFrame[i].Y, frameWidth, frameHeight);
                    spriteBatch.Draw(texture, imgPos[i] - Vector2.UnitY * 6 - screenPos, drawRectangle, drawColor * ((float)(i - imgPos.Length + 6.125f) / (imgPos.Length)), NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 1f);
                }
            }

            spriteBatch.Draw(texture, NPC.Center - Vector2.UnitY * 6 - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            int directionOffset = 4 * -hammerDirection;

            if (CAStyleDecided && !doHammerSpin)
            {
                float progress = MathHelper.Lerp(0, 1, 1 - (hammerDelay / 60f));
                if (hammerDelay == 0 && slamTimer >= 32) progress = MathHelper.Lerp(1, 0, Utils.Clamp(slamTimer - 24, 0, 60) / 60f);

                #region Glow
                Main.spriteBatch.Reload(BlendState.Additive);
                Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/Paladin_Hammer_Glow").Value;
                Rectangle drawRectangle = new Rectangle(0, frameHeight * yFrame, frameWidth, frameHeight);

                float scale = MathHelper.Lerp(NPC.scale, NPC.scale * 1.025f, progress);
                Color color = Color.Lerp(Color.Transparent, new Color(240, 221, 137), progress);
                spriteBatch.Draw(glowTexture, NPC.Center - new Vector2(directionOffset, 6) - screenPos, drawRectangle, color, NPC.rotation, drawRectangle.Size() / 2, scale, spriteEffects, 0);

                color = Color.Lerp(Color.Transparent, Color.Orange, progress);
                spriteBatch.Draw(glowTexture, NPC.Center - new Vector2(directionOffset, 6) - screenPos, drawRectangle, color, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);
                #endregion

                #region Shader
                Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                Texture2D hammerTexture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/Paladin_Hammer").Value;
                spriteBatch.Draw(hammerTexture, NPC.Center - new Vector2(directionOffset, 6) - screenPos, drawRectangle, color, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
                #endregion

                Texture2D armTexture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/Paladin_Arm").Value;
                spriteBatch.Draw(armTexture, NPC.Center - new Vector2(directionOffset, 6) - screenPos, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 0);

                if (hammerDelay <= 15 && slamTimer < 20)
                {
                    Main.spriteBatch.Reload(BlendState.Additive);

                    Texture2D flare = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flare_01").Value;
                    scale = MathHelper.Lerp(1, 0, Utils.Clamp(hammerDelay - 12, 0, 5) / 5f);
                    if (slamTimer > 0) scale = MathHelper.Lerp(1, 0, Utils.Clamp(slamTimer - 18, 0, 2) / 2f);

                    spriteBatch.Draw(flare, NPC.Center + new Vector2(-32 * hammerDirection, -16) - screenPos, null, Color.Orange, NPC.rotation, flare.Size() / 2, scale, spriteEffects, 0);

                    Main.spriteBatch.Reload(BlendState.AlphaBlend);
                }
            }

            if (CanHeal)
            {
                float progress = Utils.Clamp(HealTimer, 0, 120) / 120f;
                if (HealTimer >= 180) progress = MathHelper.Lerp(120, 0, Utils.Clamp(HealTimer - 180, 0, 120) / 120f) / 120f;

                Color color = Color.Lerp(Color.Transparent, new Color(240, 221, 137) * 0.8f, progress);

                spriteBatch.Reload(BlendState.Additive);
                Texture2D handGlow = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/Heal_Hand_Glow").Value;
                spriteBatch.Draw(handGlow, NPC.Center - new Vector2(0, 6) - screenPos, null, color, NPC.rotation, handGlow.Size() / 2, NPC.scale, spriteEffects, 0);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
                spriteBatch.Reload(BlendState.AlphaBlend);

                Main.spriteBatch.Reload(SpriteSortMode.Immediate);
                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                effect.Parameters["WhiteoutColor"].SetValue(new Color(240, 221, 137).ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(Utils.Clamp(progress, 0, 0.2f));
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                Texture2D hand = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/Heal_Hand").Value;
                spriteBatch.Draw(hand, NPC.Center - new Vector2(0, 6) - screenPos, null, drawColor, NPC.rotation, hand.Size() / 2, NPC.scale, spriteEffects, 0);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);

                spriteBatch.Reload(BlendState.Additive);
                Texture2D healRay = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Extra_60").Value;

                spriteBatch.Draw(healRay, NPC.Center + new Vector2(6, 18) - screenPos, null, color, NPC.rotation, healRay.Size() / 2, NPC.scale, spriteEffects, 0);

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            return false;
        }
    }
}
