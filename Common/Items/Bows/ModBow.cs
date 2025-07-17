using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Bows
{
    public abstract class ModBow<HeldProjectile> : ModItem where HeldProjectile : HeldBow
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Arrow;

            SafeSetDefaults();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
    }
}