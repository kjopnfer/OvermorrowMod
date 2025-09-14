using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Misc;
using OvermorrowMod.Core.Globals;
using System;
using System.Collections.Generic;
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

        public NPCTargetingConfig Config { get; private set; }

        public NPCTargetingModule(NPC npc, NPCTargetingConfig? config = null)
        {
            this.npc = npc;
            this.Config = config ?? new NPCTargetingConfig();
        }

        /// <summary>
        /// Manually set a target. Useful for effects that force target changes like Taunt.
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Entity target, bool withIndicator = false)
        {
            if (target.active)
            {
                Target = target;

                if (Config.DisplayAggroIndicator && withIndicator)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AggroIndicator>(), 1, 1f, Main.myPlayer, ai0: npc.whoAmI);
                }

                aggroTimer = Config.MaxAggroTime;
            }
        }

        /// <summary>
        /// Gets the effective aggro radius for a specific entity, accounting for player bonuses.
        /// </summary>
        private AggroRadius GetEffectiveTargetRadius(Entity entity)
        {
            AggroRadius baseRadius = Config.AggroRadius?.Clone() ?? AggroRadius.Circle(160f);

            if (entity is Player player)
            {
                int alertBonus = player.GetModPlayer<GlobalPlayer>().AlertBonus;

                // AlertBonus reduces all radius values proportionally
                float reductionFactor = Math.Max(0.1f, 1f - (alertBonus / baseRadius.GetMaxRadius()));

                return new AggroRadius(
                    right: Math.Max(0, baseRadius.Right * reductionFactor),
                    left: Math.Max(0, baseRadius.Left * reductionFactor),
                    up: Math.Max(0, baseRadius.Up * reductionFactor),
                    down: Math.Max(0, baseRadius.Down * reductionFactor),
                    flipWithDirection: baseRadius.FlipWithDirection
                );
            }

            return baseRadius;
        }

        /// <summary>
        /// Gets the effective alert radius for a specific entity, accounting for player bonuses.
        /// </summary>
        private AggroRadius? GetEffectiveAlertRadius(Entity entity)
        {
            if (Config.AlertRadius == null)
                return null;

            AggroRadius baseRadius = Config.AlertRadius.Clone();

            if (entity is Player player)
            {
                int alertBonus = player.GetModPlayer<GlobalPlayer>().AlertBonus;

                // AlertBonus increases alert radius
                return new AggroRadius(
                    right: baseRadius.Right + alertBonus,
                    left: baseRadius.Left + alertBonus,
                    up: baseRadius.Up + alertBonus,
                    down: baseRadius.Down + alertBonus,
                    flipWithDirection: baseRadius.FlipWithDirection
                );
            }

            return baseRadius;
        }

        /// <summary>
        /// Updated method to check if an entity is within the target radius using the AggroRadius system.
        /// </summary>
        private bool IsEntityInAggroRange(Entity entity)
        {
            AggroRadius effectiveRadius = GetEffectiveTargetRadius(entity);
            return effectiveRadius.IsPointInRange(npc.Center, entity.Center, npc.direction);
        }

        /// <summary>
        /// Updated method to check if an entity is within the attack radius using the AggroRadius system.
        /// </summary>
        private bool IsEntityInAttackRange(Entity entity)
        {
            if (Config.AttackRadius == null)
                return true; // No attack range restriction

            return Config.AttackRadius.IsPointInRange(npc.Center, entity.Center, npc.direction);
        }

        /// <summary>
        /// Updated method to check if an entity is within the alert radius using the AggroRadius system.
        /// </summary>
        private bool IsEntityInAlertRange(Entity entity)
        {
            AggroRadius? effectiveAlertRadius = GetEffectiveAlertRadius(entity);
            if (effectiveAlertRadius == null)
                return false;

            return effectiveAlertRadius.IsPointInRange(npc.Center, entity.Center, npc.direction);
        }

        public void Update()
        {
            if (!HasTarget() || aggroTimer <= 0)
            {
                // If no aggro, check for alert range
                if (Config.AlertRadius != null)
                {
                    Entity alertTarget = GetAlertTarget();
                    if (alertTarget != null)
                    {
                        bool inTargetRange = IsEntityInAggroRange(alertTarget);
                        bool inAlertRange = IsEntityInAlertRange(alertTarget);

                        if (inAlertRange && !inTargetRange)
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

                            if (!alreadyExists && !npc.IsStealthed())
                            {
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
            Target = FindNearestTarget(npc.Center, ignoreLineOfSight);
            if (Target != null)
            {
                if (Config.DisplayAggroIndicator)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AggroIndicator>(), 1, 1f, Main.myPlayer, ai0: npc.whoAmI);
                }

                aggroTimer = Config.MaxAggroTime;
            }
        }

        /// <summary>
        /// Updated MaintainAggro method using the new radius system.
        /// </summary>
        private void MaintainAggro()
        {
            if (!HasTarget())
            {
                aggroTimer = 0;
                return;
            }

            bool hasLineOfSight = HasLineOfSight(Target);
            bool inTargetRange = IsEntityInAttackRange(Target);

            // Reset aggro timer if within effective range and has line of sight
            if (inTargetRange && hasLineOfSight)
            {
                aggroTimer = Config.MaxAggroTime;
            }
            else if (aggroTimer > 0)
            {
                float lossBonus = 0;
                if (Target is Player player)
                {
                    lossBonus += player.GetModPlayer<GlobalPlayer>().AggroLossBonus;
                }

                aggroTimer -= Config.AggroLossRate + lossBonus;
            }
            else
            {
                Main.NewText("gave up!");
                aggroCooldown = Config.AggroCooldownTime;
                Target = null;
            }
        }

        public void OnHit()
        {
            if (!HasTarget())
            {
                FindTarget(true);
            }

            aggroTimer = Config.MaxAggroTime;
        }

        public void OnMissedAttack()
        {
            missedAttacks++;
            if (missedAttacks >= Config.MaxMissedAttacks)
            {
                aggroTimer = 0;
                missedAttacks = 0;
            }
        }

        /// <summary>
        /// Replace the existing FindNearestTarget method with this updated version.
        /// </summary>
        private Entity FindNearestTarget(Vector2 origin, bool ignoreLineOfSight = false)
        {
            Entity bestTarget = null;
            float nearestDistance = float.MaxValue;
            int highestAggro = int.MinValue;
            float bestWeight = float.MinValue;

            List<Entity> entities = new List<Entity>();
            entities.AddRange(Main.player);
            entities.AddRange(Main.npc);

            foreach (var entity in entities)
            {
                if (entity == null || !entity.active) continue;

                if (entity is NPC possibleNPC && (possibleNPC == npc || !possibleNPC.friendly)) continue;

                // Use the new radius-based range checking
                if (!IsEntityInAggroRange(entity)) continue;

                float distance = Vector2.Distance(origin, entity.Center);
                float weight = 1f;

                int facingDirection = npc.direction;
                bool isInFront = (facingDirection == 1 && entity.Center.X > npc.Center.X) ||
                                 (facingDirection == -1 && entity.Center.X < npc.Center.X);

                weight *= isInFront ? 2f : 0.5f;

                bool hasLineOfSight = ignoreLineOfSight || HasLineOfSight(entity);
                bool isVisible = IsVisible(entity);

                if (!hasLineOfSight || !isVisible) continue;

                if (entity is Player player && Config.PrioritizeAggro)
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

        /// <summary>
        /// Updated GetAlertTarget method using the new radius system.
        /// </summary>
        public Entity GetAlertTarget()
        {
            if (Config.AlertRadius == null)
                return null;

            Entity bestTarget = null;
            float nearestDistance = float.MaxValue;

            foreach (var entity in Main.player)
            {
                if (entity == null || !entity.active || entity.dead)
                    continue;

                bool inTargetRange = IsEntityInAggroRange(entity);
                bool inAlertRange = IsEntityInAlertRange(entity);

                // Must be in alert range but NOT in target range
                if (inAlertRange && !inTargetRange)
                {
                    if (!HasLineOfSight(entity) || !IsVisible(entity))
                        continue;

                    float distance = Vector2.Distance(npc.Center, entity.Center);
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
        /// Gets the distance buffer between alert range and aggro range for a specific entity.
        /// Returns null if alert range is not defined.
        /// </summary>
        public float? GetAlertBuffer(Entity entity = null)
        {
            if (Config.AlertRadius == null)
                return null;

            if (entity != null)
            {
                AggroRadius? effectiveAlertRadius = GetEffectiveAlertRadius(entity);
                AggroRadius effectiveTargetRadius = GetEffectiveTargetRadius(entity);

                if (effectiveAlertRadius != null)
                    return effectiveAlertRadius.GetMaxRadius() - effectiveTargetRadius.GetMaxRadius();
            }

            return Config.AlertRadius.GetMaxRadius() - Config.AggroRadius.GetMaxRadius();
        }

        /// <summary>
        /// Add this method to render debug visualization in your NPC's PostDraw or in a GlobalNPC PostDraw.
        /// </summary>
        public void DrawDebugVisualization(SpriteBatch spriteBatch)
        {
            if (Config.ShowDebugVisualization && AggroDebugDrawer.IsOnScreen(npc.Center))
            {
                AggroDebugDrawer.DrawTargetingDebug(spriteBatch, Config, npc.Center, npc.direction);
            }
        }
    }
}