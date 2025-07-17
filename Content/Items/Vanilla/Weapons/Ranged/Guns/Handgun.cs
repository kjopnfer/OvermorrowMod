using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class HandgunHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Handgun";
        public override int ParentItem => ItemID.Handgun;
        public override WeaponType WeaponType => WeaponType.Handgun;

        public override GunStats BaseStats => new GunBuilder()
            .AsHandgun()
            .WithMaxShots(10)
            .WithReloadTime(40)
            .WithRecoil(15)
            .WithShootSound(SoundID.Item41)
            .WithReloadSound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/HandgunReload"))
            .WithClickZone(45, 60)
            .WithPositionOffset(new Vector2(16, -8), new Vector2(14, -2))
            .WithBulletShootPosition(new Vector2(10, 20), new Vector2(-10, -10))
            .WithProjectileScale(0.9f)
            .WithShootTime(20)
            .WithShootAnimation(20)
            .Build();

        protected override void OnReloadSuccessCore(Player player)
        {
            // Reload gives faster fire rate
            CurrentStats.UseTimeModifier = -8;
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

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"),
                shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            return new List<int> { bullet };
        }
    }
}