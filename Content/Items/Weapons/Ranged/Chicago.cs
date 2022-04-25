using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class Chicago : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicago");
            Tooltip.SetDefault("'Say your prayers, buster.'");
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.width = 54;
            Item.height = 18;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item11;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
