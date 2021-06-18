using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace WardenClass
{
    // This class stores necessary player info for our custom damage class, such as damage multipliers, additions to knockback and crit, and our custom resource that governs the usage of the weapons of this damage class.
    public class WardenDamagePlayer : ModPlayer
    {
        public bool UIToggled = false;
        public bool AncientCrystal;
        public bool FireRune;
        public bool FrostburnRune;
        public bool ObsidianShackle;
        public bool PoisonRune;
        public bool ReaperBook;
        public bool SoulRing;

        public bool HemoArmor;
        public bool WaterArmor;
        public bool WaterHelmet;

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

        public int soulGainBonus = 0;

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
            soulList.Clear();
            soulResourceCurrent = 0;
            soulResourceMax = DefaultSoulResourceMax;
        }

        public override void ResetEffects()
        {
            UIToggled = false;
            soulResourceMax2 = soulResourceMax;
            AncientCrystal = false;
            FireRune = false;
            FrostburnRune = false;
            ObsidianShackle = false;
            PoisonRune = false;
            ReaperBook = false;
            SoulRing = false;
          
            HemoArmor = false;
            WaterArmor = false;
            WaterHelmet = false;

            ResetVariables();
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (UIToggled)
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
            soulResourceCurrent = 0;
        }

        // Reset variables to prevent infinite scaling
        private void ResetVariables()
        {
            piercingDamageAdd = 0f;
            piercingDamageMult = 1f;
            piercingKnockback = 0f;
            piercingCrit = 0;

            soulResourceMax2 = soulResourceMax;
            soulGainBonus = 0;
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
                modifierFactor += 4;
            }

            if (WaterHelmet)
            {
                modifierFactor += 6;
            }

            return modifierFactor;
        }
    }
}