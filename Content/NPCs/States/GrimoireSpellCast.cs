using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireSpellCast : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;
        public override bool CanExecute(OvermorrowNPC npc)
        {
            return base.CanExecute(npc);
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

            Main.NewText("spell : " + npc.AICounter);
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