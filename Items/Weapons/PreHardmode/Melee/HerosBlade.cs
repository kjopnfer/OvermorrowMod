using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.Projectiles.Melee;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class HerosBlade : ModItem
    {

        bool eyebuff = false;
        bool evilbuff = false;
        bool skelebuff = false;



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
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.width = 56;
            item.height = 56;
            item.knockBack = 2f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
        }


        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
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
                item.damage += 4;
                skelebuff = true;
            }   


            if(NPC.downedBoss2)
            {
                player.AddBuff(14, 2, true);
                player.AddBuff(105, 2, true);
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
            }
            else
            {
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