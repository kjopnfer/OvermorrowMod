using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class StaffofWorms : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of the Worms");
            Item.staff[item.type] = true;
            Tooltip.SetDefault("Spits out worms");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.damage = 15;
            item.magic = true;
            item.noMelee = true;
            item.mana = 7;
            item.useTime = 28;
            item.useAnimation = 28;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Items/Hork");
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("WormT1");
            item.shootSpeed = 16.7f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(57, 10);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
    }
}