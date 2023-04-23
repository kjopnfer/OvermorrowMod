using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.ObjectModel;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class GunStats
    {
        public int GunType;
        public int GunDamage;
        public GunType WeaponType;

        public GunStats(int GunType, int GunDamage, GunType WeaponType)
        {
            this.GunType = GunType;
            this.GunDamage = GunDamage;
            this.WeaponType = WeaponType;
        }
    }

    public class GlobalGun : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public GunType WeaponType = GunType.None;

        public Dictionary<int, GunStats> OverridedGuns = new Dictionary<int, GunStats>()
        {
            { ItemID.Revolver, new GunStats(ModContent.ProjectileType<Revolver_Held>(), 30, GunType.Revolver) },
            { ItemID.Boomstick, new GunStats(ModContent.ProjectileType<Boomstick_Held>(), 20, GunType.Shotgun) },
            { ItemID.PhoenixBlaster, new GunStats(ModContent.ProjectileType<PhoenixBlaster_Held>(), 32, GunType.Handgun) },
            { ItemID.QuadBarrelShotgun, new GunStats(ModContent.ProjectileType<QuadBarrel_Held>(), 20, GunType.Shotgun) },
            { ItemID.TheUndertaker, new GunStats(ModContent.ProjectileType<Undertaker_Held>(), 13, GunType.Revolver) },
            { ItemID.Handgun, new GunStats(ModContent.ProjectileType<Handgun_Held>(), 22, GunType.Handgun) },
            { ItemID.Minishark, new GunStats(ModContent.ProjectileType<Minishark_Held>(), 15, GunType.MachineGun) },
            { ItemID.Musket, new GunStats(ModContent.ProjectileType<Musket_Held>(), 56, GunType.Musket) },
        };

        private string ConvertWeaponTypeString(GunType weaponType)
        {
            switch (weaponType)
            {
                case GunType.MachineGun:
                    return "Machine Gun";
                case GunType.SubMachineGun:
                    return "Sub-machine Gun";
                default:
                    return weaponType.ToString();
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(tip => tip.Name.StartsWith("ItemName"));
            if (WeaponType.ToString() != "None")
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ItemType", "[c/FAD5A5:" + ConvertWeaponTypeString(WeaponType) + " Type]"));

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

                WeaponType = gun.WeaponType;
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
        }
    }
}