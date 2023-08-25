using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using OvermorrowMod.Core;
using System;

namespace OvermorrowMod.Common.VanillaOverrides.Melee
{
    public abstract partial class ThrownDagger : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        // I don't know if there is a different way to do this
        public abstract int ParentItem { get; }

        public override bool? CanDamage() => !groundCollided;
        public virtual Color IdleColor => Color.Orange;

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2; // Make the hitbox small to prevent hitting the ground too early
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public override void AI()
        {
            Tile bottomLeftTile = Main.tile[(int)Projectile.Hitbox.BottomLeft().X / 16, (int)Projectile.Hitbox.BottomLeft().Y / 16];
            Tile bottomRightTile = Main.tile[(int)Projectile.Hitbox.BottomRight().X / 16, (int)Projectile.Hitbox.BottomRight().Y / 16];

            // These are for weird slopes that don't trigger the collision code normally
            if ((bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) ||
                (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType])) HandleCollisionBounce();

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.ai[1];
                Projectile.ai[1] = 0;
            }

            // Make the hitbox normal again after 1/6th of a second
            if (Projectile.ai[0] > 10)
                Projectile.width = Projectile.height = 32;


            if (!groundCollided)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.rotation += 0.48f * (Projectile.velocity.X > 0 ? 1 : -1);

                if (Projectile.ai[0]++ > 10)
                    Projectile.velocity.Y += 0.25f;
            }
            else
            {
                Projectile.velocity.X *= 0.97f;

                if (Projectile.ai[1] == 60f)
                {
                    Projectile.velocity.X *= 0.01f;
                    oldPosition = Projectile.Center;
                }

                float rotationFactor = MathHelper.Lerp(0.48f, 0f, Utils.Clamp(Projectile.ai[1]++, 0, 60f) / 60f);
                Projectile.rotation += rotationFactor * (Projectile.velocity.X > 0 ? 1 : -1);

                Projectile.velocity.Y *= 0.96f;

                if (Projectile.ai[1] > 60f)
                {
                    Projectile.tileCollide = false;
                    //Projectile.rotation = MathHelper.Lerp(oldRotation, MathHelper.TwoPi + oldRotation, Utils.Clamp(Projectile.ai[1], 0, 20f) / 20f);
                    Projectile.Center = Vector2.Lerp(oldPosition, oldPosition - Vector2.UnitY * -24, (float)Math.Sin((Projectile.ai[1] - 60f) / 40f));
                }
            }

            // Check for if the owner has picked up the knife after it has landed
            if (groundCollided)
            {
                foreach (Player player in Main.player)
                {
                    if (player.whoAmI != Projectile.owner) continue;
                    if (player.Hitbox.Intersects(Projectile.Hitbox)) Projectile.Kill();
                }
            }

            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
            if (groundCollided)
            {
                float alpha = 0.65f * activeAlpha;
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
                Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, IdleColor * alpha, Projectile.rotation, outline.Size() / 2f, 1f * 0.1f, SpriteEffects.None, 1);

                outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
                Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, IdleColor * alpha, Projectile.rotation, outline.Size() / 2f, 1f * 0.5f, SpriteEffects.None, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Texture2D texture = TextureAssets.Item[ParentItem].Value;
            SpriteEffects spriteEffects = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color color = groundCollided ? Color.White : lightColor;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * activeAlpha, Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);

            return base.PreDraw(ref lightColor);
        }

        bool groundCollided = false;
        Vector2 oldPosition;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!groundCollided && Projectile.velocity.Y > 0)
            {
                HandleCollisionBounce();
            }
            else
            {
                Projectile.velocity *= -0.5f;
            }

            return false;
        }

        private void HandleCollisionBounce()
        {
            if (groundCollided) return;

            groundCollided = true;
            Projectile.velocity.X *= 0.5f;
            Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);
            Projectile.timeLeft = 600;
        }

        public virtual void OnThrownDaggerHit() { }
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnThrownDaggerHit();

            base.OnHitNPC(target, hit, damageDone);
        }
    }
}