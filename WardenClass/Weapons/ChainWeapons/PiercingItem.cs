using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Misc;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    // This class handles everything for our custom damage class
    // Any class that we wish to be using our custom damage class will derive from this class, instead of ModItem
    public abstract class PiercingItem : ModItem
    {
        public override bool CloneNewInstances => true;
        public float soulGainChance = 0;

        public override void HoldItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.UIToggled = true;
            modPlayer.heldGainPercentage = soulGainChance + item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance;
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

        // Allows for Dual-Use
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        // As a modder, you could also opt to make these overrides also sealed. Up to the modder
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            //Main.NewText(WardenDamagePlayer.ModPlayer(player).piercingDamageAdd);
            mult *= WardenDamagePlayer.ModPlayer(player).piercingDamageMult;
            flat += WardenDamagePlayer.ModPlayer(player).piercingDamageAdd;
        }

        // Because we want the damage tooltip to show our custom damage, we need to modify it
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(Main.LocalPlayer);

            // Get the vanilla damage tooltip
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
            if (tt != null)
            {
                // We want to grab the last word of the tooltip, which is the translated word for 'damage' (depending on what language the player is using)
                // So we split the string by whitespace, and grab the last word from the returned arrays to get the damage word, and the first to get the damage shown in the tooltip
                string[] splitText = tt.text.Split(' ');
                string damageValue = splitText.First();
                string damageWord = splitText.Last();
                // Change the tooltip text
                tt.text = damageValue + " piercing " + damageWord;
                tooltips.Add(new TooltipLine(mod, "Soul Gain Probability", $"Soul Gain Rate: {soulGainChance} [+{modPlayer.soulGainBonus + item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance}]%"));

                if (item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance > 0)
                {
                    tooltips.Add(new TooltipLine(mod, "Prefix Soul Gain Probability", $"+{item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance}% Soul Gain Chance")
                    {
                        isModifier = true
                    });
                }
            }
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            var prefixChooser = new WeightedRandom<int>();
            prefixChooser.Add(mod.PrefixType("Cursed"), 1);
            prefixChooser.Add(mod.PrefixType("Enchanted"), 1);
            prefixChooser.Add(mod.PrefixType("Faithful"), 1);
            prefixChooser.Add(mod.PrefixType("Bound"), 1);
            int choice = prefixChooser;

            if (item.maxStack == 1)
            {
                return choice;
            }
            return -1;
        }

        protected void ConsumeSouls(int numSouls, Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (modPlayer.soulResourceCurrent >= numSouls)
            {
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
                            // Main.NewText("projectile " + j + " killed");
                            // Kill the projectile
                            Main.projectile[j].Kill();
                            break;
                        }
                    }
                }
            }

            if (modPlayer.WaterArmor)
            {
                Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-9, 9) * 10, player.Center.Y + Main.rand.Next(-9, 9) * 10);
                Projectile.NewProjectile(randPos, Vector2.Zero, ModContent.ProjectileType<WaterOrbSpawner>(), 0, 0f);
            }
        }
    }
}