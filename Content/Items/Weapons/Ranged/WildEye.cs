using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class WildEye_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Ranged + "WildEye";
        public override int ParentItem => ModContent.GetInstance<WildEye>().Type;

        public override GunType GunType => GunType.Revolver;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(30, 45) };

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(20, -4));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(20, -5), new Vector2(20, -4));
        public override float ProjectileScale => 0.75f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 6;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
        }

        public override bool CanRightClick => true;

        private bool WildEyeCrit = false;
        public override void RightClickEvent(Player player, ref int BonusDamage, int baseDamage)
        {
            if (ShotsFired == 0 && BulletDisplay.Count == MaxShots)
            {
                for (int i = 0; i < 5; i++)
                {
                    PopBulletDisplay();
                }

                ShotsFired += 5;
                WildEyeCrit = true;

                for (int i = 0; i < 5; i++)
                {
                    int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                    Main.gore[gore].sticky = true;
                }

                SoundEngine.PlaySound(SoundID.Item92 with { Pitch = 0.75f });

                float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
                float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

                for (int i = 0; i < 18; i++)
                {
                    randomScale = Main.rand.NextFloat(15f, 20f);

                    Vector2 RandomVelocity = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(i * 20));
                    Color color = Color.Red;

                    randomScale = 0.5f;
                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity * 4, color, 1, randomScale, 0f, 0f, 0f);

                    //randomScale = Main.rand.NextFloat(2, 5f);
                    //Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, Vector2.Normalize(RandomVelocity) * 0.5f, Color.Red, 1f, randomScale, 0f, 0f, -1f, randomScale);
                }
            }
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            string context = WildEyeCrit ? "WildEyeCrit" : "HeldGun";
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, context), shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);
        }

        public override void OnReloadEventFail(Player player, ref int BonusAmmo)
        {
            BonusAmmo = -1;
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

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(32, 5) : new Vector2(32, -5);
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
            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }

        public override void OnReloadEnd(Player player)
        {
            int droppedBullets = WildEyeCrit ? 1 : 6;
            for (int i = 0; i < droppedBullets; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }

            if (WildEyeCrit) WildEyeCrit = false;
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage)
        {
            BonusDamage += baseDamage * 3;
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            //if (clicksLeft == 0)
            //    reloadTime = 0;
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
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

            Color color = lightColor;
            if (WildEyeCrit && ShotsFired < MaxShots) color = Color.Lerp(Color.Red * 0.5f, lightColor, (float)(Math.Sin(PrimaryCounter++ / 20f) / 2 + 0.5f));

            spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, color, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            return false;
        }
    }

    public class WildEye : ModGun<WildEye_Held>
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wild Eye");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 23;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 22;
            Item.useAnimation = 22;
        }
    }
}