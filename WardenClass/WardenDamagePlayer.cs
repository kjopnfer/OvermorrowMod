using Microsoft.Xna.Framework;
using OvermorrowMod;
using OvermorrowMod.Projectiles.Misc;
using OvermorrowMod.WardenClass;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utils = Terraria.Utils;

namespace WardenClass
{
    // This class stores necessary player info for our custom damage class, such as damage multipliers, additions to knockback and crit, and our custom resource that governs the usage of the weapons of this damage class.
    public class WardenDamagePlayer : ModPlayer
    {
        public bool UIToggled = false;
        public bool AncientCrystal;
        public bool FireRune;
        public bool FrostburnRune;
        public bool FungalRune;
        public bool ObsidianShackle;
        public bool PoisonRune;
        public bool ReaperBook;
        public bool SoulRing;

        public bool HemoArmor;
        public bool WaterArmor;
        public bool WaterHelmet;

        public float soulPercentage = 0;
        public float heldGainPercentage = 0;

        public static WardenDamagePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<WardenDamagePlayer>();
        }

        // List to keep track of the resource visuals for Soul Essences
        public List<int> soulList = new List<int>();

        // Additive bonuses for chain weapons
        public int piercingDamageAdd;
        public float piercingDamageMult = 1f;

        public float soulGainBonus = 0;

        // Here we include a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int soulResourceCurrent;
        public const int DefaultSoulResourceMax = 3;
        public int soulResourceMax;
        public int soulResourceMax2;
        public bool soulMeterMax;

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
            if (soulInput % 2 == 0)
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

        public override void Load(TagCompound tag)
        {
            soulList.Clear();
            soulResourceCurrent = 0;
        }

        public override void ResetEffects()
        {
            UIToggled = false;
            soulResourceMax2 = soulResourceMax;
            AncientCrystal = false;
            FireRune = false;
            FrostburnRune = false;
            FungalRune = false;
            ObsidianShackle = false;
            PoisonRune = false;
            ReaperBook = false;
            SoulRing = false;

            HemoArmor = false;
            WaterArmor = false;
            WaterHelmet = false;

            ResetVariables();
        }


        public void AddPercentage(float percent)
        {
            this.soulPercentage += percent;
        }
        public override void clientClone(ModPlayer clientClone)
        {
            WardenDamagePlayer clone = clientClone as WardenDamagePlayer;

            clone.soulPercentage = soulPercentage;
            clone.soulResourceCurrent = soulResourceCurrent;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();

            packet.Write((byte)Message.syncPlayer);
            packet.Write((byte)player.whoAmI);
            packet.Write(soulPercentage);
            packet.Write((byte)soulResourceCurrent);

            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            WardenDamagePlayer clone = clientPlayer as WardenDamagePlayer;
            if (clone.soulResourceCurrent != soulResourceCurrent)
            {
                var packet = mod.GetPacket();

                packet.Write((byte)Message.soulAdded);
                packet.Write((byte)player.whoAmI);
                packet.Write((byte)soulResourceCurrent);

                packet.Send();
            }
            if (clone.soulPercentage != soulPercentage)
            {
                var packet = mod.GetPacket();

                packet.Write((byte)Message.soulsChanged);
                packet.Write((byte)player.whoAmI);
                packet.Write(soulPercentage);

                packet.Send();
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {

        }

        public override void UpdateDead()
        {
            ResetVariables();
            soulResourceCurrent = 0;
            soulPercentage = 0;
        }

        // Reset variables to prevent infinite scaling
        private void ResetVariables()
        {
            piercingDamageAdd = 0;
            piercingDamageMult = 1f;

            soulResourceMax2 = soulResourceMax;
            soulGainBonus = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        private void UpdateResource()
        {
            bool meterMax = soulResourceCurrent == soulResourceMax2;

            if (soulPercentage >= 100 && !soulMeterMax)
            {
                //player.GetModPlayer<WardenSoulMeter>().frame = 0;
                soulPercentage = 100;
                soulMeterMax = true;
            }

            // bool meterMax = soulResourceCurrent == soulResourceMax2;


            // if (meterMax) {
            //     soulPercentage = 100;
            // }

            // var chargePlayer = player.GetModPlayer<WardenSoulMeter>();
            // chargePlayer.chargeProgress = player.GetModPlayer<WardenDamagePlayer>().soulPercentage;


            soulResourceCurrent = Utils.Clamp(soulResourceCurrent, 0, soulResourceMax2);
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

        public void ConsumeSouls(int numSouls, Player player)
        {
            if (soulResourceCurrent >= numSouls)
            {
                for (int i = 0; i < numSouls; i++)
                {
                    // Get the instance of the first projectile in the list
                    int removeProjectile = soulList[0];

                    // Remove the projectile from the list
                    soulList.RemoveAt(0);
                    soulResourceCurrent--;

                    // Call the projectile's method to kill itself
                    for (int j = 0; j < Main.maxProjectiles; j++) // Loop through the projectile array
                    {
                        // Check that the projectile is the same as the removed projectile and it is active
                        if (Main.projectile[j] == Main.projectile[removeProjectile] && Main.projectile[j].active)
                        {
                            // Kill the projectile
                            Main.projectile[j].Kill();
                            break;
                        }
                    }
                }
            }

            // Sullen Binder set bonus
            if (WaterArmor)
            {
                Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-9, 9) * 10, player.Center.Y + Main.rand.Next(-9, 9) * 10);
                Projectile.NewProjectile(randPos, Vector2.Zero, ModContent.ProjectileType<WaterOrbSpawner>(), 0, 0f);
            }

            var packet = mod.GetPacket();

            packet.Write((byte)Message.soulAdded);
            packet.Write((byte)player.whoAmI);
            packet.Write((byte)soulResourceCurrent);

            packet.Send();
        }
    }
}