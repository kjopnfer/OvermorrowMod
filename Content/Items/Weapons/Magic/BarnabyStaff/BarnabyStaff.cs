using OvermorrowMod.Content.Items.Weapons.Magic.GemStaves;
using OvermorrowMod.Content.Items.Weapons.Magic.SandStaff;
using OvermorrowMod.Content.Items.Weapons.Magic.WaterStaff;
using OvermorrowMod.Content.Items.Weapons.Magic.BloodStaff;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BarnabyStaff
{
    public class BarnabyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Manastorm Staff of Barnabus");
            Tooltip.SetDefault("Fires waves of raw mana to tear your enemies apart\n" +
                "'His friends call him \"Barnaby Barnaby\" '");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 16;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 37;
            Item.useTurn = false;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.width = 76;
            Item.height = 76;
            Item.shoot = ModContent.ProjectileType<ManaWave>();
            Item.shootSpeed = 9f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 6, silver: 75);
        }

        public override void AddRecipes()
        {
            Recipe BaseRecipe()
            {
                return CreateRecipe()
                    .AddIngredient<SandStaff.SandStaff>()
                    .AddIngredient<WaterStaff.WaterStaff>()
                    .AddIngredient<BloodStaff.BloodStaff>()
                    .AddTile(TileID.Anvils);
            }

            BaseRecipe().AddIngredient<AmethystStaff>().Register();
            BaseRecipe().AddIngredient<TopazStaff>().Register();
            BaseRecipe().AddIngredient<SapphireStaff>().Register();
            BaseRecipe().AddIngredient<EmeraldStaff>().Register();
            BaseRecipe().AddIngredient<AmberStaff>().Register();
            BaseRecipe().AddIngredient<RubyStaff>().Register();
            BaseRecipe().AddIngredient<DiamondStaff>().Register();
        }
    }
}