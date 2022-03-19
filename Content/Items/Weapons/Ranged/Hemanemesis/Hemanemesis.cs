using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Hemanemesis
{
    public class Hemanemesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hemanemesis");
            Tooltip.SetDefault("When using musket balls or silver bullets, enemies explode on death");
        }
        public override void SetDefaults()
        {
            item.damage = 17;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 26;
            item.useAnimation = 26;
            item.UseSound = SoundID.NPCHit1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<BloodBullet>();
            item.autoReuse = true;
            item.shootSpeed = 6f;
            item.scale = 0.86f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Terraria.Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<BloodBullet>();
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
