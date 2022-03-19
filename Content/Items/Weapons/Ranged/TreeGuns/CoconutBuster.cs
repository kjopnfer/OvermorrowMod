using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class CoconutBuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coconut Buster");
            Tooltip.SetDefault("'His coconut gun can fire in spurts'");
        }
        public override void SetDefaults()
        {
            item.damage = 13;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 20;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = Item.sellPrice(0, 0, 2, 0);
            item.scale = 0.8f;
            item.UseSound = SoundID.Item17;
            item.shoot = ProjectileType<Coconut>();
            item.autoReuse = false;
            item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
