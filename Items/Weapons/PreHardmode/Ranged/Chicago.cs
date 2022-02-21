using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
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
            item.damage = 19;
            item.width = 54;
            item.height = 18;
            item.useTime = 5;
            item.useAnimation = 5;
            item.shootSpeed = 5f;
            item.shoot = 10;
            item.useAmmo = AmmoID.Bullet;
            item.useTurn = true;
            item.autoReuse = true;
            item.ranged = true;
            item.noMelee = true;
            item.useStyle = 5;
            item.UseSound = SoundID.Item11;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
		}
        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}
    }
}
