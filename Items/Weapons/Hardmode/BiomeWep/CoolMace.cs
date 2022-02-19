/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class CoolMace : ModItem
    {


        float SwingRange = 0;
        int Timerset = 0;
        int TimerReal = 0;


        public override void SetDefaults()
        {
            item.noMelee = true;
            item.melee = true;
            item.damage = 120;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.knockBack = 2;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.Yellow;
            item.crit = 1;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("MaceArround");
            item.shootSpeed = 0f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Mace");
            Tooltip.SetDefault("Swing an Mace arround you \nCharge up range with your left click, then right Click to throw the Mace");
        }



        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }



        public override bool CanUseItem(Player player)
        {

            if(SwingRange > 15) 
            {
                SwingRange = 15;
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
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Timerset++;
                item.shoot = mod.ProjectileType("IceChunk");
                item.useTime = 22;
                item.useAnimation = 22;
                item.shootSpeed = 3f + SwingRange;
                item.UseSound = SoundID.Item19;
                item.useStyle = ItemUseStyleID.SwingThrow;
            }
            else //Sets what happens on left click(normal use)
            {
                SwingRange = SwingRange + 2.8f;
                item.shoot = mod.ProjectileType("MaceArround");
                item.useTime = 12;
                item.useAnimation = 12;
                item.shootSpeed = 0f;
                item.UseSound = SoundID.Item71;
                item.useStyle = 5;
            }

            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("FrostEssence"), 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}*/

