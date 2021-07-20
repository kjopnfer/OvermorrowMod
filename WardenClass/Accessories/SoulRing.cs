using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class SoulRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulstone Ring");
            Tooltip.SetDefault("Increases Soul Meter gain by 3%\nIncreases life regeneration\n" +
                "Regeneration rate scales with number of held Soul Essences");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 24;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.SoulRing = true;
            modPlayer.soulGainBonus += 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SoulFragment>(), 1);
            recipe.AddIngredient(ItemID.BandofRegeneration, 1);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}