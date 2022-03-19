using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged
{
    public class BrambleCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bramble Blaster");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.damage = 7;
            item.ranged = true;
            item.width = 48;
            item.height = 40;
            item.useTime = 34;
            item.useAnimation = 34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.scale = 0.8f;
            item.noMelee = true;
            item.knockBack = 0.1f;
            item.UseSound = SoundID.Item17;
            item.shoot = ProjectileID.Seed;
            item.shootSpeed = 7f;
            item.value = Item.sellPrice(0, 0, 15, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = Main.rand.Next(2, 3);
            for (int i = 0; i < numberProjectiles; i++)
            {

                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(30f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 0.5f, player.whoAmI);
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
