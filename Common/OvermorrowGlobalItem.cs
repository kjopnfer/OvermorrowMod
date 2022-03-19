using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth)
            {
                flat += 5;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman)
            {
                flat += 3;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                flat += 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().moonBuff)
            {
                flat += 4;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe Mirror = new ModRecipe(mod);
            Mirror.AddIngredient(ItemID.RecallPotion, 7);
            Mirror.SetResult(ItemID.MagicMirror);
            Mirror.AddRecipe();
        }
    }
}