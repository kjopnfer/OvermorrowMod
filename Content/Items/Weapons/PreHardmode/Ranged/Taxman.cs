using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Ranged
{
    public class Taxman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taxman");
        }
        public override void SetDefaults()
        {
            item.damage = 22;
            item.width = 38;
            item.height = 18;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 11;
            item.useAnimation = 11;
            item.shoot = 10;
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
            item.UseSound = SoundID.Item11;
            item.useStyle = 5;
            item.useTurn = true;
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
    }
}
