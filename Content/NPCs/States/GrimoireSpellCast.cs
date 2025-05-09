using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireSpellCast : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private int tileAttackDistance = 24;
        private int attackDelay = 60;
        public override bool CanExecute(OvermorrowNPC npc)
        {
            if (npc is not LivingGrimoire)
            {
                // return false;
            }

            if (npc.AIStateMachine.GetPreviousSubstates().FirstOrDefault() is not BasicFly)
            {
                Main.NewText("have not flown yet, preventin early attack", Color.Red);
                return false;
            }

            if (!npc.TargetingModule.HasTarget())
                return false;

            if (attackDelay-- > 0)
                return false;

            NPC baseNPC = npc.NPC;
            Entity target = npc.TargetingModule.Target;

            float xDistance = Math.Abs(baseNPC.Center.X - target.Center.X);
            float yDistance = Math.Abs(baseNPC.Center.Y - target.Center.Y);

            bool isWithinXRange = xDistance <= tileAttackDistance * 18;
            bool isWithinYRange = yDistance < 100;
            bool hasLineOfSight = Collision.CanHitLine(target.Center, 1, 1, baseNPC.Center, 1, 1);

            return isWithinXRange && isWithinYRange && hasLineOfSight;
        }

        public override void Enter(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            IsFinished = false;
            Main.NewText("entering cast spell");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            attackDelay = 60;
            Main.NewText("exiting spell", Color.Red);
        }

        private int castTime = 120;
        float flySpeedX = 2;
        float flySpeedY = 0;
        int distanceFromGround = 180;
        public override void Update(OvermorrowNPC npc)
        {
            npc.AICounter++;
            npc.NPC.velocity.X /= 2f;

            HandleVerticalMovement(npc);
            HandleGroundProximity(npc);

            CastSpell(npc);

            LivingGrimoire bookNPC = npc as LivingGrimoire;
            //Main.NewText("spell : " + npc.AICounter);
            if (npc.AICounter >= bookNPC.CastTime)
            {
                IsFinished = true;
            }
        }

        int chairProjectilesNeeded = 0;
        private void CastSpell(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            switch (baseNPC.ModNPC)
            {
                case PlantBook:
                    if (npc.AICounter == 30)
                    {

                        float angle = MathHelper.ToRadians(75);
                        Vector2 projectileVelocity = new Vector2(100 * baseNPC.direction, 0).RotatedByRandom(angle) * 50;
                        Projectile.NewProjectile(baseNPC.GetSource_FromAI(), npc.TargetingModule.Target.Center, Vector2.Zero, ModContent.ProjectileType<PlantAura>(), 1, 1f, Main.myPlayer);
                    }
                    break;
                case ChairBook:
                    if (chairProjectilesNeeded == 0) return;

                    if (npc.AICounter % 10 == 0 && npc.AICounter < 40)
                    {
                        float angle = MathHelper.ToRadians(75);
                        float randomDirection = Main.rand.NextBool() ? 1 : -1;

                        // Make sure the x-offset is never zero
                        Vector2 projectileVelocity = new Vector2(Main.rand.Next(-3, 2) + 1, Main.rand.Next(8, 10)).RotatedByRandom(angle);
                        Projectile.NewProjectile(baseNPC.GetSource_FromAI(), baseNPC.Center, projectileVelocity, ModContent.ProjectileType<ChairBolt>(), 1, 1f, Main.myPlayer, 0f, baseNPC.whoAmI);

                        chairProjectilesNeeded--;
                    }
                    break;
                default:
                    return;
            }
        }

        private bool AttackCondition(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            switch (baseNPC.ModNPC)
            {
                case ChairBook:
                    var chairSummons = Main.npc
                    .Where(npc => npc.active
                        && npc.ModNPC is ChairSummon chairSummon
                        && chairSummon.ParentID == baseNPC.whoAmI)
                    .Select(npc => npc.ModNPC as ChairSummon)
                    .ToList();

                    if (chairSummons.Count == 3) return false;

                    chairProjectilesNeeded = 3 - chairSummons.Count;
                    break;
            }

            float xDistance = Math.Abs(baseNPC.Center.X - npc.TargetingModule.Target.Center.X);

            bool xDistanceCheck = xDistance <= tileAttackDistance * 18;
            bool yDistanceCheck = Math.Abs(baseNPC.Center.Y - npc.TargetingModule.Target.Center.Y) < 100;

            return xDistanceCheck && yDistanceCheck && Collision.CanHitLine(npc.TargetingModule.Target.Center, 1, 1, baseNPC.Center, 1, 1);
        }

        private void HandleGroundProximity(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            float groundBuffer = distanceFromGround;

            if (RayTracing.CastTileCollisionLength(baseNPC.Center, Vector2.UnitY, groundBuffer) < groundBuffer)
            {
                baseNPC.velocity.Y -= 0.1f;
                flySpeedY = Math.Max(flySpeedY - 0.1f, -2f);
            }
        }

        private void HandleVerticalMovement(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;
            var target = npc.TargetingModule.Target;

            float verticalBuffer = 16 * 5;
            float targetSpeed = 2f;

            if (baseNPC.Center.Y <= target.Center.Y - verticalBuffer)
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