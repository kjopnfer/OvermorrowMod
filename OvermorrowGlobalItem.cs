using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing;
using OvermorrowMod.Projectiles.Ranged;
using OvermorrowMod.WardenClass;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.ChainKnife)
            {
                item.damage = 5;
                item.value = Item.sellPrice(gold: 2, silver: 75);
                item.autoReuse = false;

                item.melee = false;
                item.ranged = false;
                item.magic = false;
                item.thrown = false;
                item.summon = false;
                item.noMelee = true;
            }

            if (item.type == ItemID.GlowingMushroom)
            {
                item.ammo = item.type;
                item.shoot = ModContent.ProjectileType<ShroomFlame>();
            }

            if (item.type == ItemID.Shuriken)
            {
                item.ammo = item.type;
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth)
            {
                flat += 5;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman)
            {
                flat += 3;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                flat += 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().moonBuff)
            {
                flat += 4;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe Mirror = new ModRecipe(mod);
            Mirror.AddIngredient(ItemID.RecallPotion, 7);
            Mirror.SetResult(ItemID.MagicMirror);
            Mirror.AddRecipe();
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.ChainKnife)
            {
                // Get the class info from the player
                var modPlayer = WardenDamagePlayer.ModPlayer(player);

                if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 1 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
                {
                    item.useStyle = ItemUseStyleID.HoldingUp;
                    item.damage = 5;
                    item.shootSpeed = 0f;
                    item.shoot = ProjectileID.None;
                    item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                    PiercingItem.ConsumeSouls(1, player);
                    player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                    player.AddBuff(ModContent.BuffType<ChainRune>(), 60 * 30);
                }
                else
                {
                    item.autoReuse = false;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.useTurn = true;
                    item.shootSpeed = 8f + modPlayer.modifyShootSpeed();
                    item.damage = 5;
                    item.shoot = ModContent.ProjectileType<ChainKnifeProjectile>();
                    item.UseSound = SoundID.Item71;
                }

            }

            return base.CanUseItem(item, player);
        }

        public override void HoldItem(Item item, Player player)
        {
            if (item.type == ItemID.ChainKnife)
            {
                var modPlayer = WardenDamagePlayer.ModPlayer(player);
                modPlayer.UIToggled = true;

                modPlayer.heldGainPercentage = 4 + item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance;
            }

            base.HoldItem(item, player);
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (item.type == ItemID.ChainKnife)
            {
                return true;
            }

            return base.AltFunctionUse(item, player);
        }

        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.type == ItemID.ChainKnife)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<ChainKnifeProjectile>(), damage, 0f, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.ChainKnife)
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
                    tooltips.Add(new TooltipLine(mod, "Chain Knife Imbuement Tooltip", "[c/09DBB8:{ Imbuement }]"));
                    tooltips.Add(new TooltipLine(mod, "Chain Knife Imbuement Tooltip 2", "[c/800080:Right Click] to empower your Warden Artifacts on use"));
                    tooltips.Add(new TooltipLine(mod, "Chain Knife Imbuement Effect", "[c/09DBB8:{ All }] Slows down all projectiles on each Artifact use"));
                    tooltips.Add(new TooltipLine(mod, "Chain Knife Imbuement Effect2", "If clock hands are aligned, also slow down all non-boss enemies for 8 seconds"));
                    tooltips.Add(new TooltipLine(mod, "Chain Knife Imbuement Cost", "Consumes 1 Soul Essence"));

                    tooltips.Add(new TooltipLine(mod, "Soul Gain Probability", $"Soul Gain Rate: {4} [+{modPlayer.soulGainBonus + item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance}]%"));

                    if (item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance > 0)
                    {
                        tooltips.Add(new TooltipLine(mod, "Prefix Soul Gain Probability", $"+{item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance}% Soul Gain Chance")
                        {
                            isModifier = true
                        });
                    }
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (arg == ItemID.KingSlimeBossBag)
            {
                if (Main.rand.NextBool(5))
                {
                    player.QuickSpawnItem(ModContent.ItemType<SlimeArtifact>());
                }
                base.OpenVanillaBag(context, player, arg);
            }
        }
    }
}