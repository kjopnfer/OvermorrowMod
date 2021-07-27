using OvermorrowMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class CorruptedMirror : Artifact
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupted Mirror");
            Tooltip.SetDefault("[c/EBDE34:{ Artifact of Courage }]\n" +
                "Consume a Soul Essence to gain a buff that reflects all damage for 3 minutes\n" +
                "All players on the same team gain the same buff for 3 minutes\n" +
                "'You can't shake the feeling of something otherworldy gazing from the mirror'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 28;
            item.height = 44;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 32;
            item.useTime = 32;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;

            soulResourceCost = 1;
            defBuffDuration = 10800; // 3 minutes
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicMirror);
            recipe.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicMirror);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}