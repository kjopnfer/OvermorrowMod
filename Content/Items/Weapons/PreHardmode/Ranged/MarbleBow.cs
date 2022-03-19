using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Ranged
{
    public class MarbleBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spellbolt Bow");
            Tooltip.SetDefault("Right click to consume mana and empower your arrows");
        }

        public override void SetDefaults()
        {
            //item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 25;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 30;
            item.height = 60;
            item.shoot = AmmoID.Arrow;
            item.shootSpeed = 8f;
            item.knockBack = 10f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
            item.useAmmo = AmmoID.Arrow;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Power-attacking if player right-clicked
            if (powerAttack)
            {
                type = ModContent.ProjectileType<OvermorrowMod.Projectiles.Ranged.Ammo.SpellboltPower>();

                Vector2 direction;

                // creating first arrow
                direction = Rotate(new Vector2(speedX, speedY), 30);
                Projectile.NewProjectile(player.Center.X, player.Center.Y, direction.X, direction.Y, type, damage, knockBack, player.whoAmI);

                // creating second arrow
                direction = Rotate(new Vector2(speedX, speedY), -30);
                Projectile.NewProjectile(player.Center.X, player.Center.Y, direction.X, direction.Y, type, damage, knockBack, player.whoAmI);
            }
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useAnimation = 33;
                item.useTime = 33;
                item.autoReuse = true;
                item.mana = 10;
                item.damage = 22;
                item.shootSpeed = 10f;
                item.knockBack = 10f;
                item.shoot = ModContent.ProjectileType<OvermorrowMod.Projectiles.Ranged.Ammo.SpellboltPower>();

                powerAttack = true;
            }
            else
            {
                item.useAnimation = 19;
                item.useTime = 19;
                item.autoReuse = false;
                item.mana = 0;
                item.damage = 25;
                item.shootSpeed = 8f;
                item.knockBack = 4f;
                item.shoot = AmmoID.Arrow;

                powerAttack = false;
            }
            return base.CanUseItem(player);
        }
        private bool powerAttack = false;
        private Vector2 Rotate(Vector2 vector, float degrees)
        {
            float sin = (float)Math.Sin(degrees * (Math.PI / 180));
            float cos = (float)Math.Cos(degrees * (Math.PI / 180));

            float tx = vector.X;
            float ty = vector.Y;
            vector.X = (cos * tx) - (sin * ty);
            vector.Y = (sin * tx) + (cos * ty);
            return vector;
        }
    }
}