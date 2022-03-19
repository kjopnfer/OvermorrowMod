using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic
{
    public class EmeraldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Emerald Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 9;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 16;
            item.useTurn = false;
            item.useAnimation = 30;
            item.useTime = 30;
            item.width = 48;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<EmeraldProj>();
            item.shootSpeed = 7f;
            item.knockBack = 5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed1 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(21f));
                Vector2 perturbedSpeed2 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-21f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, ModContent.ProjectileType<EmeraldProj>(), item.damage, 3, player.whoAmI);
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<EmeraldProj>(), item.damage, 3, player.whoAmI);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.EmeraldStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}