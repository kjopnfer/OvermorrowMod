 using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
 
    public class MegaBuster : ModItem
    {

        bool eyebuff = false;
        bool evilbuff = false;
        bool skelebuff = false;
        bool slimebuff = false;
        bool beebuff = false;
        bool sandbuff = false;
        bool drakebuff = false;
        bool dripbuff = false;
        bool treebuff = false;



        int savedusetime = 0;
        int savedusetimeani = 0; 

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
            DisplayName.SetDefault("Mega Buster");
            Tooltip.SetDefault("Can be charged to make powerfull blast \nOnly shoots on the X axis \nUse rightclick to change modes and leftclick to fire mode projectile \nUpgrade descriptions: Eye Blast - Shoot out a penetrating projectile that gains damage when hitting an enemy \nVile Shotgun - Shoots a spliting, bouncing ball of evil \nHoming Stinger - Shoots a stinger that homes in and poisons enemies");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
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

            if(NPC.downedBoss1 && !eyebuff)
            {
                item.damage += 2;
                eyebuff = true;
            }   

            if(NPC.downedBoss2 && !evilbuff)
            {
                item.damage += 2;
                evilbuff = true;
            }  

            if(NPC.downedBoss3 && !skelebuff)
            {
                item.damage += 3;
                skelebuff = true;
            }   

            if(NPC.downedSlimeKing && !slimebuff)
            {
                item.damage += 1;
                slimebuff = true;
                player.moveSpeed += 0.07f;
            }  

            if(NPC.downedQueenBee && !beebuff)
            {
                item.damage += 1;
                beebuff = true;
            }  

            if(OvermorrowWorld.downedDarude && !sandbuff)
            {
                item.damage += 2;
                sandbuff = true;
            }  

            if(OvermorrowWorld.downedDrake && !drakebuff)
            {
                item.damage += 3;
                drakebuff = true;
            }   

            if(OvermorrowWorld.downedDrippler && !dripbuff)
            {
                item.scale += 0.2f;
                item.damage += 3;
                dripbuff = true;
            }   

            if(OvermorrowWorld.downedTree && !treebuff)
            {
                item.damage += 1;
                treebuff = true;
            }  


            if(Mode == 0)
            {
                item.mana = 0;
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

            if(Mode == 1 && NPC.downedBoss1)
            {
                item.color = Color.Red;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EyeBlast");
                item.mana = 9;
                item.useTime = 13;
                item.useAnimation = 13;
            }
            if(Mode == 1 && !NPC.downedBoss1)
            {
                Mode++;
            }


            if(Mode == 2 && NPC.downedBoss2)
            {
                item.color = Color.Purple;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("EvilShotBlast1");
                item.mana = 11;
                item.useTime = 13;
                item.useAnimation = 13;
            }
            
            if(Mode == 2 && !NPC.downedBoss2)
            {
                Mode++;
            }

            if(Mode == 3 && NPC.downedQueenBee)
            {
                item.color = Color.Green;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("HomingStinger");
                item.mana = 6;
                item.useTime = 13;
                item.useAnimation = 13;
            }
            if(Mode == 3 && !NPC.downedQueenBee)
            {
                Mode++;
            }

            if(Mode == 4 && NPC.downedBoss3)
            {
                item.color = Color.Gray;
                normal = 0;
                Charge = 0;
                item.shoot = mod.ProjectileType("HandHit");
                item.mana = 15;
                item.useTime = 70;
                item.useAnimation = 70;
            }
            if(Mode == 4 && !NPC.downedBoss3)
            {
                Mode++;
            }



            if(Mode == 5 && NPC.downedSlimeKing)
            {
                item.color = Color.Blue;
                item.shoot = mod.ProjectileType("SlimeBounce");
                item.mana = 5;
                item.useTime = 10;
                item.useAnimation = 30;
            }
            if(Mode == 5 && !NPC.downedSlimeKing)
            {
                Mode++;
            }

            if(Mode != 4 && Mode != 5)
            {
                item.color = new Color();
                item.useTime = 13;
                item.useAnimation = 13;
            }

            if(Mode == 6)
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

