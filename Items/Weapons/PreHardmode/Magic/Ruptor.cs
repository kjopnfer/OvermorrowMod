using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Magic;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class Ruptor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomruptor Staff");
            Tooltip.SetDefault("Bomb that sticks into enemies, Explodes after a while");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 11;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 31;
            item.useAnimation = 31;
            item.autoReuse = true;
            item.shootSpeed = 17f;
            item.knockBack = 0;
            item.magic = true;
            item.mana = 11;
            item.UseSound = SoundID.Item20;
            item.shoot = ModContent.ProjectileType<AmoungUsExplosive>();
            item.crit = 5;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(57, 12);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();


            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(1257, 12);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();
        }
    }
}
