using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class EruditeDamage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erudite Damage Talisman");
            Tooltip.SetDefault("Increases all damage by 2");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}