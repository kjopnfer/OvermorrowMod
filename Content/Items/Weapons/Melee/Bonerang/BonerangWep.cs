using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.Bonerang
{
    public class BonerangWep : ModItem
    {
        public override string Texture => AssetDirectory.Melee + "Bonerang/Bonerang";
        public override void SetDefaults()
        {
            item.melee = true;
            item.noMelee = true;
            item.damage = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.9f;
            item.rare = ItemRarityID.Orange;
            item.crit = 4;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item19;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Bonerang>();
            item.shootSpeed = 17.6f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonerang");
            Tooltip.SetDefault("Boomerang that Splits on impact");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.Bone, 69);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<Bonerang>()] < 1;
            }
        }
    }
}

