using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
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
            item.damage = 7;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 15;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.scale = 0.8f;
            item.rare = ItemRarityID.Green;
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
