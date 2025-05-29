using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class BasicFly : BaseMovementState
    {
        // TODO: These should probably be base properties of the NPC
        float flySpeedX = 2;
        float flySpeedY = 0;
        int distanceFromGround = 180;

        public override int Weight => 1;
        public override bool CanExit => true;
        public BasicFly(OvermorrowNPC npc) : base(npc) { }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            HandleHorizontalMovement(OvermorrowNPC, ref flySpeedX);
            HandleVerticalMovementToTarget(OvermorrowNPC, ref flySpeedY);
            HandleGroundProximity(OvermorrowNPC, ref flySpeedY, distanceFromGround);
            HandleObstacleAvoidance(OvermorrowNPC, ref flySpeedX, ref flySpeedY);
        }

        public static void HandleHorizontalMovement(OvermorrowNPC npc, ref float flySpeedX)
        {
            NPC baseNPC = npc.NPC;
            float targetSpeed = 2f;

            float? targetPosition = npc.TargetingModule.Target?.Center.X
                                  ?? npc.TargetingModule.MiscTargetPosition?.X;

            if (targetPosition == null)
                return;

            if (baseNPC.Center.X >= targetPosition.Value)
            {
                baseNPC.velocity.X = Math.Max(baseNPC.velocity.X - 0.05f, -targetSpeed);
                flySpeedX = Math.Max(flySpeedX - 0.1f, -targetSpeed);
            }
            else
            {
                baseNPC.velocity.X = Math.Min(baseNPC.velocity.X + 0.05f, targetSpeed);
                flySpeedX = Math.Min(flySpeedX + 0.1f, targetSpeed);
            }
        }

        public static void HandleVerticalMovementToTarget(OvermorrowNPC npc, ref float flySpeedY, float buffer = 16 * 5, float speed = 2f)
        {
            NPC baseNPC = npc.NPC;

            float? targetPosition = npc.TargetingModule.Target?.Center.Y
                                  ?? npc.TargetingModule.MiscTargetPosition?.Y;

            if (targetPosition == null)
                return;

            if (baseNPC.Center.Y <= targetPosition.Value - buffer)
            {

                baseNPC.velocity.Y = Math.Min(baseNPC.velocity.Y + 0.1f, speed);

                if (Main.rand.NextBool(3))
                    baseNPC.velocity.Y += 0.05f;

                flySpeedY = Math.Min(flySpeedY + 0.1f, speed);
            }
        }


        public static void HandleVerticalMovementToPoint(OvermorrowNPC npc, Vector2 point, ref float flySpeedY, float buffer = 16 * 5, float speed = 2f)
        {
            NPC baseNPC = npc.NPC;

            if (baseNPC.Center.Y <= point.Y - buffer)
            {
                baseNPC.velocity.Y = Math.Min(baseNPC.velocity.Y + 0.1f, speed);

                if (Main.rand.NextBool(3))
                    baseNPC.velocity.Y += 0.05f;

                flySpeedY = Math.Min(flySpeedY + 0.1f, speed);
            }
        }

        public static void HandleGroundProximity(OvermorrowNPC npc, ref float flySpeedY, int groundBuffer, float heightLimit = 192f)
        {
            NPC baseNPC = npc.NPC;

            float groundDistance = RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitY, groundBuffer);

            if (groundDistance < groundBuffer)
            {
                float t = 1f - MathHelper.Clamp(groundDistance / groundBuffer, 0f, 1f); // t ranges from 0 (far) to 1 (very close)

                float velocityBoost = MathHelper.Lerp(0.05f, 0.1f, t); // Small to strong boost based on t
                baseNPC.velocity.Y -= velocityBoost;

                flySpeedY = Math.Max(flySpeedY - velocityBoost, -1f);

            }
            else if (groundDistance > heightLimit)
            {
                baseNPC.velocity.Y += 0.1f;
                flySpeedY = Math.Min(flySpeedY + 0.1f, 2f);
            }

            float ceilingBuffer = groundBuffer / 2;
            float ceilingDistance = RayTracing.CastTileCollisionLength(baseNPC.Center, -Vector2.UnitY, ceilingBuffer);
            if (ceilingDistance < ceilingBuffer)
            {
                float t = 1f - MathHelper.Clamp(groundDistance / groundBuffer, 0f, 1f); // t ranges from 0 (far) to 1 (very close)

                float velocityBoost = MathHelper.Lerp(0.05f, 0.1f, t); // Small to strong boost based on t
                baseNPC.velocity.Y += velocityBoost;

                flySpeedY = Math.Max(flySpeedY - velocityBoost, -1f);
            }
        }

        public static void HandleObstacleAvoidance(OvermorrowNPC npc, ref float flySpeedX, ref float flySpeedY)
        {
            NPC baseNPC = npc.NPC;
            float obstacleBuffer = 45f;

            if (RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitX * baseNPC.direction, obstacleBuffer) < obstacleBuffer)
            {
                baseNPC.velocity.X -= 0.25f * baseNPC.direction;
                flySpeedX -= 0.25f * baseNPC.direction;

                baseNPC.velocity.Y -= 0.5f;
                flySpeedY = Math.Max(flySpeedY - 0.5f, -2f);
            }
        }
    }
}