using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee.ThunderClap
{
    public class ThunderClapHead : ModProjectile
    {
        // The folder path to the flail chain sprite
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Melee/ThunderClap/ThunderChain";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ThunderClap Head");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = -1; // Make the flail infinitely penetrate like other flails
            projectile.melee = true;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {
            // Spawn some dust visuals
            /*var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 1.5f);
			dust.noGravity = true;
			dust.velocity /= 2f;*/

            var player = Main.player[projectile.owner];

            Vector2 vector221 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
            float num494 = player.position.X + (player.width / 2) - vector221.X;
            float num499 = player.position.Y + (player.height / 2) - vector221.Y;

            // If owner player dies, remove the flail.
            if (player.dead)
            {
                projectile.Kill();
                return;
            }

            // This prevents the item from being able to be used again prior to this projectile dying
            player.itemAnimation = 10;
            player.itemTime = 10;

            // Here we turn the player and projectile based on the relative positioning of the player and projectile.
            int newDirection = projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(newDirection);
            projectile.direction = newDirection;

            var vectorToPlayer = player.MountedCenter - projectile.Center;
            float currentChainLength = vectorToPlayer.Length();

            // Here is what various ai[] values mean in this AI code:
            // ai[0] == 0: Just spawned/being thrown out
            // ai[0] == 1: Flail has hit a tile or has reached maxChainLength, and is now in the swinging mode
            // ai[1] == 1 or !projectile.tileCollide: projectile is being forced to retract

            // ai[0] == 0 means the projectile has neither hit any tiles yet or reached maxChainLength
            if (projectile.ai[0] == 0f)
            {
                // This is how far the chain would go measured in pixels
                float maxChainLength = 160f;
                projectile.tileCollide = true;

                if (currentChainLength > maxChainLength)
                {
                    Vector2 targetPos = projectile.position;
                    float targetDist = 100f;
                    bool target = false;
                    float targetNPC = 0;
                    for (int k = 0; k < 200; k++)
                    {
                        NPC npc = Main.npc[k];
                        if (npc.CanBeChasedBy(this))
                        {
                            float distance = Vector2.Distance(npc.Center, projectile.Center);
                            if ((distance < targetDist || !target) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                            {
                                targetDist = distance;
                                targetPos = npc.Center;
                                targetNPC = npc.whoAmI;
                                target = true;
                            }
                        }
                    }

                    if (target)
                    {
                        Vector2 shootVelocity = targetPos - projectile.Center;
                        shootVelocity.Normalize();
                        Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<LightningBurst>(), 36, 10f, projectile.owner, targetNPC, projectile.whoAmI);
                    }

                    // If we reach maxChainLength, we change behavior.
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                else if (!player.channel)
                {
                    // Once player lets go of the use button, let gravity take over and let air friction slow down the projectile
                    if (projectile.velocity.Y < 0f)
                        projectile.velocity.Y *= 0.9f;

                    projectile.velocity.Y += 1f;
                    projectile.velocity.X *= 0.9f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                // When ai[0] == 1f, the projectile has either hit a tile or has reached maxChainLength, so now we retract the projectile
                float elasticFactorA = 14f / player.meleeSpeed;
                float elasticFactorB = 0.9f / player.meleeSpeed;
                float maxStretchLength = 300f; // This is the furthest the flail can stretch before being forced to retract. Make sure that this is a bit less than maxChainLength so you don't accidentally reach maxStretchLength on the initial throw.

                if (projectile.ai[1] == 1f)
                    projectile.tileCollide = false;

                //projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;

                // If the user lets go of the use button, or if the projectile is stuck behind some tiles as the player moves away, the projectile goes into a mode where it is forced to retract and no longer collides with tiles.
                if (!player.channel || currentChainLength > maxStretchLength || !projectile.tileCollide)
                {
                    projectile.ai[1] = 1f;

                    if (projectile.tileCollide)
                        projectile.netUpdate = true;

                    projectile.tileCollide = false;

                    if (currentChainLength < 20f)
                        projectile.Kill();
                }

                if (!projectile.tileCollide)
                    elasticFactorB *= 2f;

                int restingChainLength = 60;

                // If there is tension in the chain, or if the projectile is being forced to retract, give the projectile some velocity towards the player
                if (currentChainLength > restingChainLength || !projectile.tileCollide)
                {
                    var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    projectile.velocity *= 0.98f;
                    projectile.velocity += elasticAcceleration;
                }
                else
                {
                    // Otherwise, friction and gravity allow the projectile to rest.
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
                    {
                        projectile.velocity.X *= 0.96f;
                        projectile.velocity.Y += 0.2f;
                    }
                    if (player.velocity.X == 0f)
                        projectile.velocity.X *= 0.96f;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // This custom OnTileCollide code makes the projectile bounce off tiles at 1/5th the original speed, and plays sound and spawns dust if the projectile was going fast enough.
            bool shouldMakeSound = false;

            if (oldVelocity.X != projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.X += projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.2f;
            }

            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.Y += projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            // ai[0] == 1 is used in AI to represent that the projectile has hit a tile since spawning
            projectile.ai[0] = 1f;

            if (shouldMakeSound)
            {
                // if we should play the sound..
                projectile.netUpdate = true;
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                // Play the sound
                Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = player.MountedCenter;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                // Once the remaining length is small enough, we terminate the loop
                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 22 pixels
                // 22 comes from the height of the chain and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 22 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, Color.White, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}