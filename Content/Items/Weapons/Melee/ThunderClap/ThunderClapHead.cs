using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.ThunderClap
{
    public class ThunderClapHead : ModProjectile
    {
        // The folder path to the flail chain sprite
        private const string ChainTexturePath = AssetDirectory.Chains + "Lightning";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ThunderClap Head");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Make the flail infinitely penetrate like other flails
            Projectile.DamageType = DamageClass.Melee;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {
            // Spawn some dust visuals
            /*var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 172, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 1.5f);
			dust.noGravity = true;
			dust.velocity /= 2f;*/

            var player = Main.player[Projectile.owner];

            Vector2 vector221 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
            float num494 = player.position.X + (player.width / 2) - vector221.X;
            float num499 = player.position.Y + (player.height / 2) - vector221.Y;

            // If owner player dies, remove the flail.
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }

            // This prevents the item from being able to be used again prior to this Projectile dying
            player.itemAnimation = 10;
            player.itemTime = 10;

            // Here we turn the player and Projectile based on the relative positioning of the player and Projectile.
            int newDirection = Projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(newDirection);
            Projectile.direction = newDirection;

            var vectorToPlayer = player.MountedCenter - Projectile.Center;
            float currentChainLength = vectorToPlayer.Length();

            // Here is what various ai[] values mean in this AI code:
            // ai[0] == 0: Just spawned/being thrown out
            // ai[0] == 1: Flail has hit a tile or has reached maxChainLength, and is now in the swinging mode
            // ai[1] == 1 or !Projectile.tileCollide: Projectile is being forced to retract

            // ai[0] == 0 means the Projectile has neither hit any tiles yet or reached maxChainLength
            if (Projectile.ai[0] == 0f)
            {
                // This is how far the chain would go measured in pixels
                float maxChainLength = 160f;
                Projectile.tileCollide = true;

                if (currentChainLength > maxChainLength)
                {
                    Vector2 targetPos = Projectile.position;
                    float targetDist = 100f;
                    bool target = false;
                    float targetNPC = 0;
                    for (int k = 0; k < 200; k++)
                    {
                        NPC npc = Main.npc[k];
                        if (npc.CanBeChasedBy(this))
                        {
                            float distance = Vector2.Distance(npc.Center, Projectile.Center);
                            if ((distance < targetDist || !target) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
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
                        Vector2 shootVelocity = targetPos - Projectile.Center;
                        shootVelocity.Normalize();
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, shootVelocity, ModContent.ProjectileType<LightningBurst>(), 36, 10f, Projectile.owner, targetNPC, Projectile.whoAmI);
                    }

                    // If we reach maxChainLength, we change behavior.
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                else if (!player.channel)
                {
                    // Once player lets go of the use button, let gravity take over and let air friction slow down the Projectile
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y *= 0.9f;

                    Projectile.velocity.Y += 1f;
                    Projectile.velocity.X *= 0.9f;
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                // When ai[0] == 1f, the Projectile has either hit a tile or has reached maxChainLength, so now we retract the Projectile
                float elasticFactorA = 14f / player.GetAttackSpeed(DamageClass.Melee);
                float elasticFactorB = 0.9f / player.GetAttackSpeed(DamageClass.Melee);
                float maxStretchLength = 300f; // This is the furthest the flail can stretch before being forced to retract. Make sure that this is a bit less than maxChainLength so you don't accidentally reach maxStretchLength on the initial throw.

                if (Projectile.ai[1] == 1f)
                    Projectile.tileCollide = false;

                //Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

                // If the user lets go of the use button, or if the Projectile is stuck behind some tiles as the player moves away, the Projectile goes into a mode where it is forced to retract and no longer collides with tiles.
                if (!player.channel || currentChainLength > maxStretchLength || !Projectile.tileCollide)
                {
                    Projectile.ai[1] = 1f;

                    if (Projectile.tileCollide)
                        Projectile.netUpdate = true;

                    Projectile.tileCollide = false;

                    if (currentChainLength < 20f)
                        Projectile.Kill();
                }

                if (!Projectile.tileCollide)
                    elasticFactorB *= 2f;

                int restingChainLength = 60;

                // If there is tension in the chain, or if the Projectile is being forced to retract, give the Projectile some velocity towards the player
                if (currentChainLength > restingChainLength || !Projectile.tileCollide)
                {
                    var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - Projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += elasticAcceleration;
                }
                else
                {
                    // Otherwise, friction and gravity allow the Projectile to rest.
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 6f)
                    {
                        Projectile.velocity.X *= 0.96f;
                        Projectile.velocity.Y += 0.2f;
                    }
                    if (player.velocity.X == 0f)
                        Projectile.velocity.X *= 0.96f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // This custom OnTileCollide code makes the Projectile bounce off tiles at 1/5th the original speed, and plays sound and spawns dust if the Projectile was going fast enough.
            bool shouldMakeSound = false;

            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    shouldMakeSound = true;
                }

                Projectile.position.X += Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.2f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    shouldMakeSound = true;
                }

                Projectile.position.Y += Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            // ai[0] == 1 is used in AI to represent that the Projectile has hit a tile since spawning
            Projectile.ai[0] = 1f;

            if (shouldMakeSound)
            {
                // if we should play the sound..
                Projectile.netUpdate = true;
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                // Play the sound
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];

            Vector2 mountedCenter = player.MountedCenter;
            Texture2D chainTexture = ModContent.Request<Texture2D>(ChainTexturePath).Value;

            var drawPosition = Projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (Projectile.alpha == 0)
            {
                int direction = -1;

                if (Projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            // This while loop draws the chain texture from the Projectile to the player, looping to draw the chain texture along the path
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

                Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, Color.White, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}