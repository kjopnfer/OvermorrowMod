using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract partial class OvermorrowNPC : ModNPC
    {
        public NPCTargetingModule TargetingModule { get; private set; }

        public ref Player Player => ref Main.player[NPC.target];

        /// <summary>
        /// Defines a position in the world that the NPC can target.
        /// Thie position may be set to <see cref="Target"/>'s position,
        /// but it may also be used for idling behavior if no <see cref="Target"/> is defined.
        /// </summary>
        public Vector2? TargetPosition = null;

        /// <summary>
        /// Saves the ID of the Spawner if the NPC was created by one.
        /// </summary>
        public int? SpawnerID { get; set; } = null;

        public ref float AICounter => ref NPC.ai[0];

        public AIStateMachine AIStateMachine = null;

        /// <summary>
        /// Gets the associated NPCSpawnPoint if the NPC was created by a spawner.
        /// Returns null if no valid SpawnerID exists.
        /// </summary>
        public NPCSpawnPoint SpawnPoint => SpawnerID.HasValue && TileEntity.ByID.TryGetValue(SpawnerID.Value, out TileEntity entity)
            ? entity as NPCSpawnPoint
            : null;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public virtual NPCTargetingConfig TargetingConfig() => new NPCTargetingConfig();
        public sealed override void SetDefaults()
        {
            NPC.GetGlobalNPC<BarrierNPC>().MaxBarrierPoints = (int)(NPC.lifeMax * 0.25f);
            TargetingModule = new NPCTargetingModule(NPC, TargetingConfig());
            AIStateMachine = new AIStateMachine(new List<BaseIdleState> { 
                new Wander()
            }, new List<BaseMovementState> { 
                new MeleeWalk()
            }, new List<BaseAttackState> {
                new GroundDashAttack()
            });

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        protected virtual void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor) { }

        /// <summary>
        /// Is called before <see cref="DrawOvermorrowNPC(SpriteBatch, Vector2, Color)"/>, which will always draw behind.
        /// The SpriteBatch calls here will not be captured by RenderTargets such as the NPCBarrierRenderer.
        /// </summary>
        public virtual void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }

        /// <summary>
        /// The replacement for PreDraw. Everything drawn in here can be captured by a RenderTarget.
        /// </summary>
        public virtual bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;
        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                DrawNPCBestiary(spriteBatch, drawColor);
                return false;
            }

            DrawBehindOvermorrowNPC(spriteBatch, screenPos, drawColor);

            return DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }


        public sealed override bool PreAI()
        {
            // Prevent offscreen projectiles from killing the NPC.
            NPC.dontTakeDamage = !IsOnScreen();

            TargetingModule.Update();

            return base.PreAI();
        }

        public sealed override void OnKill()
        {
            if (SpawnerID.HasValue && TileEntity.ByID.TryGetValue(SpawnerID.Value, out TileEntity entity) && entity is NPCSpawnPoint spawner)
            {
                spawner.HasBeenKilled = true;
            }
        }

        /// <summary>
        /// Finds the nearest valid target (either an NPC or a Player) within a specified range.
        /// Prioritizes targets based on distance, aggro (if applicable), and whether they are in front of the NPC.
        /// </summary>
        /// <param name="origin">The starting position from which to search for targets.</param>
        /// <param name="maxDistance">The maximum search radius for valid targets.</param>
        /// <param name="prioritizeAggro">If true, prioritizes players with higher aggro over distance and direction.</param>
        /// <returns>The best target entity based on the evaluation criteria, or null if no valid target is found.</returns>
        protected Entity FindNearestTarget(Vector2 origin, float maxDistance, bool prioritizeAggro)
        {
            Entity bestTarget = null;
            float nearestDistance = maxDistance;
            int highestAggro = int.MinValue;
            float bestWeight = float.MinValue;

            // Combine both NPCs and players into a single list
            List<Entity> entities = new List<Entity>();
            entities.AddRange(Main.player);
            entities.AddRange(Main.npc);

            foreach (var entity in entities)
            {
                if (entity == null || !entity.active) continue;

                float distance = Vector2.Distance(origin, entity.Center);

                if (distance > maxDistance) continue; // Ignore entities outside range

                float weight = 1f; // Base weight

                // Direction NPC is facing (-1 for left, 1 for right)
                int facingDirection = NPC.direction;

                // Check if entity is in front of the NPC
                bool isInFront = (facingDirection == 1 && entity.Center.X > NPC.Center.X) ||
                                 (facingDirection == -1 && entity.Center.X < NPC.Center.X);

                // Increase weight if the entity is in front
                if (isInFront)
                    weight *= 2f; // Increase priority for targets in front
                else
                    weight *= 0.5f; // Decrease priority for targets behind

                if (entity is Player player && prioritizeAggro)
                {
                    int aggro = player.aggro; // Assuming player has an 'aggro' property

                    // Prioritize based on aggro first, then distance, then weight
                    if (aggro > highestAggro ||
                        (aggro == highestAggro && distance < nearestDistance) ||
                        (aggro == highestAggro && distance == nearestDistance && weight > bestWeight))
                    {
                        highestAggro = aggro;
                        nearestDistance = distance;
                        bestWeight = weight;
                        bestTarget = entity;
                    }
                }
                else
                {
                    if (entity is NPC npc && !npc.friendly) continue;

                    // Prioritize based on distance first, then weight
                    if (distance < nearestDistance ||
                        (distance == nearestDistance && weight > bestWeight))
                    {
                        nearestDistance = distance;
                        bestWeight = weight;
                        bestTarget = entity;
                    }
                }
            }

            return bestTarget;
        }
    }
}