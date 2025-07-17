using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class MusketHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Musket";
        public override int ParentItem => ItemID.Musket;
        public override WeaponType WeaponType => WeaponType.Musket;

        public override GunStats BaseStats => new GunBuilder()
            .AsMusket()
            .WithMaxShots(1)
            .WithReloadTime(100)
            .WithRecoil(25)
            .WithShootSound(SoundID.Item40)
            .WithClickZones((15, 35), (45, 65), (75, 95))
            .WithPositionOffset(new Vector2(20, -5), new Vector2(20, -4))
            .WithBulletShootPosition(new Vector2(20, 18), new Vector2(20, -4))
            .WithProjectileScale(1f)
            .WithTwoHanded()
            .Build();

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (shootCounter > maxShootTime - 9)
            {
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(48, 4) : new Vector2(48, -5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset, 0.08f);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            GunEffects.CreateSmoke(shootPosition, velocity);
        }

        protected override void OnReloadComplete(Player player, bool wasSuccessful)
        {
            int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;
        }

        protected override List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(gunPlayer.MusketInaccuracy));

            var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"),
                shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            gunPlayer.MusketInaccuracy = 45;

            return new List<int> { bullet };
        }

        protected override void OnReloadZoneHit(Player player, int zoneIndex, int clicksLeft)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            gunPlayer.MusketInaccuracy -= 15;
        }
    }
}