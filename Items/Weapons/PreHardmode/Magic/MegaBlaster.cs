using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MegaBlaster : ModItem
    {


        int Charge = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster");
            Tooltip.SetDefault("Can be charged to make powerfull blast");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 21;
            item.magic = true;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.5f;
            item.shoot = mod.ProjectileType("Blast1");
            item.shootSpeed = 11f;
        }
        public override void HoldItem(Player player)
        {
            Charge++;

            if(Charge > 10 && Charge < 60)
            {
                item.shoot = mod.ProjectileType("Blast1");
            }
            if(Charge > 59 && Charge < 120)
            {
                item.shoot = mod.ProjectileType("Blast2");

                    int Random1 = Main.rand.Next(-70, 70);
                    int Random2 = Main.rand.Next(-70, 70);

                    float XDustposition = player.Center.X + Random1 - player.width / 2;
                    float YDustposition = player.Center.Y + Random2 - player.height / 2;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = player.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc = Color.Yellow;
                    {
                        int dust = Dust.NewDust(VDustposition, player.width, player.height, 185, 0.0f, 0.0f, 10, granitedustc, 2f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2f;
                        Main.dust[dust].velocity = Dustdirection * 2f;
                    }
            }

            if(Charge > 119)
            {
                item.shoot = mod.ProjectileType("Blast3");

                    int Random1 = Main.rand.Next(-70, 70);
                    int Random2 = Main.rand.Next(-70, 70);

                    float XDustposition = player.Center.X + Random1 - player.width / 2;
                    float YDustposition = player.Center.Y + Random2 - player.height / 2;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = player.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc = Color.Blue;
                    {
                        int dust = Dust.NewDust(VDustposition, player.width, player.height, 185, 0.0f, 0.0f, 10, granitedustc, 2f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2f;
                        Main.dust[dust].velocity = Dustdirection * 2f;
                    }
            }

            if(item.shoot == mod.ProjectileType("Blast2") && Charge < 60)
            {
                item.shoot = mod.ProjectileType("Blast1");
            }

            if(item.shoot == mod.ProjectileType("Blast3") && Charge < 120)
            {
                item.shoot = mod.ProjectileType("Blast1");
            }

        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Charge = 0;
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
