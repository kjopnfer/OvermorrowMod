using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.NPCs.Bosses.DripplerBoss;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.Projectiles.Piercing
{
    public abstract class PiercingProjectile : ModProjectile
    {
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            projectile.ai[0] = 1f;

            projectile.netUpdate = true;

            Main.PlaySound(SoundID.Dig, projectile.position); // Plays impact sound

            return false; // Prevents projectile from disappearing on contact
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage += target.defense / 2;
        }

        protected void SoulGain(NPC target, int defaultCeiling)
        {
            // Get the projectile owner
            Player player = Main.player[projectile.owner];

            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            int randChance = Main.rand.Next(0, 100);
            if ((randChance < defaultCeiling + modPlayer.soulGainBonus) && (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2) /* && target.type != NPCID.TargetDummy*/ && target.type != ModContent.NPCType<DripplerBoss>())
            {
                modPlayer.soulResourceCurrent++; // Increase number of resource

                // Add the projectile to the WardenDamagePlayer list of projectiles
                modPlayer.soulList.Add(Projectile.NewProjectile(projectile.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, projectile.owner, Main.rand.Next(70, 95), 0f));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(Main.player[projectile.owner]);
            if (modPlayer.FrostburnRune)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(BuffID.Frostburn, 240);
                }

                /*target.AddBuff(BuffID.Frostburn, 240);
                target.AddBuff(BuffID.OnFire, 240);
                target.AddBuff(BuffID.ShadowFlame, 240);
                target.AddBuff(BuffID.Poisoned, 240);
                target.AddBuff(BuffID.CursedInferno, 240);*/
            }

            // WIP code for adding buffs to NPCs to override the buff limit
            // If this works, adding stacks to buffs would be very cool
            //target.GetGlobalNPC<OvermorrowGlobalNPC>().AddNewBuff(target, ModContent.BuffType<Bleeding2>(), 240);


            if (modPlayer.HemoArmor)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(ModContent.BuffType<Bleeding2>(), 240);
                }
            }

            if (projectile.type == ModContent.ProjectileType<LightningPiercerProjectile>() || projectile.type == ModContent.ProjectileType<VilePiercerProjectile>()
                || projectile.type == ModContent.ProjectileType<CrimsonPiercerProjectile>())
            {
                SoulGain(target, 2);

                target.immune[projectile.owner] = 3;
            }

            if (projectile.type == ModContent.ProjectileType<BonePiercerProjectile>() || projectile.type == ModContent.ProjectileType<BlazePiercerProjectile>())
            {
                SoulGain(target, 3);

                if (projectile.type == ModContent.ProjectileType<BlazePiercerProjectile>())
                {
                    if (!projectile.wet) // Check if projectile is not in water
                    {
                        if (Main.rand.Next(0, 3) == 0) // 33% chance
                        {
                            target.AddBuff(BuffID.OnFire, 300); // Fire Debuff
                        }
                    }
                }

                target.immune[projectile.owner] = 3;
            }

            if (projectile.type == ModContent.ProjectileType<VinePiercerProjectile>())
            {
                SoulGain(target, 4); // 1 in 5 chance

                if (Main.rand.Next(0, 5) == 0) // 20% chance
                {
                    target.AddBuff(BuffID.Poisoned, 240); // Poison Debuff
                }
                target.immune[projectile.owner] = 3;
            }

            if (projectile.type == ModContent.ProjectileType<FungiPiercerProjectile>())
            {
                SoulGain(target, 5);
                target.immune[projectile.owner] = 3;
            }
        }

        /*public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Get the projectile owner
            Player player = Main.player[projectile.owner];

            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (Main.rand.Next(0, 5) == 0 && (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax))
            {
                modPlayer.soulResourceCurrent++; // Increase number of resource

                // Add the projectile to the WardenDamagePlayer list of projectiles
                modPlayer.soulList.Add(Projectile.NewProjectile(projectile.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, projectile.owner, Main.rand.Next(70, 95), 0f));
                //Projectile.NewProjectile(projectile.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, projectile.owner, Main.rand.Next(70, 95), 0f); 
                // Kill projectiles by index when consumed and decrease resource
                // Remove the projectile from the array
                
            }
            target.immune[projectile.owner] = 3;
        }*/
    }
}
