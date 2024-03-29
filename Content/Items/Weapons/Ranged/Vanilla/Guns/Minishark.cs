using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class Minishark_Held : HeldGun
    {
        public override int ParentItem => ItemID.Minishark;
        public override GunType GunType => GunType.MachineGun;
        public override bool TwoHanded => true;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(20, -4));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(20, -5), new Vector2(20, -4));
        public override float ProjectileScale => 1f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 120;
            RecoilAmount = 5;
            ShootSound = SoundID.Item11;
            maxChargeTime = 60;
        }

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
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(44, 0) : new Vector2(44, -2);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Vector2 randomScale = new Vector2(Main.rand.NextFloat(0.09f, 0.12f), 0.09f);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, randomScale, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, randomScale, rotationSpriteEffects, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-36 * player.direction, 0);

            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            for (int i = 0; i < 4; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Vector2 smokeOffset = new Vector2(8 * player.direction, 4);
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition + smokeOffset, particleVelocity, Color.DarkGray);
            }
        }

        private int spinCounter = 0;
        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            maxChargeTime = 60;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
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