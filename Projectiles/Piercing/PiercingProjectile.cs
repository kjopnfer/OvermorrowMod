using Microsoft.Xna.Framework;
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
            // Makes dust based on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            projectile.ai[0] = 1f;

            projectile.netUpdate = true;

            Main.PlaySound(SoundID.Dig, projectile.position); // Plays impact sound

            return false; // Prevents projectile from disappearing on contact
        }

        protected void SoulGain(NPC target, int randCeiling)
        {
            // Get the projectile owner
            Player player = Main.player[projectile.owner];

            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (Main.rand.Next(0, randCeiling) == 0 && (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2) && target.type != NPCID.TargetDummy)
            {
                modPlayer.soulResourceCurrent++; // Increase number of resource

                // Add the projectile to the WardenDamagePlayer list of projectiles
                modPlayer.soulList.Add(Projectile.NewProjectile(projectile.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, projectile.owner, Main.rand.Next(70, 95), 0f));
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
