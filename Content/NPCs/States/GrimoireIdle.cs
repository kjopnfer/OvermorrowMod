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
        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;

            if (OvermorrowNPC.SpawnPoint != null)
            {
                int attempts = 0;
                float distanceRange = ModUtils.TilesToPixels(1);
                Vector2 spawnPosition = OvermorrowNPC.SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position

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
                    Vector2.Distance(lastTarget.Value, newPosition) < ModUtils.TilesToPixels(10) &&
                    attempts < 100 // Prevent infinite loops
                );

                lastTarget = newPosition;
                OvermorrowNPC.TargetingModule.MiscTargetPosition = newPosition;
            }
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            Main.NewText("exit idle");

        }

        private int castTime = 120;
        float flySpeedX = 2;
        float flySpeedY = 0;

        /// <summary>
        /// Max distance from ground the NPC should float above.
        /// </summary>
        int distanceFromGround = 180;
        public override void Update()
        {
            //npc.NPC.velocity.X /= 2f;
            distanceFromGround = ModUtils.TilesToPixels(8);

            if (OvermorrowNPC.SpawnPoint != null)
            {
                if (OvermorrowNPC.TargetingModule.HasTarget())
                {
                    IsFinished = true;
                }
                else if (OvermorrowNPC.TargetingModule.MiscTargetPosition.HasValue)
                {

                    Vector2 targetPosition = OvermorrowNPC.TargetingModule.MiscTargetPosition.Value;
                    NPC.direction = NPC.GetDirection(targetPosition);
                    float xDistance = Math.Abs(NPC.Center.X - targetPosition.X);

                    BasicFly.HandleHorizontalMovement(OvermorrowNPC, ref flySpeedX);
                    BasicFly.HandleVerticalMovementToTarget(OvermorrowNPC, ref flySpeedY);
                    BasicFly.HandleGroundProximity(OvermorrowNPC, ref flySpeedY, distanceFromGround);
                    BasicFly.HandleObstacleAvoidance(OvermorrowNPC, ref flySpeedX, ref flySpeedY);

                    //Dust.NewDust(targetPosition, 1, 1, DustID.BlueTorch);

                    //Main.NewText(flySpeedX + " " + flySpeedY);
                    if (xDistance <= ModUtils.TilesToPixels(1))
                    {
                        NPC.velocity.X /= 2f;

                        OvermorrowNPC.AICounter++;

                        if (OvermorrowNPC.AICounter >= 180)
                        {
                            Main.NewText("Finished wandering.");

                            OvermorrowNPC.IdleCounter = 5;
                            NPC.velocity.X = 0;
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