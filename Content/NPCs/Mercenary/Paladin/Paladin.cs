using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent;
using OvermorrowMod.Core;
using Terraria.DataStructures;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public partial class Paladin : Mercenary
    {
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

        #region Walk
        // Used in FrameUpdate() for walk cycles
        private int moveFrameY;
        private int moveTimer;
        #endregion

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
        public override float DetectRadius => 350;
        public override int MaxHealth => 500;
        public override int Defense => 40;
        public override float KnockbackResist => 0.66f;
        public override int HealCooldown => 10;

        // The maximum number of Y frames the NPC has
        public override int MaxFrames() => 15;

        // TODO: Refactor all code for readability
        public override void AI()
        {
            drawAfterimage = false;
            BaseAI();

            if (targetNPC == null && !DangerThreshold()) // Remove direction lock
            {
                hammerDirection = 0;
                closeAttackStyle = false;
            }

            if (targetNPC == null)
            {
                CAStyleDecided = false;
                doHammerSpin = false;
            }

            // Allow another projectile reaction to happen if not on a solid tile
            if (projReaction && OnSolidTile() != null) projReaction = false;

            // The paladin will speed up if it goes in the same direction
            if (SameDirection() && !DangerThreshold())
            {
                velocity += 0.001f * (Spinning() != null ? 50 : 1);
                velocity = MathHelper.Clamp(velocity, 0, 0.5f);
            }
            else if (stunDuration < 1)
            {
                velocity = 0.125f;
                canCheckTiles = true;
            }

            // The paladin moves faster than usual while in danger, but is more careful (the above code does not run)
            if (DangerThreshold()) velocity = 0.2f;

            // The paladin is catching up to the player and is not in danger of dying
            if (catchingUp && !DangerThreshold())
            {
                drawAfterimage = true;
                if (catchUpLanded) // If the leap has started
                    if (FrameUpdate(FrameType.CatchUp)) // Declare that the paladin has caught up when the animation is finished
                        catchUpFinished = true;
            }
            else
            {
                catchUpLanded = false;
                catchUpFinished = false;

                catchingUp = false;
            }

            if (velocity > 0.4f)
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

            if (targetNPC == null || velocity > 0.4f)
            {
                if ((HammerAlive() == null && !catchingUp) || closeAttackStyle) // Resets attack variables
                {
                    hammerDelay = 0;
                    slamTimer = 0;
                    throwStyle = 0;

                    //hammerChuck = new int[3];
                    targetPosition = Vector2.Zero;
                }
                else if (HammerAlive() != null)
                    NPC.velocity.X = 0;

                // Starts the "walk" animation cycle
                if (targetNPC == null && HammerAlive() == null && !catchingUp) FrameUpdate(FrameType.Walk);
            }

            // Starts the "walkBattle" animation cycle if the paladin finds an enemy
            if (targetNPC != null)
            {
                if (restore[0] > 0 && !catchingUp || (HammerAlive() == null && (!closeAttackStyle || DangerThreshold())))
                    FrameUpdate(FrameType.WalkBattle);
            }

            // Stun and attackDelay cooldown update
            if (stunDuration-- < 0) stunDuration = 0;
            if (attackDelay-- < 0) attackDelay = 0;

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

        public enum FrameType
        {
            Walk,
            WalkBattle,
            HammerSpin,
            HammerChuck,
            HammerChuckAwait,
            CatchUp
        }

        public override bool RestoreHealth()
        {
            bool finished = false;
            NPC.direction = NPC.velocity.X < 1 ? -1 : 1;
            if (OnSolidTile() != null)
            {
                // If the NPC is on a solid tile, slow the NPC to a halt, and when the NPC is not moving, wait two seconds, then heal
                restore[1]++;
                if (targetNPC == null && incomingProjectile == null) NPC.velocity.X *= 0.125f;

                if (restore[1]++ >= 60 && Spinning() == null && HammerAlive() == null)
                {
                    NPC.velocity.X = 0;
                    if (restore[1] >= 120)
                    {
                        Heal();
                        restore[0] = 2;
                        restore[1] = 0;
                    }
                }
            }

            // When healing is done, return true (and reset variables)
            if (restore[0] == 2)
            {
                if (restore[1]++ > 60)
                {
                    restore = new int[3];
                    finished = true;
                }
            }

            return finished;
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
                        StandardAI(Rect(decoy));
                    }
                }
            }
            //else
            //StandardAI(Rect(FollowPlayer()));
        }

        // Reset the needed healing charge to perform RestoreHealth()
        public override void HitEffect(int hitDirection, double damage) { restore[1] = 0; }
        public override void ProjectileReact()
        {
            // This is supposed to send the NPC in a direction perpendicular to the projectile, but it needs to be reworked, along with the projectile check itself
            if (HammerAlive() == null && restore[0] < 1)
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
        bool CanAttack() => Spinning() == null && HammerAlive() == null && attackDelay < 1;
        public override bool FarAttack()
        {
            Vector2 scout;
            // Sets the target, and makes the NPC face towards it
            if (targetNPC != null)
            {
                scout = targetNPC.Center;
                targetPosition = scout;
                NPC.direction = NPC.Center.X < scout.X ? 1 : -1;
                hammerDirection = NPC.direction;
            }

            if (CanAttack())
            {
                // Perform three different kinds of hammer throws; each of the motions are solely performed by the projectile
                bool check = throwStyle == 2 ? true : FrameUpdate(FrameType.HammerChuck);
                if (check)
                {
                    throwStyle++;
                    if (throwStyle > 3) throwStyle = 1;

                    hammerDelay = 0;
                    slamTimer = 1;

                    PaladinHammer hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammer>(), 20, 1f, hiredBy).ModProjectile as PaladinHammer;
                    hammer.owner = this;
                    hammer.direction = targetPosition.X < NPC.Center.X ? -1 : 1;
                    hammer.startEnd = Start();
                }
                else
                    slamTimer = 0;
            }

            // If a hammer is out and not on the third kind of hammer throw, set to a crouched stance
            if (throwStyle == 3 && slamTimer == 1) FrameUpdate(FrameType.HammerChuckAwait);

            return HammerAlive() == null && targetNPC == null;
        }

        public override bool CloseAttack()
        {
            if (!CAStyleDecided)
            {
                // Check the distance between the enemy and the NPC; if *really* close, perform a hammer spin, otherwise perform a hammer slam
                Vector2 scout;
                if (targetNPC != null)
                {
                    scout = targetNPC.Center;
                    float general = Vector2.Distance(new Vector2(scout.X, 0), new Vector2(NPC.Center.X, 0));
                    float height = Vector2.Distance(new Vector2(scout.Y, 0), new Vector2(NPC.Center.Y, 0));
                    doHammerSpin = height > 50 || OnSolidTile() == null || general < 75;
                    CAStyleDecided = true;
                }
            }

            if (doHammerSpin)
            {
                drawAfterimage = true;
                FrameUpdate(FrameType.HammerSpin);

                // Lock the direction to forward to prevent unusual appearance
                hammerDirection = 1;
                if (targetNPC != null) NPC.direction = NPC.Center.X < targetNPC.Center.X ? 1 : -1;

                velocity = 0.33f;

                // Go towards the target
                if (!DangerThreshold() && targetNPC != null) StandardAI(Rect(targetNPC));

                // Create a projectile that moves forth and back from the paladin
                if (CanAttack())
                {
                    PaladinHammerSpin hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammerSpin>(), 20, 1f, hiredBy).ModProjectile as PaladinHammerSpin;
                    hammer.owner = this;
                }

                // If there is still an enemy, keep the spinning projectile alive
                if (Spinning() != null && targetNPC != null) Spinning().Projectile.timeLeft = 2;

                // Revert the paladin's stats to normal
                if (Spinning() == null)
                {
                    NPC.defense = Defense;
                    NPC.knockBackResist = KnockbackResist;

                    CAStyleDecided = false;
                    doHammerSpin = false;

                    imgPos = new Vector2[6] { new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2() };
                    Array.Clear(imgFrame);
                    //closeAttack = new bool[2];
                }

                return Spinning() == null && targetNPC == null; ;
            }
            else
            {
                // Face the paladin towards the enemy, and keep it still
                if (attackDelay < 0) hammerDirection = targetNPC.Center.X < NPC.Center.X ? -1 : 1;

                NPC.velocity.X = 0;

                if (FrameUpdate(FrameType.CatchUp) && attackDelay < 1) slamTimer = 0;

                // Create a projectile with lerped motion towards the faced direction, and shake the screen
                if (slamTimer > 9 && Shockwave() == null && attackDelay < 1)
                {
                    ScreenShake.ScreenShakeEvent(NPC.Center, 15, 9, 250);
                    PaladinHammerHit shockwave = Projectile.NewProjectileDirect(Source(), NPC.Center, new Vector2(hammerDirection == -1 ? -16 : 16, 0), ModContent.ProjectileType<PaladinHammerHit>(), 30, 1, hiredBy).ModProjectile as PaladinHammerHit;
                    shockwave.owner = this;
                    attackDelay = AttackDelay + 20;
                }
                else if (slamTimer < 10) // Set the frames to a standing walkBattle frame in the middle of the delay
                    moveFrame = new Point(0, 3);

                if (attackDelay > 0 && attackDelay < 40) moveFrame = new Point(1, 7);

                // Check for the nearest NPC if a hammer spin attack is necessary
                if (attackDelay < 0)
                {
                    CAStyleDecided = false;
                    doHammerSpin = false;

                    //closeAttack = new bool[2];
                }

                return Shockwave() == null && moveFrame.X > 0; ;
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

        int tempCounter = 0;
        private bool FrameUpdate(FrameType type, bool condition = true)
        {
            #region Key frames
            Point prepareSwing = new Point(0, 3);
            Point getHammerStance = new Point(1, 3);
            Point holdHammerFall = new Point(0, 7);
            Point holdHammerStill = new Point(1, 7);
            #endregion

            if (condition)
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
                            int add = (int)Math.Round(Math.Abs(NPC.velocity.X));
                            moveTimer += add;
                            if (moveTimer > 8)
                            {
                                moveTimer = 0;
                                moveFrameY++;
                            }

                            if (moveFrameY <= 5)
                            {
                                moveFrame.Y = 7;
                                moveFrame.X = moveFrameY + 1;

                                if (moveFrameY > 4) moveFrame.Y = 8;
                            }

                            if (moveFrameY <= 12 && moveFrameY > 5)
                            {
                                moveFrame.Y = 8;
                                moveFrame.X = moveFrameY - 6;
                                if (moveFrameY > 11)
                                {
                                    moveFrameY = 1;
                                    moveFrame.Y = 7;
                                }
                            }

                            if (NPC.velocity.Y != 0) moveFrame = holdHammerFall;

                            if (NPC.velocity.Y == 0 && NPC.velocity.X == 0) moveFrame = holdHammerStill;

                            return true;
                        }
                    #endregion
                    #region HammerSpin
                    case FrameType.HammerSpin:
                        {
                            xFrame = 3;
                            NPC.direction = 1;

                            if (yFrame < 7 || yFrame == 14) yFrame = 7;

                            if (tempCounter++ >= 2)
                            {
                                tempCounter = 0;
                                yFrame++;
                            }

                            return true;
                        }
                    #endregion
                    case FrameType.HammerChuck:
                        {
                            // Wait for a delay (poise the hammer), then prepare to receive the hammer; return when the delay is finished
                            if (++hammerDelay < 30)
                                moveFrame = prepareSwing;
                            else
                                moveFrame = getHammerStance;

                            return moveFrame == getHammerStance;
                        }
                    case FrameType.HammerChuckAwait:
                        {
                            // Used for the third hammer throw; set the arm in the general direction of the hammer
                            Vector2 hammer = HammerAlive().Projectile.Center;
                            Point add = getHammerStance;

                            if (hammer.Y < NPC.Center.Y - 33) add.X = 2;

                            if (hammer.Y >= NPC.Center.Y + 22 && hammer.Y < NPC.Center.Y + 33) add.X = 3;

                            if (hammer.Y > NPC.Center.Y + 33) add.X = 4;

                            if (hammer.Y > NPC.Center.Y + 66) add.X = 5;

                            moveFrame = add;

                            return true;
                        }
                    case FrameType.CatchUp:
                        {
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
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Vector2.UnitY * 6 - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            if (drawAfterimage)
            {
                for (int i = 0; i < imgPos.Length; i++)
                {
                    if (imgPos[i] == new Vector2()) continue;

                    // Each frame on the spritesheet is 116 x 72
                    const int frameWidth = 116;
                    const int frameHeight = 72;

                    Rectangle drawRectangle = new Rectangle(frameWidth * imgFrame[i].X, frameHeight * imgFrame[i].Y, frameWidth, frameHeight);
                    spriteBatch.Draw(texture, imgPos[i] - Vector2.UnitY * 6 - screenPos, drawRectangle, drawColor * ((float)(i - imgPos.Length + 6.125f) / (imgPos.Length)), NPC.rotation, drawRectangle.Size() / 2, NPC.scale, spriteEffects, 1f);
                }
            }
            return false;
        }

        // TODO: Fix framing issues regarding afterimages
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            /*int check = hammerDirection != 0 ? hammerDirection : NPC.direction;
            Vector2 offset = FrameOffset();
            Vector2 FrameOffset()
            {
                Vector2 set = new Vector2((check == -1 ? 4 : 8), 4);
                if (moveFrame.Y <= 2 && check == -1) set.X += -15;

                if (moveFrame.Y > 2)
                    set.X += (check == -1 ? (moveFrame.Y == 3 ? -16 : 16) : -32) * (moveFrame.Y == 3 ? 0.25f : 1);

                if (moveFrame.Y > 4 && check == 1) set.X += 16;

                if (moveFrame.Y > 4)
                {
                    if (moveFrame.Y <= 6)
                    {
                        if (moveFrame.Y > 5)
                            set.X += 30 * check;

                        set.X += check == -1 ? -6 : -12;
                    }
                    else
                        set.X += check == -1 ? -1 : -14;
                }

                return set;
            }

            

            Helper().Draw(spriteBatch, moveFrame, NPC.Center - screenPos, drawColor, fx, offset);*/
        }
    }

    public abstract class PaladinProjectile : ModProjectile
    {
        public Paladin owner;

        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public virtual void DoScreenShake()
        {
            owner.stunDuration = 30;
            owner.velocity = 0;
            owner.NPC.velocity.X *= -2f;
            ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 100);
        }

        public void CheckCollision()
        {
            // If the paladin collides with a tile while spinning, send the paladin backwards and briefly stun the paladin
            Point checkTile = new Point(MathFunctions.AGF.Round(Projectile.Center.X / 16), MathFunctions.AGF.Round(Projectile.Center.Y / 16));
            Tile tile = Main.tile[checkTile.X, checkTile.Y];
            if (!WorldGen.TileEmpty(checkTile.X, checkTile.Y) && WorldGen.SolidOrSlopedTile(tile) && Main.tileSolid[tile.TileType])
            {
                DoScreenShake();
                Projectile.Kill();
            }
        }
    }

    public class PaladinHammer : PaladinProjectile
    {
        public float sine = 1;
        public int direction;
        bool[] far = new bool[2];
        public float[] startEnd = new float[2];
        bool initialize = false;
        float rotation;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.timeLeft = 3000;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            if (!initialize)
            {
                sine = startEnd[1];
                initialize = true;
            }

            if (owner != null && owner.NPC.active)
            {
                // Rotate the hammer forth, back, or forth but faster
                switch (owner.throwStyle)
                {
                    case 1:
                        rotation += 0.125f * (direction == 1 ? 1 : -1);
                        break;
                    case 2:
                        rotation += 0.125f * (direction == 1 ? -1 : 1);
                        break;
                    case 3:
                        rotation += direction == 1 ? 0.25f : -0.25f;
                        break;
                }

                float distance = Vector2.Distance(owner.NPC.Center, owner.targetPosition);
                if (sine < startEnd[0]) // 1st cycle ends at 3f, second cycle ends at 5f
                {
                    if (distance < 450)
                    {
                        // Move the hammer in a unique functioned movement; the cycle is based on the starting x
                        // https://www.desmos.com/calculator/8zryrzykns
                        Vector2 center = owner.targetPosition - owner.NPC.Center;
                        float f2 = (float)Math.Atan(center.Y / center.X);
                        float[] rot = new float[2] { -direction * (float)(Math.PI / 2) + f2, direction * (float)(Math.PI / 1) + f2 };
                        sine += 2f / 75;
                        float g = (float)(Math.Cos(sine + Math.PI) / 2) + 0.5f;
                        float h = (float)(Math.Cos(sine * Math.PI) / 2) + 0.5f;
                        h *= (float)Math.Sqrt(distance / 16);
                        float c = rot[0] + (rot[1] - rot[0]) * g;
                        Vector2 pos = new Vector2((float)(h * Math.Sin(c)), (float)((h * -0.5f) * Math.Cos(c)));
                        Projectile.Center = (pos * 100) + owner.NPC.Center;
                    }
                    else
                    {
                        far[0] = true;
                        sine = 5f;
                    }
                }
                else if (sine >= 5f && sine <= 5f + (Math.PI))
                {
                    // 3rd hammer attack; lerp the position of the hammer towards the target; this is performed as the only attack if the target is very far away (slower, but more powerful)
                    sine += (float)Math.PI / (far[0] ? 180 : 90);
                    if (!far[1])
                    {
                        Projectile.damage = MathFunctions.AGF.Round(Projectile.damage * 1.5f);
                        far[1] = true;
                    }
                    float sin = sine - 5f;
                    float sine2 = (float)Math.Pow(Math.Sin(sin), 2);
                    Projectile.Center = new Vector2(MathHelper.Lerp(owner.NPC.Center.X, owner.targetPosition.X, sine2), MathHelper.Lerp(owner.NPC.Center.Y, owner.targetPosition.Y, sine2));
                }
                else
                {
                    // When it returns to the paladin, shake the screen
                    Projectile.Kill();
                }
            }
            else
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // If it hits an npc, shake the screen
            if (owner != null && owner.NPC.active) ScreenShake.ScreenShakeEvent(Projectile.Center, 8, 4, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = tex.Size() / 2f;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, rotation, new Vector2(tex.Width / 2, tex.Height / 2), Projectile.scale, SpriteEffects.None, 0);
        }
    }

    public class PaladinHammerSpin : PaladinProjectile
    {
        public float sine = (float)Math.PI / 2;
        public override bool PreDraw(ref Color lightColor) => false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) { Projectile.timeLeft = 180; }
        public override string Texture => AssetDirectory.NPC + "Mercenary/Paladin/PaladinHammer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 2;
            AIType = ProjectileID.Bullet;
        }

        public override void DoScreenShake()
        {
            owner.stunDuration = 30;
            owner.velocity = 0;
            owner.NPC.velocity *= -1f;
            ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 100);
        }

        public override void AI()
        {
            if (owner != null && owner.NPC.active && !owner.DangerThreshold())
            {
                // Make the paladin more resistant, and lerp the position of this projectile back and forth
                CheckCollision();
                owner.NPC.defense = 90;
                owner.NPC.knockBackResist = 0;
                sine += (float)Math.PI / 60 * (owner.NPC.direction == -1 ? -1 : 1);
                Projectile.velocity = Vector2.Zero;
                float pos = MathHelper.Lerp(-35, 35, (float)Math.Pow(Math.Sin(sine), 2));
                Projectile.Center = owner.NPC.Center + new Vector2(pos, 0);
            }
            else
                Projectile.Kill();
        }
    }

    public class PaladinHammerHit : PaladinProjectile
    {
        public float sine;
        float initialVelocity;
        bool initialize;

        public override string Texture => AssetDirectory.NPC + "Mercenary/Paladin/PaladinHammer";
        public override bool PreDraw(ref Color lightColor) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 180;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            // Dust offset
            Vector2 offset = new Vector2(35 * (owner.hammerDirection == -1 ? -1 : 1), 25);
            if (!initialize)
            {
                for (int a = 0; a < 20; a++) // Create an oval dust shape
                {
                    float pi = (float)Math.PI / 10;
                    Dust dust = Dust.NewDustPerfect(new Vector2(20f * (float)Math.Cos(a * pi) + Projectile.Center.X, 5f * (float)Math.Sin(a * pi) + Projectile.Center.Y) + offset, 6);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Zero;
                }

                initialVelocity = Projectile.velocity.X;
            }

            #region shockwave dust
            // Create sine shaped dust formations as the projectile travels
            float sin2 = (float)Math.Pow(Math.Abs(1.06f * Math.Sin(5f * sine)), 40) - 0.1f;
            if (sin2 > 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset + new Vector2(0, -sin2 * 2.5f), DustID.Torch);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
            }
            #endregion

            // Lerp the velocity of the projectile to 0
            sine += (float)Math.PI / 90;
            if (sine <= Math.PI / 2)
                Projectile.velocity = new Vector2(MathHelper.Lerp(initialVelocity, 0, (float)Math.Sin(sine)), 0);
            else
                Projectile.Kill();
        }
    }

    public class PaladinRamHitbox : PaladinProjectile
    {
        public override bool? CanCutTiles() => false;
        public override bool PreDraw(ref Color lightColor) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rookie Paladin");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3000;
        }

        public override void AI()
        {
            // Set the position directly in front of the paladin
            if (owner != null && owner.NPC.active && owner.velocity >= 0.4f)
                Projectile.Center = owner.NPC.Center + new Vector2(owner.NPC.direction == -1 ? -50 : 50, 4);
            else
                Projectile.Kill();

            CheckCollision();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //ram the non-boss npc and send them flying
            if (target.knockBackResist < 1 && !target.boss)
                target.velocity += new Vector2((owner.NPC.direction == -1 ? -7.5f : 7.5f) / target.knockBackResist, -8f / target.knockBackResist);
            //if the paladin is still alive and the above does not occur, shake the screen
            else if (owner != null && owner.NPC.active && owner.velocity >= 0.4f)
                DoScreenShake();
        }
    }
}
