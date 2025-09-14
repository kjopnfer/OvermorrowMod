using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.NPCs
{
    public class ClockworkSpiderRoll : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        public ClockworkSpiderRoll(OvermorrowNPC npc) : base(npc) { }

        int rollCooldown = 0;
        public override bool CanExecute()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                return false;


            if (rollCooldown-- > 0)
                return false;

            return NPC.collideY && !NPC.noGravity;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
            NPC.noGravity = false;

            NPC.velocity.Y = -6f;
            //NPC.velocity.X *= 0.25f;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            rollCooldown = ModUtils.SecondsToTicks(Main.rand.Next(3, 5));

            collideSound = true;
        }

        bool collideSound = true;
        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            float acceleration = 0.15f; // how quickly it speeds up
            float maxSpeed = 12f;


            if (OvermorrowNPC.AICounter >= ModUtils.SecondsToTicks(2.5f) || NPC.velocity.X == 0)
            {
                IsFinished = true;
                NPC.velocity.X = 0;
                NPC.noGravity = false;
            }

            //if (OvermorrowNPC.AICounter >= ModUtils.SecondsToTicks(0.5f) && OvermorrowNPC.AICounter <= ModUtils.SecondsToTicks(1.5f))
            if (OvermorrowNPC.AICounter >= ModUtils.SecondsToTicks(0.25f))
            {
                // Clamp to max speed
                if (MathF.Abs(NPC.velocity.X) > maxSpeed)
                    NPC.velocity.X = NPC.direction * maxSpeed;

                float rotationSpeedFactor = 0.05f; // Adjust as needed
                NPC.rotation += NPC.direction * MathF.Abs(NPC.velocity.X) * rotationSpeedFactor;
            }

            if (NPC.collideY)
            {
                if (collideSound)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit42 with
                    {
                        Pitch = 0.6f,
                        PitchVariance = 0.4f
                    });

                    collideSound = false;
                }

                NPC.velocity.X += NPC.direction * acceleration;
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }
        }
    }
}