using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Melee;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class BonerangWep : ModItem
    {
        public override void SetDefaults()
        {
            item.melee = true;
            item.noMelee = true;
            item.damage = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.9f;
            item.rare = ItemRarityID.Orange;
            item.crit = 4;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item19;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Bonerang>();
            item.shootSpeed = 17.6f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonerang");
            Tooltip.SetDefault("Boomerang that Splits on impact");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(154, 12);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

