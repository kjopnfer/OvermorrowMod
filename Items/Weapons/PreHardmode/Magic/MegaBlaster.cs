using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MegaBlaster : ModItem
    {

        int Soundtimer = 0;
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
            item.damage = 13;
            item.magic = true;
            item.UseSound = SoundID.Item12;
            item.noMelee = true;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.5f;
            item.shoot = mod.ProjectileType("Blast1");
            item.shootSpeed = 0f;
        }
        public override void HoldItem(Player player)
        {
            Charge++;
            Soundtimer++;

            if(Soundtimer > 10)
            {
                Soundtimer = 0;
            }
            if(Charge > 10 && Charge < 60)
            {
                item.shoot = mod.ProjectileType("Blast1");
            }
            if(Charge > 59 && Charge < 120)
            {
                if(Soundtimer == 2)
                {
                    Main.PlaySound(SoundID.Item46, player.Center);
                }
                    item.shoot = mod.ProjectileType("Blast2");

                    int Random1 = Main.rand.Next(-57, 57);
                    int Random2 = Main.rand.Next(-57, 57);

                    float XDustposition = player.Center.X + Random1 - player.width / 2;
                    float YDustposition = player.Center.Y + Random2 - player.height / 2;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = player.Center - new Vector2(player.width / 2, player.height / 2);
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc = Color.Yellow;
                    {
                        int dust = Dust.NewDust(VDustposition, player.width, player.height, 16, 0.0f, 0.0f, 10, granitedustc, 1.8f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2f;
                        Main.dust[dust].velocity = Dustdirection * 2f;
                    }
            }

            if(Charge > 119)
            {
                if(Soundtimer == 2)
                {
                    Main.PlaySound(SoundID.Item20, player.Center);
                }
                    item.shoot = mod.ProjectileType("Blast3");

                    int Random1 = Main.rand.Next(-57, 57);
                    int Random2 = Main.rand.Next(-57, 57);

                    float XDustposition = player.Center.X + Random1 - player.width / 2;
                    float YDustposition = player.Center.Y + Random2 - player.height / 2;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = player.Center - new Vector2(player.width / 2, player.height / 2);
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc = Color.Cyan;
                    {
                        int dust = Dust.NewDust(VDustposition, player.width, player.height, 16, 0.0f, 0.0f, 10, granitedustc, 1.8f);
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
