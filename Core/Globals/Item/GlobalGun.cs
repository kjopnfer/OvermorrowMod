using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged;
using OvermorrowMod.Common;
using Terraria.Localization;

namespace OvermorrowMod.Core.Globals
{

    // TODO: Generalize this to not only be guns
    public class GlobalGun : GlobalItem
    {
        public override bool InstancePerEntity => true;

        /*public GunType GunType = GunType.None;

        public Dictionary<int, GunStats> OverridedGuns = new Dictionary<int, GunStats>()
        {
            { ItemID.Boomstick, new GunStats(ModContent.ProjectileType<BoomstickHeld>(), 20, GunType.Shotgun) },
            { ItemID.Handgun, new GunStats(ModContent.ProjectileType<HandgunHeld>(), 22, GunType.Handgun) },
            { ItemID.Minishark, new GunStats(ModContent.ProjectileType<MinisharkHeld>(), 15, GunType.MachineGun) },
            { ItemID.Revolver, new GunStats(ModContent.ProjectileType<RevolverHeld>(), 30, GunType.Revolver) },
            { ItemID.QuadBarrelShotgun, new GunStats(ModContent.ProjectileType<QuadBarrelHeld>(), 20, GunType.Shotgun) },
            { ItemID.TheUndertaker, new GunStats(ModContent.ProjectileType<UndertakerHeld>(), 13, GunType.Revolver) },
            { ItemID.PhoenixBlaster, new GunStats(ModContent.ProjectileType<PhoenixBlasterHeld>(), 32, GunType.Handgun) },
            { ItemID.Musket, new GunStats(ModContent.ProjectileType<MusketHeld>(), 56, GunType.Musket) }
        };

        private string ConvertWeaponTypeString(GunType weaponType)
        {
            switch (weaponType)
            {
                case GunType.MachineGun:
                    return "Machine Gun";
                case GunType.SubMachineGun:
                    return "Submachine Gun";
                default:
                    return weaponType.ToString();
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(tip => tip.Name.StartsWith("ItemName"));

            if (OverridedGuns.ContainsKey(item.type))
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
                    //tooltips.Add(new TooltipLine(Mod, $"{tooltipKey}0", "bruh"));
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override void SetDefaults(Item item)
        {
            if (OverridedGuns.ContainsKey(item.type))
            {
                GunStats gun = OverridedGuns[item.type];
                item.shoot = gun.GunType;
                item.damage = gun.GunDamage;
                item.noUseGraphic = true;
                item.UseSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw") { Volume = 0f }; // just a random sound set to 0

                GunType = gun.WeaponType;
            }

            if (item.type == ItemID.Handgun) item.useTime = item.useAnimation = 18;
            if (item.type == ItemID.Minishark)
            {
                item.crit = 1;
                item.useTime = item.useAnimation = 6;
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            if (ModContent.GetModProjectile(item.shoot) is HeldGun && player.ownedProjectileCounts[player.HeldItem.shoot] < 1)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, item.shoot, item.damage, item.knockBack, player.whoAmI);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (OverridedGuns.ContainsKey(weapon.type))
            {
                return false;
            }

            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (OverridedGuns.ContainsKey(item.type))
            {
                return player.ownedProjectileCounts[item.shoot] <= 0;
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (OverridedGuns.ContainsKey(item.type))
            {
                type = item.shoot;
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (OverridedGuns.ContainsKey(item.type))
            {
                Projectile.NewProjectile(null, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }*/
    }
}