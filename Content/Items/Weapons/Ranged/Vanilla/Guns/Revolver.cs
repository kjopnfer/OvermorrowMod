using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class Revolver_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Revolver";
        public override GunType GunType => GunType.Revolver;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };
        public override int ParentItem => ItemID.Revolver;
        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(15, 16), new Vector2(15, -6));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(18, -5), new Vector2(18, -5));
        public override float ProjectileScale => 0.85f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 6;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (shootCounter > maxShootTime - 9)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(28, -5) : new Vector2(28, 5);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, rotationSpriteEffects, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }

        public override void OnReloadEnd(Player player)
        {
            for (int i = 0; i < 6; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            BonusDamage = (int)(baseDamage * 0.2f);
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            if (clicksLeft == 0) reloadTime = 0;
        }
    }
}