using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
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

    public class HeavenBlade : HeldSword
    {
        public override string Texture => "OvermorrowMod/Items/Weapons/PreHardmode/Melee/HeavenBlade";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heaven's Blade");
        }
        public override void SetDefaults()
        {
            SwingTime = 30;
            holdOffset = 50f;
            base.SetDefaults();
            projectile.width = projectile.height = 75;
            projectile.friendly = true;
            projectile.localNPCHitCooldown = SwingTime;
            projectile.usesLocalNPCImmunity = true;
        }
        public override float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 0f
                ? 0f
                : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // draws the slash
            Player player = Main.player[projectile.owner];
            Texture2D slash = ModContent.GetTexture("OvermorrowMod/Textures/slash_01");
            float mult = Lerp(Utils.InverseLerp(0f, SwingTime, projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + projectile.velocity * (40f - mult * 30f);
            spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.Yellow * alpha, projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, projectile.scale / 2, SpriteEffects.None, 0f);
            // draws the main blade
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 orig = texture.Size() / 2;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}