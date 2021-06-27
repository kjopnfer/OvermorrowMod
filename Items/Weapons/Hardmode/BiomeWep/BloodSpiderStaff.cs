using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class BloodSpiderStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Spider Staff");
            Tooltip.SetDefault("Summons a Blood Cloud to fight for you");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Yellow;
            item.width = 32;
            item.height = 32;
            item.damage = 45;
            item.summon = true;
            item.channel = false;
            item.noMelee = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("BloodSumm");
            item.shootSpeed = 2f;
            item.buffType = ModContent.BuffType<BloodCrawlerBuff>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("CrimsonEssence"), 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}