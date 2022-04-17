using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.GraniteChomper
{
    public class GraniteChomper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Chomper");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<GraniteGrabber>();
            Item.shootSpeed = 11f;
            Item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GraniteGrabber>()] < 1)
                {
                    Item.shoot = ModContent.ProjectileType<GraniteGrabber>();
                    Item.UseSound = SoundID.Item71;
                }
                else
                {
                    Item.shoot = ProjectileID.MoonlordTurretLaser;
                    Item.UseSound = SoundID.Item120;
                }
            }
        }
    }
}
