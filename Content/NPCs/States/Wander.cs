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

        public override void Enter(OvermorrowNPC npc)
        {
            IsFinished = false;

            Main.NewText("entering the wander state", Color.Red);

            npc.NPC.velocity.X = 0;
            npc.NPC.RemoveStealth();

            if (npc.SpawnPoint != null)
            {
                Vector2 spawnPosition = npc.SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position

                Vector2 newPosition;
                do
                {
                    float offsetX = Main.rand.NextFloat(-10f, 10f) * 16; // Random X within 10 tiles
                    float offsetY = Main.rand.NextFloat(-10f, 10f) * 16; // Random Y within 10 tiles
                    newPosition = spawnPosition + new Vector2(offsetX, offsetY);

                    // Find the nearest ground position for the generated newPosition
                    newPosition = TileUtils.FindNearestGround(newPosition);

                } while (Vector2.Distance(npc.NPC.Center, newPosition) < 4 * 16); // Ensure at least 4 tiles away

                // There is no target so pick a random position to walk towards.
                npc.TargetingModule.MiscTargetPosition = newPosition;
            }
            else
            {
                if (npc.SpawnerID.HasValue)
                {

                }
                else
                {
                }
            }
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            npc.TargetingModule.MiscTargetPosition = null;

            Main.NewText("exited wander");
        }

        public override void Update(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;

            // TODO: Change this to an NPC properties module.
            float maxSpeed = 1.8f;
            if (npc.SpawnPoint != null)
            {
                if (npc.TargetingModule.MiscTargetPosition.HasValue)
                {
                    Vector2 targetPosition = npc.TargetingModule.MiscTargetPosition.Value;
                    baseNPC.direction = baseNPC.GetDirection(targetPosition);
                    Vector2 distance = baseNPC.Move(targetPosition, 0.2f, maxSpeed, 8f);

                    if (distance.X <= 16)
                    {
                        if (!IsFinished) // Prevent setting multiple times
                        {
                            Main.NewText("Finished wandering.");

                            npc.IdleCounter = 120;
                            IsFinished = true;
                            npc.NPC.velocity.X = 0;
                        }
                    }
                }
            }
        }
    }
}