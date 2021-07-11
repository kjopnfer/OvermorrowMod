/*using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardSummon
{
    public class NecromancyGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromancy Multi Glaive");
            Tooltip.SetDefault("Summons Skeletons to Fight for you \nSkeletons Last for 5 Seconds \nEach skeleton only counts for half a minion slot");
        }
        public override void SetDefaults()
        {
            item.mana = 4;
            item.width = 32;
            item.height = 32;
            item.damage = 37;
            item.summon = true;
            item.noMelee = true;
            item.scale = 0.5f;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("SkeleSummon");
            item.shootSpeed = 12f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(547, 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}*/