using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class BoltStream : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt Stream");
            Tooltip.SetDefault("Shoots 3 electric bolts that rapidly accelerate towards the target");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 8;
            item.UseSound = SoundID.Item75;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 24;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.width = 28;
            item.height = 32;
            item.shoot = mod.ProjectileType("BoltStreamBolt");
            item.shootSpeed = 18f;
            item.knockBack = 1f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(45);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;

            for(int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                Projectile.NewProjectile(position.X + perturbedSpeed.X, position.Y + perturbedSpeed.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }

            return false;
        }
    }
}