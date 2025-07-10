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
    public class MinisharkHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Minishark";
        public override int ParentItem => ItemID.Minishark;
        public override WeaponType WeaponType => WeaponType.MachineGun;

        public override GunStats BaseStats => new GunBuilder()
            .AsMachineGun()
            .WithMaxShots(120)
            .WithReloadTime(60)
            .WithRecoil(5)
            .WithShootSound(SoundID.Item11)
            .WithClickZone(45, 60)
            .WithPositionOffset(new Vector2(20, -5), new Vector2(20, -4))
            .WithBulletShootPosition(new Vector2(20, 18), new Vector2(20, -4))
            .WithProjectileScale(1f)
            .WithChargeTime(60)
            .WithTwoHanded()
            .Build();

        private int spinCounter = 0;

        public override bool CanConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.33f;
        }

        public override void OnChargeShootEffects(Player player)
        {
            player.velocity.X *= 0.95f;
        }

        public override void OnChargeUpEffects(Player player, int chargeCounter)
        {
            player.velocity.X *= 0.95f;
        }

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
            var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"),
                shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            return new List<int> { bullet };
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(44, 0) : new Vector2(44, -2);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new(-36 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition + new Vector2(16 * player.direction, 0), velocity);
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Resprites + Name).Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = player.direction == -1 ? new Vector2(0, -10) : Vector2.Zero;

            int frameCounter = (int)MathHelper.Lerp(0, 6, chargeCounter / (float)(MaxChargeTime / 2));
            if (chargeCounter > MaxChargeTime / 2)
            {
                spinCounter++;
                frameCounter = (int)MathHelper.Lerp(6, 10, (spinCounter % 20) / 20f);
            }

            Rectangle drawRectangle = new(0, texture.Height / 11 * frameCounter, texture.Width, texture.Height / 11);
            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, drawRectangle, lightColor, Projectile.rotation + reloadRotation, drawRectangle.Size() / 2f, ProjectileScale, spriteEffects, 1);

            return false;
        }
    }
}