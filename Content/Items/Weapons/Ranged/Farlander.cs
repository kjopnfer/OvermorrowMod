using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
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
    public class Farlander_Scope : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 34;
            Projectile.penetrate = -1;
        }

        float maxChargeTime = 120;
        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            if (Main.mouseRight && Main.player[Projectile.owner].active) Projectile.timeLeft = 2;

            Projectile.Center = Main.MouseWorld;
            if (AICounter < maxChargeTime + 30) AICounter++;
            Projectile.rotation += 0.052f;

            if (Main.mouseLeft) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float flashCounter = Utils.Clamp(AICounter - maxChargeTime, 0, 60);
            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 8f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 yOffset = Vector2.UnitY * -12;

            if (AICounter < maxChargeTime)
            {
                float scale = MathHelper.Lerp(2f, 0.5f, Utils.Clamp(AICounter, 0, maxChargeTime) / maxChargeTime);
                Main.spriteBatch.Draw(texture, Projectile.Center + yOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);

                Texture2D outerScope = ModContent.Request<Texture2D>(AssetDirectory.Ranged + "Farlander_OuterScope").Value;
                Main.spriteBatch.Draw(outerScope, Projectile.Center + yOffset - Main.screenPosition, null, Color.White * 0.75f, 0f, outerScope.Size() / 2f, scale, SpriteEffects.None, 1);
            }
            else
            {
                Main.spriteBatch.Draw(texture, Projectile.Center + yOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }
    }

    public class Farlander_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Ranged + "Farlander";
        public override int ParentItem => ModContent.GetInstance<Farlander>().Type;

        public override GunType GunType => GunType.Sniper;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };

        public override bool TwoHanded => true;

        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(26, -12));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(28, -9), new Vector2(28, -2));

        public override float ProjectileScale => 0.95f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 200;
            MaxShots = 2;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
            UsesRightClickDelay = false;
        }

        public override bool CanRightClick => true;
        public override void RightClickEvent(Player player, ref int BonusDamage, int baseDamage)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Farlander_Scope>()] < 1 && ShotsFired < MaxShots)
            {
                Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Farlander_Scope>(), 0, 0f, Projectile.owner);
            }
            //player.scope = true;
        }

        public override void Update(Player player)
        {
            if (ShotsFired < MaxShots)
                player.scope = true;
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            //player.GetModPlayer<GunPlayer>().ChicagoBonusShots = true;
        }

        public override void OnReloadStart(Player player)
        {
            //player.GetModPlayer<GunPlayer>().ChicagoBonusShots = false;
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

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(54, 4) : new Vector2(54, -4);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-40 * player.direction, 0);

            int gore = Gore.NewGore(null, shootPosition + shootOffset, new Vector2(player.direction * -0.03f, 0.01f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;

            for (int i = 0; i < 8; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.2f)).RotatedByRandom(MathHelper.ToRadians(40));
                Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
            }
        }

    }

    public class Farlander : ModGun<Farlander_Held>
    {
        public override GunType GunType => GunType.Sniper;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Farlander");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 56;
            Item.width = 82;
            Item.height = 22;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 22;
            Item.useAnimation = 22;
        }
    }
}