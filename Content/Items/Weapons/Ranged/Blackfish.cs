using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class Blackfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blackfish");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useAmmo = AmmoID.Bullet;
            Item.damage = 6;
            Item.crit = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 5f;
            Item.noMelee = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item11;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
        }
    }
}
