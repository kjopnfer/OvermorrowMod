using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns;
using Terraria.DataStructures;
using Terraria.Audio;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class GunStats
    {
        public int GunType;
        public int GunDamage;

        public GunStats(int GunType, int GunDamage)
        {
            this.GunType = GunType;
            this.GunDamage = GunDamage;
        }
    }

    public class GlobalGun : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public Dictionary<int, GunStats> OverridedGuns = new Dictionary<int, GunStats>()
        {
            { ItemID.Boomstick, new GunStats(ModContent.ProjectileType<Boomstick_Held>(), 30) },
            { ItemID.PhoenixBlaster, new GunStats(ModContent.ProjectileType<PhoenixBlaster_Held>(), 32) },
        };

        public override void SetDefaults(Item item)
        {
            if (OverridedGuns.ContainsKey(item.type))
            {
                GunStats gun = OverridedGuns[item.type];
                item.shoot = gun.GunType;
                item.damage = gun.GunDamage;
                item.noUseGraphic = true;
                item.UseSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw") { Volume = 0f }; // just a random sound set to 0
            }
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