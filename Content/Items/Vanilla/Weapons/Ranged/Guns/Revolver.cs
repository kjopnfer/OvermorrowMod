using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class RevolverHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Revolver";
        public override int ParentItem => ItemID.Revolver;
        public override GunType GunType => GunType.Revolver;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Revolver)
                .WithMaxShots(6)
                .WithReloadTime(60)
                .WithRecoil(10)
                .WithSound(SoundID.Item41)
                .WithReloadZones((45, 60))
                .WithPositionOffset(new Vector2(18, -5), new Vector2(18, -5))
                .WithBulletPosition(new Vector2(15, 16), new Vector2(15, -6))
                .WithScale(0.85f)
                .Build();
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
                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(28, -5) : new Vector2(28, 5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            GunEffects.CreateSmoke(shootPosition, velocity);
        }

        public override void OnReloadEnd(Player player)
        {
            DropMultipleCasings(Projectile, player, 6);
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