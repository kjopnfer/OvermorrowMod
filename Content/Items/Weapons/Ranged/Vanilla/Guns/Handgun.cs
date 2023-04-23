using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class Handgun_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Handgun";
        public override int ParentItem => Terraria.ID.ItemID.Handgun;
        public override GunType GunType => GunType.Handgun;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(10, 20), new Vector2(-10, -10));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(16, -8), new Vector2(14, -2));
        public override float ProjectileScale => 0.9f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 40;
            MaxShots = 10;
            RecoilAmount = 15;
            ShootSound = Terraria.ID.SoundID.Item41;
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
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(22, 5) : new Vector2(24, -6);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, rotationSpriteEffects, 1);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-10 * player.direction, 0);
            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            for (int i = 0; i < 3; i++)
            {
                Vector2 smokeOffset = new Vector2(18 * player.direction, 0);
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition + smokeOffset, particleVelocity, Color.DarkGray);
            }
        }
    }
}