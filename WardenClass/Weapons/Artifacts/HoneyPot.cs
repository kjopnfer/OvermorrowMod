using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class HoneyPot : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overflowing Honey Pot");
            Tooltip.SetDefault("[c/EBDE34:{ Artifact of Courage }]\n" +
                "Consumes all your Soul Essences\n" +
                "Each Soul Essence consumed heals for 10 life each\n" +
                "Additionally, gain the Honey buff for 1 minute\n" +
                "All players on the same team gain the same effects\n" +
                "'Keep away from any anthromorphic yellow bears'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 60;
            item.useTime = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;

            soulResourceCost = 1;
            defBuffDuration = 3600; // 1 minute
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HoneyComb, 12);
            recipe.AddIngredient(ItemID.BottledHoney, 9);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}