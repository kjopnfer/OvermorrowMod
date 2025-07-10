using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class UndertakerHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Undertaker";
        public override int ParentItem => ItemID.TheUndertaker;
        public override WeaponType WeaponType => WeaponType.Revolver;

        public override GunStats BaseStats => new GunBuilder()
            .AsRevolver()
            .WithMaxShots(4)
            .WithReloadTime(60)
            .WithRecoil(10)
            .WithShootSound(SoundID.Item41)
            .WithClickZones((20, 45), (60, 85))
            .WithPositionOffset(new Vector2(18, -5), new Vector2(18, -5))
            .WithBulletShootPosition(new Vector2(15, 16), new Vector2(15, -6))
            .WithProjectileScale(0.9f)
            .Build();

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            string context = "HeldGun_Undertaker";
            var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, context),
                shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            return new List<int> { bullet };
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(36, 4) : new Vector2(36, -5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset, 0.08f);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            GunEffects.CreateSmoke(shootPosition, velocity);
        }

        protected override void OnReloadComplete(Player player, bool wasSuccessful)
        {
            for (int i = 0; i < 6; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }
        }

        protected override void OnReloadSuccessCore(Player player)
        {
            // Perfect reload gives 2 bonus ammo and faster fire rate
            CurrentStats.BonusAmmo = 2;
            CurrentStats.UseTimeModifier = -10;
        }

        protected override void OnReloadFailCore(Player player)
        {
            // Failed reload reduces ammo by 2
            CurrentStats.BonusAmmo = -2;
        }
    }
}