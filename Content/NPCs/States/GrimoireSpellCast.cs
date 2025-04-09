using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireSpellCast : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private int tileAttackDistance = 24;
        private int attackDelay = 0;
        public override bool CanExecute(OvermorrowNPC npc)
        {
            if (npc is not LivingGrimoire)
            {
                // return false;
                Main.NewText("I AM NOT");
            }

            if (npc.AIStateMachine.GetPreviousSubstates().FirstOrDefault() is not BasicFly)
            {
                Main.NewText("test", Color.Red);
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
            Main.NewText("cast spell " + IsFinished);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
            attackDelay = 60;
            Main.NewText("exit spell");
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

            //Main.NewText("spell : " + npc.AICounter);
            if (npc.AICounter >= castTime)
            {
                IsFinished = true;
            }
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