using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public partial class Paladin : Mercenary
    {
        // The position where the paladin should aim attacks
        public Vector2 targetPosition;
        // Used for checking if the paladin is running in the same direction
        private int previousDirection;
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
        // The frame that is used while drawing the NPC
        private Point moveFrame;
        // Used for the CatchUp() method
        private MathFunctions.Parabola parabola;

        // TODO: Probably find a more intuitive way of doing these?
        #region Draw Variables
        // Used in FrameUpdate() for walk cycles
        int[] walk = new int[2];
        // Used as an alternate direction lock for attacks
        public int hammerDirection;
        // Used for attack animation cycles
        public int[] hammerChuck = new int[3];
        // Used for hammer slam animation cycles (used in CatchUp())
        bool[] catchUp = new bool[2];
        // Determines if the afterimate effect is drawn
        bool drawAfterimage;
        // Used for differentiating between hammer spin and hammer slam for close attacks
        bool[] closeAttack = new bool[2];

        #region Afterimage variables
        //Past position
        Vector2[] imgPos = new Vector2[6] { new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2() };
        //Past frame
        Point[] imgFrame = new Point[6];
        #endregion

        #endregion
        public override List<MercenaryDialogue> Dialogue
        {
            get => PlaceholderDialogue();
        }
        public override string MercenaryName => "Rookie Paladin";
        public override int AttackDelay => 90;
        public override float DetectRadius => 350;
        public override int MaxHealth => 500;
        public override int Defense => 40;
        public override float KnockbackResist => 0.66f;

        // TODO: Refactor all code for readability
        public override void AI()
        {
            drawAfterimage = false;

            MainAI();
            if (scoutNPC == null && !DangerThreshold()) // Remove direction lock
            {
                hammerDirection = 0;
                closeAttackStyle = false;
            }

            if (scoutNPC == null)
                closeAttack = new bool[2];

            //Allow another projectile reaction to happen if not on a solid tile
            if (projReaction && OnSolidTile() != null)
                projReaction = false;
            
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
                //If the leap has started
                if (catchUp[0])
                    if (FrameUpdate("catchUp"))
                        //Declare that the paladin has caught up when the animation is finished
                        catchUp[1] = true;
            }
            else
            {
                catchUp = new bool[2];
                catchingUp = false;
            }
            if (velocity > 0.4f)
            {
                drawAfterimage = true;
                if (!DangerThreshold())
                {
                    //The paladin will not perform wall or pit checks, and if it runs into a tile (if the projectile collides), the paladin is stunned for about 1 second
                    canCheckTiles = false;
                    if (!IsRamming() && Spinning() == null && HammerAlive() == null)
                    {
                        PaladinRamHitbox ram = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinRamHitbox>(), 50, 0, hiredBy).ModProjectile as PaladinRamHitbox;
                        ram.owner = this;
                    }
                }
            }
            if (scoutNPC == null || velocity > 0.4f)
            {
                //resets attack variables
                if ((HammerAlive() == null && !catchingUp) || closeAttackStyle)
                {
                    hammerChuck = new int[3];
                    targetPosition = Vector2.Zero;
                }
                else if (HammerAlive() != null)
                    NPC.velocity.X = 0;
                //starts the "walk" animation cycle
                if (scoutNPC == null && HammerAlive() == null && !catchingUp)
                    FrameUpdate("walk");
            }
            //starts the "walkBattle" animation cycle if the paladin finds an enemy
            if (scoutNPC != null)
            {
                if (restore[0] > 0 && !catchingUp || (HammerAlive() == null && (!closeAttackStyle || DangerThreshold())))
                    FrameUpdate("walkBattle");
            }
            //Stun and attackDelay cooldown update
            if (stunDuration-- < 0)
                stunDuration = 0;
            if (attackDelay-- < 0)
                attackDelay = 0;
            if (drawAfterimage)
            {
                imgFrame[5] = moveFrame;
                imgPos[5] = NPC.Center;
                for (int a = 1; a < 6; a++)
                {
                    imgFrame[a - 1] = imgFrame[a];
                    imgPos[a - 1] = imgPos[a];
                }
            }
        }

        // TODO: Use an enum instead of a string type
        bool FrameUpdate(string type, bool condition = true)
        {
            #region Key frames
            Point hammerFall = new Point(0, 0);
            Point hammerStill = new Point(0, 2);
            Point prepareSwing = new Point(0, 3);
            Point getHammerStance = new Point(1, 3);
            Point holdHammerFall = new Point(0, 7);
            Point holdHammerStill = new Point(1, 7);
            #endregion
            if (condition)
            {
                switch (type)
                {
                    case "walk":
                        {
                            //change the X frame at a speed depending on the velocity
                            int add = (int)Math.Round(Math.Abs(NPC.velocity.X));
                            walk[1] += add;
                            if (walk[1] > 8)
                            {
                                walk[1] = 0;
                                walk[0]++;
                            }
                            if (walk[0] <= 5)
                            {
                                //Initial Y frame, change to alternate Y frame when X frame is at the end
                                moveFrame.Y = 0;
                                moveFrame.X = walk[0] + 1;
                                if (walk[0] > 4)
                                    moveFrame.Y = 1;
                            }
                            if (walk[0] <= 12 && walk[0] > 5)
                            {
                                //Alternate Y frame, change to initial Y frame when X frame is at the end
                                moveFrame.Y = 1;
                                moveFrame.X = walk[0] - 6;
                                if (walk[0] > 11)
                                {
                                    walk[0] = 1;
                                    moveFrame.Y = 0;
                                }
                            }
                            //Frames for falling and standing still
                            if (NPC.velocity.Y != 0)
                                moveFrame = hammerFall;
                            if (NPC.velocity.Y == 0 && NPC.velocity.X == 0)
                                moveFrame = hammerStill;
                            return true;
                        }
                    case "walkBattle":
                        {
                            //Just like "walk", but at different Y frames
                            int add = (int)Math.Round(Math.Abs(NPC.velocity.X));
                            walk[1] += add;
                            if (walk[1] > 8)
                            {
                                walk[1] = 0;
                                walk[0]++;
                            }
                            if (walk[0] <= 5)
                            {
                                moveFrame.Y = 7;
                                moveFrame.X = walk[0] + 1;
                                if (walk[0] > 4)
                                    moveFrame.Y = 8;
                            }
                            if (walk[0] <= 12 && walk[0] > 5)
                            {
                                moveFrame.Y = 8;
                                moveFrame.X = walk[0] - 6;
                                if (walk[0] > 11)
                                {
                                    walk[0] = 1;
                                    moveFrame.Y = 7;
                                }
                            }
                            if (NPC.velocity.Y != 0)
                                moveFrame = holdHammerFall;
                            if (NPC.velocity.Y == 0 && NPC.velocity.X == 0)
                                moveFrame = holdHammerStill;
                            return true;
                        }
                    case "hammerSpin":
                        {
                            //Similar to "walk", but the X frame update has a static speed, and **needs much more maticulous offset setting in PostDraw
                            //**The frames should be the same, so the paladin at the lower row is further to the right during the spinning frames
                            if (walk[1]++ >= 1)
                            {
                                walk[1] = 0;
                                walk[0]++;
                            }
                            if (walk[0] <= 3)
                            {
                                moveFrame.Y = 5;
                                moveFrame.X = walk[0];
                                if (walk[0] > 2)
                                    moveFrame.Y = 6;
                            }
                            if (walk[0] <= 6 && walk[0] > 2)
                            {
                                moveFrame.Y = 6;
                                moveFrame.X = walk[0] - 3;
                            }
                            if (walk[0] > 6)
                            {
                                moveFrame = new Point(3, 5);
                                if (walk[0] >= 8)
                                {
                                    walk[0] = 0;
                                    moveFrame = new Point(0, 5);
                                }
                            }
                            return true;
                        }
                    case "hammerChuck":
                        {
                            //Wait for a delay (poise the hammer), then prepare to receive the hammer; return when the delay is finished
                            hammerChuck[0]++;
                            if (hammerChuck[0] < 30)
                                moveFrame = prepareSwing;
                            else
                                moveFrame = getHammerStance;
                            return moveFrame == getHammerStance;
                        }
                    case "hammerChuckAwait":
                        {
                            //Used for the third hammer throw; set the arm in the general direction of the hammer
                            Vector2 hammer = HammerAlive().Projectile.Center;
                            Point add = getHammerStance;
                            if (hammer.Y < NPC.Center.Y - 33)
                                add.X = 2;
                            if (hammer.Y >= NPC.Center.Y + 22 && hammer.Y < NPC.Center.Y + 33)
                                add.X = 3;
                            if (hammer.Y > NPC.Center.Y + 33)
                                add.X = 4;
                            if (hammer.Y > NPC.Center.Y + 66)
                                add.X = 5;
                            moveFrame = add;
                            return true;
                        }
                    case "catchUp":
                        {
                            //Or "hammer slam"; default to "prepareSwing" (0, 4), then move the X frame every 10 ticks
                            hammerChuck[1]++;
                            moveFrame.Y = 4;
                            switch (hammerChuck[1])
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
                            return hammerChuck[1] >= 40;
                        }
                }
            }
            return false;
        }

        // TODO: Make this trigger when the NPC isn't in combat
        public override bool RestoreHealth()
        {
            bool finished = false;
            NPC.direction = NPC.velocity.X < 1 ? -1 : 1;
            if (OnSolidTile() != null)
            {
                //If the NPC is on a solid tile, slow the NPC to a halt, and when the NPC is not moving, wait two seconds, then heal
                restore[1]++;
                if (scoutNPC == null && scoutProjectile == null)
                    NPC.velocity.X *= 0.125f;
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
            //When healing is done, return true (and reset variables)
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
            if (scoutNPC != null)
            {
                //Create an NPC decoy of the detected threat to pass in StandardAI(), then walk towards **the decoy's defined position
                //**A far distance away from the detected threat based on the threat's width
                NPC threat = scoutNPC;
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
        //Reset the needed healing charge to perform RestoreHealth()
        public override void HitEffect(int hitDirection, double damage) { restore[1] = 0; }
        public override void ProjectileReact()
        {
            //This is supposed to send the NPC in a direction perpendicular to the projectile, but it needs to be reworked, along with the projectile check itself
            if (HammerAlive() == null && restore[0] < 1)
            {
                Vector2 velocity = scoutProjectile.velocity + new Vector2(10 * (scoutProjectile.velocity.X < 0 ? -1 : 1), 10 * (scoutProjectile.velocity.Y < 0 ? -1 : 1));
                if (!projReaction)
                {
                    projReaction = true;
                    float inverse = (-velocity.X / velocity.Y);
                    float x = (inverse * velocity.X) * (velocity.Y / velocity.X);
                    NPC.velocity += new Vector2(x, x * inverse);
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y *= -1;
                    NPC.velocity = new Vector2(MathHelper.Clamp(NPC.velocity.X, -5, 5), MathHelper.Clamp(NPC.velocity.Y, -5, 0));
                    NPC.position += NPC.velocity;
                }
            }
        }
        //Checks if there is a solid tile directly below the NPC
        Tile OnSolidTile()
        {
            Point checktile = new Point(MathFunctions.AGF.Round(NPC.Center.X) / 16, MathFunctions.AGF.Round(NPC.Center.Y + 32) / 16);
            Tile tile = Main.tile[checktile.X, checktile.Y];
            if (!WorldGen.TileEmpty(checktile.X, checktile.Y) && (Main.tileSolidTop[tile.TileType] || Main.tileSolid[tile.TileType]))
                return tile;
            return new Tile();
        }
        public override bool CatchUp(Vector2 location = new Vector2())
        {
            Player player = Main.player[hiredBy];
            if (!catchUp[0])
            {
                if (HammerAlive() == null && Spinning() == null)
                {
                    //After 2/3 of a second
                    if (leapDelay >= 40)
                    {
                        if (!isLeaping)
                        {
                            //If on a solid tile
                            if (OnSolidTile() != null || NPC.velocity.Y == 0)
                            {
                                //Create a parabolic function that is expected to be done in at least 1/2 a second
                                //The parabolic function is from the NPC center, to the expected player's position plus an additive to prevent extremely small distanced leaps
                                Vector2 place = location != new Vector2() ? location : player.MountedCenter;
                                Vector2 additive = Vector2.Zero;
                                if (Math.Abs(NPC.Center.X - place.X) < 75)
                                    additive.X = place.X < NPC.Center.X ? -125 : 125;
                                if (Math.Abs(NPC.Center.Y - place.Y) < 50)
                                    additive.Y = -35;
                                parabola = MathFunctions.Parabola.ParabolaFromCoords(NPC.Center, place + additive);
                                parabola.SetIncrement(30);
                                isLeaping = true;
                                moveFrame = new Point(0, 3);
                            }
                        }
                        else
                        {
                            //Updates the parabola, and updates the velocity based on the y value from the parabola
                            drawAfterimage = true;
                            NPC.noTileCollide = true;
                            leapDist += parabola.increment[0];
                            NPC.velocity.Y = parabola.GetY(leapDist - parabola.increment[0]) - parabola.GetY(leapDist);
                            NPC.velocity.X = parabola.increment[0] * (parabola.backwards ? -1 : 1);
                            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                        }
                        if (parabola != null && leapDist >= ((parabola.z1 + parabola.z2) * 0.5f))
                        {
                            //If the NPC is on a solid tile, invert velocity to prevent falling through platforms, and set the position directly above the tile
                            //Create a screenshake event and reset leap variables
                            //This will only be called once, and will set a variable for the hammer slam animation to play;
                            //This method will be called until that animation is finished
                            Tile tile = OnSolidTile();
                            if (tile != new Tile())
                            {
                                NPC.velocity.Y *= -0.5f;
                                NPC.position.Y = MathFunctions.AGF.Round(NPC.Center.Y - 8) - 24;
                                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0, -4);
                                NPC.noTileCollide = false;
                                ScreenShake.ScreenShakeEvent(NPC.Center, 15, 9f, 400);
                                isLeaping = false;
                                leapDist = 0;
                                leapDelay = 0;
                                catchUp[0] = true;
                            }
                        }
                    }
                    else
                    {
                        //While waiting to leap, set the frame to a prepared stance 
                        moveFrame = new Point(3, 4);
                        leapDelay++;
                    }
                }
            }
            return catchUp[1];
        }
        public override bool FarAttack()
        {
            Vector2 scout;
            //Sets the target, and makes the NPC face towards it
            if (scoutNPC != null)
            {
                scout = scoutNPC.Center;
                targetPosition = scout;
                NPC.direction = NPC.Center.X < scout.X ? 1 : -1;
                hammerDirection = NPC.direction;
            }
            if (CanAttack())
            {
                //Perform three different kinds of hammer throws; each of the motions are solely performed by the projectile
                bool check = hammerChuck[2] == 2 ? true : FrameUpdate("hammerChuck");
                if (check)
                {
                    hammerChuck[2]++;
                    if (hammerChuck[2] > 3)
                        hammerChuck[2] = 1;
                    hammerChuck[0] = 0;
                    hammerChuck[1] = 1;
                    PaladinHammer hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammer>(), 20, 1f, hiredBy).ModProjectile as PaladinHammer;
                    hammer.owner = this;
                    hammer.direction = targetPosition.X < NPC.Center.X ? -1 : 1;
                    hammer.startEnd = Start();
                }
                else
                    hammerChuck[1] = 0;
            }
            //If a hammer is out and not on the third kind of hammer throw, set to a crouched stance
            if (hammerChuck[2] == 3 && hammerChuck[1] == 1)
                FrameUpdate("hammerChuckAwait");
            float[] Start()
            {
                //Return; at what X must the projectile be killed, and the starting X
                switch (hammerChuck[2])
                {
                    case 2:
                        return new float[2] { 4.9f, 3 };
                    case 3:
                        return new float[2] { 5, 5 };
                    default:
                        return new float[2] { 3, 1 };
                }
            }
            return HammerAlive() == null && scoutNPC == null;
        }
        bool CanAttack() => Spinning() == null && HammerAlive() == null && attackDelay < 1;
        public override bool CloseAttack()
        {
            if (!closeAttack[0])
            {
                //Check the distance between the enemy and the NPC; if *really* close, perform a hammer spin, otherwise perform a hammer slam
                Vector2 scout;
                if (scoutNPC != null)
                {
                    scout = scoutNPC.Center;
                    float general = Vector2.Distance(new Vector2(scout.X, 0), new Vector2(NPC.Center.X, 0));
                    float height = Vector2.Distance(new Vector2(scout.Y, 0), new Vector2(NPC.Center.Y, 0));
                    closeAttack[1] = height > 50 || OnSolidTile() == null || general < 75;
                    closeAttack[0] = true;
                }
            }
            if (closeAttack[1])
            {
                drawAfterimage = true;
                FrameUpdate("hammerSpin");
                //Lock the direction to forward to prevent unusual appearance
                hammerDirection = 1;
                if (scoutNPC != null)
                    NPC.direction = NPC.Center.X < scoutNPC.Center.X ? 1 : -1;
                velocity = 0.33f;
                //Go towards the target
                if (!DangerThreshold() && scoutNPC != null)
                    StandardAI(Rect(scoutNPC));
                //Create a projectile that moves forth and back from the paladin
                if (CanAttack())
                {
                    PaladinHammerSpin hammer = Projectile.NewProjectileDirect(Source(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PaladinHammerSpin>(), 20, 1f, hiredBy).ModProjectile as PaladinHammerSpin;
                    hammer.owner = this;
                }
                //If there is still an enemy, keep the spinning projectile alive
                if (Spinning() != null && scoutNPC != null)
                    Spinning().Projectile.timeLeft = 2;
                //Revert the paladin's stats to normal
                if (Spinning() == null)
                {
                    NPC.defense = Defense;
                    NPC.knockBackResist = KnockbackResist;
                    closeAttack = new bool[2];
                }
                bool stop = Spinning() == null && scoutNPC == null;
                return stop;
            }
            else
            {
                //Face the paladin towards the enemy, and keep it still
                if (attackDelay < 0)
                    hammerDirection = scoutNPC.Center.X < NPC.Center.X ? -1 : 1;
                NPC.velocity.X = 0;
                if (FrameUpdate("catchUp") && attackDelay < 1)
                    hammerChuck[1] = 0;
                //Create a projectile with lerped motion towards the faced direction, and shake the screen
                if (hammerChuck[1] > 9 && Shockwave() == null && attackDelay < 1)
                {
                    ScreenShake.ScreenShakeEvent(NPC.Center, 15, 9, 250);
                    PaladinHammerHit shockwave = Projectile.NewProjectileDirect(Source(), NPC.Center, new Vector2(hammerDirection == -1 ? -16 : 16, 0), ModContent.ProjectileType<PaladinHammerHit>(), 30, 1, hiredBy).ModProjectile as PaladinHammerHit;
                    shockwave.owner = this;
                    attackDelay = AttackDelay + 20;
                }
                //Set the frames to a standing walkBattle frame in the middle of the delay
                else if (hammerChuck[1] < 10)
                    moveFrame = new Point(0, 3);
                if (attackDelay > 0 && attackDelay < 40)
                    moveFrame = new Point(1, 7);
                //Check for the nearest NPC if a hammer spin attack is necessary
                if (attackDelay < 0)
                    closeAttack = new bool[2];
                bool stop = Shockwave() == null && moveFrame.X > 0;
                return stop;
            }
        }
        /// <summary>
        /// Returns the thrown hammer attack projectile
        /// </summary>
        /// <returns></returns>
        PaladinHammer HammerAlive()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammer>())
                {
                    PaladinHammer hammer = p.ModProjectile as PaladinHammer;
                    if (hammer.owner == this)
                        return hammer;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the hammer spin projectile
        /// </summary>
        /// <returns></returns>
        PaladinHammerSpin Spinning()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammerSpin>())
                {
                    PaladinHammerSpin hammer = p.ModProjectile as PaladinHammerSpin;
                    if (hammer.owner == this)
                        return hammer;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the hammer slam shockwave projectile
        /// </summary>
        /// <returns></returns>
        PaladinHammerHit Shockwave()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammerHit>())
                {
                    PaladinHammerHit hammer = p.ModProjectile as PaladinHammerHit;
                    if (hammer.owner == this)
                        return hammer;
                }
            }
            return null;
        }
        bool IsRamming()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinRamHitbox>())
                {
                    PaladinRamHitbox hammer = p.ModProjectile as PaladinRamHitbox;
                    if (hammer.owner == this)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Shorthand for return the shorthand of the mod instance of the paladin draw helper
        /// </summary>
        /// <returns></returns>
        PaladinDrawHelper Helper() => PaladinDrawHelper.Helper();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;

        // TODO: Fix framing issues regarding afterimages
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int check = hammerDirection != 0 ? hammerDirection : NPC.direction;
            Vector2 offset = FrameOffset();
            Vector2 FrameOffset()
            {
                Vector2 set = new Vector2((check == -1 ? 4 : 8), 4);
                if (moveFrame.Y <= 2 && check == -1)
                    set.X += -15;
                if (moveFrame.Y > 2)
                    set.X += (check == -1 ? (moveFrame.Y == 3 ? -16 : 16) : -32) * (moveFrame.Y == 3 ? 0.25f : 1);
                if (moveFrame.Y > 4 && check == 1)
                    set.X += 16;
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
            SpriteEffects fx = check == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (drawAfterimage)
            {
                for (int a = 0; a < imgPos.Length; a++)
                {
                    if (imgPos[a] != new Vector2())
                        Helper().Draw(spriteBatch, imgFrame[a], (imgPos[a] + new Vector2(imgFrame[a].Y > 4 ? (imgFrame[a].Y > 5 ? -16 : 16) : 0, 0)) - screenPos, drawColor * ((float)(a - imgPos.Length + 6.125f) / (imgPos.Length)), fx, imgFrame[a].Y > 4 ? new Vector2(0, 4) : offset);
                }
            }
            Helper().Draw(spriteBatch, moveFrame, NPC.Center - screenPos, drawColor, fx, offset);
        }
    }
    public class PaladinHammer : ModProjectile
    {
        public float sine = 1;
        public Paladin owner;
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
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3000;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
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
                //Rotate the hammer forth, back, or forth but faster
                switch (owner.hammerChuck[2])
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
                //1st cycle ends at 3f, second cycle ends at 5f
                if (sine < startEnd[0])
                {
                    if (distance < 450)
                    {
                        //Move the hammer in a unique functioned movement; the cycle is based on the starting x
                        //https://www.desmos.com/calculator/8zryrzykns
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
                    //3rd hammer attack; lerp the position of the hammer towards the target; this is performed as the only attack if the target is very far away (slower, but more powerful)
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
                    //when it returns to the paladin, shake the screen
                    Projectile.Kill();
                    //ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 300);
                }
            }
            else
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //If it hits an npc, shake the screen
            if (owner != null && owner.NPC.active)
                ScreenShake.ScreenShakeEvent(Projectile.Center, 8, 4, 100);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Afterimage
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);
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
    public class PaladinHammerSpin : ModProjectile
    {
        public float sine = (float)Math.PI / 2;
        public Paladin owner;
        public override string Texture => "OvermorrowMod/Content/NPCs/Mercenary/Paladin/PaladinHammer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            if (owner != null && owner.NPC.active && !owner.DangerThreshold())
            {
                //Make the paladin more resistant, and lerp the position of this projectile back and forth
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
            void CheckCollision()
            {
                //if the paladin collides with a tile while spinning, send the paladin backwards and briefly stun the paladin
                Point checkTile = new Point(MathFunctions.AGF.Round(Projectile.Center.X / 16), MathFunctions.AGF.Round(Projectile.Center.Y / 16));
                Tile tile = Main.tile[checkTile.X, checkTile.Y];
                if (!WorldGen.TileEmpty(checkTile.X, checkTile.Y) && WorldGen.SolidOrSlopedTile(tile) && Main.tileSolid[tile.TileType])
                {
                    DoScreenShake();
                    Projectile.Kill();
                }
            }
            //Like the ram hitbox, but less drastic
            void DoScreenShake() { owner.stunDuration = 30; owner.velocity = 0; owner.NPC.velocity *= -1f; ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 100); }
        }
        public override bool PreDraw(ref Color lightColor) => false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) { Projectile.timeLeft = 180; }
    }
    public class PaladinHammerHit : ModProjectile
    {
        public Paladin owner;
        public float sine;
        bool initialize;
        float vel;
        public override string Texture => "OvermorrowMod/Content/NPCs/Mercenary/Paladin/PaladinHammer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            //offest for dust
            Vector2 offset = new Vector2(35 * (owner.hammerDirection == -1 ? -1 : 1), 25);
            if (!initialize)
            {
                //Create an oval dust shape
                for (int a = 0; a < 20; a++)
                {
                    float pi = (float)Math.PI / 10;
                    Dust dust = Dust.NewDustPerfect(new Vector2(20f * (float)Math.Cos(a * pi) + Projectile.Center.X, 5f * (float)Math.Sin(a * pi) + Projectile.Center.Y) + offset, 6);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Zero;
                }
                vel = Projectile.velocity.X;
            }
            #region shockwave dust
            //Create sine shaped dust formations as the projectile travels
            float sin2 = (float)Math.Pow(Math.Abs(1.06f * Math.Sin(5f * sine)), 40) - 0.1f;
            if (sin2 > 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset + new Vector2(0, -sin2 * 2.5f), DustID.Torch);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
            }
            #endregion
            //Lerp the velocity of the projectile to 0
            initialize = true;
            sine += (float)Math.PI / 90;
            if (sine <= Math.PI / 2)
                Projectile.velocity = new Vector2(MathHelper.Lerp(vel, 0, (float)Math.Sin(sine)), 0);
            else
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor) => false;
    }
    public class PaladinRamHitbox : ModProjectile
    {
        public Paladin owner;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rookie Paladin");
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3000;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            //Set the position directly in front of the paladin
            if (owner != null && owner.NPC.active && owner.velocity >= 0.4f)
                Projectile.Center = owner.NPC.Center + new Vector2(owner.NPC.direction == -1 ? -50 : 50, 4);
            else
                Projectile.Kill();
            CheckCollision();
        }
        void CheckCollision()
        {
            //If the projectile collides with a tile
            Point checktile = new Point(MathFunctions.AGF.Round(Projectile.Center.X / 16), MathFunctions.AGF.Round(Projectile.Center.Y / 16));
            Tile tile = Main.tile[checktile.X, checktile.Y];
            if (!WorldGen.TileEmpty(checktile.X, checktile.Y) && WorldGen.SolidOrSlopedTile(tile) && Main.tileSolid[tile.TileType])
            {
                DoScreenShake();
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor) => false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //ram the non-boss npc and send them flying
            if (target.knockBackResist < 1 && !target.boss)
                target.velocity += new Vector2((owner.NPC.direction == -1 ? -7.5f : 7.5f) / target.knockBackResist, -8f / target.knockBackResist);
            //if the paladin is still alive and the above does not occur, shake the screen
            else if (owner != null && owner.NPC.active && owner.velocity >= 0.4f)
                DoScreenShake();
        }
        //shake the screen, send the paladin backwards and stun the paladin
        void DoScreenShake() { owner.stunDuration = 30; owner.velocity = 0; owner.NPC.velocity.X *= -2f; ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 100); }
        public override bool? CanCutTiles() => false;
    }
    public class PaladinDrawHelper : MercenaryDrawHelper
    {
        public const int Paladin = 0;
        public override Texture2D SetTex { get => ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Mercenary/Paladin/PaladinSpriteSheet").Value; }
        public override Point Frame { get => new Point(38, 33); }
        public static PaladinDrawHelper Helper() => (PaladinDrawHelper)OvermorrowMod.Common.OvermorrowModFile.Instance.drawHelpers[Paladin];
    }
}
