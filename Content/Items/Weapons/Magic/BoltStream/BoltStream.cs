using Microsoft.Xna.Framework;
using Terraria;
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
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 10;
            item.UseSound = SoundID.Item75;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 24;
            item.useTurn = false;
            item.useAnimation = 14;
            item.useTime = 14;
            item.width = 28;
            item.height = 32;
            item.shoot = ModContent.ProjectileType<LightningCursor>();
            item.shootSpeed = 18f;
            item.knockBack = 1f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1);
            item.channel = true;
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightningCursor>()] <= 0)
            {
                Projectile.NewProjectile(Main.MouseWorld, new Vector2(speedX, speedY), ModContent.ProjectileType<LightningCursor>(), item.damage, 0f, player.whoAmI);
            }

            return false;
        }

        /*public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightningCursor>()] <= 0)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<LightningCursor>(), item.damage, 0f, player.whoAmI);
            }
        }*/
    }
}