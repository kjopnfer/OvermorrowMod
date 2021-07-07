using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Projectiles.Melee;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class CrimWarAxe : ModItem
    {


        float SwingRange = 0;
        int Timerset = 0;
        int TimerReal = 0;


        public override void SetDefaults()
        {
            item.noMelee = true;
            item.melee = true;
            item.damage = 30;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 2;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.Blue;
            item.crit = 2;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("BloodWaraxe");
            item.shootSpeed = 0.001f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Waraxe");
            Tooltip.SetDefault("Swing an axe arround you \nCharge up range with your left click, then right Click to throw the axe");
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(799, 1);
            recipe.AddIngredient(1329, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(1257, 11);
            recipe2.AddIngredient(1329, 10);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();
        }


        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }



        public override bool CanUseItem(Player player)
        {

            if(SwingRange > 9) 
            {
                SwingRange = 9;
            }


            if(Timerset > 0) 
            {
                SwingRange = 0;
                TimerReal = 0;
                Timerset = 0;
            }

            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            if (player.altFunctionUse != 2)//Sets what happens on right click(special ability)
            {
                SwingRange = SwingRange + 0.5f;
                item.shoot = mod.ProjectileType("BloodWaraxe");
                item.useTime = 40;
                item.useAnimation = 40;
                item.shootSpeed = 0f;
                item.UseSound = SoundID.Item71;
                item.useStyle = ItemUseStyleID.SwingThrow;
            }
            else
            {
                Timerset++;
                item.shoot = mod.ProjectileType("BloodWaraxeBack");
                item.useTime = 50;
                item.useAnimation = 50;
                item.shootSpeed = 3f + SwingRange;
                item.UseSound = SoundID.Item19;
                item.useStyle = ItemUseStyleID.SwingThrow;
            }
            return true;
        }
    }
}

