using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Misc;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.NPCs
{
    public class NPCTargetingModule
    {
        private NPC npc;
        public Entity Target { get; private set; }

        /// <summary>
        /// Used for targeting areas that may or may not be the target.
        /// </summary>
        public Vector2? MiscTargetPosition = null;

        private float aggroTimer;
        private float aggroCooldown;
        private int missedAttacks;

        private NPCTargetingConfig config;

        public NPCTargetingModule(NPC npc, NPCTargetingConfig? config = null)
        {
            this.npc = npc;
            this.config = config ?? new NPCTargetingConfig();
        }

        /// <summary>
        /// Manually set a target. Useful for effects that force target changes like Taunt.
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Entity target)
        {
            if (target.active)
            {
                Target = target;

                if (config.DisplayAggroIndicator)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AggroIndicator>(), 1, 1f, Main.myPlayer, ai0: npc.whoAmI);
                
                aggroTimer = config.MaxAggroTime;
            }
        }

        public void Update()
        {
            if (!HasTarget() || aggroTimer <= 0)
            {
                // If no aggro, check for alert range
                if (config.AlertRange.HasValue)
                {
                    float alertRange = config.AlertRange.Value;

                    Entity alertTarget = GetAlertTarget();
                    if (alertTarget != null)
                    {
                        float distance = Vector2.Distance(npc.Center, alertTarget.Center);
                        if (distance > config.MaxTargetRange && distance <= alertRange)
                        {
                            bool alreadyExists = false;

                            // Check all projectiles for an existing AlertIndicator owned by this NPC
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile proj = Main.projectile[i];
                                if (proj.active && proj.type == ModContent.ProjectileType<AlertIndicator>() && proj.ai[0] == npc.whoAmI)
                                {
                                    alreadyExists = true;
                                    break;
                                }
                            }

                            if (!alreadyExists)
                            {
                                Main.NewText("Target is within alert range.");
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AlertIndicator>(), 1, 1f, Main.myPlayer, ai0: npc.whoAmI);
                            }
                        }
                    }
                }

                FindTarget();
            }
            else
            {
                MaintainAggro();
            }
        }

        /// <summary>
        /// Determines whether the NPC has an active Entity target
        /// </summary>
        /// <returns></returns>
        public bool HasTarget()
        {
            return Target != null && Target.active;
        }

        private void FindTarget(bool ignoreLineOfSight = false)
        {
            Target = FindNearestTarget(npc.Center, config.MaxTargetRange, config.PrioritizeAggro, ignoreLineOfSight);
            if (Target != null)
            {
                Main.NewText("found target.");

                if (config.DisplayAggroIndicator)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AggroIndicator>(), 1, 1f, Main.myPlayer, ai0: npc.whoAmI);
                
                aggroTimer = config.MaxAggroTime;
            }
        }

        private void MaintainAggro()
        {
            if (!HasTarget())
            {
                aggroTimer = 0;
                return;
            }

            float distanceToTarget = Vector2.Distance(npc.Center, Target.Center);
            bool hasLineOfSight = HasLineOfSight(Target); // Ensure this method properly checks LoS

            // Reset aggro timer if within range and has line of sight
            if (distanceToTarget <= config.MaxTargetRange && hasLineOfSight)
            {
                aggroTimer = config.MaxAggroTime;
            }
            else if (aggroTimer > 0)
            {
                aggroTimer -= config.AggroLossRate;
            }
            else
            {
                Main.NewText("gave up!");

                aggroCooldown = config.AggroCooldownTime;
                Target = null;
            }
        }

        public void OnHit()
        {
            if (!HasTarget())
            {
                FindTarget(true);
            }

            aggroTimer = config.MaxAggroTime;
        }

        public void OnMissedAttack()
        {
            missedAttacks++;
            if (missedAttacks >= config.MaxMissedAttacks)
            {
                aggroTimer = 0;
                missedAttacks = 0;
            }
        }

        private Entity FindNearestTarget(Vector2 origin, float maxDistance, bool prioritizeAggro, bool ignoreLineOfSight = false)
        {
            Entity bestTarget = null;
            float nearestDistance = maxDistance;
            int highestAggro = int.MinValue;
            float bestWeight = float.MinValue;

            List<Entity> entities = new List<Entity>();
            entities.AddRange(Main.player);
            entities.AddRange(Main.npc);

            foreach (var entity in entities)
            {
                if (entity == null || !entity.active) continue;

                if (entity is NPC possibleNPC && (possibleNPC == npc || !possibleNPC.friendly)) continue;

                float distance = Vector2.Distance(origin, entity.Center);
                if (distance > maxDistance) continue;

                float weight = 1f;

                int facingDirection = npc.direction;
                bool isInFront = (facingDirection == 1 && entity.Center.X > npc.Center.X) ||
                                 (facingDirection == -1 && entity.Center.X < npc.Center.X);

                weight *= isInFront ? 2f : 0.5f;

                bool hasLineOfSight = ignoreLineOfSight || HasLineOfSight(entity);
                bool isVisible = IsVisible(entity);

                if (!hasLineOfSight || !isVisible) continue;

                if (entity is Player player && prioritizeAggro)
                {
                    int aggro = player.aggro;
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
                    if (distance < nearestDistance || (distance == nearestDistance && weight > bestWeight))
                    {
                        nearestDistance = distance;
                        bestWeight = weight;
                        bestTarget = entity;
                    }
                }
            }

            return bestTarget;
        }

        public Entity GetAlertTarget()
        {
            if (!config.AlertRange.HasValue)
                return null;

            float alertRange = config.AlertRange.Value;
            Entity bestTarget = null;
            float nearestDistance = alertRange;

            foreach (var entity in Main.player)
            {
                if (entity == null || !entity.active || entity.dead)
                    continue;

                float distance = Vector2.Distance(npc.Center, entity.Center);
                if (distance > config.MaxTargetRange && distance <= alertRange)
                {
                    if (!HasLineOfSight(entity) || !IsVisible(entity))
                        continue;

                    if (distance < nearestDistance)
                    {
                        bestTarget = entity;
                        nearestDistance = distance;
                    }
                }
            }

            return bestTarget;
        }


        private bool HasLineOfSight(Entity entity)
        {
            // Implement logic to determine if the entity is visible (raycasting, tile checks, etc.)
            bool canSee = Collision.CanHitLine(npc.Center, 1, 1, entity.Center, 1, 1);
            return canSee;
        }

        private bool IsVisible(Entity entity)
        {
            // Implement visibility logic based on light, direction, etc.
            return true;
        }

        /// <summary>
        /// Checks whether there is a valid entity within alert range but outside aggro range.
        /// </summary>
        /// <returns>True if an alert target exists, false otherwise.</returns>
        public bool IsInAlertState()
        {
            return GetAlertTarget() != null;
        }

        /// <summary>
        /// Gets the distance buffer between alert range and aggro range.
        /// Returns null if alert range is not defined.
        /// </summary>
        public float? GetAlertBuffer()
        {
            if (!config.AlertRange.HasValue)
                return null;

            return config.AlertRange.Value - config.MaxTargetRange;
        }
    }
}
