using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Melee;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class SoulSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Saber");
            Tooltip.SetDefault("Shoots a lost, homing soul.");
        }

        public override void SetDefaults()
        {
            item.damage = 29;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<Skull>();
            item.shootSpeed = 10; //Feel free to change ig

        }
        public override void AddRecipes() //prolly needs a change
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 35);
            recipe.AddIngredient(ModContent.ItemType<SoulFire>(), 1); //idk how this is obtained so...
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}