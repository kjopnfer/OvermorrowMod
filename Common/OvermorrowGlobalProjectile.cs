using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Buffs.Debuffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalProjectile : GlobalProjectile
    {
        // Life is pain
        public override bool InstancePerEntity => true;

        private bool spawnedBlood = false;
        public bool slowedTime = false;

        public bool RetractSlow = false;

        private bool WildEyeCrit = false;
        private bool Undertaker = false;
        private int UndertakerCounter = 0;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (WildEyeCrit) crit = true;

            if (Undertaker)
            {
                Main.NewText("wtf");

                float pointBlankBonus = MathHelper.Lerp(1.5f, 0, UndertakerCounter / 15f);
                damage += (int)(damage * pointBlankBonus);
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            /*if (source is EntitySource_ItemUse_WithAmmo { Item:  })
            {
                Main.NewText("this is from a held gun");
            }*/


            if (source != null && source.Context != null)
            {

                if (source is EntitySource_ItemUse_WithAmmo)
                {
                    if (source.Context.ToString() == "HeldGun")
                    {
                        //Main.NewText("this is from a held gun");
                    }

                    if (source.Context.ToString() == "WildEyeCrit") WildEyeCrit = true;
                    else if (source.Context.ToString() == "HeldGun_Undertaker") Undertaker = true; 
                }


            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (slowedTime && !projectile.friendly)
            {
                projectile.position -= projectile.velocity * 0.95f;
            }

            if (Undertaker && UndertakerCounter < 15) UndertakerCounter++;

            return base.PreAI(projectile);
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == 590)
            {
                projectile.timeLeft = 100;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.type == 130 || projectile.type == 131)
            {
                target.AddBuff(ModContent.BuffType<FungalInfection>(), 500);
            }
            if (projectile.type == 30)
            {
                target.immune[projectile.owner] = 0;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (RetractSlow)
            {
                speed = 4;
            }

            base.GrappleRetreatSpeed(projectile, player, ref speed);
        }
    }
}