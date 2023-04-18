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
    public class Chicago_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Ranged + "Chicago";
        public override int ParentItem => ModContent.GetInstance<Chicago>().Type;

        public override GunType GunType => GunType.Rifle;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(20, 35), new ReloadZone(60, 75) };

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(26, -12));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(14, -8), new Vector2(14, -2));
        public override float ProjectileScale => 1f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 200;
            MaxShots = 50;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
        }

        public override int shootTime => 14;
        public override int shootAnimation => Main.player[Projectile.owner].GetModPlayer<GunPlayer>().ChicagoBonusShots ? 7 : 14;
        public override bool ConsumePerShot => true;

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            player.GetModPlayer<GunPlayer>().ChicagoBonusShots = true;
        }

        public override void OnReloadStart(Player player)
        {
            player.GetModPlayer<GunPlayer>().ChicagoBonusShots = false;
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(6));
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"), shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (shootCounter > maxShootTime - 7)
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
            Vector2 shootOffset = new Vector2(-36 * player.direction, 0);
            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }
    }

    public class Chicago : ModGun<Chicago_Held>
    {
        public override GunType GunType => GunType.Revolver;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicago");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 8;
            Item.width = 46;
            Item.height = 18;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 14;
            Item.useAnimation = 14;
        }
    }
}