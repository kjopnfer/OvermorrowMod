using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class Luger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luger P08");
            Tooltip.SetDefault("'Eat some Pfefferpotthast.'");
        }
        public override void SetDefaults()
        {
            item.damage = 18;
            item.width = 38;
            item.height = 24;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = 5;
            item.shoot = 10;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 9f;
            item.autoReuse = false;
            item.UseSound = SoundID.Item11;
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
