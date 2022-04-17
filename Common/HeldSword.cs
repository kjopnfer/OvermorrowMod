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
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
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
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                return;
            }

            int direction = (int)Projectile.ai[1];
            float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            // the actual rotation it should have
            float defRot = Projectile.velocity.ToRotation();
            // starting rotation
            float start = defRot - ((MathHelper.PiOver2) - 0.2f);
            // ending rotation
            float end = defRot + ((MathHelper.PiOver2) - 0.2f);
            // current rotation obv
            float rotation = direction == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
            // offsetted cuz sword sprite
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
            position += rotation.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = rotation * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
    }
}
