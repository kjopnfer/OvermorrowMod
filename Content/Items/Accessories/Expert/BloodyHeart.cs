using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.Expert
{
    public class BloodyHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Heart");
            Tooltip.SetDefault("Increases max health by 50\nReleases bouncing blood projectiles and a Blood Clot when damaged\nBlood Clots drop hearts when killed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 32;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Expert;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().BloodyHeart = true;
            player.statLifeMax2 += 50;
        }
    }
}