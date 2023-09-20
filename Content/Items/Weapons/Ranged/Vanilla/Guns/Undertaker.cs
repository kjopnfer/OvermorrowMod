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
    public class Undertaker_Held : HeldGun
    {
        public override GunType GunType => GunType.Revolver;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(20, 45), new ReloadZone(60, 85) };
        public override int ParentItem => ItemID.TheUndertaker;
        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(15, 16), new Vector2(15, -6));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(18, -5), new Vector2(18, -5));
        public override float ProjectileScale => 0.9f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 4;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            string context = "HeldGun_Undertaker";
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, context), shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);
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

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(36, 4) : new Vector2(36, -5);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.08f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.08f, rotationSpriteEffects, 1);
                
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
            BonusAmmo = 2;
            useTimeModifier = -10;
        }


        public override void OnReloadEventFail(Player player, ref int BonusAmmo, ref int useTimeModifier)
        {
            BonusAmmo = -2;
        }
    }
}