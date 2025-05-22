using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class MinisharkHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Minishark";
        public override int ParentItem => ItemID.Minishark;
        public override GunType GunType => GunType.MachineGun;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.MachineGun)
                .WithMaxShots(120)
                .WithReloadTime(60)
                .WithRecoil(5)
                .WithSound(SoundID.Item11)
                .WithReloadZones((45, 60))
                .WithPositionOffset(new Vector2(20, -5), new Vector2(20, -4))
                .WithBulletPosition(new Vector2(20, 18), new Vector2(20, -4))
                .WithScale(1f)
                .TwoHanded()
                .Build();

            maxChargeTime = 60;
        }

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

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"), shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);
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
            Vector2 shootOffset = new Vector2(-36 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            maxChargeTime = 60;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Resprites + Name).Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = player.direction == -1 ? new Vector2(0, -10) : Vector2.Zero;

            int frameCounter = (int)MathHelper.Lerp(0, 6, chargeCounter / (float)(maxChargeTime / 2));
            if (chargeCounter > maxChargeTime / 2)
            {
                spinCounter++;
                frameCounter = (int)MathHelper.Lerp(6, 10, (spinCounter % 20) / 20f);
            }

            Rectangle drawRectangle = new Rectangle(0, texture.Height / 11 * frameCounter, texture.Width, texture.Height / 11);
            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, drawRectangle, lightColor, Projectile.rotation + reloadRotation, drawRectangle.Size() / 2f, ProjectileScale, spriteEffects, 1);

            return false;
        }
    }
}