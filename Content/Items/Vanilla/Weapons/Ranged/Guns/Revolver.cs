using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class RevolverHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Revolver";
        public override int ParentItem => ItemID.Revolver;
        public override WeaponType WeaponType => WeaponType.Revolver;

        public override GunStats BaseStats => new GunBuilder()
            .AsRevolver()
            .WithMaxShots(6)
            .WithReloadTime(60)
            .WithRecoil(10)
            .WithShootSound(SoundID.Item41)
            .WithClickZone(45, 60)
            .WithPositionOffset(new Vector2(18, -5), new Vector2(18, -5))
            .WithBulletShootPosition(new Vector2(15, 16), new Vector2(15, -6))
            .WithProjectileScale(0.85f)
            .Build();

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

        protected override void OnReloadComplete(Player player, bool wasSuccessful)
        {
            DropMultipleCasings(Projectile, player, 6);
        }

        protected override void OnReloadSuccessCore(Player player)
        {
            // Perfect reload gives 20% damage bonus
            CurrentStats.BonusDamage = (int)(Projectile.damage * 0.2f);
        }

        protected override void OnReloadZoneHit(Player player, int zoneIndex, int clicksLeft)
        {
            // If this is the last zone hit, instantly complete the reload
            if (clicksLeft == 0)
            {
                // Set reload time to 0 to complete instantly
                reloadTime = 0;
            }
        }
    }
}