using OvermorrowMod.Projectiles.Melee.GraniteGrabber;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee
{
    public class GraniteChomper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Chomper");
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
            item.shoot = ModContent.ProjectileType<GraniteGrabber>();
            item.shootSpeed = 11f;
            item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GraniteGrabber>()] < 1)
            {
                item.shoot = ModContent.ProjectileType<GraniteGrabber>();
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
