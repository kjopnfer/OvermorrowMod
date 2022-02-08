using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs
{
    public abstract class CollideableNPC : ModNPC
    {
        public override void AI()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.aiStyle == 7 && npc.Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.ai[0] = 2f;

                    Main.NewText("a");
                }
            }

            /*for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    Rectangle PlayerLeft = new Rectangle((int)player.position.X, (int)player.position.Y, 1, player.height);
                    Rectangle PlayerRight = new Rectangle((int)player.position.X + player.width, (int)player.position.Y, 1, player.height);

                    Rectangle NPCRight = new Rectangle((int)npc.position.X + npc.width, (int)npc.position.Y, 8 + (int)Math.Max(player.velocity.X, 0), npc.height);
                    Rectangle NPCLeft = new Rectangle((int)npc.position.X, (int)npc.position.Y, 8 - (int)Math.Max(player.velocity.X, 0), npc.height);
              
                    if (PlayerLeft.Intersects(NPCRight))
                    {
                        if (player.position.X >= npc.position.X + npc.width && player.velocity.X <= 0)
                        {
                            player.velocity.X = 0;
                            player.position.X = (npc.position.X + npc.width) + 4;
                        }
                    }

                    if (PlayerRight.Intersects(NPCLeft))
                    {
                        if (player.position.X <= npc.position.X && player.velocity.X >= 0)
                        {
                            player.velocity.X = 0;
                            player.position.X = npc.position.X - player.width;
                        }
                    }
                }
            }*/
        }
    }
}