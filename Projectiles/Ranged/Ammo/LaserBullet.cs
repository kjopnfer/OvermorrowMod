using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class LaserBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Bullet");
            Tooltip.SetDefault("Shoots a laser");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;             //You need to set the item consumable so that the ammo would automatically consumed
            item.knockBack = 0f;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<Mousebullet>();
            item.shootSpeed = 5f;                  //The speed of the projectile
            item.ammo = AmmoID.Bullet;              //The ammo class this ammo belongs to.
        }
    }
}
