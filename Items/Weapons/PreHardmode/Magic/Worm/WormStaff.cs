using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic.Worm
{
    public class WormStaff : ModItem
    {
        public override void SetDefaults()
        {
            item.magic = true;
            item.noMelee = true;
            item.damage = 135;
            item.useTime = 20;
            item.mana = 5;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3;
            item.rare = ItemRarityID.Yellow;
            item.crit = 1;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("WormHead");
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Items/Hork");
            item.shootSpeed = 7.5f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Staff");
            Tooltip.SetDefault("Shoots Homing worm that leaves a trail");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("CorruptEssence"), 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

