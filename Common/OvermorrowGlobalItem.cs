using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalItem : GlobalItem
    {

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
            Mod.CreateRecipe(ItemID.MagicMirror)
                .AddIngredient(ItemID.RecallPotion, 7)
                .Register();
        }
    }
}