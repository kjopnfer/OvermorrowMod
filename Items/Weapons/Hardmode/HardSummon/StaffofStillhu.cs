using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardSummon
{
    public class StaffofStillhu : ModItem
    {
        public override void SetDefaults()
        {
            item.summon = true;
            item.noMelee = true;
            item.damage = 72;
            item.useTime = 35;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.knockBack = 7;
            item.rare = ItemRarityID.Blue;
            item.crit = 1;
            item.autoReuse = true;
            item.channel = false;
            item.shoot = mod.ProjectileType("EyeStill");
            item.shootSpeed = 0f;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.buffType = mod.BuffType("EyeStillBuff");
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


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(549, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Stillhu");
            Tooltip.SetDefault("Summons an Eye of Stillhu \nUse this with the Summon Stick\nThe eye charges at your mouse then comes back");
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}

