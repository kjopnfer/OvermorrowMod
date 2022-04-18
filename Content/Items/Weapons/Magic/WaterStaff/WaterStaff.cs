using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.WaterStaff
{
    public class WaterStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Staff");
            Tooltip.SetDefault("'If you can't handle me at my worst, obey your thirst'");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.mana = 9;
            Item.UseSound = SoundID.Item21;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 16;
            Item.useTurn = false;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.width = 50;
            Item.height = 56;
            Item.shoot = ModContent.ProjectileType<WaterStaffProj>();
            Item.shootSpeed = 8f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
            velocity = perturbedSpeed;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WaterBar>(7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}