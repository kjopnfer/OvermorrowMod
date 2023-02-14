using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract class ModGun<HeldProjectile> : ModItem where HeldProjectile : HeldGun
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Held Gun");
        }

        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Bullet;

            SafeSetDefaults();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
    }

    public class TestGun_Held : HeldGun
    {
        //public override int ConvertBullet => ItemID.MusketBall;
        //public override int BulletType => ProjectileID.VenomBullet;
        public override GunType GunType => GunType.Revolver;
        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(30, 45), new ReloadZone(60, 75) };
        public override int ParentItem => ModContent.GetInstance<TestGun>().Type;
        public override (Vector2, Vector2) PositionOffset => (new Vector2(18, -5), new Vector2(18, -5));
        public override float ProjectileScale => 0.75f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 6;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
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

                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(28, -5) : new Vector2(28, 5);
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

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            BonusDamage = baseDamage;
        }
       
        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            if (clicksLeft == 0)
                reloadTime = 0;
        }

        public override void OnReloadStart(Player player)
        {

        }
    }

    public class TestGun : ModGun<Minishark_Held>
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Gamer Gun");
        }

        public override void SafeSetDefaults()
        {
            // stuff used for revolver
            /*Item.damage = 50;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Lime;
            Item.useTime = 22;
            Item.useAnimation = 22;*/

            Item.damage = 30;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Lime;
            Item.useTime = 8;
            Item.useAnimation = 8;
        }
    }
}