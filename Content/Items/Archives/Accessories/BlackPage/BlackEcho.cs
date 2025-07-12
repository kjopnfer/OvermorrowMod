using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class BlackEcho : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;

        public override bool MinionContactDamage() => true;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);

            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        private Vector2 previousCenter;
        private float wavePhase = 0f;
        private float baseDirection = MathHelper.ToRadians(-90);

        public override void OnSpawn(IEntitySource source)
        {
            previousCenter = Projectile.Center;
        }

        public enum AIStates
        {
            Spawned = -1,
            Idle = 0,
            Alert = 1,
            Attack = 2
        }

        public ref float AIState => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<BlackEchoes>());
            }

            if (owner.HasBuff(ModContent.BuffType<BlackEchoes>()))
            {
                Projectile.timeLeft = 2;
            }

            //Projectile.timeLeft = 5;

            switch (AIState)
            {
                case (int)AIStates.Spawned:
                    Projectile.velocity *= 0.99f;

                    if (AICounter++ > 60)
                    {
                        AICounter = 0;
                        AIState = (int)AIStates.Idle;
                    }
                    break;
                case (int)AIStates.Idle:
                    IdleState(owner);
                    Projectile.rotation = 0f;
                    break;
                case (int)AIStates.Alert:
                    AlertState(owner);
                    break;
                case (int)AIStates.Attack:
                    AttackState(owner);

                    if (Projectile.velocity.LengthSquared() > 0.1f)
                    {
                        float offset = Projectile.direction == -1 ? MathHelper.ToRadians(180) : 0;
                        Projectile.rotation = Projectile.velocity.ToRotation() + offset;
                    }

                    Vector2 velocity = Projectile.Center - previousCenter;
                    previousCenter = Projectile.Center;
                    break;
            }
        }

        private void IdleState(Player owner)
        {
            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);

            if (SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter))
            {
                AIState = (int)AIStates.Alert;
                AICounter = 0;
                return;
            }

            IdleMovement(distanceToIdlePosition, vectorToIdlePosition);
            Projectile.friendly = false;
        }

        private void AlertState(Player owner)
        {
            AICounter++;

            // Just play animation and transition, no movement or target searching
            if (AICounter >= 60)
            {
                AIState = (int)AIStates.Attack;
                AICounter = 0;
            }

            Projectile.friendly = false;
        }

        private void AttackState(Player owner)
        {
            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);

            if (SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter))
            {
                AttackMovement(foundTarget, distanceFromTarget, targetCenter);
                Projectile.friendly = true;
            }
            else
            {
                // No target found, return to idle
                AIState = (int)AIStates.Idle;
                AICounter = 0;
                Projectile.friendly = false;
            }
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f;

            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX;

            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            float overlapVelocity = 0.04f;

            foreach (var other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private bool SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            distanceFromTarget = ModUtils.TilesToPixels(40);
            targetCenter = Projectile.position;
            foundTarget = false;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    return true;
                }
            }

            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                    bool closeThroughWall = between < ModUtils.TilesToPixels(12);

                    if (((closest && inRange) || !foundTarget) /*&& (lineOfSight || closeThroughWall)*/)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }

            return foundTarget;
        }

        private void IdleMovement(float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            float speed = 4f;
            float inertia = 80f;

            if (distanceToIdlePosition > 600f)
            {
                speed = 12f;
                inertia = 60f;
            }

            if (distanceToIdlePosition > 20f)
            {
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }

        private void AttackMovement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter)
        {
            if (foundTarget)
            {
                Vector2 directionToTarget = targetCenter - Projectile.Center;
                directionToTarget.Normalize();

                // Add random inaccuracy to make it chaotic
                float randomAngle = (Main.rand.NextFloat() - 0.5f) * MathHelper.ToRadians(120f); // Up to 60 degrees off
                directionToTarget = directionToTarget.RotatedBy(randomAngle);

                float homingSpeed = 22f + Main.rand.NextFloat(6f, 9f);

                // Distance-based turn resistance - less drag when further away
                float baseTurnResistance = 0.01f;
                float maxTurnResistance = 0.06f;
                float maxDistance = ModUtils.TilesToPixels(50);

                float distanceRatio = Math.Min(distanceFromTarget / maxDistance, 1f);
                float turnResistance = baseTurnResistance + (distanceRatio * (maxTurnResistance - baseTurnResistance));

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * homingSpeed, turnResistance);

                if (Main.rand.NextBool(15)) // 1 in 15 chance each frame
                    Projectile.velocity += Main.rand.NextVector2Unit() * 4f;
            }
        }

        private void DrawEyeTrail(Vector2 center, Texture2D texture, Effect effect = null, float segmentSpacing = 5f, float baseWidth = 32f)
        {
            if (Projectile.oldPos.Length < 2) return;

            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                // Skip if position is zero (uninitialized)
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i + 1] == Vector2.Zero) continue;

                float prog1 = (float)i / (float)(Projectile.oldPos.Length - 1);
                float prog2 = (float)(i + 1) / (float)(Projectile.oldPos.Length - 1);

                // Get cached positions and add projectile center offset since oldPos stores top-left corner
                Vector2 v0 = Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2f;
                Vector2 v1 = Projectile.oldPos[i + 1] + new Vector2(Projectile.width, Projectile.height) / 2f;

                // Add smokey drift to positions to make them less rigid
                float timeOffset = wavePhase + i * 0.4f;
                float driftStrength = MathHelper.Lerp(0.02f, 0.3f, prog1);

                Vector2 smokeOffset = new Vector2(
                    (float)Math.Sin(timeOffset * 0.8f) * driftStrength * 15f,
                    (float)Math.Cos(timeOffset * 1.1f) * driftStrength * 10f
                );

                //v0 += smokeOffset;
                //v1 += smokeOffset * 0.8f;

                Vector2 normaldir = v1 - v0;
                if (normaldir.LengthSquared() > 0)
                {
                    normaldir = new Vector2(normaldir.Y, -normaldir.X);
                    normaldir.Normalize();
                }
                else
                {
                    // Fallback to using rotation if positions are too close
                    float rotation = i < Projectile.oldRot.Length ? Projectile.oldRot[i] : Projectile.rotation;
                    normaldir = new Vector2((float)Math.Cos(rotation + MathHelper.PiOver2), (float)Math.Sin(rotation + MathHelper.PiOver2));
                }

                // Your preferred easing - tapering with EaseInOutBounce
                float width1 = baseWidth * MathHelper.Lerp(1f, 0f, EasingUtils.EaseInOutBounce(prog1));
                float width2 = baseWidth * MathHelper.Lerp(1f, 0f, EasingUtils.EaseInOutBounce(prog2));

                Vector2 pos1Top = v0 + width1 * normaldir;
                Vector2 pos1Bottom = v0 - width1 * normaldir;
                Vector2 pos2Top = v1 + width2 * normaldir;
                Vector2 pos2Bottom = v1 - width2 * normaldir;

                // Convert to screen space
                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                // Your preferred alpha calculation with EaseInQuad
                float alpha = MathHelper.Lerp(2f, 0f, MathHelper.Clamp(EasingUtils.EaseInQuad(prog1), 0, 1f));
                Color color1 = Color.Lerp(Color.Red, Color.Black, EasingUtils.EaseOutQuint(prog1)) * alpha;
                Color color2 = Color.Lerp(Color.Red, Color.Black, EasingUtils.EaseOutQuint(prog2)) * alpha;

                // UV mapping
                float segmentSize = 1.0f / (float)(Projectile.oldPos.Length - 1);
                float startOffset = segmentSize * i;
                float endOffset = segmentSize * (i + 1);

                // Define triangles to map textures to
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color1, new Vector2(startOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color2, new Vector2(endOffset, 1f)));
            }

            if (vertices.Count > 2)
            {
                DrawPrimitives(vertices, texture, effect ?? OvermorrowModFile.Instance.TrailShader.Value);
            }
        }

        private void DrawPrimitives(List<VertexPositionColorTexture> vertices, Texture2D texture, Effect effect)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            if (effect.Parameters["WorldViewProjection"] != null)
                effect.Parameters["WorldViewProjection"].SetValue(model * projection);
            else if (effect.Parameters["WVP"] != null)
                effect.Parameters["WVP"].SetValue(model * projection);
            else if (effect.Parameters.Count > 0)
                effect.Parameters[0].SetValue(model * projection);

            effect.SafeSetParameter("uImage0", texture);
            effect.CurrentTechnique.Passes["Texturized"].Apply();

            Main.instance.GraphicsDevice.Textures[0] = texture;
            Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                vertices.ToArray(), 0, vertices.Count / 3);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void OnKill(int timeLeft)
        {
            ShadowGrasp.SpawnSmoke(Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float alpha = 1f;
            //Rectangle drawRectangle = new Rectangle(0, (texture.Height / 3) * 2, texture.Width, texture.Height / 3);
            var frame = 0;
            float rotation = Projectile.rotation;
            switch (AIState)
            {
                case (int)AIStates.Alert:
                    frame = 1;
                    if (AICounter > 10)
                    {
                        // Calculate shake intensity that increases then decreases
                        float shakeProgress = (AICounter - 10f) / 50f; // 50 ticks from start to end of alert
                        float intensityCurve = (float)(Math.Sin(shakeProgress * Math.PI)); // Creates 0 -> 1 -> 0 curve
                        float maxShakeIntensity = MathHelper.ToRadians(30f);
                        float currentIntensity = intensityCurve * maxShakeIntensity;

                        float shakeSpeed = 0.8f;
                        float shakeOffset = (float)Math.Sin(AICounter * shakeSpeed) * currentIntensity;
                        rotation += shakeOffset;
                    }
                    break;
                case (int)AIStates.Attack:
                    frame = 2;
                    break;
                default:
                    frame = 0;
                    break;
            }

            var frameHeight = texture.Height / 3;
            Rectangle drawRectangle = new Rectangle(0, frameHeight * frame, texture.Width, frameHeight);

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * alpha, rotation, drawRectangle.Size() / 2f, 1f, spriteEffects, 1);

            if (AIState == (int)AIStates.Attack)
            {
                Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Jagged").Value;
                DrawEyeTrail(Projectile.Center, trailTexture, null, segmentSpacing: 8f, baseWidth: 16f);
            }

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = circle.Size() / 2f;

            (float scale, float alpha, Color color)[] glowLayers =
            [
                (0.15f, 0.85f, new Color(150, 30, 30)),
                (0.1f, 0.6f,  new Color(180, 40, 40)),
                (0.075f,  0.4f,  new Color(210, 50, 50)),
                (0.05f, 0.35f, new Color(230, 60, 60)),
                (0.025f, 0.5f,  new Color(255, 70, 70)),
                (0.02f, 1f,  Color.Yellow)
            ];

            foreach (var (scale, alpha, color) in glowLayers)
            {
                spriteBatch.Draw(circle, center, null, color * alpha, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}