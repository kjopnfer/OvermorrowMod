using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using System;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    /// <summary>
    /// A grounded backoff taken right after a dash. Walks away from the target to
    /// reset spacing before re-engaging. Distance and repeat count scale with Caution.
    /// </summary>
    public class MoveBack : BaseMovementState
    {
        public override int Weight => 5;

        private const float MinDistance = 60f;
        private const float MaxDistance = 200f;
        private const int MaxHops = 3;
        private const int MaxDurationTicks = 90;
        private const int MaxWallTicks = 15;

        private int hopsAllowed;
        private int hopsDone;
        private float distance;
        private float startX;
        private int elapsed;
        private int wallTicks;

        public MoveBack(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (!OvermorrowNPC.TargetingModule.HasTarget()) return false;

            State last = OvermorrowNPC.AIStateMachine.GetPreviousSubstates().FirstOrDefault();
            if (last is not GroundDashAttack) return false;

            return Main.rand.NextFloat() < OvermorrowNPC.Personality.Caution;
        }

        public override void Enter()
        {
            float caution = OvermorrowNPC.Personality.Caution;
            distance = MathHelper.Lerp(MinDistance, MaxDistance, caution);
            hopsAllowed = 1 + (int)(caution * (MaxHops - 1));
            hopsDone = 0;
            elapsed = 0;
            wallTicks = 0;
            startX = NPC.Center.X;
            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.velocity.X = 0;
        }

        public override void Update()
        {
            if (!OvermorrowNPC.TargetingModule.HasTarget())
            {
                IsFinished = true;
                return;
            }

            Entity target = OvermorrowNPC.TargetingModule.Target;
            int away = target.Center.X < NPC.Center.X ? 1 : -1;

            // Face the target while stepping away from it.
            NPC.direction = -away;
            NPC.velocity.X = 2.5f * away;

            if (NPC.collideX)
            {
                if (NPC.collideY) NPC.velocity.Y = -3.5f;

                if (++wallTicks >= MaxWallTicks)
                {
                    IsFinished = true;
                    return;
                }
            }
            else
            {
                wallTicks = 0;
            }

            if (Math.Abs(NPC.Center.X - startX) >= distance)
            {
                hopsDone++;
                if (hopsDone >= hopsAllowed)
                {
                    IsFinished = true;
                    return;
                }

                startX = NPC.Center.X;
            }

            if (++elapsed >= MaxDurationTicks)
                IsFinished = true;
        }
    }
}
