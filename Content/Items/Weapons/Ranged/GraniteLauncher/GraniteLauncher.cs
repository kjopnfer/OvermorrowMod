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

namespace OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher
{
    public class GraniteLauncher_Held : HeldGun
    {
        public override string Texture => AssetDirectory.Ranged + "GraniteLauncher/GraniteLauncher";

        public override int ParentItem => ModContent.GetInstance<GraniteLauncher>().Type;

        public override GunType GunType => GunType.Launcher;

        public override List<ReloadZone> ClickZones => new List<ReloadZone>() { new ReloadZone(30, 45) };

        public override bool CanReload() => false;
        public override (Vector2, Vector2) BulletShootPosition => (new Vector2(20, 18), new Vector2(20, -4));
        public override (Vector2, Vector2) PositionOffset => (new Vector2(20, -5), new Vector2(20, -4));

        public override bool CanRightClick => true;
        public override bool TwoHanded => true;
        public override float ProjectileScale => 0.85f;

        public override void SafeSetDefaults()
        {
            MaxReloadTime = 60;
            MaxShots = 99;
            RecoilAmount = 30;
            ShootSound = SoundID.Item92;
        }

        private int shardCounter = 0;
        public override void RightClickEvent(Player player, ref int BonusDamage, int baseDamage)
        {
            if (shardCounter < 2) return;

            foreach (Projectile projectile in Main.projectile)
            {
                if (!projectile.active || projectile.owner != Projectile.owner) continue;

                if (projectile.ModProjectile is GraniteShard shard && !shard.HasActivated) shard.ActivateNextChain();
            }

            Main.NewText("arm granite shards");
            shardCounter = 0;
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            base.DrawGunOnShoot(player, spriteBatch, lightColor, shootCounter, maxShootTime);
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            string context = "GraniteLauncher";
            Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem, context), shootPosition, velocity, ModContent.ProjectileType<GraniteShard>(), damage, knockBack, player.whoAmI, shardCounter);

            shardCounter++;
        }

        public override void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets)
        {
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                float randomScale = Main.rand.NextFloat(0.1f, 0.15f);
                Vector2 RandomVelocity = Vector2.Normalize(velocity).RotatedByRandom(MathHelper.ToRadians(45)) * 4f;

                Color color = Color.Cyan;
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, RandomVelocity, color, 1, randomScale);
            }
        }

        public override bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0, 0.7f, 0.7f));
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1) directionOffset = new Vector2(0, -10);

            spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);


            Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.Ranged + "GraniteLauncher/GraniteLauncher_Glow").Value;
            spriteBatch.Draw(glowTexture, Projectile.Center + directionOffset - Main.screenPosition, null, Color.White, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            return false;
        }
    }

    public class GraniteLauncher : ModGun<GraniteLauncher_Held>
    {
        public override GunType GunType => GunType.Launcher;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockshard Launcher");
            Tooltip.SetDefault("Launches high velocity shards that stick to blocks");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 23;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 22;
            Item.useAnimation = 22;
        }

        public bool InHotbar(Player player, int type)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == type) return true;
            }

            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (InHotbar(player, Item.type)) player.GetModPlayer<GunPlayer>().GraniteLauncher = true;
        }
    }
}