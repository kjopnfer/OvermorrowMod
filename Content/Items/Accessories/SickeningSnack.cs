using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class SickeningSnack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sickening Snack");
            Tooltip.SetDefault("Ranged attacks have a 33% chance to inflict Fungal Infection for 3 seconds\n" +
                "Whenever the target is struck by ranged attacks, increase debuff durations on the target by 2 seconds.\n" +
                "If the target has less than 50% health, increase debuff durations by 4 seconds instead.\n" +
                "Fungal Infection reduces armor by 2 for each debuff active.");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().SickeningSnack = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 20)
                .AddIngredient<StaleBread>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}