using OvermorrowMod.Content.Items.Consumable;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<ReforgeStone>()) {
                for (int lines = 0; lines < tooltips.Count; lines++)
                {
                    if (tooltips[lines].Name == "Damage") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "CritChance") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Speed") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Knockback") tooltips.RemoveAt(lines);
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth)
            {
                damage.Flat += 5;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman)
            {
                damage.Flat += 3;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                damage.Flat += 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().moonBuff)
            {
                damage.Flat += 4;
            }
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.MagicMirror)
                .AddIngredient(ItemID.RecallPotion, 7)
                .Register();
        }
    }
}