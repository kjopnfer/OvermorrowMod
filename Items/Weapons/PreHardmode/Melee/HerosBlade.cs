using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class HerosBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hero's Blade");
            Tooltip.SetDefault("Grows in strength as you move forward in your journey\n'Worn from age, but with limitless potential'");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 20;
            item.useTime = 25;
            item.useAnimation = 25;
            item.width = 56;
            item.height = 56;
            item.knockBack = 2f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void HoldItem(Player player)
        {
            player.armorPenetration += 5;
        }

        // Pre-Hardmode:
        // - 20 base damage(gains 2 for every beaten boss except Skeletron where it gains 4)
        // - 15 use time
        // - Ignores 5 defense(increases by one for every beaten boss)

        // Hardmode:
        // - 60 base damage (+5 after boss except Plantera, where it gains 10)
        // - Ignores 20 defense(+3 after every boss)
        // - 20 use time
        // - 30% larger than its old sprite
        // - Fill in the rest of the stats however you like
        // Tooltip: 'The legendary blade, restored to its lethal form.'

    }
}