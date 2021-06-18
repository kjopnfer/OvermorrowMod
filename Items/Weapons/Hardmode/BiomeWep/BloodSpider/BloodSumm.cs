using OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider
{
    public class BloodSumm : ModProjectile
    {

        private int movement = 0;
        private int timer = 60;
        private int timer2 = 0;
        private int timer3 = 0;
        private int movement2 = 0;
        private int penet = 0;
        private int penet2 = 0;
        private int savedDMG = 0;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Raven);
            aiType = ProjectileID.Raven;
            projectile.netImportant = true;
            projectile.minion = true;
            projectile.minionSlots = 1.5f;
            projectile.penetrate = -1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Spider");
            Main.projFrames[base.projectile.type] = 8;
        }
        public override void AI()
        {


            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<BloodCrawlerBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<BloodCrawlerBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion



			if (player.channel) 
            {
			timer++;
			if(timer == 62)
			{

                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                Vector2 newpoint2 = new Vector2(direction.X,  direction.Y).RotatedByRandom(MathHelper.ToRadians(1.5f));
                float speed = 20f;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Items/Hork"), projectile.position);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, mod.ProjectileType("RottingEgg"), projectile.damage + 5, 1f, projectile.owner, 0f);
			    timer = 0;
			}

            if (Main.player[projectile.owner].Center.X > projectile.Center.X)
            {
                projectile.spriteDirection = -1;
            }
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.height = 80;
            projectile.width = 80;

            projectile.minion = true;
            projectile.minionSlots = 1.5f;

            projectile.height = 37;




            timer3++;
            if(timer3 == 1)
            {
                savedDMG = projectile.damage;
            }

            if(penet > 0)
            {
			    projectile.damage = 0;
                penet++;
            }
            
            if(projectile.damage == 0)
            {
                penet2++;
            }

            if(penet2 > 34)
            {
                penet = 0;
                penet2 = 0;
                projectile.damage = savedDMG;
            }
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            penet++;
            projectile.velocity.X *= 4;
            projectile.velocity.Y *= 4;
        }
    }
}