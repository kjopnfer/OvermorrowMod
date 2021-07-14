using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.Projectiles.Melee;
using OvermorrowMod.Projectiles.Boss;


namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class HerosBlade : ModItem
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


        int treeshoot = 0;
        int scaletime = 0;
        int dashproj = 8;
        int dashreuse = 0;
        int saveddamage = 0;    
        int savedusetime = 0;
        int savedusetimeani = 0; 
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hero's Blade");
            Tooltip.SetDefault("Grows in strength as you move forward in your journey\n" +
                "'Worn from age, but with limitless potential'");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 20;
            item.useTime = 25;
            item.useAnimation = 25;
            item.width = 56;
            item.height = 56;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.knockBack = 1f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {

            if(NPC.downedQueenBee)
            {
                target.AddBuff(20, 200);
            }

            if(NPC.downedBoss3)
            {
                target.GetGlobalNPC<OvermorrowGlobalNPC>().Homingdie = true;
            }
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            if(NPC.downedBoss1)
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Tooltip1")
                    {
                        line.text = "Rightclick to do a double damage dash, dashing makes you immune to damage. Plays a sound when able to dash";
                    }
                }
            }

            if(NPC.downedBoss2)
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Tooltip2")
                    {
                        line.text = "Permanent thorns and heartreach while holding";
                    }
                }
            }

            if(NPC.downedBoss3)
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Tooltip3")
                    {
                        line.text = "Killing enemies spawns a homing projectile";
                    }
                }
            }

        }

        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if(NPC.downedBoss1 && player.altFunctionUse == 2 && dashreuse < 0)
            {

                Vector2 PLYposition = player.Center;
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - PLYposition;
                direction.Normalize();
                player.velocity = direction * 15;
                dashreuse = 120;
            }


            if(OvermorrowWorld.downedTree)
            {
                treeshoot++;
                if(treeshoot > 9)
                {
                    Vector2 PLAYposition = player.Center;
                    Vector2 ShootPosition = Main.MouseWorld;
                    Vector2 Tardirection = ShootPosition - PLAYposition;
                    Tardirection.Normalize();
                    int proj = Projectile.NewProjectile(player.Center + new Vector2(0, -40), Tardirection * 18.4f, ModContent.ProjectileType<NatureWave>(), item.damage - 9, 3, player.whoAmI);
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].width = 70;
                    Main.projectile[proj].height = 76;
                    Main.projectile[proj].penetrate = 1;
                    Main.projectile[proj].timeLeft = 160;
                    treeshoot = 0;
                }
            }


            return true;
        }

        public override void HoldItem(Player player)
        {

            scaletime++;
            if(scaletime == 1)
            {
                item.scale = 0.85f;
            }

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

            if(NPC.downedBoss2)
            {
                player.AddBuff(14, 2, true);
                player.AddBuff(105, 2, true);
            }

            if(OvermorrowWorld.downedDarude) 
            {
                player.buffImmune[BuffID.OnFire] = true;
                player.buffImmune[194] = true;
            }





            dashreuse--;
            if(dashreuse == -1 && NPC.downedBoss1)
            {
                Main.PlaySound(SoundID.Item125, player.Center);
            }
 
            if(dashreuse == 59)
            {
                item.damage = saveddamage;
                item.useTime = savedusetime;
                item.useAnimation = savedusetimeani;
            }
            if(dashreuse < 59)
            {
                saveddamage = item.damage;    
                savedusetime = item.useTime;
                savedusetimeani = item.useAnimation;
            }
            if(dashreuse > 60)  
            {
                player.immuneTime = 20;
                player.immune = true;
                item.damage = saveddamage * 2;
                item.useTime = 63;
                item.useAnimation = 63;
			    player.eocDash = 60;
			    player.armorEffectDrawShadowEOCShield = true;
                if(OvermorrowWorld.downedDrake)
                {
                    dashproj++;
                    if(dashproj > 9)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<StarAxe>(), item.damage / 2 - 7, 3, player.whoAmI);
                        dashproj = 0;
                    }
                }
            }
            else
            {
                dashproj = 8;
                player.eocDash = 0;
			    player.armorEffectDrawShadowEOCShield = false;
            }
        }





        // Pre-Hardmode:
        // - 20 base damage(gains 2 for every beaten boss except Skeletron where it gains 4)
        // - 15 use time

        // Hardmode:
        // - 60 base damage (+5 after boss except Plantera, where it gains 10)
        // - Ignores 20 defense(+3 after every boss)
        // - 20 use time
        // - 30% larger than its old sprite
        // - Fill in the rest of the stats however you like
        // Tooltip: 'The legendary blade, restored to its lethal form.'

    }
}