using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod;
using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
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

        public override void clientClone(ModPlayer clientClone)
        {
            WardenDamagePlayer clone = clientClone as WardenDamagePlayer;
            // Here we would make a backup clone of values that are only correct on the local players Player instance.
            // Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
            clone.soulList = soulList;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            for (int i = 0; i < soulList.Count; i++)
            {
                packet.Write(soulList[i]);
            }
            //packet.Write((byte[])soulList);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            for (int i = 0; i < soulList.Count; i++)
            {
                packet.Write(soulList[i]);
            }
            packet.Send();
        }

        public override TagCompound Save()
        {
            // Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
            return new TagCompound {
				// {"somethingelse", somethingelse}, // To save more data, add additional lines
				{"soulList", soulList},
            };
            //note that C# 6.0 supports indexer initializers
            //return new TagCompound {
            //	["score"] = score
            //};
        }

        public override void Load(TagCompound tag)
        {
            soulList = (List<int>)tag.GetList<int>("soulList");
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
            //Main.NewText(soulPercentage);
            if (soulPercentage >= 100)
            {
                soulPercentage = 0;
                AddSoul(1);
            }

            var chargePlayer = player.GetModPlayer<WardenSoulMeter>();
            chargePlayer.chargeProgress = player.GetModPlayer<WardenDamagePlayer>().soulPercentage;


            // Limit exampleResourceCurrent from going over the limit imposed by exampleResourceMax.
            soulResourceCurrent = Utils.Clamp(soulResourceCurrent, 0, soulResourceMax2);
        }

        public void AddSoul(int soulEssence)
        {
            if (Main.gameMenu)
            {
                return;
            }

            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            int soul = Projectile.NewProjectile(player.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f);
            Main.projectile[soul].active = true;
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, soul);
            //modPlayer.soulList.Add(Projectile.NewProjectile(projectile.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, projectile.owner, Main.rand.Next(70, 95), 0f));
            modPlayer.soulList.Add(soul);

            soulResourceCurrent += soulEssence;
            Color color = new Color(146, 227, 220);
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 50, player.width, player.height), color, "Soul Essence Gained", true, false);

            UpdatePosition(modPlayer);
        }

        private void UpdatePosition(WardenDamagePlayer player)
        {
            int direction = 1;
            for (int i = 0; i < player.soulList.Count; i++)
            {
                if (i % 5 == 4)
                {
                    direction *= -1;
                }

                int radiusBuffer = (int)(20 * System.Math.Floor(i / 4f));
                Main.projectile[player.soulList[i]].knockBack = direction;
                Main.projectile[player.soulList[i]].ai[0] = 70 + radiusBuffer;
                Main.projectile[player.soulList[i]].ai[1] = i * 90;
            }
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