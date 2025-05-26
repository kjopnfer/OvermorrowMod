using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class UndertakerHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Undertaker";
        public override int ParentItem => ItemID.TheUndertaker;
        public override GunType GunType => GunType.Revolver;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Revolver)
                .WithMaxShots(4)
                .WithReloadTime(60)
                .WithRecoil(10)
                .WithSound(SoundID.Item41)
                .WithReloadZones((20, 45), (60, 85))
                .WithPositionOffset(new Vector2(18, -5), new Vector2(18, -5))
                .WithBulletPosition(new Vector2(15, 16), new Vector2(15, -6))
                .WithScale(0.9f)
                .Build();
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(36, 4) : new Vector2(36, -5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset, 0.08f);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            GunEffects.CreateSmoke(shootPosition, velocity);
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