using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow
{
    public class IorichBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatwood Longbow");
            Tooltip.SetDefault("Hold down shoot to charge up the bow\n" +
                "Releases a volley of arrows when reaching maximum charge\n" +
                "Additionally, restore 5 life to the player during the volley");
        }

        public override void SetDefaults()
        {
            item.damage = 26;
            item.noMelee = true;
            item.ranged = true;
            item.rare = ItemRarityID.Orange;
            item.UseSound = null;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.channel = true;
            item.noUseGraphic = true;
            item.useAnimation = 10;
            item.useTime = 10;
            item.reuseDelay = 5;
            item.width = 28;
            item.height = 72;
            item.shoot = ModContent.ProjectileType<IorichBowHoldout>()/*AmmoID.Arrow*/;
            item.shootSpeed = 8f;
            item.knockBack = 10f;
            item.value = Item.sellPrice(gold: 1);
            item.ammo = AmmoID.Arrow;
            //item.useAmmo = AmmoID.Arrow;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        /*public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
        }*/

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}