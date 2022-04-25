using Microsoft.Xna.Framework;
using Terraria;
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
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 25;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.UseSound = SoundID.NPCHit1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<BloodBullet>();
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.scale = 0.86f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<BloodBullet>();
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
