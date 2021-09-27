using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Boss;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class StormWhelp : ModProjectile
    {
        int randDelay;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Whelp");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 72;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 2f;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DrakeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<DrakeBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion

            #region General Behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                projectile.position = idlePosition;
                projectile.velocity *= 0.1f;
                projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // Fix overlap with other minions
                Projectile other = Main.projectile[i];
                if (i != projectile.whoAmI && other.active && other.owner == projectile.owner && Math.Abs(projectile.position.X - other.position.X) + Math.Abs(projectile.position.Y - other.position.Y) < projectile.width)
                {
                    if (projectile.position.X < other.position.X) projectile.velocity.X -= overlapVelocity;
                    else projectile.velocity.X += overlapVelocity;

                    if (projectile.position.Y < other.position.Y) projectile.velocity.Y -= overlapVelocity;
                    else projectile.velocity.Y += overlapVelocity;
                }
            }
            #endregion

            #region Find target
            // Starting search distance
            float distanceFromTarget = 2000f; // range of 700f
            Vector2 targetCenter = projectile.position;
            NPC targetNPC = null;
            bool foundTarget = false;


            //projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            if (projectile.velocity.X > 0f)
            {
                projectile.spriteDirection = projectile.direction = -1;
            }
            else if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = projectile.direction = 1;
            }

            // This code is required if your minion weapon has the targeting feature
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, projectile.Center);
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
                        float between = Vector2.Distance(npc.Center, projectile.Center); // distance between the npc and minion
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between; // targetcenter = npc center
                        bool inRange = between < distanceFromTarget; // if the distance between npc and minion is less than 700
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
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
            projectile.friendly = foundTarget;
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
                        projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                    }
                    else if (projectile.velocity == Vector2.Zero)
                    {
                        // If there is a case where it's not moving at all, give it a little "poke"
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    /*Vector2 direction = targetCenter - projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;*/

                    if ((targetCenter - projectile.Center).X > 0f)
                    {
                        projectile.spriteDirection = projectile.direction = -1;
                    }
                    else if ((targetCenter - projectile.Center).X < 0f)
                    {
                        projectile.spriteDirection = projectile.direction = 1;
                    }

                    Vector2 delta = targetCenter - projectile.Center;
                    float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    if (magnitude > 0)
                    {
                        delta *= 12f / magnitude;
                    }
                    else
                    {
                        delta = new Vector2(0f, 5f);
                    }

                    if (projectile.ai[0] == 1)
                    {
                        randDelay = Main.rand.Next(200, 300);
                    }

                    bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);

                    if (projectile.ai[0] % randDelay == 0 && lineOfSight) // prevent from instantly shooting when spawned
                    {
                        Main.PlaySound(SoundID.Item109, (int)projectile.position.X, (int)projectile.position.Y);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(projectile.Center, delta, ModContent.ProjectileType<LightningBreathFriendly>(), projectile.damage / 2, 0f, projectile.owner);
                        }
                    }
                }
                else
                {
                    bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);

                    if (lineOfSight)
                    {
                        // The immediate range around the target (so it doesn't latch onto it when close)
                        Vector2 direction = targetCenter - projectile.Center;
                        direction.Normalize();
                        direction *= speed;
                        projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
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
                            projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                        }
                        else if (projectile.velocity == Vector2.Zero)
                        {
                            // If there is a case where it's not moving at all, give it a little "poke"
                            projectile.velocity.X = -0.15f;
                            projectile.velocity.Y = -0.05f;
                        }
                    }
                }
                projectile.ai[0]++;
                if (projectile.ai[0] > 300)
                {
                    projectile.ai[0] = 0;
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
                    projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            #endregion

            #region Animation and visuals
            // So it will lean slightly towards the direction it's moving
            projectile.rotation = projectile.velocity.X * 0.05f;

            // Loop through the 4 animation frames, spending 5 ticks on each.
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Texture2D texture = mod.GetTexture("Projectiles/Summon/StormWhelp_Glowmask");

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Summon/StormWhelp_Glowmask");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f + 1f
                ),
                drawRectangle,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
    }
}