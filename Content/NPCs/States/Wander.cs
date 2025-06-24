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

        /// <summary>
        /// Minimum range of tiles that the NPC must choose to wander from its spawn point.
        /// </summary>
        public int MinWanderRange { get; private set; }

        /// <summary>
        /// Maximum range of tiles that the NPC can wander from its spawn point.
        /// </summary>
        public int MaxWanderRange { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="minRange">The minimum range in tiles that the NPC must choose to wander from.</param>
        /// <param name="maxRange">The maxmimum range in tiles that the NPC can choose to wander to.</param>
        public Wander(OvermorrowNPC npc, int minRange = 4, int maxRange = 10) : base(npc)
        {
            MinWanderRange = minRange;
            MaxWanderRange = maxRange;
        }

        public override void Enter()
        {
            IsFinished = false;

            NPC.velocity.X = 0;
            //NPC.RemoveStealth();

            if (OvermorrowNPC.SpawnPoint != null)
            {
                Vector2 spawnPosition = OvermorrowNPC.SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position

                Vector2 newPosition;
                do
                {
                    //float offsetX = Main.rand.NextFloat(-10f, 10f) * 16; // Random X within 10 tiles
                    //float offsetY = Main.rand.NextFloat(-10f, 10f) * 16; // Random Y within 10 tiles
                    float offsetX = ModUtils.TilesToPixels(Main.rand.Next(-MaxWanderRange, MaxWanderRange));
                    float offsetY = ModUtils.TilesToPixels(Main.rand.Next(-MaxWanderRange, MaxWanderRange));
                    newPosition = spawnPosition + new Vector2(offsetX, offsetY);

                    // Find the nearest ground position for the generated newPosition
                    newPosition = TileUtils.FindNearestGround(newPosition);

                } while (Vector2.Distance(NPC.Center, newPosition) < ModUtils.TilesToPixels(MinWanderRange));

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
        }

        public override void Update()
        {
            NPC.noGravity = false;

            // TODO: Change this to an NPC properties module.
            float maxSpeed = 1.8f;
            if (OvermorrowNPC.SpawnPoint != null)
            {
                if (OvermorrowNPC.TargetingModule.MiscTargetPosition.HasValue)
                {
                    Vector2 targetPosition = OvermorrowNPC.TargetingModule.MiscTargetPosition.Value;
                    NPC.direction = NPC.GetDirection(targetPosition);
                    Vector2 distance = NPC.Move(targetPosition, 0.2f, maxSpeed, 8f);

                    if (distance.X <= ModUtils.TilesToPixels(1))
                    {
                        if (!IsFinished) // Prevent setting multiple times
                        {
                            OvermorrowNPC.IdleCounter = Main.rand.Next(12, 15) * 10;
                            IsFinished = true;
                            NPC.velocity.X = 0;
                        }
                    }
                }
            }
        }
    }
}