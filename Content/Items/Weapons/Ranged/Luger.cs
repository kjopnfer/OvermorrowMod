using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
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
            Item.damage = 18;
            Item.width = 38;
            Item.height = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 9f;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item11;
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
