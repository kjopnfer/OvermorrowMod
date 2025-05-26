using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class MusketHeld : HeldGun
    {
        public override string Texture => AssetDirectory.Resprites + "Musket";
        public override int ParentItem => ItemID.Musket;
        public override GunType GunType => GunType.Musket;

        public override void SafeSetDefaults()
        {
            new GunBuilder(this)
                .AsType(GunType.Musket)
                .WithMaxShots(1)
                .WithReloadTime(100)
                .WithRecoil(25)
                .WithSound(SoundID.Item40)
                .WithReloadZones((15, 35), (45, 65), (75, 95))
                .WithPositionOffset(new Vector2(20, -5), new Vector2(20, -4))
                .WithBulletPosition(new Vector2(20, 18), new Vector2(20, -4))
                .WithScale(1f)
                .TwoHanded()
                .Build();
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
                Vector2 muzzleDirectionOffset = player.direction == -1 ? new Vector2(48, 4) : new Vector2(48, -5);
                GunEffects.DrawMuzzleFlash(spriteBatch, Projectile, player, muzzleDirectionOffset, 0.08f);
            }
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            GunEffects.CreateSmoke(shootPosition, velocity);
        }

        public override void OnReloadEnd(Player player)
        {
            int gore = Gore.NewGore(null, Projectile.Center, new Vector2(player.direction * -0.01f, 0f), Mod.Find<ModGore>("BulletCasing").Type, 0.75f);
            Main.gore[gore].sticky = true;
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(gunPlayer.MusketInaccuracy));
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"), shootPosition, rotatedVelocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            gunPlayer.MusketInaccuracy = 45;
        }

        public override void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            gunPlayer.MusketInaccuracy -= 15;
        }
    }
}