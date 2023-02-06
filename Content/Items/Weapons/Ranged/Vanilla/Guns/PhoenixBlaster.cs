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
    public class PhoenixBlaster_Held : HeldGun
    {
        public override int ParentItem => Terraria.ID.ItemID.PhoenixBlaster;
        public override GunType GunType => GunType.Pistol;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(10, 20), new Vector2(-10, -10));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(18, -6), new Vector2(16, -4));
        public override float ProjectileScale => 0.75f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 10;
            RecoilAmount = 15;
            ShootSound = Terraria.ID.SoundID.Item41;
            ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/HandgunReload");
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            /*Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (shootCounter > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.NewText("e");

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(36, -5) : new Vector2(36, 5);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }*/
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-10 * player.direction, 0);

            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            shootOffset = new Vector2(26, 0).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 4; i++)
            {
                float randomScale = Main.rand.NextFloat(0.05f, 0.15f);

                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.3f, 0.6f)).RotatedByRandom(MathHelper.ToRadians(15));
                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), shootPosition + shootOffset, particleVelocity, Color.Orange, randomScale, 0.5f, 0f, randomScale, 0f, 20f);
            }
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PhoenixBurst>(), Projectile.damage, 10f, player.whoAmI);
        }

        public override void OnReloadEventFail(Player player)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PhoenixMisfire>(), (int)(Projectile.damage / 2f), 0, player.whoAmI);
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            MaxReloadTime = 40;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (reloadDelay > 0 && reloadSuccess)
            {
                float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                reloadRotation -= spinRate * player.direction;
            }
            else
                reloadRotation = 0;

            float progress = shotsFired / (float)MaxShots;

            float shakeDistance = MathHelper.Lerp(0, 1.1f, progress);
            Vector2 shakeOffset = new Vector2(Main.rand.NextFloat(-shakeDistance, shakeDistance), Main.rand.NextFloat(-shakeDistance, shakeDistance));

            Main.spriteBatch.Reload(BlendState.Additive);

            float alpha = MathHelper.Lerp(0.5f, 0.85f, progress);
            float scaleOffset = MathHelper.Lerp(1f, 1.15f, progress);
            Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.Ranged + "Vanilla/Guns/PhoenixBlaster_Outline").Value;
            Main.spriteBatch.Draw(outline, Projectile.Center + shakeOffset + directionOffset - Main.screenPosition, null, Color.Red * alpha, Projectile.rotation + reloadRotation, outline.Size() / 2f, ProjectileScale * scaleOffset, spriteEffects, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.Orange.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Main.spriteBatch.Draw(texture, Projectile.Center + shakeOffset + directionOffset - Main.screenPosition, null, Color.White, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);


            return false;
        }
    }
}