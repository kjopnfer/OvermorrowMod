using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class MeatMissile : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meat Missile");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.NPCHit1;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 23;
            item.useAnimation = 43;
            item.useTime = 43;
            item.width = 62;
            item.height = 32;
            item.shoot = ModContent.ProjectileType<MeatMissileProj>();
            item.shootSpeed = 12f;
            item.knockBack = 10f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 43f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}