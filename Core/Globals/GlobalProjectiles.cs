using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Items.Archives.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            WhitePage.TryApplyStellarCorona(projectile, target);
        }
    }
}