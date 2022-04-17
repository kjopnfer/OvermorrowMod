using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Utilities;
using System;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace OvermorrowMod.Content.Items.Weapons.Melee.Katana
{
    public class Katana : ModItem
    {
        public int currentAttack = 1;
        public int swordSwings = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of the Fake Assassin");
            Tooltip.SetDefault("Every 4th use attacks the target thrice in a single strike\n" +
                "'Breaking time and space in order to kill a single sparrow'");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.damage = 60;
            Item.crit = 32;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<KatanaHeld>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (++swordSwings % 4 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int dir = currentAttack;
                    currentAttack = -currentAttack;
                    Particle.CreateParticle(Particle.ParticleType<Slash>(), position, velocity, new Color(56, 38, 208), 1f, 0.5f);
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);

                }
            }
            else
            {
                int dir = currentAttack;
                currentAttack = -currentAttack;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Vector2 pos = Item.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D spot = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            float alpha = 0.8f;
            float s = 0.5f * scale;
            spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 6; i++)
            {
                spriteBatch.Draw(
                    spot,
                    pos,
                    null,
                    Color.Lerp(Color.White, Color.LightYellow, (float)i / 3) * alpha,
                    0f,
                    new Vector2(spot.Width, spot.Height) / 2,
                    s * (1f + ((float)Math.Sin((Main.GlobalTimeWrappedHourly * 2) + (MathHelper.PiOver4 * i)) * 0.15f)),
                    SpriteEffects.None,
                    0f);
                s *= 2;
                alpha *= 0.9f;
            }
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture, pos, null, Color.White, -MathHelper.PiOver4, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
        {
            Vector2 pos = ModUtils.GetInventoryPosition(position, frame, origin, scale);
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D flash = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Flash2").Value;
            spriteBatch.Reload(BlendState.Additive);
            UnifiedRandom rand = new UnifiedRandom(Item.whoAmI);
            for (int i = 0; i < 5; i++)
            {
                float alpha = 0.3f + (0.15f * i);
                float r = rand.NextFloat();
                float s = 0.6f - (0.06f * i);
                spriteBatch.Draw(
                    flash,
                    pos,
                    null,
                    Color.LightYellow * alpha,
                    Main.GlobalTimeWrappedHourly * r,
                    new Vector2(flash.Width / 2, flash.Height),
                    new Vector2(0.2f, s),
                    SpriteEffects.None,
                    0f);
            }
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture, pos, null, Color.White, 0f, texture.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}