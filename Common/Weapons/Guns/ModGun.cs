using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Items.Guns;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Common.Utilities;

namespace OvermorrowMod.Common.Weapons.Guns
{
    /// <summary>
    /// Base class for all gun items. Automatically connects to a corresponding HeldGun projectile.
    /// </summary>
    /// <typeparam name="HeldProjectile">The HeldGun projectile type associated with this gun</typeparam>
    public abstract class ModGun<HeldProjectile> : ModItem, IWeaponClassification where HeldProjectile : HeldGun
    {
        /// <summary>
        /// The type of weapon this gun represents using the unified classification system.
        /// </summary>
        public abstract WeaponType WeaponType { get; }

        /// <summary>
        /// Checks if the player can use this gun. Prevents using if the held projectile already exists.
        /// </summary>
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0;

        /// <summary>
        /// Prevents consuming ammo with the base gun mechanics, since the HeldGun handles ammo consumption.
        /// </summary>
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        /// <summary>
        /// Override to set up display attributes like name and tooltip.
        /// </summary>
        public override void SetStaticDefaults()
        {
            // Implementations should override this
        }

        /// <summary>
        /// Implement this method to customize the gun's settings.
        /// </summary>
        public virtual void SafeSetDefaults() { }

        /// <summary>
        /// Sets up basic gun defaults and calls SafeSetDefaults for customizations.
        /// </summary>
        public sealed override void SetDefaults()
        {
            Item.SetWeaponType(WeaponType);

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Bullet;

            SafeSetDefaults();
        }

        /// <summary>
        /// Modifies the shooting parameters to use the HeldGun projectile.
        /// </summary>
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
    }
}