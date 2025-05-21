using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons
{
    public class FarlanderHeld : HeldGun
    {
        public override string Texture => AssetDirectory.ArchiveItems + "Farlander";
        public override int ParentItem => ModContent.GetInstance<Farlander>().Type;
        public override GunType GunType => GunType.Sniper;

        //public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(45, 60) };

        //public override bool TwoHanded => true;

        //public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(26, -12));
        //public override (Vector2, Vector2) PositionOffset => (new Vector2(28, -9), new Vector2(28, -2));

        //public override float ProjectileScale => 0.95f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 200;
            MaxShots = 2;
            RecoilAmount = 10;
            ShootSound = SoundID.Item41;
            UsesRightClickDelay = false;

            new GunBuilder(this)
                .AsType(GunType.Sniper)
                .WithMaxShots(2)
                .WithReloadTime(200)
                .WithRecoil(10)
                .WithSound(SoundID.Item41)
                .WithReloadZones((45, 60))
                .WithPositionOffset(new Vector2(28, -9), new Vector2(28, -2))
                .WithBulletPosition(new Vector2(20, 18), new Vector2(26, -12))
                .WithScale(0.95f)
                .TwoHanded()
                .CanRightClick()
                .WithRightClickDelay(false)
                .Build();
        }

        //public override bool CanRightClick => true;
        public override void RightClickEvent(Player player, ref int BonusDamage, int baseDamage)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FarlanderScope>()] < 1 && ShotsFired < MaxShots)
            {
                Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<FarlanderScope>(), 0, 0f, Projectile.owner);
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(54, 4) : new Vector2(54, -4);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            Vector2 shootOffset = new Vector2(-40 * player.direction, 0);

            SpawnBulletCasing(Projectile, player, shootPosition, shootOffset);
            GunEffects.CreateSmoke(shootPosition, velocity);
        }
    }

    public class Farlander : ModGun<FarlanderHeld>
    {
        public override GunType GunType => GunType.Sniper;
        public override string Texture => AssetDirectory.ArchiveItems + Name;

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