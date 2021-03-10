using IL.Terraria.GameContent.Biomes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Graphics.Shaders;
using OvermorrowMod.UI;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod;

namespace WardenClass
{
    // This class stores necessary player info for our custom damage class, such as damage multipliers, additions to knockback and crit, and our custom resource that governs the usage of the weapons of this damage class.
    public class WardenDamagePlayer : ModPlayer
    {
        public bool UIToggled = false;
        public bool AncientCrystal;
        public bool ObsidianShackle;
        public bool ReaperBook;
        public bool SoulRing;

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

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (AncientCrystal)
            {
                soulResourceMax2 += 1;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (SoulRing)
            {
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 0.5 life gained per 2 Souls per second.
                player.lifeRegen += SoulRingCalculation(soulResourceCurrent);
            }
        }

        private int SoulRingCalculation(int soulInput)
        {
            if(soulInput % 2 == 0)
            {
                return soulInput / 2;
            }
            else
            {
                return (int)(((float)soulInput / 2) + 0.5f);
            }
        }

        public override void Initialize()
        {
            soulResourceCurrent = 0;
            soulResourceMax = DefaultSoulResourceMax;
        }

        public override void ResetEffects()
        {
            UIToggled = false;
            soulResourceMax2 = soulResourceMax;
            AncientCrystal = false;
            ObsidianShackle = false;
            ReaperBook = false;
            SoulRing = false;

            //ResetVariables();
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if(UIToggled)
            {
                ModContent.GetInstance<OvermorrowModFile>().ShowMyUI();
            }
            else
            {
                ModContent.GetInstance<OvermorrowModFile>().HideMyUI();
            }
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

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            packet.Send(toWho, fromWho);
        }

        public void AddSoul(int soulEssence)
        {
            if (Main.gameMenu)
            {
                return;
            }

            soulResourceCurrent += soulEssence;
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 50, player.width, player.height), Color.DarkCyan, "Soul Essence Gained", true, false);
        }

        public float modifyShootSpeed()
        {
            int modifierFactor = 0;

            if (ObsidianShackle)
            {
                modifierFactor += 6;
            }

            return modifierFactor;
        }
    }
}