using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class Wander : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => true;
        public Wander(OvermorrowNPC npc) : base(npc) { }

        public override void Enter()
        {
            IsFinished = false;

            Main.NewText("entering the wander state", Color.Red);

            NPC.velocity.X = 0;
            NPC.RemoveStealth();

            if (OvermorrowNPC.SpawnPoint != null)
            {
                Vector2 spawnPosition = OvermorrowNPC.SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position

                Vector2 newPosition;
                do
                {
                    float offsetX = Main.rand.NextFloat(-10f, 10f) * 16; // Random X within 10 tiles
                    float offsetY = Main.rand.NextFloat(-10f, 10f) * 16; // Random Y within 10 tiles
                    newPosition = spawnPosition + new Vector2(offsetX, offsetY);

                    // Find the nearest ground position for the generated newPosition
                    newPosition = TileUtils.FindNearestGround(newPosition);

                } while (Vector2.Distance(NPC.Center, newPosition) < 4 * 16); // Ensure at least 4 tiles away

                // There is no target so pick a random position to walk towards.
                OvermorrowNPC.TargetingModule.MiscTargetPosition = newPosition;
            }
            else
            {
                if (OvermorrowNPC.SpawnerID.HasValue)
                {

                }
                else
                {
                }
            }
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            OvermorrowNPC.TargetingModule.MiscTargetPosition = null;

            Main.NewText("exited wander");
        }

        public override void Update()
        {
            // TODO: Change this to an NPC properties module.
            float maxSpeed = 1.8f;
            if (OvermorrowNPC.SpawnPoint != null)
            {
                if (OvermorrowNPC.TargetingModule.MiscTargetPosition.HasValue)
                {
                    Vector2 targetPosition = OvermorrowNPC.TargetingModule.MiscTargetPosition.Value;
                    NPC.direction = NPC.GetDirection(targetPosition);
                    Vector2 distance = NPC.Move(targetPosition, 0.2f, maxSpeed, 8f);

                    if (distance.X <= 16)
                    {
                        if (!IsFinished) // Prevent setting multiple times
                        {
                            Main.NewText("Finished wandering.");

                            OvermorrowNPC.IdleCounter = 120;
                            IsFinished = true;
                            NPC.velocity.X = 0;
                        }
                    }
                }
            }
        }
    }
}