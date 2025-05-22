using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class HandgunHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Handgun";
        public override int ParentItem => ItemID.Handgun;
        public override GunType GunType => GunType.Handgun;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Handgun)
                .WithMaxShots(10)
                .WithReloadTime(40)
                .WithRecoil(15)
                .WithSound(SoundID.Item41)
                .WithReloadZones((45, 60))
                .WithPositionOffset(new Vector2(16, -8), new Vector2(14, -2))
                .WithBulletPosition(new Vector2(10, 20), new Vector2(-10, -10))
                .WithScale(0.9f)
                .Build();

            ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/HandgunReload");
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            useTimeModifier = -8;
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (shootCounter > 3)
            {
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(22, 5) : new Vector2(24, -6);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-10 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);
        }
    }
}