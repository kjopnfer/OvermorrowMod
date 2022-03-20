using Terraria;

namespace OvermorrowMod.Common.NPCs
{
    public abstract class PushableNPC : CollideableNPC
    {
        public override void AI()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.aiStyle == 7 && npc.Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.ai[0] = 2f;
                    projectile.position += npc.velocity;
                }
            }
        }
    }
}