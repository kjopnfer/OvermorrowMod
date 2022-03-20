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
                if (projectile.active && projectile.aiStyle == 7 && npc.Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.ai[0] = 2f;
                    projectile.position += npc.velocity;
                    Grappled = true;
                    //Main.NewText("a");
                }
            }  
        }
    }
}