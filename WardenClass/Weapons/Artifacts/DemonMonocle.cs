using OvermorrowMod.Projectiles.Artifact;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class DemonMonocle : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baleful Spine");
            Tooltip.SetDefault("[c/DE3A28:{ Artifact of Power }]\n" +
                               "Consume a Soul Essence to summon 6 Crimeras\n" +
                               "Crimeras will home in on nearby enemies\n" +
                               "'How many crimes could a criminal Crimera commit if a criminal Crimera could commit crimes?'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 46;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<DemonEye>();
            item.shootSpeed = 3f;
            item.damage = 21;

            soulResourceCost = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe.AddIngredient(ItemID.Vertebrae, 4);
            recipe.AddIngredient(ItemID.TissueSample, 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}