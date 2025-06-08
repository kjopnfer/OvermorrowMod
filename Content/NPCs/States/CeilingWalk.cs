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
        public CeilingWalk(OvermorrowNPC npc) : base(npc) { }

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

            float maxSpeed = 1.8f;

            float dropSpeed = 2f;

            bool isTouchingCeiling = NPC.collideY;
            if (isTouchingCeiling)
            {
                // Inline check for tall wall in front
                Vector2 start = NPC.Top + new Vector2(NPC.direction * (NPC.width / 2 + 2), 0);
                Point tileStart = start.ToTileCoordinates();

                int solidTileCount = 0;
                const int checkDepth = 8;      // how far down to scan
                const int requiredHeight = 3;  // how many solid tiles make it a "wall"

                for (int i = 0; i < checkDepth; i++) // check 8 tiles downward
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
                solidTileCount = 0;
            }

            NPC.direction = NPC.GetDirection(OvermorrowNPC.TargetingModule.Target);
            Vector2 distance = NPC.MoveCeiling(OvermorrowNPC.TargetingModule.Target.Center, 0.2f, maxSpeed, dropSpeed);
        }
    }
}