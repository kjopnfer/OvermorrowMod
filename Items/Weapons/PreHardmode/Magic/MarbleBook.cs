using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MarbleBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollo Pythios");
            Tooltip.SetDefault("Summons three magical arrows that launch toward nearby enemies\n'No aiming required'");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.mana = 12; //11;
            item.UseSound = SoundID.Item9;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 18;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 28;
            item.width = 30;
            item.height = 36;
            item.shoot = ModContent.ProjectileType<MarbleArrow>();
            item.shootSpeed = 20f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1);
        }

        // A lot of old code, this will just refund mana for now until I get around to making this actually work
        
        //int limit = 0;
        //int capone = 0;
        //int z = 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            /*for (int x = (int)player.Center.X - 7; x > player.Center.X + 7; x++)
             {
                 for (int y = (int)player.Center.Y - 7; x > player.Center.Y + 7; y++)
                 {
                     if (Main.tile[x / 16, y / 16].active())
                     {
                         z += 1;
                         Dust.NewDust(new Vector2(x, y), 1, 1, 16, 0, 0, 0);
                     }
                 }
             }
             Main.NewText(z);
             /*if(z == 225)
             {
                 return false;
             }
             else
             {
                 for (int i = 0; i < 3; i++)
                 {
                     Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-7, 7) * 10, player.Center.Y + Main.rand.Next(-7, 7) * 10);
                     if (!Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].active())
                     {
                         Projectile.NewProjectile(randPos, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                     }
                     else if (Math.Abs(i) == 15)
                     {
                         continue;
                     }
                     else
                     {
                         --i;
                     }
                 }
             }
             /*for (int i = 0; i < 3; i++)
             {
                 //randposition = mainrand etc.
                 Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-7, 7) * 10, player.Center.Y + Main.rand.Next(-7, 7) * 10);
                 while (Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].active() || capone < limit)
                 {
                     //randpos = mainrand etc.
                     capone++;
                 }
                 if (limit == capone)
                 {
                     //reset capone = 0;
                     capone = 0;
                     continue;
                 }
                 else
                 {
                     //reset cap = 0
                     capone = 0;
                     //projectile.newproj(randpos, etc)
                     Projectile.NewProjectile(randPos, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                 }
             }*/
            for (int i = 0; i < 3; i++)
            {
                Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-7, 7) * 10, player.Center.Y + Main.rand.Next(-7, 7) * 10);
                if (!Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].active())
                {
                    Projectile.NewProjectile(randPos, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    player.statMana += 4;
                    continue;
                }
            }
            return false;
        }
    }
}