using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class TorchGod : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torch God's Wrath");
            Tooltip.SetDefault("[c/DE3A28:{ Artifact of Power }]\n" +
                               "Consume a Soul Essence to summon a fireball to deal massive damage to an enemy\n" +
                               "'He's done handing out favors'");
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
            item.UseSound = SoundID.DD2_BetsyFlameBreath;
            item.consumable = false;
            item.autoReuse = false;
            //item.shoot = ModContent.ProjectileType<TorchBall>();
            item.shoot = ModContent.ProjectileType<BigBlueFire>();
            item.shootSpeed = 12f;
            item.damage = 69;
            item.knockBack = 3f;

            soulResourceCost = 1;
        }

        // May replace with a different recipe in 1.4
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumBar, 8);
            recipe.AddIngredient(ItemID.Torch, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldBar, 8);
            recipe.AddIngredient(ItemID.Torch, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}