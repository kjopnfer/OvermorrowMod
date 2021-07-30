using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class EaterArtifact : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Maw of the Eater");
            Tooltip.SetDefault("[c/DE3A28:{ Artifact of Power }]\n" +
                               "Consume 1 Soul Essence to summon 3 worms\n" +
                               "Worms will home in on nearby enemies\n" +
                               "'I don't get it, what do you mean by calamitous worms?'");
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
            item.shoot = ModContent.ProjectileType<WormHead>();
            item.shootSpeed = 3f;
            item.damage = 18;

            soulResourceCost = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddIngredient(ItemID.ShadowScale, 10);
            recipe.AddIngredient(ItemID.WormTooth, 2);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}