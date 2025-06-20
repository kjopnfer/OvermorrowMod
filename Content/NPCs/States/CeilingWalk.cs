using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class CeilingWalk : BaseMovementState
    {
        public override int Weight => 2;
        public override bool CanExit => true;

        private float maxSpeed = 1.8f;
        private float moveSpeed = 0.2f;
        public CeilingWalk(OvermorrowNPC npc, float moveSpeed = 0.2f, float maxSpeed = 1.8f) : base(npc)
        {
            this.maxSpeed = maxSpeed;
            this.moveSpeed = moveSpeed;
        }

        public override bool CanExecute()
        {
            if (OvermorrowNPC is ClockworkSpider spider)
            {
                return NPC.noGravity;
            }

            return base.CanExecute();
        }

        public override void Enter()
        {
            NPC.noGravity = true;
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            NPC.velocity.Y -= 0.1f;

            //float maxSpeed = 1.8f;

            float dropSpeed = 2f;

            bool isTouchingCeiling = NPC.collideY;
            if (isTouchingCeiling)
            {
                // Check for tall wall beneath to boost drop speed
                Vector2 start = NPC.Top + new Vector2(NPC.direction * (NPC.width / 2 + 2), 0);
                Point tileStart = start.ToTileCoordinates();

                int solidTileCount = 0;
                const int checkDepth = 8;
                const int requiredHeight = 3;

                for (int i = 0; i < checkDepth; i++)
                {
                    int checkY = tileStart.Y + i;
                    if (WorldGen.SolidTile(tileStart.X, checkY))
                    {
                        solidTileCount++;
                        if (solidTileCount >= requiredHeight)
                        {
                            dropSpeed = 3.5f;
                            break;
                        }
                    }
                }
            }

            NPC.direction = NPC.GetDirection(OvermorrowNPC.TargetingModule.Target);

            Vector2 distance = NPC.MoveCeiling(
                OvermorrowNPC.TargetingModule.Target.Center,
                moveSpeed,
                maxSpeed,
                dropSpeed
            );
        }
    }
}