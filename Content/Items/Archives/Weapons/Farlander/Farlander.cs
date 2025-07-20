using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class FarlanderHeld : HeldGun
    {
        public override string Texture => AssetDirectory.ArchiveItems + "Farlander";
        public override int ParentItem => ModContent.GetInstance<Farlander>().Type;
        public override WeaponType WeaponType => WeaponType.Sniper;

        public override GunStats BaseStats => new GunBuilder()
            .AsSniper()
            .WithMaxShots(2)
            .WithReloadTime(200)
            .WithRecoil(10)
            .WithShootSound(SoundID.Item41)
            .WithClickZone(45, 50)
            .WithPositionOffset(new Vector2(28, -9), new Vector2(28, -2))
            .WithBulletShootPosition(new Vector2(20, 18), new Vector2(26, -12))
            .WithProjectileScale(0.95f)
            .WithTwoHanded()
            .WithRightClick()
            .WithRightClickDelay(false)
            .Build();

        public override void RightClickEvent(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FarlanderScope>()] < 1 && ShotsFired < MaxShots)
            {
                Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<FarlanderScope>(), 0, 0f, Projectile.owner);
            }
        }

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            float chargeProgress = Utils.Clamp(gunPlayer.FarlanderCharge / 120f, 0, 1);
            float accuracy = MathHelper.Lerp(35, 0, chargeProgress);

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

            return new List<int> { projectile };
        }

        public override void Update(Player player)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            float chargeProgress = Utils.Clamp(gunPlayer.FarlanderCharge / 120f, 0, 1);

            if (ShotsFired < MaxShots) player.scope = true;
        }

        protected override void OnReloadSuccessCore(Player player)
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(48, 4) : new Vector2(48, -4);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new(-40 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);
        }
    }

    public class Farlander : ModGun<FarlanderHeld>
    {
        public override WeaponType WeaponType => WeaponType.Sniper;
        public override string Texture => AssetDirectory.ArchiveItems + Name;

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