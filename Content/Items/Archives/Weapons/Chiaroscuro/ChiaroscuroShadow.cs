using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class ChiaroscuroShadow : ModProjectile, IDrawAdditive, IProjectileClassification
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "ChiaroscuroProjectile";

        public int maxTimeLeft;
        public float animationDuration = 0.7f;

        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public Player Owner => Main.player[Projectile.owner];
        public WeaponType WeaponType => WeaponType.Rapier;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            //Projectile.ownerHitCheck = true;

            Projectile.DamageType = DamageClass.Melee;
        }

        public Vector2 anchorOffset;
        int shadowIndex;
        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.timeLeft = (int)(Projectile.timeLeft * (1f / Owner.GetTotalAttackSpeed(DamageClass.Melee)));
            //maxTimeLeft = Projectile.timeLeft;
            int existingShadows = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type &&
                    Main.projectile[i].owner == Projectile.owner && Main.projectile[i].whoAmI != Projectile.whoAmI)
                {
                    existingShadows++;
                }
            }

            shadowIndex = existingShadows % 3; // Limit to 3 positions

            // Define the different anchor positions
            Vector2[] positions = new Vector2[]
            {
                new(-40, -60),     // Left side
                new(40, -60) ,      // Right side
                new(0, -105)     // Center/above
            };

            anchorOffset = positions[shadowIndex];
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public enum AIStates
        {
            Idle = 0,
            Invisible = 1,
            Attacking = 2
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float AIState => ref Projectile.ai[1];

        private float scale = 1f;
        public NPC AttackTarget { get; private set; }
        private Vector2 attackStartPos;
        public override void AI()
        {
            if (!Owner.active) Projectile.Kill();

            Projectile.timeLeft = 5;
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, MathHelper.Clamp(AICounter, 0, 15f) / 15f);

            switch (AIState)
            {
                case (int)AIStates.Invisible:
                    if (AICounter > 0)
                        AICounter--;

                    if (AICounter <= 0)
                    {
                        Projectile.Opacity = 0;
                    }

                    if (Projectile.Opacity == 0 && !Owner.HasBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>()))
                    {
                        Projectile.Kill();
                    }
                    break;
                case (int)AIStates.Attacking:
                    scale = 1f;
                    if (AttackTarget == null || !AttackTarget.active)
                    {
                        // Target is gone, return to invisible
                        AIState = (int)AIStates.Invisible;
                        AttackTarget = null;
                        AICounter = 0;
                        break;
                    }
                    if (AICounter == 0)
                    {
                        attackStartPos = Owner.MountedCenter + anchorOffset;

                        // Define the preset positions
                        Vector2[] positions = new Vector2[]
                        {
                            new(-80, -100),     // Left side
                            new(80, -100),      // Right side
                            new(0, -145)       // Center/above
                        };

                        // Calculate angle from target to player to determine rotation
                        Vector2 targetToPlayer = -Vector2.Normalize(Owner.Center - AttackTarget.Center);
                        float rotationAngle = targetToPlayer.ToRotation() + MathHelper.PiOver2; // +90° to align properly

                        // Rotate the position based on player's location relative to target
                        Vector2 selectedPosition = positions[shadowIndex].RotatedBy(rotationAngle);

                        // Store the final attack position
                        Projectile.localAI[0] = selectedPosition.X;
                        Projectile.localAI[1] = selectedPosition.Y;
                    }

                    // Use the stored attack position
                    Vector2 attackOffset = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                    Vector2 attackPos = AttackTarget.Center + attackOffset; // Position around target
                    Vector2 attackDirection = Vector2.Normalize(AttackTarget.Center - attackPos);

                    // Maintain rotation throughout the entire attack
                    Projectile.rotation = attackDirection.ToRotation() + MathHelper.PiOver2;

                    if (AICounter < 20)
                    {
                        // Move to attack position
                        float progress = AICounter / 20f;
                        Projectile.Center = Vector2.Lerp(attackStartPos, attackPos, EasingUtils.EaseOutCirc(progress));
                    }
                    else if (AICounter < 40)
                    {
                        // Stab toward target
                        float progress = (AICounter - 20f) / 20f;
                        Vector2 stabPos = AttackTarget.Center + attackDirection * -30f; // Position past target
                        Projectile.Center = Vector2.Lerp(attackPos, stabPos, EasingUtils.EaseOutBack(progress));
                    }
                    else if (AICounter < 60)
                    {
                        // Return to start position
                        float progress = (AICounter - 40f) / 20f;
                        Vector2 currentStabPos = AttackTarget.Center + attackDirection * -30f;
                        Projectile.Center = Vector2.Lerp(currentStabPos, attackPos, EasingUtils.EaseOutCirc(progress));
                        Projectile.Opacity = MathHelper.Lerp(1f, 0f, MathHelper.Clamp(AICounter - 40, 0, 20f) / 20f);
                    }
                    if (AICounter++ >= 60)
                    {
                        AIState = (int)AIStates.Invisible;
                        AttackTarget = null;
                        AICounter = 0;
                    }
                    break;
                default:
                    scale = 0.75f;
                    if (AICounter < 15)
                    {
                        AICounter++;
                    }

                    float floatSpeed = 0.025f;
                    float floatRange = 10f;

                    float floatOffset = MathF.Sin(Main.GameUpdateCount * floatSpeed) * floatRange;
                    Projectile.Center = Owner.MountedCenter + anchorOffset + new Vector2(0, floatOffset);

                    Projectile.rotation = 0;
                    break;
            }

            if (AICounter == 0 && Projectile.Opacity == 1)
            {
                Projectile.Opacity = 0;
            }
            //Main.NewText("AISTATE: " + AIState + " AICOUNTER: " + AICounter);

            //Projectile.Center = Owner.MountedCenter + new Vector2(0, -105 + floatOffset);
        }

        public void SetActive()
        {
            AIState = (int)AIStates.Invisible;
        }

        public void SetAttackTarget(NPC target)
        {
            if (AIState != (int)AIStates.Invisible) return;

            if (AttackTarget == null || !AttackTarget.active)
            {
                AIState = (int)AIStates.Attacking;
                AttackTarget = target;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (AICounter < 20 || AICounter > 40)
            {
                return false;
            }
            return AIState == (int)AIStates.Attacking && target == AttackTarget;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            /*var hitboxHeight = 140;
            Vector2 swordStart = Projectile.Center;
            Vector2 swordEnd = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            Vector2 hitPoint = Vector2.Lerp(swordStart, swordEnd,
                MathHelper.Clamp(Vector2.Dot(target.Center - swordStart, swordEnd - swordStart) /
                Vector2.DistanceSquared(swordStart, swordEnd), 0f, 1f));

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D circleTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < 3; i++)
            {
                float randomScale = Main.rand.NextFloat(0.15f, 0.3f);
                Vector2 RandomVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(9, 11);
                Color color = new Color(149, 149, 239);

                var lightSpark = new Spark(sparkTexture, 0f, false, 0f);
                lightSpark.endColor = new Color(108, 108, 224);
                ParticleManager.CreateParticleDirect(lightSpark, hitPoint, RandomVelocity, color, 1f, randomScale, MathHelper.Pi, useAdditiveBlending: true);

                var impact = new Circle(circleTexture, ModUtils.SecondsToTicks(0.5f), false, false);
                impact.endColor = new Color(108, 108, 224);

                randomScale = Main.rand.NextFloat(0.25f, 0.45f);
                ParticleManager.CreateParticleDirect(impact, target.Center, Vector2.Zero, color, 0.5f, randomScale, MathHelper.Pi, useAdditiveBlending: true);
            }*/

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox)) return true;

            var hitboxHeight = 40;
            var hitboxWidth = 18;
            float _ = float.NaN;
            Vector2 endPosition = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPosition, hitboxWidth * scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.Black.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(1f);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.White * Projectile.Opacity, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, scale, 0, 0);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Color color = new Color(108, 108, 224);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "ChiaroscuroProjectileGlow").Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, color * Projectile.Opacity, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, scale, 0, 0);
        }
    }
}