using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Ruptor
{
    public class Ruptor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomruptor Cannon");
            Tooltip.SetDefault("Fires a bomb that sticks into enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 31;
            item.useAnimation = 31;
            item.autoReuse = true;
            item.shootSpeed = 17f;
            item.knockBack = 0;
            item.DamageType = DamageClass.Ranged;
            item.UseSound = SoundID.Item61;
            item.shoot = ModContent.ProjectileType<AmoungUsExplosive>();
            item.scale = 0.86f;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(ItemID.Bone, 30);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}
