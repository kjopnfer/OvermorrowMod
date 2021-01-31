using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace WardenClass
{
    // This class stores necessary player info for our custom damage class, such as damage multipliers, additions to knockback and crit, and our custom resource that governs the usage of the weapons of this damage class.
    public class WardenDamagePlayer : ModPlayer
    {
        public static WardenDamagePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<WardenDamagePlayer>();
        }

        // List to keep track of the resource visuals for Soul Essences
        public List<int> soulList = new List<int> ();

        // Vanilla only really has damage multipliers in code
        // And crit and knockback is usually just added to
        // As a modder, you could make separate variables for multipliers and simple addition bonuses
        public float piercingDamageAdd;
        public float piercingDamageMult = 1f;
        public float piercingKnockback;
        public int piercingCrit;

        // Here we include a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int soulResourceCurrent;
        public const int DefaultSoulResourceMax = 3;
        public int soulResourceMax;
        public int soulResourceMax2;


        // We can use this for CombatText, if you create an item that replenishes exampleResourceCurrent.
        public static readonly Color GainSoulResource = new Color(187, 91, 201);

        public override void Initialize()
        {
            soulResourceCurrent = 0;
            soulResourceMax = DefaultSoulResourceMax;
        }

        public override void ResetEffects()
        {
            //ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        private void ResetVariables()
        {
            soulResourceCurrent = 0;
            soulResourceMax2 = soulResourceMax;
        }

        /*public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }*/

        private void UpdateResource()
        {
            // Limit exampleResourceCurrent from going over the limit imposed by exampleResourceMax.
            soulResourceCurrent = Utils.Clamp(soulResourceCurrent, 0, soulResourceMax2);
        }
    }
}