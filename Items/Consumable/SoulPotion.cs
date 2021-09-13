using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Consumable
{
    public class SoulPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Potion");
            Tooltip.SetDefault("Increases Soul Gain by 4%");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 34;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.maxStack = 20;
            item.noMelee = true;
            item.consumable = true;
            item.autoReuse = false;
            item.potion = true;
            item.buffType = ModContent.BuffType<SoulfulBuff>();
            item.buffTime = 7200; // 2 mins
            item.value = Item.buyPrice(gold: 4);
        }
    }
}