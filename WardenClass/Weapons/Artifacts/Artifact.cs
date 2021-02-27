using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public abstract class Artifact : ModItem
    {
        public override bool CloneNewInstances => true;
        public int soulResourceCost = 0;

        public override void HoldItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.UIToggled = true;
        }

        public virtual void SafeSetDefaults()
        {

        }

        // By making the override sealed, we prevent derived classes from further overriding the method and enforcing the use of SafeSetDefaults()
        // We do this to ensure that the vanilla damage types are always set to false, which makes the custom damage type work
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            // all vanilla damage types must be false for custom damage types to work
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            item.thrown = false;
            item.summon = false;
            item.noMelee = true;
        }

        protected void ConsumeSouls(int numSouls, Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            for (int i = 0; i < numSouls; i++)
            {
                // Get the instance of the first projectile in the list
                int removeProjectile = modPlayer.soulList[0];

                // Remove the projectile from the list
                modPlayer.soulList.RemoveAt(0);
                modPlayer.soulResourceCurrent--;

                // Call the projectile's method to kill itself
                for (int j = 0; j < Main.maxProjectiles; j++) // Loop through the projectile array
                {
                    // Check that the projectile is the same as the removed projectile and it is active
                    if (Main.projectile[j] == Main.projectile[removeProjectile] && Main.projectile[j].active)
                    {
                        // Kill the projectile
                        Main.projectile[j].Kill();
                    }
                }
            }
        }
    }
}