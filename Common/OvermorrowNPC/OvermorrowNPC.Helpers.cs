using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract partial class OvermorrowNPC : ModNPC
    {
        /// <summary>
        /// Determines whether the NPC is currently on the screen, based on player proximity.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if any active player is within the defined screen range of the NPC; otherwise, returns <c>false</c>.
        /// </returns>
        public bool IsOnScreen()
        {
            // Adapted vanilla code for NPC despawning
            int activeRangeX = (int)((double)NPC.sWidth * 2.1);
            int activeRangeY = (int)((double)NPC.sHeight * 2.1);
            /*Rectangle rectangle = new Rectangle((int)(NPC.position.X + (float)(NPC.width / 2) - (float)activeRangeX),
                                                (int)(NPC.position.Y + (float)(NPC.height / 2) - (float)activeRangeY),
                                                activeRangeX * 2, activeRangeY * 2);*/
            Rectangle rectangle2 = new Rectangle((int)((double)(NPC.position.X + (float)(NPC.width / 2)) - (double)NPC.sWidth * 0.5 - (double)NPC.width),
                                                 (int)((double)(NPC.position.Y + (float)(NPC.height / 2)) - (double)NPC.sHeight * 0.5 - (double)NPC.height),
                                                 NPC.sWidth + NPC.width * 2, NPC.sHeight + NPC.height * 2);

            for (int i = 0; i < 255; i++)
            {
                if (!Main.player[i].active)
                {
                    continue;
                }

                Rectangle hitbox = Main.player[i].Hitbox;
                /*if (rectangle.Intersects(hitbox))
                {
                    isOffscreen = false;
                    Main.NewText("hit 1");

                }*/

                if (rectangle2.Intersects(hitbox))
                {
                    return true;
                }
            }

            return false;
        }
    }
}