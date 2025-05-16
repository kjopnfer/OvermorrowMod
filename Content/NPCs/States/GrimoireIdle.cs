using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireIdle : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;
        public GrimoireIdle(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute(OvermorrowNPC npc)
        {
            //Main.NewText(npc.AIStateMachine.get)
            return true;
        }

        Vector2? lastTarget = null;
        public override void Enter(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            IsFinished = false;

            if (npc.SpawnPoint != null)
            {
                int attempts = 0;
                float distanceRange = 16f;
                Vector2 spawnPosition = npc.SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position

                Vector2 newPosition;
                do
                {
                    float offsetX = Main.rand.NextFloat(-distanceRange, distanceRange) * 16; // Random X within 10 tiles
                    float offsetY = Main.rand.NextFloat(-distanceRange, distanceRange) * 16; // Random Y within 10 tiles
                    newPosition = spawnPosition + new Vector2(offsetX, offsetY);

                    newPosition = TileUtils.FindNearestGround(newPosition);
                    if (!lastTarget.HasValue)
                        break;

                    attempts++;
                } while (
                    lastTarget.HasValue &&
                    Vector2.Distance(lastTarget.Value, newPosition) < 10 * 16 &&
                    attempts < 100 // Prevent infinite loops
                );

                lastTarget = newPosition;
                npc.TargetingModule.MiscTargetPosition = newPosition;
            }
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            Main.NewText("exit idle");

        }

        private int castTime = 120;
        float flySpeedX = 2;
        float flySpeedY = 0;

        /// <summary>
        /// Max distance from ground the NPC should float above.
        /// </summary>
        int distanceFromGround = 180;
        public override void Update(OvermorrowNPC npc)
        {
            //npc.NPC.velocity.X /= 2f;
            distanceFromGround = 16 * 8;
            NPC baseNPC = npc.NPC;

            if (npc.SpawnPoint != null)
            {
                if (npc.TargetingModule.HasTarget())
                {
                    IsFinished = true;
                }
                else if (npc.TargetingModule.MiscTargetPosition.HasValue)
                {

                    Vector2 targetPosition = npc.TargetingModule.MiscTargetPosition.Value;
                    baseNPC.direction = baseNPC.GetDirection(targetPosition);
                    float xDistance = Math.Abs(baseNPC.Center.X - targetPosition.X);

                    BasicFly.HandleHorizontalMovement(OvermorrowNPC, ref flySpeedX);
                    BasicFly.HandleVerticalMovementToTarget(OvermorrowNPC, ref flySpeedY);
                    BasicFly.HandleGroundProximity(OvermorrowNPC, ref flySpeedY, distanceFromGround);
                    BasicFly.HandleObstacleAvoidance(OvermorrowNPC, ref flySpeedX, ref flySpeedY);

                    Dust.NewDust(targetPosition, 1, 1, DustID.BlueTorch);

                    //Main.NewText(flySpeedX + " " + flySpeedY);
                    if (xDistance <= 16)
                    {
                        baseNPC.velocity.X /= 2f;

                        npc.AICounter++;

                        if (npc.AICounter >= 180)
                        {
                            Main.NewText("Finished wandering.");

                            npc.IdleCounter = 5;
                            npc.NPC.velocity.X = 0;
                        }
                    }
                }
            }
            else
            {
                IsFinished = true;
            }
        }
    }
}