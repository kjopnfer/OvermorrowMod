using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class PhoenixBlasterHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "PhoenixBlaster";
        public override int ParentItem => ItemID.PhoenixBlaster;
        public override GunType GunType => GunType.Handgun;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Handgun)
                .WithMaxShots(10)
                .WithReloadTime(40)
                .WithRecoil(15)
                .WithSound(SoundID.Item41)
                .WithReloadZones((35, 65))
                .WithPositionOffset(new Vector2(18, -6), new Vector2(16, -4))
                .WithBulletPosition(new Vector2(10, 20), new Vector2(-10, -10))
                .WithScale(0.75f)
                .Build();

            ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/HandgunReload");
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-10 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);

            shootOffset = new Vector2(26, 0).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 4; i++)
            {
                float randomScale = Main.rand.NextFloat(0.05f, 0.15f);

                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.3f, 0.6f)).RotatedByRandom(MathHelper.ToRadians(15));
                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), shootPosition + shootOffset, particleVelocity, Color.Orange, randomScale, 0.5f, 0f, randomScale, 0f, 20f);
            }
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PhoenixBurst>(), Projectile.damage, 10f, player.whoAmI);
        }

        public override void OnReloadEventFail(Player player, ref int BonusAmmo, ref int useTimeModifier)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PhoenixMisfire>(), (int)(Projectile.damage / 2f), 0, player.whoAmI);
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = player.direction == -1 ? new Vector2(0, -10) : Vector2.Zero;

            if (reloadDelay > 0 && reloadSuccess)
            {
                float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                reloadRotation -= spinRate * player.direction;
            }
            else
                reloadRotation = 0;

            float progress = shotsFired / (float)MaxShots;

            float glowAmount = MathHelper.Lerp(0.5f, 1.4f, progress);
            Lighting.AddLight(Projectile.Center, new Vector3(1.2f * glowAmount, 0.7f * glowAmount, 0));

            float shakeDistance = MathHelper.Lerp(0, 1.1f, progress);
            Vector2 shakeOffset = new Vector2(Main.rand.NextFloat(-shakeDistance, shakeDistance), Main.rand.NextFloat(-shakeDistance, shakeDistance));

            Main.spriteBatch.Reload(BlendState.Additive);

            float alpha = MathHelper.Lerp(0.5f, 0.85f, progress);
            float scaleOffset = MathHelper.Lerp(1f, 1.15f, progress);
            Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "PhoenixBlasterOutline").Value;
            Main.spriteBatch.Draw(outline, Projectile.Center + shakeOffset + directionOffset - Main.screenPosition, null, Color.Red * alpha, Projectile.rotation + reloadRotation, outline.Size() / 2f, ProjectileScale * scaleOffset, spriteEffects, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.Orange.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(progress);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Main.spriteBatch.Draw(texture, Projectile.Center + shakeOffset + directionOffset - Main.screenPosition, null, Color.White, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }
    }
}