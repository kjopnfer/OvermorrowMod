/*
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    public class RubyStorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruby Storm");
        }
        public override void SetDefaults()
        {
            item.damage = 40;
            item.noMelee = true;
            item.magic = true;
            item.channel = true;
            item.rare = ItemRarityID.Green;
            item.width = 28;
            item.knockBack = 3;
            item.height = 30;
            item.useTime = 16;
            item.autoReuse = true;
            item.UseSound = SoundID.Item20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 12f;
            item.useAnimation = 16;
            item.useTime = 8;
            item.mana = 6;
            item.shoot = ModContent.ProjectileType<RubyPro>();
            item.value = Item.sellPrice(silver: 50);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numberProjectiles = 3;
            for (int index = 0; index < numberProjectiles; ++index)
            {
                Vector2 vector2_1 = new Vector2((float)(player.position.X + player.width * 0.5 + (Main.rand.Next(201) * -player.direction) + (Main.mouseX + (double)Main.screenPosition.X - player.position.X)), (float)(player.position.Y + player.height * 0.5 - 600.0));
                vector2_1.X = (float)((vector2_1.X + (double)player.Center.X) / 2.0) + Main.rand.Next(-200, 201);
                vector2_1.Y -= (100 * index);
                float num12 = Main.mouseX + Main.screenPosition.X - vector2_1.X;
                float num13 = Main.mouseY + Main.screenPosition.Y - vector2_1.Y;
                if (num13 < 0.0) num13 *= -1f;
                if (num13 < 20.0) num13 = 20f;
                float num14 = (float)Math.Sqrt(num12 * num12 + num13 * num13);
                float num15 = item.shootSpeed / num14;
                float num16 = num12 * num15;
                float num17 = num13 * num15;
                float SpeedX = num16 + Main.rand.Next(-40, 41) * 0.02f;
                float SpeedY = num17 + Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(vector2_1.X, vector2_1.Y, SpeedX, SpeedY, type, damage, knockBack, Main.myPlayer, 0.0f, Main.rand.Next(5));
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(1526, 1);
            recipe.AddIngredient(548, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}*/