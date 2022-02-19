using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class Eaterang : ModItem
    {
        public override void SetDefaults()
        {
            item.melee = true;
            item.noMelee = true;
            item.damage = 17;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.9f;
            item.rare = ItemRarityID.Orange;
            item.crit = 10;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item19;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EaterBoomerang>();
            item.shootSpeed = 15f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eaterang");
            Tooltip.SetDefault("Boomerang that does triple damage when coming back");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.DemoniteBar, 8);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<EaterBoomerang>()] < 1;
            }
        }
    }
}

