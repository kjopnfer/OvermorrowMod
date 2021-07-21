using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Projectiles.Other;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
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
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 12;
            item.mana = 14;
            item.noUseGraphic = true;
            item.magic = true;
            item.noMelee = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = ModContent.ProjectileType<TPproj>();
            item.shootSpeed = 17f;
        }



        public override bool CanUseItem(Player player)
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<TPproj>()] < 1;
            }
        }
    }
}
