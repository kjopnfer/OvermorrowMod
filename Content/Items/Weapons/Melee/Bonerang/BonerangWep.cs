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
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0.9f;
            Item.rare = ItemRarityID.Orange;
            Item.crit = 4;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item19;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Bonerang>();
            Item.shootSpeed = 17.6f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonerang");
            Tooltip.SetDefault("Boomerang that Splits on impact");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 69)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool CanUseItem(Player player)
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<Bonerang>()] < 1;
            }
        }
    }
}

