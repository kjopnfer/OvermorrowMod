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
            Item.width = 28;
            Item.height = 34;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().ShatteredOrb = true;
            player.GetDamage(DamageClass.Magic) += 0.05f;
        }
    }
}