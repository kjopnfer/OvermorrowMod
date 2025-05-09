using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireIdle : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;
        public override bool CanExecute(OvermorrowNPC npc)
        {
            //Main.NewText(npc.AIStateMachine.get)
            return true;

            if (npc.AIStateMachine.GetPreviousSubstates().FirstOrDefault() is not BasicFly)
            {
                //Main.NewText("grimoire idle cannot run", Color.Red);
                return false;
            }

            return true;
        }

        public override void Enter(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            IsFinished = false;
            Main.NewText("do idling " + IsFinished);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            Main.NewText("exit idle");

        }

        private int castTime = 120;
        float flySpeedX = 2;
        float flySpeedY = 0;
        int distanceFromGround = 180;
        public override void Update(OvermorrowNPC npc)
        {
            npc.AICounter++;
            npc.NPC.velocity.X /= 2f;

            // TODO: Change this so tha the NPC picks a random spot and then hovers around it
            //if (npc.TargetingModule.HasTarget())
            {

                HandleVerticalMovement(npc);
                HandleGroundProximity(npc);

                IsFinished = true;
            }
        }

        private void HandleGroundProximity(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            float groundBuffer = distanceFromGround;
            float heightLimit = 16 * 12;


            if (RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitY, groundBuffer) < groundBuffer)
            {
                baseNPC.velocity.Y -= 0.2f;
                flySpeedY = Math.Max(flySpeedY - 0.1f, -2f);
            }
            else if (RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitY, groundBuffer) > heightLimit)
            {
                baseNPC.velocity.Y += 0.1f;
                flySpeedY = Math.Max(flySpeedY - 0.1f, -2f);
            }
        }

        private void HandleVerticalMovement(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            //var target = npc.TargetingModule.Target;
            float verticalBuffer = 16 * 5;
            float targetSpeed = 2f;

            //if (baseNPC.Center.Y <= target.Center.Y - verticalBuffer)
            {
                baseNPC.velocity.Y = Math.Min(baseNPC.velocity.Y + 0.1f, targetSpeed);

                // Add randomness to avoid straight-line movement
                if (Main.rand.NextBool(3))
                    baseNPC.velocity.Y += 0.05f;

                flySpeedY = Math.Min(flySpeedY + 0.1f, targetSpeed);
            }
        }

    }
}