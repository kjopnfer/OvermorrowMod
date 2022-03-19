using Terraria;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

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
            item.width = 30;
            item.height = 22;
            item.ranged = true;
            item.autoReuse = true;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useAmmo = AmmoID.Bullet;
            item.damage = 6;
            item.crit = 8;
            item.useStyle = 5;
            item.shootSpeed = 5f;
            item.noMelee = true;
            item.shoot = 10;
            item.useTurn = true;
            item.UseSound = SoundID.Item11;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(20));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}
    }
}
