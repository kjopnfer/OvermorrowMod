using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BoltStream
{
    public class BoltStream : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt Stream");
            Tooltip.SetDefault("Shoots 3 electric bolts that rapidly accelerate towards the target");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 10;
            Item.UseSound = SoundID.Item75;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 24;
            Item.useTurn = false;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.width = 28;
            Item.height = 32;
            Item.shoot = ModContent.ProjectileType<LightningCursor>();
            Item.shootSpeed = 18f;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1);
            Item.channel = true;
        }

        /*public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(45);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                Projectile.NewProjectile(position.X + perturbedSpeed.X, position.Y + perturbedSpeed.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }

            return false;
        }*/

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightningCursor>()] <= 0)
            {
                Projectile.NewProjectile(source, Main.MouseWorld, velocity, ModContent.ProjectileType<LightningCursor>(), Item.damage, 0f, player.whoAmI);
            }

            return false;
        }

        /*public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightningCursor>()] <= 0)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<LightningCursor>(), Item.damage, 0f, player.whoAmI);
            }
        }*/
    }
}