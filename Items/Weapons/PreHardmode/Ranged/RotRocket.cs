using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class RotRocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot Rocket");
            Tooltip.SetDefault("'Or \"Rotket\" for short'");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.NPCHit1;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 20;
            item.useAnimation = 39;
            item.useTime = 39;
            item.width = 60;
            item.height = 34;
            item.shoot = ModContent.ProjectileType<SnotRocket>();
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