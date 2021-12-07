using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class SaintRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Saint's Ring");
            Tooltip.SetDefault("Transforms consumed Soul Essences into Devoured Souls\n" +
                "Devoured Souls home in on nearby enemies, inflicting [c/00FF00:Possession] on hit\n" +
                "Enemies that are Possessed have reduced damage equal to number of Devoured Souls\n" +
                "'Once used to guide lost souls to an alternative purpose, now used to bend hollow shells to your will'");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.SaintRing = true;
        }
    }
}