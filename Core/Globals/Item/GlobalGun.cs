using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalGun : GlobalItem
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// Maps vanilla gun items to their corresponding HeldGun projectile types.
        /// </summary>
        public static Dictionary<int, int> VanillaGunReplacements = new Dictionary<int, int>()
        {
            { ItemID.Boomstick, ModContent.ProjectileType<BoomstickHeld>() },
            { ItemID.Handgun, ModContent.ProjectileType<HandgunHeld>() },
            { ItemID.Minishark, ModContent.ProjectileType<MinisharkHeld>() },
            { ItemID.Revolver, ModContent.ProjectileType<RevolverHeld>() },
            { ItemID.QuadBarrelShotgun, ModContent.ProjectileType<QuadBarrelHeld>() },
            { ItemID.TheUndertaker, ModContent.ProjectileType<UndertakerHeld>() },
            { ItemID.PhoenixBlaster, ModContent.ProjectileType<PhoenixBlasterHeld>() },
            { ItemID.Musket, ModContent.ProjectileType<MusketHeld>() }
        };

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (VanillaGunReplacements.ContainsKey(item.type))
            {
                string tooltipKey = item.type switch
                {
                    ItemID.PhoenixBlaster => "PhoenixBlaster",
                    ItemID.TheUndertaker => "TheUndertaker",
                    ItemID.QuadBarrelShotgun => "QuadBarrelShotgun",
                    _ => item.Name
                };

                // Only add tooltip if the localization key exists
                var localizationKey = LocalizationPath.Items + tooltipKey + ".Tooltip";
                if (Language.Exists(localizationKey))
                {
                    tooltips.Add(new TooltipLine(Mod, $"{tooltipKey}0", Language.GetTextValue(localizationKey)));
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override void SetDefaults(Item item)
        {
            if (VanillaGunReplacements.ContainsKey(item.type))
            {
                int projectileType = VanillaGunReplacements[item.type];
                item.shoot = projectileType;
                item.noUseGraphic = true;
                item.UseSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw") { Volume = 0f }; // Silent sound

                // Apply item-specific tweaks
                ApplyItemSpecificTweaks(item);
            }
        }

        /// <summary>
        /// Apply specific tweaks to individual vanilla guns that can't be handled by the stats system.
        /// </summary>
        private void ApplyItemSpecificTweaks(Item item)
        {
            switch (item.type)
            {
                case ItemID.Handgun:
                    item.useTime = item.useAnimation = 18;
                    break;

                case ItemID.Minishark:
                    item.crit = 1;
                    item.useTime = item.useAnimation = 6;
                    break;
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            // Automatically spawn the held projectile when the item is held
            if (ModContent.GetModProjectile(item.shoot) is HeldGun && player.ownedProjectileCounts[player.HeldItem.shoot] < 1)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, item.shoot, item.damage, item.knockBack, player.whoAmI);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            // Do not consume ammo, the gun handles the logic manually
            if (VanillaGunReplacements.ContainsKey(weapon.type))
            {
                return false;
            }

            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            // Prevent using the item if the held gun projectile already exists
            if (VanillaGunReplacements.ContainsKey(item.type))
            {
                int projectileType = VanillaGunReplacements[item.type];
                return player.ownedProjectileCounts[projectileType] <= 0;
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Redirect the projectile type to custom HeldGun
            if (VanillaGunReplacements.ContainsKey(item.type))
            {
                type = VanillaGunReplacements[item.type];
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Handle shooting for replaced vanilla guns
            if (VanillaGunReplacements.ContainsKey(item.type))
            {
                int projectileType = VanillaGunReplacements[item.type];
                Projectile.NewProjectile(null, position, velocity, projectileType, damage, knockback, player.whoAmI);
                return false; // Prevent vanilla shooting
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        /// <summary>
        /// Helper method to check if an item is a replaced vanilla gun.
        /// </summary>
        public static bool IsReplacedVanillaGun(int itemType)
        {
            return VanillaGunReplacements.ContainsKey(itemType);
        }

        /// <summary>
        /// Helper method to get the HeldGun projectile type for a vanilla gun.
        /// </summary>
        public static int GetHeldGunProjectileType(int itemType)
        {
            return VanillaGunReplacements.TryGetValue(itemType, out int projectileType) ? projectileType : -1;
        }
    }
}