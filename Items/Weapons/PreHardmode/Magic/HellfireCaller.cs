using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    class HellfireCaller : ModItem
    {

        float zob = 5.567f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Calls down hellfire upon foes near your mouse when used \nOnly works when foes are near your mouse");
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.width = 24;
            item.height = 24;
            item.magic = true;
            item.noMelee = true;
            item.useAnimation = 45;
            item.useTime = 45;
            item.shootSpeed = 3.8f;
            item.knockBack = 4f;
            item.damage = 35;
            item.mana = 10;
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 3, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.channel = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(175, 20);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }

        public override bool UseItem(Player player)
        {
            for (int i = 0; i < 200; i++)
            {

                NPC nPC = Main.npc[i];
                if (Vector2.Distance(Main.MouseWorld, nPC.Center) < 200)
                {
                    if (nPC.friendly == false && nPC.life > 0)
                    {
                        Projectile.NewProjectile(nPC.Center.X, nPC.Center.Y - 165, 0, 0, mod.ProjectileType("HellFireDown"), item.damage, 3, player.whoAmI);
                        Main.PlaySound(SoundID.Item45, Main.MouseWorld);
                    }
                }
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
