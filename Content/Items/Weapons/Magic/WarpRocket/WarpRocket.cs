using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.WarpRocket
{
    public class WarpRocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Rocket");
            Tooltip.SetDefault("When the Projectile hits something it teleports you to it");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 12;
            Item.mana = 14;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<TPproj>();
            Item.shootSpeed = 17f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<TPproj>()] < 1;
        }
    }
}
