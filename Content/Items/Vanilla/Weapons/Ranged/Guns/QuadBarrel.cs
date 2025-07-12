using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class QuadBarrelHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "QuadBarrel";
        public override int ParentItem => ItemID.QuadBarrelShotgun;
        public override WeaponType WeaponType => WeaponType.Shotgun;

        private int currentBonusAmmo = 0;
        public override GunStats BaseStats
        {
            get
            {
                var stats = new GunBuilder()
                    .AsShotgun()
                    .WithMaxShots(2)
                    .WithReloadTime(120)
                    .WithRecoil(25)
                    .WithShootSound(SoundID.Item36)
                    .WithReloadSound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ShotgunReload"))
                    .WithClickZones((24, 38), (60, 74))
                    .WithPositionOffset(new Vector2(14, -7), new Vector2(14, -4))
                    .WithBulletShootPosition(new Vector2(15, 15), new Vector2(15, -5))
                    .WithProjectileScale(0.8f)
                    .WithTwoHanded()
                    .Build();

                // Apply the persistent bonus shots to MaxShotsBonus instead of BonusAmmo
                stats.MaxShotsBonus = currentBonusAmmo;
                return stats;
            }
        }

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            var bullets = new List<int>();
            int numberProjectiles = Main.rand.Next(5, 7) + BonusBullets;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType),
                    shootPosition, perturbedSpeed, bulletType, damage, knockBack, player.whoAmI);
                bullets.Add(bullet);
            }

            return bullets;
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
                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(36, 0) : new Vector2(36, 0);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset, 0.09f);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-40 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);

            player.velocity += -Vector2.Normalize(velocity) * (3 + 0.5f * bonusBullets);
        }

        public override void OnReloadStart(Player player)
        {
            currentBonusAmmo = 0;
        }

        protected override void OnReloadZoneHit(Player player, int zoneIndex, int clicksLeft)
        {
            //CurrentStats.BonusAmmo++;
            currentBonusAmmo++;
        }
    }
}