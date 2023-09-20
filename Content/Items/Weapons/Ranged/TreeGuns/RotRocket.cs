using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class RotRocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rot Rocket");
            // Tooltip.SetDefault("'Or \"Rotket\" for short'");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.NPCHit1;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 20;
            Item.useAnimation = 39;
            Item.useTime = 39;
            Item.width = 60;
            Item.height = 34;
            Item.shoot = ModContent.ProjectileType<SnotRocket>();
            Item.shootSpeed = 12f;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 43f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}