using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class GraniteChomper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Chomper");
            Tooltip.SetDefault("Shoots out a chomping projectile");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 15;
            item.ranged = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("GraniteGrabber");
            item.shootSpeed = 11f;
            item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            {
                if(player.ownedProjectileCounts[mod.ProjectileType("GraniteGrabber")] < 1)
                {
                    item.shoot = mod.ProjectileType("GraniteGrabber");
                    item.UseSound = SoundID.Item71;
                }
                else
                {
                    item.shoot = ProjectileID.MoonlordTurretLaser;
                    item.UseSound = SoundID.Item120;
                }
            }
        }
    }
}
