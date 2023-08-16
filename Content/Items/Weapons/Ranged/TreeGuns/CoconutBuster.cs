using Microsoft.Xna.Framework;
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
            // DisplayName.SetDefault("Coconut Buster");
            // Tooltip.SetDefault("'His coconut gun can fire in spurts'");
        }
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 25;
            Item.useTime = 20;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.knockBack = 0.2f;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.scale = 0.8f;
            Item.UseSound = SoundID.Item17;
            Item.shoot = ProjectileType<Coconut>();
            Item.autoReuse = false;
            Item.shootSpeed = 8f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
