using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class SpikeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike Staff");
            Tooltip.SetDefault("Creates spikes in a circle arround you when use is held down\nYou can only have 9 spikes at a time \nWhen use is released the spikes fire out");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 22;
            item.magic = true;
            item.mana = 4;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useTime = 33;
            item.channel = true;
            item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("CircleMagic");
            item.shootSpeed = 0f;
        }
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Spike, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
