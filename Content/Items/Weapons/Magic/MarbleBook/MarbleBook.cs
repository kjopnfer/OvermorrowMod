using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.MarbleBook
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
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.mana = 12; //11;
            Item.UseSound = SoundID.Item9;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 18;
            Item.useTurn = false;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.width = 30;
            Item.height = 36;
            Item.shoot = ModContent.ProjectileType<MarbleArrow>();
            Item.shootSpeed = 20f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1);
        }

        // A lot of old code, this will just refund mana for now until I get around to making this actually work

        //int limit = 0;
        //int capone = 0;
        //int z = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
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
                if (!Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].HasTile)
                {
                    Projectile.NewProjectile(source, randPos, velocity, type, damage, knockBack, player.whoAmI);
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