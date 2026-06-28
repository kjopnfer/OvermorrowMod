using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged.Bows;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalBow : GlobalItem
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// Maps vanilla bow items to their corresponding HeldBow projectile types.
        /// </summary>
        public static Dictionary<int, int> VanillaBowReplacements = new Dictionary<int, int>()
        {
            { ItemID.WoodenBow, ModContent.ProjectileType<WoodenBow_Held>() },
            { ItemID.CopperBow, ModContent.ProjectileType<CopperBow_Held>() },
            { ItemID.TinBow, ModContent.ProjectileType<TinBow_Held>() },
            { ItemID.IronBow, ModContent.ProjectileType<IronBow_Held>() },
            { ItemID.LeadBow, ModContent.ProjectileType<LeadBow_Held>() },
            { ItemID.SilverBow, ModContent.ProjectileType<SilverBow_Held>() },
            { ItemID.TungstenBow, ModContent.ProjectileType<TungstenBow_Held>() },
            { ItemID.GoldBow, ModContent.ProjectileType<GoldBow_Held>() },
            { ItemID.PlatinumBow, ModContent.ProjectileType<PlatinumBow_Held>() },
            { ItemID.DemonBow, ModContent.ProjectileType<DemonBow_Held>() },
            { ItemID.TendonBow, ModContent.ProjectileType<TendonBow_Held>() },
            { ItemID.BorealWoodBow, ModContent.ProjectileType<BorealWoodBow_Held>() },
            { ItemID.PalmWoodBow, ModContent.ProjectileType<PalmWoodBow_Held>() },
            { ItemID.EbonwoodBow, ModContent.ProjectileType<EbonwoodBow_Held>() },
            { ItemID.ShadewoodBow, ModContent.ProjectileType<ShadewoodBow_Held>() },
            { ItemID.RichMahoganyBow, ModContent.ProjectileType<RichMahoganyBow_Held>() },
            { ItemID.MoltenFury, ModContent.ProjectileType<MoltenFury_Held>() }
        };

        public override void SetDefaults(Item item)
        {
            if (VanillaBowReplacements.TryGetValue(item.type, out int projectileType))
            {
                item.shoot = projectileType;
                item.noUseGraphic = true;
                item.useAmmo = AmmoID.Arrow;
                item.UseSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw") { Volume = 0f };
            }
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (VanillaBowReplacements.ContainsKey(weapon.type))
                return false;

            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (VanillaBowReplacements.TryGetValue(item.type, out int projectileType))
                return player.ownedProjectileCounts[projectileType] <= 0;

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (VanillaBowReplacements.TryGetValue(item.type, out int projectileType))
                type = projectileType;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (VanillaBowReplacements.TryGetValue(item.type, out int projectileType))
            {
                Projectile.NewProjectile(null, position, velocity, projectileType, damage, knockback, player.whoAmI);
                return false;
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public static bool IsReplacedVanillaBow(int itemType) => VanillaBowReplacements.ContainsKey(itemType);
    }
}
