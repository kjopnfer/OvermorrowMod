using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class WildEye_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Ranged + "WildEye";
        public override int ParentItem => ModContent.GetInstance<WildEye>().Type;

        public override GunType GunType => GunType.Revolver;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(30, 45), new ReloadZone(60, 75) };

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
            if (ShotsFired == 0)
            {
                Main.NewText("right click");

                for (int i = 0; i < 5; i++)
                {
                    PopBulletDisplay();
                    Main.NewText("discard");
                }

                ShotsFired += 5;
                WildEyeCrit = true;
                BonusDamage = baseDamage * 5;
            }
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            string context = WildEyeCrit ? "WildEyeCrit" : "HeldGun";
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, context), shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            if (WildEyeCrit) WildEyeCrit = false;
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
            for (int i = 0; i < 4; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.12f)).RotatedByRandom(MathHelper.ToRadians(25));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }

        public override void OnReloadEnd(Player player)
        {
            for (int i = 0; i < 6; i++)
            {
                int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
                Main.gore[gore].sticky = true;
            }
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage)
        {
            BonusDamage = baseDamage;
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            if (clicksLeft == 0)
                reloadTime = 0;
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