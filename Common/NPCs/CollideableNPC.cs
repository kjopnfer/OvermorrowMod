using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs
{
    public abstract class CollideableNPC : ModNPC
    {
        public bool Grappled = false;
        public override void AI()
        {
            Grappled = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.ai[0] != 1 && projectile.active && projectile.aiStyle == 7 && projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    projectile.ai[0] = 2f;
                    projectile.position += npc.velocity;
                    Grappled = true;
                    //Main.NewText("a");

                    projectile.netUpdate = true;
                }
            }  
        }
    }
}