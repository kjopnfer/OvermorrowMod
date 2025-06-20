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
        private int maxBounces = 6;
        private float pinballSpeed = 8f;
        private bool startedPinball = false;
        private int pinballCooldown = ModUtils.SecondsToTicks(Main.rand.Next(5, 7));

        public ClockworkSpiderPinball(OvermorrowNPC npc, float pinballSpeed = 15) : base(npc) {
            this.pinballSpeed = pinballSpeed;
        }

        public override bool CanExecute()
        {
            if (pinballCooldown-- > 0)
                return false;

            return OvermorrowNPC is ClockworkSpider && NPC.collideY;
        }

        private int verticalDirection = -1; // -1 = up, 1 = down
        private int horizontalDirection = -1; // -1 = left, 1 = right
        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
            numBounces = 0;
            startedPinball = false;

            NPC.velocity.X = 0;

            horizontalDirection = NPC.direction; // 1 = right, -1 = left
            verticalDirection = -1; // Start going upward

            // Start with a leap (either up or down based on gravity)
            if (NPC.noGravity)
                NPC.velocity.Y = 6f;
            else
            {
                NPC.velocity.Y = -6f;
            }

            //NPC.noGravity = true;
        }

        public override void Exit()
        {
            //pinballCooldown = ModUtils.SecondsToTicks(5);
            pinballCooldown = ModUtils.SecondsToTicks(Main.rand.Next(5, 7));
            //pinballCooldown = ModUtils.SecondsToTicks(2);
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
                //if (OvermorrowNPC.AICounter > 16) NPC.velocity.Y = 0;

                if (OvermorrowNPC.AICounter == 16)
                {
                    NPC.velocity.X = 1.5f * NPC.direction;
                    NPC.velocity.Y = -6;
                }


                if (OvermorrowNPC.AICounter > delay && NPC.collideY)
                {
                    TryGetPinballDirection(Vector2.Zero);
                    NPC.noGravity = true;
                    NPC.velocity = currentDirection * pinballSpeed;
                    startedPinball = true;
                }
                return;
            }

            if (OvermorrowNPC.AICounter >= delay && (NPC.collideX || NPC.collideY))
            {
                numBounces++;

                if (numBounces >= maxBounces)
                {
                    // go straight down and re-enable gravity on floor impact
                    currentDirection = Vector2.UnitY;
                    NPC.velocity = currentDirection * pinballSpeed;

                    if (NPC.collideY)
                    {
                        NPC.noGravity = false;
                        IsFinished = true;
                    }
                    return;
                }

                TryGetPinballDirection(currentDirection);
                NPC.velocity = currentDirection * pinballSpeed;
            }
        }

        private void TryGetPinballDirection(Vector2 previousDirection)
        {
            const float angleDegrees = 80f;
            float angleRadians = MathF.PI * angleDegrees / 180f;

            float x = MathF.Cos(angleRadians) * horizontalDirection;
            float y = MathF.Sin(angleRadians) * verticalDirection;

            currentDirection = new Vector2(x, y).SafeNormalize(Vector2.UnitY);

            verticalDirection *= -1; // Alternate up/down each bounce
        }
    }
}
