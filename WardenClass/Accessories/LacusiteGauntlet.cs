using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class LacusiteGauntlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Gauntlet");
            Tooltip.SetDefault("Increases defense by 12 and Piercing Damage by 3\n" +
                "Loses these bonuses after getting damaged\n" +
                "Bonuses will return after 20 seconds" +
                "\n'Everyone has a plan until they get punched in the mouth'");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 40;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.LacusiteGauntlet = true;

            if (!modPlayer.Shattered)
            {
                player.statDefense += 12;
                modPlayer.piercingDamageAdd += 3;
            }
        }
    }
}