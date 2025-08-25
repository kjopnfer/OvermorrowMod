using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class HeldDagger : ModProjectile
    {
        protected Player Owner => Main.player[Projectile.owner];

        public virtual string DaggerTexture => Texture;
        public virtual Color SlashColor => Color.White;
        public virtual int SlashDuration => 20;
        public virtual float SlashRange => 80f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = SlashDuration;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public ref float SlashDirection => ref Projectile.ai[0];
        public ref float OffhandFlag => ref Projectile.ai[1];

        private bool hasHit = false;

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            // Position at player center
            Projectile.Center = Owner.Center;

            // Calculate slash progress
            float progress = 1f - (Projectile.timeLeft / (float)SlashDuration);
            float angle = GetSlashAngle(progress);

            // Set rotation for visual
            Projectile.rotation = angle;

            // Update hitbox position
            Vector2 slashOffset = new Vector2(SlashRange * 0.6f, 0).RotatedBy(angle);
            Projectile.Center = Owner.Center + slashOffset;
        }

        protected virtual float GetSlashAngle(float progress)
        {
            float baseAngle = Owner.DirectionTo(Main.MouseWorld).ToRotation();
            float swingRange = MathHelper.ToRadians(120);
            float swingProgress = (float)Math.Sin(progress * MathHelper.Pi);

            // Offset for dual wielding
            float offhandOffset = OffhandFlag == 1 ? MathHelper.ToRadians(30) : 0f;

            return baseAngle + (swingRange * SlashDirection * swingProgress) + offhandOffset;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasHit)
            {
                hasHit = true;
                CreateSlashHitEffects(target.Center);
                OnDaggerHit(target, hit, damageDone);
            }
        }

        protected virtual void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        protected virtual void CreateSlashHitEffects(Vector2 hitPosition)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}