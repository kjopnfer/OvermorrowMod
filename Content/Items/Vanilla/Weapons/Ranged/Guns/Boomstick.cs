using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class BoomstickHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Boomstick";
        public override int ParentItem => ItemID.Boomstick;
        public override GunType GunType => GunType.Shotgun;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Shotgun)
                .WithMaxShots(2)
                .WithReloadTime(90)
                .WithRecoil(25)
                .WithSound(SoundID.Item36)
                .WithReloadSound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ShotgunReload"))
                .WithReloadZones((25, 40), (65, 80))
                .WithPositionOffset(new Vector2(14, -7), new Vector2(14, -2))
                .WithBulletPosition(new Vector2(15, 15), new Vector2(15, -5))
                .WithScale(1f)
                .TwoHanded()
                .Build();
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            int numberProjectiles = Main.rand.Next(3, 5) + BonusBullets;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(8));
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
                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(36, -5) : new Vector2(36, 5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-36 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);

            player.velocity += -Vector2.Normalize(velocity) * (5 + 1f * bonusBullets);

        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            BonusBullets++;
        }
    }
}