using OvermorrowMod.Common;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Loot.Rarities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Collectibles
{
    public abstract class CollectibleItem : ModItem
    {
        public override string Texture => AssetDirectory.Collectibles + Name;

        public abstract Rarity Rarity { get; }
        protected abstract CollectibleBonus[] Bonuses { get; }

        /// <summary>
        /// Effects not expressed as <see cref="Bonuses"/> (the bespoke flags set in
        /// <see cref="OnConsumed"/>). Each resolves to a localized description line.
        /// </summary>
        protected virtual CollectibleEffect[] DescribedEffects => null;

        public sealed override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item2;
            Item.rare = Rarity switch
            {
                Rarity.Rare => ModContent.RarityType<RareRarity>(),
                Rarity.Epic => ModContent.RarityType<EpicRarity>(),
                Rarity.Legendary => ModContent.RarityType<LegendaryRarity>(),
                _ => ModContent.RarityType<CommonRarity>(),
            };
            Item.value = Item.sellPrice(silver: 50);
            SafeSetDefaults();
        }

        protected virtual void SafeSetDefaults() { }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                var collectible = player.GetModPlayer<CollectiblePlayer>();
                int heal = 0;
                bool lowersAStat = false;
                foreach (var bonus in Bonuses)
                {
                    collectible.Add(bonus.Stat, bonus.Amount);
                    if (bonus.Stat == CollectibleStat.MaxLife && bonus.Amount > 0f)
                        heal += (int)bonus.Amount;
                    if (bonus.Amount < 0f)
                        lowersAStat = true;
                }

                if (lowersAStat)
                    collectible.DecreasingSourceCount++;

                if (heal > 0)
                {
                    player.statLife += heal;
                    player.HealEffect(heal);
                }

                OnConsumed(player);
            }
            return true;
        }

        /// <summary>
        /// Sets any non-stat effect flags on the consuming player. Stat bonuses
        /// declared in <see cref="Bonuses"/> are already applied before this runs.
        /// </summary>
        protected virtual void OnConsumed(Player player) { }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "CollectibleHeader", "{Keyword:Collectible}"));

            foreach (var bonus in Bonuses)
                tooltips.Add(new TooltipLine(Mod, "CollectibleBonus_" + bonus.Stat, DescribeBonus(bonus)));

            CollectibleEffect[] effects = DescribedEffects;
            if (effects != null)
            {
                foreach (var effect in effects)
                    tooltips.Add(new TooltipLine(Mod, "CollectibleEffect_" + effect, Language.GetTextValue(LocalizationPath.CollectibleEffect + effect)));
            }
        }

        private static string DescribeBonus(CollectibleBonus bonus)
        {
            bool positive = bonus.Amount >= 0f;
            string token = positive ? "Increase" : "Decrease";
            string unit = IsPercent(bonus.Stat) ? "%" : "";
            string value = "{" + token + ":" + Math.Abs(bonus.Amount).ToString("0") + unit + "}";
            string noun = Language.GetTextValue(LocalizationPath.CollectibleBonus + "Stat." + bonus.Stat);
            string verbKey = LocalizationPath.CollectibleBonus + (positive ? "Increase" : "Decrease");
            return Language.GetTextValue(verbKey, noun, value);
        }

        private static bool IsPercent(CollectibleStat stat) => stat switch
        {
            CollectibleStat.DamagePercent => true,
            CollectibleStat.DefensePercent => true,
            CollectibleStat.CritChance => true,
            CollectibleStat.MoveSpeedPercent => true,
            _ => false,
        };
    }
}
