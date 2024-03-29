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
        public ref float AICounter => ref Main.player[Projectile.owner].GetModPlayer<GunPlayer>().FarlanderCharge;
        public ref float DeathFlag => ref Projectile.ai[0];

        public override void AI()
        {
            if (DeathFlag == 1) return;

            if (Main.mouseRight && Main.player[Projectile.owner].active) Projectile.timeLeft = 2;

            float playerChargeBonus = 0.25f * Main.player[Projectile.owner].GetModPlayer<GunPlayer>().FarlanderSpeedBoost;
            float countRate = 1;

            // Apply rate penalty if the player is moving the mouse around very quickly
            float mouseSpeed = Math.Abs(Main.lastMouseX - Main.mouseX);
            if (AICounter < maxChargeTime) countRate = MathHelper.Lerp(1f, 0.05f, Utils.Clamp(mouseSpeed, 0, 35) / 35f);

            Projectile.Center = Main.MouseWorld;

            if (AICounter < maxChargeTime + 30) AICounter += countRate + playerChargeBonus;
            else AICounter++;

            if (Main.mouseLeft)
            {
                // Initially, the projectile would sometimes die before the gun was fired, resetting the counter
                // Thus, the gun would have a max inaccuracy shot even though it was fully charged
                // This fixes the issue by giving a brief window to always allow the gun to fire before resetting the counter
                DeathFlag = 1;
                Projectile.timeLeft = 10;
            }
        }

        public override void Kill(int timeLeft)
        {
            AICounter = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float flashCounter = Utils.Clamp(AICounter - maxChargeTime, 0, 999999);
            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 8f), 0, 1);
            if (DeathFlag == 1) flashProgress = 0;

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
                Main.spriteBatch.Draw(texture, Projectile.Center + yOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);


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
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            float chargeProgress = Utils.Clamp(gunPlayer.FarlanderCharge / 120f, 0, 1);
            float accuracy = MathHelper.Lerp(12, 0, chargeProgress);

            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(accuracy));
            int chargeDamage = (int)(chargeProgress == 1 ? damage * 1.5f : damage);

            string action = gunPlayer.FarlanderCharge < 120 ? "_Farlander" : "_FarlanderPowerShot";
            int projectile = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun" + action), shootPosition, rotatedVelocity, LoadedBulletType, chargeDamage, knockBack, player.whoAmI);
            if (gunPlayer.FarlanderPierce)
            {
                Main.projectile[projectile].penetrate++;
                Main.projectile[projectile].usesLocalNPCImmunity = true;
                Main.projectile[projectile].localNPCHitCooldown = -1;
            }
        }

        public override void Update(Player player)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            float chargeProgress = Utils.Clamp(gunPlayer.FarlanderCharge / 120f, 0, 1);

            if (ShotsFired < MaxShots) player.scope = true;
        }

        public override void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier)
        {
            player.GetModPlayer<GunPlayer>().FarlanderPierce = true;
        }

        public override void OnReloadStart(Player player)
        {
            player.GetModPlayer<GunPlayer>().FarlanderPierce = false;
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
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(Core.AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(54, 4) : new Vector2(54, -4);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);
                spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.09f, rotationSpriteEffects, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                Main.spriteBatch.Reload(SpriteSortMode.Deferred);
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
            // DisplayName.SetDefault("Farlander");
            /* Tooltip.SetDefault("{Keyword:Reload}: Your next clip gains 1 piercing\n" +
                "{Keyword:Alt}: Increase view range and charge\n" +
                "{Keyword:Focus}: Gain increased damage and accuracy\n" +
                "Improves charge time for each enemy hit by a {Keyword:Focus} shot\n" +
                "Charge bonus resets on miss"); */
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 98;
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