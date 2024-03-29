using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class SnakeBite : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Snakebite");
            /* Tooltip.SetDefault("{Type:Arrows} and {Type:Melee} gain {Increase:5} armor penetration\n" +
                "{Type:Arrows} and {Type:Melee} have a {Increase:20%} chance to inflict <Debuff:Poison>\n" +
                "If the target's defense is less than your armor penetration, inflict <Debuff:Acid Venom> instead"); */
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 24;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().SnakeBite = true;
            player.GetModPlayer<OvermorrowModPlayer>().SnakeBiteHide = hideVisual;

            player.GetArmorPenetration(DamageClass.Melee) += 5;
            player.GetModPlayer<BowPlayer>().ArrowArmorPenetration += 5;
        }
    }
}