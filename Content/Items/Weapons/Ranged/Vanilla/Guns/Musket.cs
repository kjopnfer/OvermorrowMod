using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class Musket_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Musket";
        public override int ParentItem => ItemID.Musket;

        public override GunType GunType => GunType.Musket;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(15, 35), new ReloadZone(45, 65), new ReloadZone(75, 95) };

        public override bool TwoHanded => true;
        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(20, -4));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(20, -5), new Vector2(20, -4));
        public override float ProjectileScale => 1f;
        public override void SafeSetDefaults()
        {
            MaxReloadTime = 100;
            MaxShots = 1;
            RecoilAmount = 25;
            ShootSound = SoundID.Item40;
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
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(48, 4) : new Vector2(48, -5);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.08f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.08f, rotationSpriteEffects, 1);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }

        public override void OnReloadEnd(Player player)
        {
            int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(gunPlayer.MusketInaccuracy));
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"), shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            gunPlayer.MusketInaccuracy = 45;
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            gunPlayer.MusketInaccuracy -= 15;
        }
    }
}