using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;

namespace OvermorrowMod.Content.NPCs.Mercenary
{
    public partial class Mercenary : ModNPC
    {
        #region Adjustable Values
        //The general detect radius for mercenaries; for close attacks, it checks entities that are in a third of this radius
        public virtual float DetectRadius { get { return 100; } }
        //Should not be a part of the base attack system, but as a built in delay (atkDelay = AttackDelay; and then put a check, for example)
        public virtual int AttackDelay { get { return 60; } }
        #region NPC values
        public virtual string MercenaryName { get { return "Mercenary"; } }
        public virtual int MaxHealth { get { return 400; } }
        public virtual int Defense { get { return 20; } }
        public virtual float KnockbackResist { get { return 0.25f; } }
        //How many seconds a mercenary must wait until they can heal again (until RestoreHealth() can be called)
        public virtual int HealCooldown { get { return 30; } }
        //A list of MercenaryDialogue classes, containing strings (the dialogue), ints (priority of the dialogue) and bools (can display or not)
        public virtual List<MercenaryDialogue> Dialogue { get; set; }
        #endregion
        #endregion
        //A variable for RestoreHealth() method, all variables from this method are drained to 0 when not healing
        //Use in an override of RestoreHealth() as a timer or other variable
        public int[] restore = new int[3];
        //Determines if the mercenary should continue attacking (calls FarAttack() or CloseAttack() to set this variable)
        public bool continueAttack;
        //Determines if the mercenary should call CloseAttack()
        public bool closeAttackStyle;
        //Determines if StandardAI() should account for walls and ceilings (or be called at all)
        public bool canCheckTiles = true;
        //How fast should the mercenary go when StandardAI() is called
        public float velocity = 0.25f;
        //Generally used with AttackDelay
        public int attackDelay;
        //The player that has hired this mercenary (the player index)
        public int hiredBy = -1;
        //If this reaches a certain amount, CatchUp() will be called (unless the mercenary is in danger)
        int farTimer;
        //The closest hostile projectile to the mercenary
        public Projectile scoutProjectile;
        // The closest hostile NPC to the mercenary
        public NPC scoutNPC;
        public bool catchingUp;
        //These two are only used in StandardAI() for storing distance and delays for leaps, how many tiles must be accounted, etc.
        int[] detectValue = new int[2];
        int[] detectTimer = new int[2];
        //A minimum of tiles required for a leap; at least 2
        int checkGap;
        //Used for randomized closing distances when walking to the player (StandardAI())
        int[] runCond = new int[2] { 350, 350 };
        //Used for cancelling an unused pit jump
        int pitJumpExpire;
        //The direction the NPC should go while performing a leap
        int direction;
        //The current wall detection scanner position
        Vector2 wallDetectPos;
        //The current pit detection scanner position
        Vector2 groundDetectPos;
        //Where the NPC should expect to start a leap when a pit is detected
        Vector2 pitJumpRelative;
        //How many minutes are displayed (how many minutes the mercenary will fight for you) when you open chat
        public int hireTime;
        //the literal time left (in ticks), determintes hireTime
        public int hireTimer;
        //Sets to 3600 every minute; decreases 1 per tick to update hireTime
        public int currentMinute;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(MercenaryName);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.width = 40;
            NPC.height = 54;
            NPC.lifeMax = MaxHealth;
            NPC.friendly = true;
            NPC.defense = Defense;
            NPC.knockBackResist = KnockbackResist;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }
        //**This is mostly irrelevant except for the base NPC of this class
        public override void AI()
        {
            //**Everything except the last line here is already included in the base AI
            if (hireTimer < 0)
            {
                hiredBy = -1;
                hireTimer = 0;
            }
            hireTimer--;
            if (currentMinute-- < 1)
            {
                hireTime--;
                currentMinute = 3600;
            }
            MainAI();
        }
        /// <summary>
        /// Restores 1/5 of the mercenary's max health when it can successfully perform RestoreHealth()
        /// </summary>
        public void Heal()
        {
            int heal = NPC.lifeMax / 5;
            NPC.life += heal;
            CombatText.NewText(NPC.getRect(), Color.SpringGreen, $"{heal}", false, false);
        }
        public void StandardAI(RBC targ)
        {
            Vector2 target = targ.position;
            //When safe: When the player is not too far
            //Otherwise: When in danger and the target (generally scoutNPC) is not null
            if (!TooFar() || (DangerThreshold() && target != Vector2.Zero))
            {
                //The mercenary will fall through platforms if the target is below it
                void FallThrough()
                {
                    if ((target.Y - (targ.height / 2)) > NPC.Center.Y && OnPlatform(NPC.Center, NPC.height / 2, false) && (OnPlatform(new Vector2(NPC.Center.X, target.Y), targ.height / 2, false) || OnPlatform(new Vector2(NPC.Center.X, target.Y), targ.height / 2, true)))
                        NPC.noTileCollide = true;
                    else
                        NPC.noTileCollide = false;
                }
                //Checks if the tile it is standing on is a platform
                bool OnPlatform(Vector2 check, float additive, bool solidTile)
                {
                    Point checkTile = new Point((int)check.X / 16, (int)(check.Y + additive) / 16);
                    Tile tile = Main.tile[checkTile.X, checkTile.Y];
                    return solidTile ? Main.tileSolid[tile.TileType] && WorldGen.TileEmpty(checkTile.X, checkTile.Y) : Main.tileSolidTop[tile.TileType];
                }
                //Checks the vertical distance to the target; some randomization is added with runCond[0]
                bool NextToTarget() => Vector2.Distance(new Vector2(NPC.Center.X), new Vector2(target.X)) < runCond[0];
                if (hiredBy != -1)
                {
                    if (!NextToTarget() || target != Vector2.Zero)
                    {
                        //Add randomization for NextToTarget()
                        if (runCond[1] == runCond[0])
                            runCond[0] = Main.rand.Next(150, 400);
                        //Determines which direction the NPC should go
                        bool moveCondition = direction != 0 && NPC.velocity.Y != 0 && groundDetectPos != Vector2.Zero ? (direction > 0) : (NPC.Center.X < target.X);
                        NPC.velocity.X += moveCondition ? velocity : -velocity;
                        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -7, 7);
                        //The mercenary cannot check tiles if it is in the air
                        if (NPC.velocity.Y == 0)
                        {
                            if (groundDetectPos == Vector2.Zero)
                                direction = 0;
                            if (canCheckTiles)
                                CheckTiles();
                        }
                    }
                    else
                        runCond[1] = runCond[0];
                    FallThrough();
                }
                void CheckTiles()
                {
                    //Changes the position of the wall detection scanner
                    wallDetectPos = new Vector2(NPC.Center.X + (NPC.width * (NPC.direction == 1 ? 0.25f : -0.25f)), (NPC.Center.Y + 8) - (detectTimer[0] * 16));
                    #region Wall check
                    //If there is no obstruction between the scanned position, and the wall detection scanner, there is no tile there
                    bool noWall = Collision.CanHitLine(wallDetectPos, 12, 12, new Vector2(wallDetectPos.X + (96 * (NPC.direction == 1 ? 1 : -1)), wallDetectPos.Y - 8), 12, 12);
                    if (!noWall)
                        detectValue[0]++;
                    //A minimum amount of time must pass until a leap can be performed (>= num represents how many tiles can be accounted upwards)
                    if (detectTimer[0]++ >= 5)
                    {
                        //Make the mercenary jump a height based on how many obstructions were found, and reset the values
                        #region Wall check jump
                        if (detectValue[0] > 0)
                            NPC.velocity.Y -= (detectValue[0] * 6f) * (1.5f / detectValue[0]);
                        #endregion
                        detectValue[0] = 0;
                        detectTimer[0] = 0;
                    }
                    #endregion
                    #region Horizontal check
                    if (groundDetectPos != Vector2.Zero)
                    {
                        //Like wall detection, but it checks downwards and also checks for a **gap quota
                        bool pit = Pit(0) && Pit(1);
                        bool Pit(int x) => Collision.CanHitLine(groundDetectPos, 12, 12, new Vector2(groundDetectPos.X + (NPC.direction == 1 ? 16 * x : -16 * x), groundDetectPos.Y + 16), 12, 12) && !Main.tileSolidTop[Main.tile[MathFunctions.AGF.Round((groundDetectPos.X + (NPC.direction == 1 ? 16 * x : -16 * x)) / 16), MathFunctions.AGF.Round((groundDetectPos.Y / 16))].TileType];
                        if (pit)
                        {
                            //**The gap must be at least two tiles wide in order for the mercenary to jump; to prevent unnecessary leaps
                            checkGap++;
                            if (checkGap == 2)
                            {
                                //Sets a place for the mercenary to go towards to perform the leap
                                pitJumpRelative = groundDetectPos;
                                direction = NPC.Center.X - groundDetectPos.X < 0 ? 1 : -1;
                            }
                            detectValue[1]++;
                        }
                        else
                            checkGap = 0;
                        //Same story with wall detection; can check a maximum of around 17 tiles before checking if a leap is needed
                        if (detectTimer[1] < 17)
                            detectTimer[1]++;
                        if (detectTimer[1] >= 15)
                        {
                            //If a leap is needed (and the quota is met i.e. "checkGap == 2") 
                            if (pitJumpRelative != Vector2.Zero)
                            {
                                //perform a leap if the NPC is close enough to where pitJumpRelative was set (where the leap should be performed)
                                float dist = Vector2.Distance(new Vector2(NPC.Center.X, 0), new Vector2(pitJumpRelative.X, 0)) * (NPC.direction == -1 ? 0.5f : 1);
                                float condition = (16 * 4) / (NPC.direction == -1 ? 2 : 1);
                                //Erases the expected leap position after a brief period (brief because the NPC would be near it anyways)
                                pitJumpExpire++;
                                if (pitJumpExpire >= 15)
                                    pitJumpRelative = Vector2.Zero;
                                //if close enough to the expected position
                                if (dist < condition && condition < 75)
                                {
                                    //Perform a leap based on how many tiles were detected, and reset the values
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
                            //If a leap was not expected, reset values
                            else
                            {
                                pitJumpExpire = 0;
                                detectValue[1] = 0;
                                detectTimer[1] = 0;
                            }
                        }
                    }
                    //Update the pit detection position
                    if (detectValue[0] < 1)
                        groundDetectPos = new Vector2(NPC.Center.X + (detectTimer[1] * (NPC.direction == 1 ? 16f : -16f)), NPC.Center.Y + (NPC.height / 2.33f));
                    else
                        groundDetectPos = Vector2.Zero;
                    #endregion
                }
            }
        }
        /// <summary>
        /// Checks for hostile threats (NPCs and projectiles)
        /// </summary>
        public void ScoutThreats()
        {
            scoutProjectile = RadialProjectileCheck();
            scoutNPC = RadialNPCCheck();
        }
        /// <summary>
        /// Determines if RestoreHealth() can be called
        /// </summary>
        /// <returns></returns>
        public bool CanHeal()
        {
            int check = 0;
            for (int a = 0; a < restore.Length; a++)
                if (restore[a] < 1)
                    check++;
            return check == restore.Length;
        }
        /// <summary>
        /// A check for if the NPC can follow the player; this is in place so as the AI will not have to account with cavern systems and similar tile formations. This can be removed if a pathing algorithm, or something similar, is made.
        /// </summary>
        /// <returns></returns>
        public bool CanFollowPlayer()
        {
            Player player = Main.player[hiredBy];
            int count = 0;
            for (int y = -1; y > -24; y--)
            {
                for (int x = -4; x < 4; x++)
                {
                    Point checkTile = new Point((MathFunctions.AGF.Round(player.Center.X) / 16) + x, (MathFunctions.AGF.Round(player.Center.Y) / 16) + y);
                    Tile tile = Main.tile[checkTile.X, checkTile.Y];
                    if (!WorldGen.TileEmpty(checkTile.X, checkTile.Y) && WorldGen.SolidOrSlopedTile(tile))
                    {
                        count++;
                        if (tile.WallType != WallID.None)
                            count++;
                    }
                }
            }
            return count < 7;
        }
        /// <summary>
        /// Returns a class that contains the width, height and the exact position of an entity (usually used for StandardAI())
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RBC Rect(Entity entity) => new RBC(entity.width, entity.height, entity.Center);
        /// <summary>
        /// Returns the player that the mercenary is supposed to follow; if not hired, return null
        /// </summary>
        /// <returns></returns>
        public Player FollowPlayer()
        {
            if (hiredBy != -1)
                return Main.player[hiredBy];
            return null;
        }
        public virtual void MainAI()
        {
            if (hiredBy != -1)
            {
                if (!TooFar() && !DangerThreshold() && !catchingUp)
                {
                    if (scoutNPC != null)
                        continueAttack = true;
                    if (continueAttack)
                        continueAttack = closeAttackStyle ? CloseAttack() : FarAttack();
                }
                Player player = FollowPlayer();
                //Update the time that the mercenary is hired for
                if (hireTimer < 0)
                {
                    hiredBy = -1;
                    hireTimer = 0;
                }
                hireTimer--;
                if (currentMinute-- < 1)
                {
                    hireTime--;
                    currentMinute = 3600;
                }
                //Perform safety behaviour if in danger, or perform a health restore (RestoreHealth()) if possible
                if (DangerThreshold())
                {
                    continueAttack = false;
                    SafetyBehaviour();
                    if (CanHeal())
                        restore[0] = 1;
                }
                if (CanFollowPlayer())
                {
                    NPC.dontTakeDamage = false;
                    NPC.dontTakeDamageFromHostiles = false;
                    //If the NPC does not have to / cannot heal
                    if (restore[0] < 1)
                    {
                        //Perform a method whenever a projectile is detected; the second check may be removed if fully optimized
                        //The second check is mainly for the NPC to not perform extra actions to (usually get away from) a threat
                        if (scoutProjectile != null && !DangerThreshold())
                            ProjectileReact();
                        //Decrease the heal cooldown
                        if (restore[1]-- < 0)
                            restore[1] = 0;
                        if (restore[2]-- < 0)
                            restore[2] = 0;
                        //Make the NPC walk towards the player and stop when nearby if not in combat
                        if (!continueAttack && !catchingUp && scoutNPC == null && scoutProjectile == null)
                        {
                            if (Vector2.Distance(new Vector2(NPC.Center.X, 0), new Vector2(player.Center.X, 0)) > 125)
                                StandardAI(Rect(player));
                        }
                        if (hiredBy != -1)
                        {
                            //Cancel all attacks (if any) and catch up to the player
                            if (catchingUp)
                            {
                                if (!DangerThreshold())
                                {
                                    continueAttack = false;
                                    if (CatchUp())
                                        catchingUp = false;
                                }
                            }
                        }
                    }
                    //If health has been successsfully restored, set a cooldown
                    else if (RestoreHealth())
                    {
                        restore[0] = 0;
                        restore[2] = HealCooldown * 60;
                    }
                }
            }
            //The NPC is invulnerable if not hired
            else
            {
                NPC.dontTakeDamage = true;
                NPC.dontTakeDamageFromHostiles = true;
            }
            //Look for threats if hired
            if (hiredBy != -1)
                ScoutThreats();
        }
        public List<MercenaryDialogue> PlaceholderDialogue()
        {
            bool threats = scoutNPC != null;
            return new List<MercenaryDialogue>()
            {
                //These 4 are; perfect health, good health, half health, low health
                new MercenaryDialogue(":>", !threats && NPC.life == NPC.lifeMax, 2),
                new MercenaryDialogue(":)", !threats && NPC.life > 0.5f * NPC.lifeMax && NPC.life < NPC.lifeMax, 2),
                new MercenaryDialogue(":|", !threats && NPC.life <= 0.5f * NPC.lifeMax && !DangerThreshold(), 2),
                new MercenaryDialogue(":(", !threats && DangerThreshold(), 3),
                //Never shows; used for testing priority 
                new MercenaryDialogue("'-'", true, 1),
                //When an enemy is present
                new MercenaryDialogue(">:|=|", threats, 5),
                //When an enemy is present and at low health (escaping)
                new MercenaryDialogue("'o-o", threats && DangerThreshold(), 10),
                //When not hired
                new MercenaryDialogue(":/", hiredBy == -1, 11)
            };
        }
        /// <summary>
        /// Performed when in danger; usually used for evading
        /// </summary>
        public virtual void SafetyBehaviour()
        {

        }
        /// <summary>
        /// Performed when a projectile is detected; usually dodging the projectile
        /// </summary>
        public virtual void ProjectileReact()
        {

        }
        /// <summary>
        /// Used for catching up to the player; returns if the NPC has catched up
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool CatchUp(Vector2 location = new Vector2())
        {
            return false;
        }
        /// <summary>
        /// Returns if the NPC is too far away from the player; always returns false if in danger
        /// </summary>
        /// <returns></returns>
        public bool TooFar()
        {
            if (scoutNPC == null && scoutProjectile == null && !DangerThreshold())
            {
                float distance = Vector2.Distance(Main.player[hiredBy].MountedCenter, NPC.Center);
                if (distance >= 1000)
                {
                    catchingUp = true;
                    return true;
                }
                if (distance >= 500 && farTimer++ > 120)
                {
                    catchingUp = true;
                    return true;
                }
                else
                    return catchingUp;
            }
            return false;
        }
        /// <summary>
        /// Returns if the NPC is in danger of dying if their life falls below 25%
        /// </summary>
        /// <returns>A boolean value of whether the NPC is below 25% life</returns>
        public bool DangerThreshold() => NPC.life < NPC.lifeMax * 0.25f;
        /// <summary>
        /// Returns if the NPC should stop calling this method; called when a detected enemy is nearby
        /// </summary>
        /// <returns></returns>
        public virtual bool CloseAttack()
        {
            return false;
        }
        /// <summary>
        /// Returns if the NPC should stop calling this method; called when a detected enemy is far away
        /// </summary>
        /// <returns></returns>
        public virtual bool FarAttack()
        {
            return false;
        }
        Projectile RadialProjectileCheck()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && !p.friendly)
                {
                    //Unlike NPCs, projectiles must be in a set radius to be detected
                    int[] results = WithinDetectRange(p.Center, 0.5f);
                    if (results[0] == 1)
                    {
                        float dimensions = (float)Math.Sqrt(Math.Pow(NPC.width, 1) + Math.Pow(NPC.height, 1));
                        //Checks if the projectile, with its velocity, is expected to hit the mercenary, based on the projectile's dimensions
                        //(NEEDS WORK)
                        if (DistanceToPoint(p.Center, p.velocity, new Point(p.width + 8, p.height + 8)) < (float)Math.Pow(dimensions, 2))
                            return p;
                    }
                }
            }
            return null;
        }
        public virtual bool RestoreHealth()
        {
            return false;
        }
        NPC RadialNPCCheck()
        {
            List<bool> nearby = new List<bool>();
            List<float> distance = new List<float>();
            List<NPC> possibleTargets = new List<NPC>();
            foreach (NPC n in Main.npc)
            {
                if (n != null && n.active && !n.immortal && !n.dontTakeDamage && n.type != NPCID.TargetDummy && ((!n.friendly && !n.CountsAsACritter) || n.boss))
                {
                    //Checks if the NPC is in a set radius
                    int[] results = WithinDetectRange(n.Center, 2);
                    if (results[0] == 1)
                    {
                        //distance is used for finding the closest NPC
                        distance.Add(Vector2.Distance(n.Center, NPC.Center));
                        possibleTargets.Add(n);
                        //used for checking if the closest NPC needs a close attack or a far attack
                        nearby.Add(results[1] == 1 && Vector2.Distance(new Vector2(n.Center.Y, 0), new Vector2(NPC.Center.Y, 0)) < 75);
                    }
                }
            }
            if (distance.Count > 0)
            {
                //Finds the smallest distance and bases the needed attack performed based on their "nearby" placement
                float lowest = 99999;
                for (int a = 0; a < distance.Count; a++)
                    if (distance[a] < lowest)
                        lowest = distance[a];
                for (int a = 0; a < distance.Count; a++)
                    if (distance[a] == lowest)
                    {
                        closeAttackStyle = nearby[a];
                        return possibleTargets[a];
                    }
            }
            return null;
        }
        public int[] WithinDetectRange(Vector2 pos, float multiplier = 1)
        {
            //Checks if an NPC is withing a radial section
            int[] result = new int[2] { 0, 0 };
            if (Math.Pow(pos.X - NPC.Center.X, 2) + Math.Pow(pos.Y - NPC.Center.Y, 2) < Math.Pow(DetectRadius * multiplier, 2))
            {
                result[0] = 1;
                //Used for RadialNPCCheck() (the nearby check)
                if (Math.Pow(pos.X - NPC.Center.X, 2) + Math.Pow(pos.Y - NPC.Center.Y, 2) < Math.Pow((DetectRadius * multiplier) / 3f, 2))
                    result[1] = 1;
            }
            return result;
        }
        //Used in RadialProjectileCheck(); checks if a projectile is going towards an NPC, and is expected to collide with the hitbox 
        //(NEEDS WORK)
        float DistanceToPoint(Vector2 pos, Vector2 velocity, Point dimensions)
        {
            float multiplier = 0.66f;
            Vector2[] points = new Vector2[2] { pos, pos + velocity };
            Vector2 point = NPC.Center;
            float function = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            // |1g - (h / f) + ((b - af) / f) divided by sqrt(1^2 + (-1 / f)^2
            float distance = (float)Math.Abs(point.X - (point.Y / function) + ((points[0].Y - (points[0].X * function)) / function)) / (float)Math.Sqrt(1 + Math.Pow(-1 / function, 2));
            float totalDimensions = (float)Math.Sqrt((multiplier * dimensions.X) + (multiplier * dimensions.Y));
            return distance / totalDimensions;
        }
        //Usually overrided; otherwise visualizes the wall and pit detection
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D checkTest = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Mercenary/TestDetection").Value;
            spriteBatch.Draw(checkTest, wallDetectPos - screenPos, new Rectangle(0, 0, checkTest.Width, checkTest.Height), drawColor, 0, new Vector2(checkTest.Width / 2, checkTest.Height / 2), 1, NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            spriteBatch.Draw(checkTest, groundDetectPos - screenPos, new Rectangle(0, 0, checkTest.Width, checkTest.Height), drawColor, (float)Math.PI * 0.5f, new Vector2(checkTest.Width / 2, checkTest.Height / 2), 1, SpriteEffects.FlipHorizontally, 0);
        }
        public override bool CheckActive() => false;
        public override string GetChat() => MercenaryDialogue.PickDialogueOption(Dialogue);
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = $"{(hiredBy == -1 ? "Hire" : "Prolong (10 minutes)")}: 1 gold";
            if (hiredBy != -1)
                button2 = $"{hireTime} minutes";
        }
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                //Hires (if possible) and extends the hired duration by 10 minutes if a gold is spent
                Player player = Main.LocalPlayer;
                if (player.CanBuyItem(10000))
                {
                    if (hiredBy == -1)
                        hiredBy = player.whoAmI;
                    player.BuyItem(10000);
                    ExtendTimer(10);
                    SoundEngine.PlaySound(SoundID.Coins, NPC.Center);
                }
            }
        }
        public IEntitySource Source() => NPC.GetSource_FromAI();
        public void ExtendTimer(int minutes) { hireTime += minutes + 1; hireTimer += 3600 * minutes; }
    }
    public class ScreenShake
    {
        public float duration;
        public float strength;
        public bool dead;
        public ScreenShake(float d, float s) { duration = d; strength = s; }
        public Vector2 Update()
        {
            //Returns the screen position plus an amplified, random offset
            strength -= strength / duration;
            if (strength <= 0)
                dead = true;
            if (!dead)
                return new Vector2(Main.screenPosition.X + Main.rand.NextFloat(-strength, strength), Main.screenPosition.Y + Main.rand.NextFloat(-strength, strength));
            return Main.screenPosition;
        }
        public static void ScreenShakeEvent(Vector2 center, float duration2, float strength2, float radius = 100)
        {
            //Updates ScreenShakePlayer for every player in a given radius, and starts a screen shake event for each player
            foreach (Player p in Main.player)
            {
                if (p != null && p.active && !p.dead)
                {
                    if (Math.Pow(p.MountedCenter.X - center.X, 2) + Math.Pow(p.MountedCenter.Y - center.Y, 2) < Math.Pow(radius * 2, 2))
                        p.GetModPlayer<ScreenShakePlayer>().screenShake = new ScreenShake(duration2, strength2);
                }
            }
        }
    }
    public class ScreenShakePlayer : ModPlayer
    {
        public ScreenShake screenShake = new ScreenShake(1, 0);
        public override void ModifyScreenPosition()
        {
            if (!screenShake.dead)
                Main.screenPosition = screenShake.Update();
        }
    }
    public partial class MercenaryDrawHelper
    {
        //public Texture2D useTex;
        public virtual Texture2D SetTex { get; set; }
        public virtual Point Frame { get; set; }
        public void Draw(SpriteBatch spriteBatch, Point moveFrame, Vector2 pos, Color drawColor, SpriteEffects effects, Vector2 drawOffset = new Vector2(), float rotation = 0, float size = 0)
        {
            //A general drawing method; this class must have a new instance if a new mercenary must be added with a unique spritesheet
            Point frame2 = new Point(Frame.X * 2, Frame.Y * 2);
            Point moveOffset = new Point(moveFrame.X * Frame.X * 2, moveFrame.Y * Frame.Y * 2);
            Vector2 offset = new Vector2(Frame.X, Frame.Y) + drawOffset;
            float sz = size == 0 ? 1 : size;
            spriteBatch.Draw(SetTex, pos, new Rectangle(moveOffset.X, moveOffset.Y, frame2.X, frame2.Y), drawColor, rotation, offset, sz, effects, 0f);
        }
    }
    /// <summary>
    /// Rectangle but cool :sunglasses:
    /// </summary>
    public class RBC
    {
        public float width;
        public float height;
        public Vector2 position;
        public RBC(float w, float h, Vector2 p) { width = w; height = h; position = p; }
    }
    public class MercenaryDialogue
    {
        public string dialogue;
        public bool condition;
        public int priority;
        public MercenaryDialogue(string d, bool c, int p) { dialogue = d; condition = c; priority = p; }
        public static string PickDialogueOption(List<MercenaryDialogue> dialogueOptions)
        {
            int highest = 0;
            string result = "Blank";
            foreach (MercenaryDialogue d in dialogueOptions)
            {
                if (d.priority > highest && d.condition)
                {
                    highest = d.priority;
                    result = d.dialogue;
                }
            }
            return result;
        }
    }
}
