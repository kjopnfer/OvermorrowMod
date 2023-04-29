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
using Terraria.UI.Chat;

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

        public override bool CanUseGun(Player player)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            return gunPlayer.GraniteEnergyCount > 0;
        }

        private int shardCounter = 0;
        public override void RightClickEvent(Player player, ref int BonusDamage, int baseDamage)
        {
            if (shardCounter < 2) return;

            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            foreach (KeyValuePair<int, GraniteShard> entry in gunPlayer.ShardList)
            {
                int id = gunPlayer.ShardList.ContainsKey(entry.Key + 1) ? gunPlayer.ShardList[entry.Key + 1].Entity.whoAmI : -1;
                entry.Value.ActivateNextChain(id);
                gunPlayer.ShardList.Remove(entry.Key);
            }

            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ElectricActivate") { Volume = 0.5f });

            // If for no apparent reason all of them didn't activate, clear the list
            gunPlayer.ShardList.Clear();

            shardCounter = 0;
        }

        public override void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime)
        {
            base.DrawGunOnShoot(player, spriteBatch, lightColor, shootCounter, maxShootTime);
        }

        public override void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            string context = "GraniteLauncher";
            gunPlayer.ShardList.Add(shardCounter, Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem, context), shootPosition, velocity, ModContent.ProjectileType<GraniteShard>(), damage, knockBack, player.whoAmI, shardCounter)].ModProjectile as GraniteShard);

            if (gunPlayer.GraniteEnergyCount > 0) gunPlayer.GraniteEnergyCount--;

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
            //Vector2 randomPosition = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 8f;
            //if (Main.rand.NextBool(20) && !Main.gamePaused)
            //    Particle.CreateParticle(Particle.ParticleType<Electricity>(), Projectile.Center + randomPosition, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 5f, Color.Cyan, 1, 1f);

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

        float offsetCounter = 0;
        public override bool PreDrawAmmo(Player player, SpriteBatch spriteBatch)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Ranged + "GraniteLauncher/GraniteShard").Value;

            float gapOffset = 6 * Utils.Clamp(gunPlayer.GraniteEnergyCount, 0, 6);
            float total = texture.Width * gunPlayer.GraniteEnergyCount + gapOffset;

            float startOffset = 12;
            float startPosition = (-total / 2) + startOffset;

            for (int i = 0; i < gunPlayer.GraniteEnergyCount; i++)
            {
                float heightOffset = 42;
                // spriteBatch.Draw(activeBullets, position + positionOffset - Main.screenPosition, null, Color.White, rotation, activeBullets.Size() / 2f, scale, SpriteEffects.None, 1);
                Vector2 positionOffset = new Vector2(startPosition + 20 * i, heightOffset + (MathHelper.Lerp(3, -2, (float)Math.Sin((offsetCounter + i * 12) / 20f)) / 2 + 0.5f));
                spriteBatch.Draw(texture, player.Center + positionOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            offsetCounter++;

            return false;
        }
    }

    public class GraniteLauncher : ModGun<GraniteLauncher_Held>
    {
        public override GunType GunType => GunType.Launcher;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockshard Launcher");
            Tooltip.SetDefault("<Passive>: Gain 1 shard on Critical Hit\n" +
                "<Alt>: Activate all unarmed shards <Power Shot>\n" +
                "Starts with 0 ammo and does not reload\n" +
                "Launches high velocity shards that stick to blocks");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 15;
            Item.width = 32;
            Item.height = 74;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useAmmo = AmmoID.None;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory) return;

            int shardCount = Main.LocalPlayer.GetModPlayer<GunPlayer>().GraniteEnergyCount;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, shardCount.ToString(), position + new Vector2(0f, 10f) * Main.inventoryScale, Color.White, 0f, Vector2.Zero, new Vector2(Main.inventoryScale), -1f, Main.inventoryScale);
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