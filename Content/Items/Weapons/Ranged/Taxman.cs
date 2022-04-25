using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class Taxman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taxman");
        }
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.width = 38;
            Item.height = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
