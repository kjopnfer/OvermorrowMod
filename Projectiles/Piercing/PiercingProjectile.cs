using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public abstract class PiercingProjectile : ModProjectile
    {
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust based on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            projectile.ai[0] = 1f;

            projectile.netUpdate = true;

            Main.PlaySound(SoundID.Dig, projectile.position); // Plays impact sound

            return false; // Prevents projectile from disappearing on contact
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.NewText("yo");
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
