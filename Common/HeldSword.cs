using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract class HeldSword : ModProjectile
    {
        public int SwingTime = 0;
        public float holdOffset = 50f;
        public override void SetDefaults()
        {
            projectile.timeLeft = SwingTime;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.melee = true;
        }
        public virtual float Lerp(float val)
        {
            return val;
        }
        public override void AI()
        {
            AttachToPlayer();
        }
        public override bool ShouldUpdatePosition() => false;
        public void AttachToPlayer()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                return;
            }

            int direction = (int)projectile.ai[1];
            float swingProgress = Lerp(Utils.InverseLerp(0f, SwingTime, projectile.timeLeft));
            // the actual rotation it should have
            float defRot = projectile.velocity.ToRotation();
            // starting rotation
            float start = defRot - ((MathHelper.PiOver2) - 0.2f);
            // ending rotation
            float end = defRot + ((MathHelper.PiOver2) - 0.2f);
            // current rotation obv
            float rotation = direction == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
            // offsetted cuz sword sprite
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
            position += rotation.ToRotationVector2() * holdOffset;
            projectile.Center = position;
            projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

            player.ChangeDir(projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = rotation * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
    }
}
