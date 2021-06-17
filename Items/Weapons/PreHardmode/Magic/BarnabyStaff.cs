using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class BarnabyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Manastorm Staff of Barnabus");
            Tooltip.SetDefault("Fires waves of raw mana to tear your enemies apart\n" +
                "'His friends call him \"Barnaby Barnaby\" '");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 16;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 37;
            item.useTurn = false;
            item.useAnimation = 32;
            item.useTime = 32;
            item.width = 76;
            item.height = 76;
            item.shoot = ModContent.ProjectileType<ManaWave>();
            item.shootSpeed = 9f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 6, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<AmethystStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<TopazStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<SapphireStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<EmeraldStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<AmberStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<RubyStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandStaff>());
            recipe.AddIngredient(ModContent.ItemType<WaterStaff>());
            recipe.AddIngredient(ModContent.ItemType<BloodStaff>());
            recipe.AddIngredient(ModContent.ItemType<DiamondStaff>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}