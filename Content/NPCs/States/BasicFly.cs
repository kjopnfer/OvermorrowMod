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
        float aggroDelay = 60;
        int distanceFromGround = 180;

        public override int Weight => 1;
        public override bool CanExit => true;
        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("starting basic fly");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("exiting basic fly");
        }

        public override void Update(OvermorrowNPC npc)
        {
            HandleHorizontalMovement(npc);
            HandleVerticalMovement(npc);
            HandleGroundProximity(npc);
            HandleObstacleAvoidance(npc);
        }

        private void HandleHorizontalMovement(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            float targetSpeed = 2f;

            var target = npc.TargetingModule.Target;

            if (baseNPC.Center.X >= target.Center.X)
            {
                baseNPC.velocity.X = Math.Max(baseNPC.velocity.X - 0.05f, -targetSpeed);
                flySpeedX = Math.Max(flySpeedX - 0.1f, -targetSpeed);
            }
            else if (baseNPC.Center.X <= target.Center.X)
            {
                baseNPC.velocity.X = Math.Min(baseNPC.velocity.X + 0.05f, targetSpeed);
                flySpeedX = Math.Min(flySpeedX + 0.1f, targetSpeed);
            }
        }

        private void HandleVerticalMovement(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            var target = npc.TargetingModule.Target;

            float verticalBuffer = 16 * 5;
            float targetSpeed = 2f;

            if (baseNPC.Center.Y <= target.Center.Y - verticalBuffer)
            {
                baseNPC.velocity.Y = Math.Min(baseNPC.velocity.Y + 0.1f, targetSpeed);

                // Add randomness to avoid straight-line movement
                if (Main.rand.NextBool(3))
                    baseNPC.velocity.Y += 0.05f;

                flySpeedY = Math.Min(flySpeedY + 0.1f, targetSpeed);
            }
        }

        private void HandleGroundProximity(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            float groundBuffer = distanceFromGround;

            if (RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitY, groundBuffer) < groundBuffer)
            {
                baseNPC.velocity.Y -= 0.1f;
                flySpeedY = Math.Max(flySpeedY - 0.1f, -2f);
            }
        }

        private void HandleObstacleAvoidance(OvermorrowNPC npc)
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