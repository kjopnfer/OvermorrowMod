using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class GrimoireFly : BaseMovementState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private static int TerrainMarginTiles = 4;

        private static int ActiveTime = 240;

        /// <summary>
        /// Distance away from the player in tiles
        /// </summary>
        private static int HoverDistance = 15;

        private static int NormalMinDistanceTiles = 10;
        private static int NormalMaxDistanceTiles = 15;
        private static int MinDistanceTiles = 5;
        private static int MaxDistanceTiles = 30;

        private static int MinPauseTime = 30;
        private static int MaxPauseTime = 90;

        private static float MoveSpeed = 5f;

        private Vector2 destination;
        private int activeTimer;
        private int pauseTimer;
        private int dodgeCooldown;
        private bool savedNoGravity;

        private float currentMoveSpeed;
        private int currentNormalMin;
        private int currentNormalMax;

        public GrimoireFly(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute() => OvermorrowNPC.TargetingModule.HasTarget();

        public override void Enter()
        {
            IsFinished = false;
            savedNoGravity = NPC.noGravity;
            NPC.noGravity = true;

            Personality p = OvermorrowNPC.Personality;

            float aggressionTimeScale = MathHelper.Lerp(1.4f, 0.6f, p.Aggression);
            int center = (int)(ActiveTime * aggressionTimeScale);
            activeTimer = center + Main.rand.Next(-30, 31);

            currentMoveSpeed = MoveSpeed * MathHelper.Lerp(0.7f, 1.4f, p.Reactivity);

            float distScale = MathHelper.Lerp(0.8f, 1.3f, p.Caution);
            currentNormalMin = (int)(NormalMinDistanceTiles * distScale);
            currentNormalMax = (int)(NormalMaxDistanceTiles * distScale);

            pauseTimer = 0;
            dodgeCooldown = 0;
            PickDestination();
        }

        public override void Exit()
        {
            NPC.velocity *= 0.5f;
            NPC.noGravity = savedNoGravity;
        }

        public override void Update()
        {
            if (!OvermorrowNPC.TargetingModule.HasTarget() || --activeTimer <= 0)
            {
                IsFinished = true;
                return;
            }

            if (dodgeCooldown > 0) dodgeCooldown--;

            if (dodgeCooldown == 0 && ProjectileIncoming(out _))
            {
                dodgeCooldown = 30;
                PickDestination();
                pauseTimer = 0;
            }

            if (pauseTimer > 0)
            {
                NPC.velocity *= 0.85f;
                if (--pauseTimer == 0) PickDestination();
            }
            else if (Vector2.DistanceSquared(NPC.Center, destination) < 8f * 8f)
            {
                pauseTimer = Main.rand.Next(MinPauseTime, MaxPauseTime + 1);
                NPC.velocity *= 0.5f;
            }
            else
            {
                Vector2 desired = (destination - NPC.Center).SafeNormalize(Vector2.Zero) * currentMoveSpeed;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desired, 0.3f);
                ApplyCollisionGuards();
            }
        }

        private void PickDestination()
        {
            Vector2 playerCenter = OvermorrowNPC.TargetingModule.Target.Center;
            float playerDist = Vector2.Distance(NPC.Center, playerCenter);

            if (ProjectileIncoming(out Projectile threat))
            {
                Vector2 perp = threat.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitX);
                if (Vector2.Dot(perp, NPC.Center - threat.Center) < 0f) perp = -perp;
                Vector2 sidestep = NPC.Center + perp * (MinDistanceTiles * 16f);
                if (IsClearAndReachable(sidestep))
                {
                    destination = sidestep;
                    return;
                }
            }

            float pressureRadius = HoverDistance * 16f;
            float pressure = MathHelper.Clamp(1f - playerDist / pressureRadius, 0f, 1f);

            if (PickRandomPosition(playerCenter, currentNormalMin, currentNormalMax, pressure, 8)) return;
            if (PickRandomPosition(playerCenter, MinDistanceTiles, MaxDistanceTiles, pressure, 6)) return;

            destination = NPC.Center;
        }

        private bool PickRandomPosition(Vector2 playerCenter, int minTiles, int maxTiles, float pressure, int attempts)
        {
            float minDistance = minTiles * 16f;
            float ringRadius = HoverDistance * (1f + pressure) * 16f;
            Vector2 awayFromNpc = (playerCenter - NPC.Center).SafeNormalize(Vector2.UnitX);
            float angleSpread = MathHelper.Lerp(MathHelper.Pi, MathHelper.PiOver4, pressure);

            for (int i = 0; i < attempts; i++)
            {
                Vector2 angleDir = awayFromNpc.RotatedByRandom(angleSpread);
                Vector2 ringPoint = playerCenter + angleDir * ringRadius;

                Vector2 toRing = ringPoint - NPC.Center;
                float distance = toRing.Length();

                int rolledMax = Main.rand.Next(minTiles, maxTiles + 1);
                int pressuredMax = (int)MathHelper.Lerp(rolledMax, maxTiles, pressure);
                float maxMove = pressuredMax * 16f;
                if (distance > maxMove)
                {
                    ringPoint = NPC.Center + toRing.SafeNormalize(Vector2.Zero) * maxMove;
                    distance = maxMove;
                }

                if (distance < minDistance) continue;
                if (!IsClearAndReachable(ringPoint)) continue;

                destination = ringPoint;
                return true;
            }
            return false;
        }

        private bool IsClearAndReachable(Vector2 to)
        {
            int pad = TerrainMarginTiles * 16;
            Vector2 topLeft = to - NPC.Size / 2f - new Vector2(pad, pad);
            if (Collision.SolidCollision(topLeft, NPC.width + pad * 2, NPC.height + pad * 2))
                return false;

            return Collision.CanHitLine(NPC.Center, 1, 1, to, 1, 1);
        }

        private void ApplyCollisionGuards()
        {
            if (NPC.collideY)
            {
                if (NPC.oldVelocity.Y < 0f && NPC.velocity.Y < 0f) NPC.velocity.Y = 0f;
                if (NPC.oldVelocity.Y > 0f && NPC.velocity.Y > 0f) NPC.velocity.Y = 0f;
            }

            if (NPC.collideX && Math.Sign(NPC.oldVelocity.X) == Math.Sign(NPC.velocity.X) && NPC.velocity.X != 0f)
                NPC.velocity.X = 0f;
        }

        private bool ProjectileIncoming(out Projectile incomingProjectile)
        {
            incomingProjectile = null;
            foreach (Projectile p in Main.projectile)
            {
                if (!p.active || !p.friendly || p.hostile) continue;
                if (Vector2.Distance(p.Center, NPC.Center) > 100f) continue;
                if (Vector2.Dot(NPC.Center - p.Center, p.velocity) <= 0f) continue;
                incomingProjectile = p;
                return true;
            }
            return false;
        }
    }
}
