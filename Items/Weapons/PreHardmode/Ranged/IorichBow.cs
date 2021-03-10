using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class IorichBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatwood Longbow");
            Tooltip.SetDefault("Fires two arrows for each arrow");
        }

        public override void SetDefaults()
        {
            //item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 35;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 32;
            item.height = 62;
            item.shoot = AmmoID.Arrow;
            item.shootSpeed = 8f;
            item.knockBack = 10f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(30);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                Projectile.NewProjectile(position.X + perturbedSpeed.X, position.Y + perturbedSpeed.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}