using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.Buffs.Hexes;
using OvermorrowMod.Projectiles.Piercing;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass
{
    public abstract class PiercingProjectile : ModProjectile
    {
        // Use these to modify immunity frames, debuffs and other shit
        private Dictionary<string, int> ChainType = new Dictionary<string, int>()
        {
            { "Sky", ModContent.ProjectileType<LightningPiercerProjectile>() },
            { "Corruption", ModContent.ProjectileType<VilePiercerProjectile>() },
            { "Crimson", ModContent.ProjectileType<CrimsonPiercerProjectile>() },
            { "Bone", ModContent.ProjectileType<BonePiercerProjectile>() },
            { "Hell", ModContent.ProjectileType<BlazePiercerProjectile>() },
            { "Jungle", ModContent.ProjectileType<VinePiercerProjectile>() },
            { "Mushroom", ModContent.ProjectileType<FungiPiercerProjectile>() }
        };

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

        // Calculate how much you gain per hit
        protected void SoulGain(NPC target, int defaultCeiling)
        {
            // Get the projectile owner
            Player player = Main.player[projectile.owner];

            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2)
            {
                // Gain souls between the default and the default + bonus
                modPlayer.soulPercentage += Main.rand.Next(defaultCeiling, defaultCeiling + modPlayer.soulGainBonus);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(Main.player[projectile.owner]);

            // Accessories
            if (modPlayer.FrostburnRune)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(BuffID.Frostburn, 240);
                }
            }

            if (modPlayer.PoisonRune)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(BuffID.Poisoned, 240);
                }
            }

            if (modPlayer.FireRune)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(BuffID.OnFire, 240);
                }
            }

            // WIP code for adding buffs to NPCs to override the buff limit
            // If this works, adding stacks to buffs would be very cool
            //target.GetGlobalNPC<OvermorrowGlobalNPC>().AddNewBuff(target, ModContent.BuffType<Bleeding2>(), 240);


            // Armor
            if (modPlayer.HemoArmor)
            {
                int randChance = Main.rand.Next(3);
                if (randChance == 0)
                {
                    target.AddBuff(ModContent.BuffType<Bleeding2>(), 240);
                }
            }

            target.immune[projectile.owner] = 8;

            // Held item sends back the Soul Gain Rate back to the modPlayer
            // Retrieve the values from the modPlayer as the gain rate
            SoulGain(target, modPlayer.heldGainPercentage);

            /*if (projectile.type == ChainType["Sky"] || projectile.type == ChainType["Corruption"] || projectile.type == ChainType["Crimson"])
            {
                target.immune[projectile.owner] = 4;
            }*/

            if (projectile.type == ChainType["Bone"] || projectile.type == ChainType["Hell"])
            {
                if (projectile.type == ChainType["Hell"])
                {
                    if (!projectile.wet) // Check if projectile is not in water
                    {
                        if (Main.rand.Next(0, 3) == 0) // 33% chance
                        {
                            target.AddBuff(BuffID.OnFire, 300); // Fire Debuff
                        }
                    }
                }

                if (projectile.type == ChainType["Bone"]) 
                {
                    target.AddHex(Hex.HexType<SoulFlame>(), 600);
                }
            }

            if (projectile.type == ChainType["Jungle"])
            {
                if (Main.rand.Next(0, 5) == 0) // 20% chance
                {
                    target.AddBuff(BuffID.Poisoned, 240); // Poison Debuff
                }
                //target.immune[projectile.owner] = 4;
            }

            if (projectile.type == ChainType["Mushroom"])
            {
                if (Main.rand.Next(0, 5) == 0) // 20% chance
                {
                    target.AddBuff(ModContent.BuffType<FungalInfection>(), 400);
                }
                //target.immune[projectile.owner] = 4;
            }
        }
    }
}