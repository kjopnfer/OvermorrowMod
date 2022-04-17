using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class SpruceSprayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spruce Sprayer");
            Tooltip.SetDefault("'This wasn't what I meant by wanting to spruce things up'");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.damage = 7;
            item.DamageType = DamageClass.Ranged;
            item.width = 70;
            item.height = 46;
            item.useTime = 7;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = 10000;
            item.scale = 0.8f;
            item.UseSound = SoundID.Item17;
            item.shoot = ProjectileID.PineNeedleFriendly;
            item.autoReuse = true;
            item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(50));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
