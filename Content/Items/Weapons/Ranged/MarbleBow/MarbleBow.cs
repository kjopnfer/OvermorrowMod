using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.MarbleBow
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
            //Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 25;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.width = 30;
            Item.height = 60;
            Item.shoot = AmmoID.Arrow;
            Item.shootSpeed = 8f;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(gold: 1);
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Power-attacking if player right-clicked
            if (powerAttack)
            {
                type = ModContent.ProjectileType<SpellboltPower>();

                Vector2 direction;

                // creating first arrow
                direction = Rotate(velocity, 30);
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, direction.X, direction.Y, type, damage, knockback, player.whoAmI);

                // creating second arrow
                direction = Rotate(velocity, -30);
                Projectile.NewProjectile(source, player.Center.X, player.Center.Y, direction.X, direction.Y, type, damage, knockback, player.whoAmI);
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
                Item.useAnimation = 33;
                Item.useTime = 33;
                Item.autoReuse = true;
                Item.mana = 10;
                Item.damage = 22;
                Item.shootSpeed = 10f;
                Item.knockBack = 10f;
                Item.shoot = ModContent.ProjectileType<SpellboltPower>();

                powerAttack = true;
            }
            else
            {
                Item.useAnimation = 19;
                Item.useTime = 19;
                Item.autoReuse = false;
                Item.mana = 0;
                Item.damage = 25;
                Item.shootSpeed = 8f;
                Item.knockBack = 4f;
                Item.shoot = AmmoID.Arrow;

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