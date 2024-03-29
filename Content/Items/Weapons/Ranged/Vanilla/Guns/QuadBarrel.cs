using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class QuadBarrel_Held : HeldGun
    {
        public override int ParentItem => Terraria.ID.ItemID.QuadBarrelShotgun;

        public override GunType GunType => GunType.Shotgun;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(24, 38), new ReloadZone(60, 74) };
        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(15, 15), new Vector2(15, -5));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(14, -7), new Vector2(14, -4));
        public override float ProjectileScale => 0.8f;
        public override bool TwoHanded => true;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 120;
            MaxShots = 2;
            RecoilAmount = 25;
            ShootSound = Terraria.ID.SoundID.Item36;
            ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ShotgunReload");
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            int numberProjectiles = Main.rand.Next(5, 7) + BonusBullets;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType), shootPosition, perturbedSpeed, bulletType, damage, knockBack, player.whoAmI);
            }
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

                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(36, 0) : new Vector2(36, 0);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-40 * player.direction, 0);

            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }

            player.velocity += -Vector2.Normalize(velocity) * (3 + 0.5f * bonusBullets);
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            BonusAmmo++;
        }
    }
}