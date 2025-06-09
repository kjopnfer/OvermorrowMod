using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class ClockworkSpiderPinball : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private int numBounces = 0;
        private Vector2 currentDirection;
        private const int maxBounces = 5;
        private const float pinballSpeed = 8f;
        private bool startedPinball = false;
        private int pinballCooldown = 0;
        public ClockworkSpiderPinball(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (pinballCooldown-- > 0)
                return false;

            return OvermorrowNPC is ClockworkSpider && NPC.collideY;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
            numBounces = 0;
            startedPinball = false;

            NPC.velocity.X = 0;

            // Start with a leap (either up or down based on gravity)
            if (NPC.noGravity)
                NPC.velocity.Y = 6f;
            else
            {
                Main.NewText("GO UP");
                NPC.velocity.Y = -6f;
            }

            NPC.noGravity = true;
        }

        public override void Exit()
        {
            pinballCooldown = ModUtils.SecondsToTicks(2);
            OvermorrowNPC.AICounter = 0;
        }

        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            int delay = 18;
            if (!startedPinball)
            {
                // Decelerate after leap
                //NPC.velocity.Y *= 0.9f;
                if (OvermorrowNPC.AICounter > 16) NPC.velocity.Y = 0;
                //Main.NewText(Math.Abs(NPC.velocity.Y));
                //if (Math.Abs(NPC.velocity.Y) < 1f /*|| (NPC.collideY || NPC.collideX*/)
                if (OvermorrowNPC.AICounter > delay)
                {
                    TryGetPinballDirection(Vector2.Zero);
                    NPC.noGravity = true;
                    NPC.velocity = currentDirection * 10;
                    startedPinball = true;
                }
                return;
            }

            if (OvermorrowNPC.AICounter >= delay && (NPC.collideX || NPC.collideY))
            {
                numBounces++;

                if (numBounces >= maxBounces)
                {
                    // Final bounce: go straight down and re-enable gravity on floor impact
                    currentDirection = Vector2.UnitY;
                    NPC.velocity = currentDirection * 10;

                    if (NPC.collideY)
                    {
                        NPC.noGravity = false;
                        IsFinished = true;
                    }
                    return;
                }

                TryGetPinballDirection(currentDirection);
                NPC.velocity = currentDirection * 10;
            }
        }

        private void TryGetPinballDirection(Vector2 previousDirection)
        {
            float maxRayDistance = ModUtils.TilesToPixels(30);
            const float minAngleDifference = 0.5f;
            const float minTargetDot = 0.25f; // Try to bias towards target

            Vector2 targetDirection = OvermorrowNPC.TargetingModule?.Target != null
                    ? Vector2.Normalize(OvermorrowNPC.TargetingModule.Target.Center - NPC.Center)
                    : Vector2.Zero;
            for (int i = 0; i < 30; i++)
            {
                Vector2 dir = Vector2.Normalize(Main.rand.NextVector2CircularEdge(1f, 1f));
                if (previousDirection != Vector2.Zero && Vector2.Dot(dir, previousDirection) > MathF.Cos(minAngleDifference))
                    continue;
                if (targetDirection != Vector2.Zero && Vector2.Dot(dir, targetDirection) < minTargetDot)
                    continue;

                float distance = RayTracing.CastTileCollisionLength(NPC.Center, dir, maxRayDistance);
                if (distance >= maxRayDistance * 0.95f)
                {
                    currentDirection = dir;
                    return;
                }
            }

            do
            {
                currentDirection = Vector2.Normalize(Main.rand.NextVector2CircularEdge(1f, 1f));
            }
            while ((previousDirection != Vector2.Zero && Vector2.Dot(currentDirection, previousDirection) > MathF.Cos(minAngleDifference)) ||
                   (targetDirection != Vector2.Zero && Vector2.Dot(currentDirection, targetDirection) < minTargetDot));
        }
    }
}
