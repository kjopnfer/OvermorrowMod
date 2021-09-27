using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class HotAxeW : ModItem
    {
        float SwingRange = 0;
        int Timerset = 0;

        public override void SetDefaults()
        {
            item.noMelee = true;
            item.melee = true;
            item.damage = 38;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.Orange;
            item.crit = 1;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("AxeArround");
            item.shootSpeed = 0f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hot Axe");
            Tooltip.SetDefault("Swing an axe arround you \nCharge up range with your left click, then right Click to throw the axe");
        }



        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }



        public override bool CanUseItem(Player player)
        {

            if (SwingRange > 11)
            {
                SwingRange = 11;
            }

            if (Timerset > 0)
            {
                SwingRange = 0;
                Timerset = 0;
            }

            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Timerset++;
                item.shoot = mod.ProjectileType("Axearang");
                item.useTime = 33;
                item.useAnimation = 33;
                item.shootSpeed = 3f + SwingRange;
                item.UseSound = SoundID.Item19;
                item.useStyle = ItemUseStyleID.SwingThrow;
            }
            else //Sets what happens on left click(normal use)
            {
                SwingRange = SwingRange + 1.8f;
                item.shoot = mod.ProjectileType("AxeArround");
                item.useTime = 25;
                item.useAnimation = 25;
                item.shootSpeed = 0f;
                item.UseSound = SoundID.Item71;
                item.useStyle = ItemUseStyleID.HoldingOut;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.HellstoneBar, 27);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
    }
}

