using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class QuadBarrelHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "QuadBarrel";
        public override int ParentItem => ItemID.QuadBarrelShotgun;
        public override GunType GunType => GunType.Shotgun;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Shotgun)
                .WithMaxShots(2)
                .WithReloadTime(120)
                .WithRecoil(25)
                .WithSound(SoundID.Item36)
                .WithReloadZones((24, 38), (60, 74))
                .WithPositionOffset(new Vector2(14, -7), new Vector2(14, -4))
                .WithBulletPosition(new Vector2(15, 15), new Vector2(15, -5))
                .WithScale(0.8f)
                .TwoHanded()
                .Build();

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

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            BonusAmmo++;
        }
    }
}