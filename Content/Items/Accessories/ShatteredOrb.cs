using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Balloon)]
    public class ShatteredOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cracked Blood Orb");
            Tooltip.SetDefault("5% increased magic damage\nExpired magic projectiles have a chance to explode into blood" +
                "\n'Something foul is leaking out...'");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 34;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().ShatteredOrb = true;
            player.magicDamage += .05f;
        }
    }
}