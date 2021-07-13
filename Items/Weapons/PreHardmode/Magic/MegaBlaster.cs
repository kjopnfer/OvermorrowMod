
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MegaBlaster : ModItem
    {

        int Soundtimer = 0;
        int Charge = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis");
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
            if(Charge < 60)
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



    public class EyeBlaster : ModItem
    {

        int Soundtimer = 0;
        int Charge = 0;
        int Mode = 0;
        int eye = 0;
        int normal = 0;
        public override string Texture => "OvermorrowMod/Items/Weapons/PreHardmode/Magic/MegaBlaster";
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster V1");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis \nUse rightclick to change modes and leftclick to fire mode projectile \nUpgrade descriptions: Eye Blast - Shoot out a penetrating projectile that gains damage when hitting an enemy");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 17;
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



        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            { 
                Main.NewText(text, Color.Blue);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Blue);
            }
        }


        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if(Mode == 0)
            {
                item.mana = 0;
                normal++;
                eye = 0;
                Charge++;
                Soundtimer++;

                if(Soundtimer > 10)
                {
                    Soundtimer = 0;
                }
                if(Charge < 60)
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
                
            }

            if(Mode == 1)
            {
                item.color = Color.Red;
                eye++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EyeBlast");
                item.mana = 11;
            }

            if(Mode == 0)
            {
                item.color = new Color();
            }

            if(eye == 1)
            {
                BossText("Eye Blast");
            }
            
            if(normal == 1)
            {
                BossText("Normal Shot");
            }

            if(Mode == 2)
            {
                Mode = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Mode += 1;
            }
            return true;
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




    public class EvilBlaster : ModItem
    {

        int Soundtimer = 0;
        int Charge = 0;
        int Mode = 0;
        int eye = 0;
        int evil = 0;
        int normal = 0;
        public override string Texture => "OvermorrowMod/Items/Weapons/PreHardmode/Magic/MegaBlaster";
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster V2");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis \nUse rightclick to change modes and leftclick to fire mode projectile \nUpgrade descriptions: Eye Blast - Shoot out a penetrating projectile that gains damage when hitting an enemy \n Vile Shotgun - Shoots a spliting, bouncing ball of evil");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.damage = 20;
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



        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            { 
                Main.NewText(text, Color.Blue);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Blue);
            }
        }


        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if(Mode == 0)
            {
                item.mana = 0;
                normal++;
                evil = 0;
                eye = 0;
                Charge++;
                Soundtimer++;

                if(Soundtimer > 10)
                {
                    Soundtimer = 0;
                }
                if(Charge < 60)
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
                
            }

            if(Mode == 1)
            {
                item.color = Color.Red;
                eye++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EyeBlast");
                item.mana = 9;
            }


            if(Mode == 2)
            {
                item.color = Color.Purple;
                evil++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EvilShotBlast1");
                item.mana = 11;
            }




            if(Mode == 0)
            {
                item.color = new Color();
            }

            if(eye == 1)
            {
                BossText("Eye Blast");
            }
            
            if(normal == 1)
            {
                BossText("Normal Shot");
            }

            if(evil == 1)
            {
                BossText("Vile Shotgun");
            }




            if(Mode == 3)
            {
                Mode = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Mode += 1;
            }
            return true;
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






    public class StingerBlaster : ModItem
    {

        int Soundtimer = 0;
        int Charge = 0;
        int Mode = 0;
        int eye = 0;
        int evil = 0;
        int normal = 0;
        int sting = 0;
        public override string Texture => "OvermorrowMod/Items/Weapons/PreHardmode/Magic/MegaBlaster";
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster V3");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis \nUse rightclick to change modes and leftclick to fire mode projectile \nUpgrade descriptions: Eye Blast - Shoot out a penetrating projectile that gains damage when hitting an enemy \nVile Shotgun - Shoots a spliting, bouncing ball of evil \nHoming Stinger - Shoots a stinger that homes in and poisons enemies");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.damage = 22;
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



        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            { 
                Main.NewText(text, Color.Blue);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Blue);
            }
        }


        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if(Mode == 0)
            {
                item.mana = 0;
                sting = 0;
                normal++;
                evil = 0;
                eye = 0;
                Charge++;
                Soundtimer++;

                if(Soundtimer > 10)
                {
                    Soundtimer = 0;
                }
                if(Charge < 60)
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
                
            }

            if(Mode == 1)
            {
                item.color = Color.Red;
                eye++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EyeBlast");
                item.mana = 9;
            }


            if(Mode == 2)
            {
                item.color = Color.Purple;
                evil++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EvilShotBlast1");
                item.mana = 11;
            }


            if(Mode == 3)
            {
                item.color = Color.Green;
                sting++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("HomingStinger");
                item.mana = 6;
            }


            if(Mode == 0)
            {
                item.color = new Color();
            }

            if(eye == 1)
            {
                BossText("Eye Blast");
            }
            
            if(normal == 1)
            {
                BossText("Normal Shot");
            }

            if(evil == 1)
            {
                BossText("Vile Shotgun");
            }

            if(sting == 1)
            {
                BossText("Homing Stinger");
            }


            if(Mode == 4)
            {
                Mode = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Mode += 1;
            }
            return true;
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




    public class SkeleBlaster : ModItem
    {

        int Soundtimer = 0;
        int Charge = 0;
        int Mode = 0;
        int eye = 0;
        int evil = 0;
        int normal = 0;
        int bone = 0;
        int sting = 0;
        public override string Texture => "OvermorrowMod/Items/Weapons/PreHardmode/Magic/MegaBlaster";
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster V4");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis \nUse rightclick to change modes and leftclick to fire mode projectile \nUpgrade descriptions: Eye Blast - Shoot out a penetrating projectile that gains damage when hitting an enemy \nVile Shotgun - Shoots a spliting, bouncing ball of evil \nHoming Stinger - Shoots a stinger that homes in and poisons enemies");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.damage = 27;
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



        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            { 
                Main.NewText(text, Color.Blue);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Blue);
            }
        }


        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if(Mode == 0)
            {
                item.mana = 0;
                sting = 0;
                normal++;
                bone = 0;
                evil = 0;
                eye = 0;
                Charge++;
                Soundtimer++;

                if(Soundtimer > 10)
                {
                    Soundtimer = 0;
                }
                if(Charge < 60)
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
                
            }

            if(Mode == 1)
            {
                item.color = Color.Red;
                eye++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EyeBlast");
                item.mana = 9;
                item.useTime = 13;
                item.useAnimation = 13;
            }


            if(Mode == 2)
            {
                item.color = Color.Purple;
                evil++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EvilShotBlast1");
                item.mana = 11;
                item.useTime = 13;
                item.useAnimation = 13;
            }


            if(Mode == 3)
            {
                item.color = Color.Green;
                sting++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("HomingStinger");
                item.mana = 6;
                item.useTime = 13;
                item.useAnimation = 13;
            }


            if(Mode == 4)
            {
                item.color = Color.Gray;
                bone++;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("HandHit");
                item.mana = 15;
                item.useTime = 70;
                item.useAnimation = 70;
            }


            if(Mode == 0)
            {
                item.color = new Color();
                item.useTime = 13;
                item.useAnimation = 13;
            }

            if(eye == 1)
            {
                BossText("Eye Blast");
            }
            
            if(normal == 1)
            {
                BossText("Normal Shot");
            }

            if(evil == 1)
            {
                BossText("Vile Shotgun");
            }

            if(sting == 1)
            {
                BossText("Homing Stinger");
            }

            if(bone == 1)
            {
                BossText("Bone Slapper");
            }

            if(Mode == 5)
            {
                Mode = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                Mode += 1;
            }
            return true;
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


