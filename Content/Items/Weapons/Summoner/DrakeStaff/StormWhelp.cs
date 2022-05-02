using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Summon;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.DrakeStaff
{
    public class StormWhelp : ModProjectile
    {
        int randDelay;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Whelp");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 72;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region Active check
            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DrakeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<DrakeBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            #region General Behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is Projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the Projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // Fix overlap with other minions
                Projectile other = Main.projectile[i];
                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
                    else Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
                    else Projectile.velocity.Y += overlapVelocity;
                }
            }
            #endregion

            #region Find target
            // Starting search distance
            float distanceFromTarget = 2000f; // range of 700f
            Vector2 targetCenter = Projectile.position;
            NPC targetNPC = null;
            bool foundTarget = false;


            //Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            if (Projectile.velocity.X > 0f)
            {
                Projectile.spriteDirection = Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = Projectile.direction = 1;
            }

            // This code is required if your minion weapon has the targeting feature
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);
                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 50f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center); // distance between the npc and minion
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between; // targetcenter = npc center
                        bool inRange = between < distanceFromTarget; // if the distance between npc and minion is less than 700
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 2000f;
                        if (((closest && inRange) || !foundTarget) && ((lineOfSight) || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            targetNPC = npc;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
            #endregion

            #region Movement
            // Default movement parameters (here for attacking)
            float speed = 15f;
            float inertia = 20f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 300f)
                {
                    // Minion doesn't have a target: return to player and idle
                    if (distanceToIdlePosition > 600f)
                    {
                        // Speed up the minion if it's away from the player
                        speed = 20f;
                        inertia = 60f;
                    }
                    else
                    {
                        // Slow down the minion if closer to the player
                        speed = 4f;
                        inertia = 80f;
                    }
                    if (distanceToIdlePosition > 20f)
                    {
                        // The immediate range around the player (when it passively floats about)

                        // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                        vectorToIdlePosition.Normalize();
                        vectorToIdlePosition *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                    }
                    else if (Projectile.velocity == Vector2.Zero)
                    {
                        // If there is a case where it's not moving at all, give it a little "poke"
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    /*Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;*/

                    if ((targetCenter - Projectile.Center).X > 0f)
                    {
                        Projectile.spriteDirection = Projectile.direction = -1;
                    }
                    else if ((targetCenter - Projectile.Center).X < 0f)
                    {
                        Projectile.spriteDirection = Projectile.direction = 1;
                    }

                    Vector2 delta = targetCenter - Projectile.Center;
                    float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    if (magnitude > 0)
                    {
                        delta *= 12f / magnitude;
                    }
                    else
                    {
                        delta = new Vector2(0f, 5f);
                    }

                    if (Projectile.ai[0] == 1)
                    {
                        randDelay = Main.rand.Next(200, 300);
                    }

                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);

                    if (Projectile.ai[0] % randDelay == 0 && lineOfSight) // prevent from instantly shooting when spawned
                    {
                        SoundEngine.PlaySound(SoundID.Item109, (int)Projectile.position.X, (int)Projectile.position.Y);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, delta, ModContent.ProjectileType<LightningBreathFriendly>(), Projectile.damage / 2, 0f, Projectile.owner);
                        }
                    }
                }
                else
                {
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);

                    if (lineOfSight)
                    {
                        // The immediate range around the target (so it doesn't latch onto it when close)
                        Vector2 direction = targetCenter - Projectile.Center;
                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                    else
                    {
                        // Minion doesn't have a target: return to player and idle
                        if (distanceToIdlePosition > 600f)
                        {
                            // Speed up the minion if it's away from the player
                            speed = 20f;
                            inertia = 60f;
                        }
                        else
                        {
                            // Slow down the minion if closer to the player
                            speed = 4f;
                            inertia = 80f;
                        }
                        if (distanceToIdlePosition > 20f)
                        {
                            // The immediate range around the player (when it passively floats about)

                            // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            // If there is a case where it's not moving at all, give it a little "poke"
                            Projectile.velocity.X = -0.15f;
                            Projectile.velocity.Y = -0.05f;
                        }
                    }
                }
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 300)
                {
                    Projectile.ai[0] = 0;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 20f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 80f;
                }
                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            #endregion

            #region Animation and visuals
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // Loop through the 4 animation frames, spending 5 ticks on each.
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            #endregion
        }

        // Here you can decide if your minion breaks things like grass or pots
        // (in this example false is returned, since having this on true might cause the queen bee larva to break and summon the boss accidently)
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            //Texture2D texture = mod.GetTexture("Projectiles/Summon/StormWhelp_Glowmask");

            int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y2 = num154 * Projectile.frame;

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Projectiles/Summon/StormWhelp_Glowmask").Value;
            Rectangle drawRectangle = new Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);
            /*spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height - drawRectangle.Height * 0.5f + 1f
                ),
                drawRectangle,
                Color.White,
                Projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );*/
        }
    }
}