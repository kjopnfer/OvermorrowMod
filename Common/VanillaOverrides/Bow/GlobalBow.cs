using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class BowStats
    {
        public int BowType;
        public int BowDamage;
        public BowType WeaponType;

        public BowStats(int BowType, int BowDamage, BowType WeaponType)
        {
            this.BowType = BowType;
            this.BowDamage = BowDamage;
            this.WeaponType = WeaponType;
        }
    }

    public class GlobalBow : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public BowType WeaponType = BowType.None;

        public Dictionary<int, BowStats> OverridedBows = new Dictionary<int, BowStats>()
        {
            { ItemID.WoodenBow, new BowStats(ModContent.ProjectileType<WoodenBow_Held>(), 14, BowType.Shortbow) },
            { ItemID.BorealWoodBow, new BowStats(ModContent.ProjectileType<BorealWoodBow_Held>(), 16, BowType.Shortbow) },
            { ItemID.RichMahoganyBow, new BowStats(ModContent.ProjectileType<RichMahoganyBow_Held>(), 16, BowType.Shortbow) },
            { ItemID.PalmWoodBow, new BowStats(ModContent.ProjectileType<PalmWoodBow_Held>(), 16, BowType.Shortbow) },
            { ItemID.EbonwoodBow, new BowStats(ModContent.ProjectileType<EbonwoodBow_Held>(), 18, BowType.Shortbow) },
            { ItemID.ShadewoodBow, new BowStats(ModContent.ProjectileType<ShadewoodBow_Held>(), 18, BowType.Shortbow) },
            { ItemID.CopperBow, new BowStats(ModContent.ProjectileType<CopperBow_Held>(), 16, BowType.Shortbow) },
            { ItemID.TinBow, new BowStats(ModContent.ProjectileType<TinBow_Held>(), 17, BowType.Shortbow) },
            { ItemID.IronBow, new BowStats(ModContent.ProjectileType<IronBow_Held>(), 18, BowType.Shortbow) },
            { ItemID.LeadBow, new BowStats(ModContent.ProjectileType<LeadBow_Held>(), 19, BowType.Shortbow) },
            { ItemID.SilverBow, new BowStats(ModContent.ProjectileType<SilverBow_Held>(), 19, BowType.Shortbow) },
            { ItemID.TungstenBow, new BowStats(ModContent.ProjectileType<TungstenBow_Held>(), 20, BowType.Shortbow) },
            { ItemID.GoldBow, new BowStats(ModContent.ProjectileType<GoldBow_Held>(), 21, BowType.Shortbow) },
            { ItemID.PlatinumBow, new BowStats(ModContent.ProjectileType<PlatinumBow_Held>(), 23, BowType.Shortbow) },
            { ItemID.DemonBow, new BowStats(ModContent.ProjectileType<DemonBow_Held>(), 24, BowType.Shortbow) },
            { ItemID.TendonBow, new BowStats(ModContent.ProjectileType<TendonBow_Held>(), 29, BowType.Shortbow) },
            { ItemID.MoltenFury, new BowStats(ModContent.ProjectileType<MoltenFury_Held>(), 51, BowType.Shortbow) },
        };

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ContentSamples.ProjectilesByType[item.shoot].ModProjectile is HeldBow bow)
            {
                TooltipLine speedLine = tooltips.FirstOrDefault(t => t.Name == "Speed");
                if (speedLine is not null)
                {
                    var chargeTime = (bow.MaxChargeTime / bow.ChargeSpeed) / 60f;
                    var secondType = ((float)bow.MaxChargeTime / 60f == 1f) ? "second" : "seconds";
                    speedLine.Text = $"Takes {chargeTime} {secondType} to fully charge";
                }
            }

            int index = tooltips.FindIndex(tip => tip.Name.StartsWith("ItemName"));
            if (WeaponType.ToString() != "None")
                tooltips.Insert(index + 1, new TooltipLine(Mod, "ItemType", "[c/FAD5A5:" + WeaponType.ToString() + " Type]"));
        }

        public override void SetDefaults(Item item)
        {
            if (OverridedBows.ContainsKey(item.type))
            {
                BowStats bow = OverridedBows[item.type];
                item.shoot = bow.BowType;
                item.damage = bow.BowDamage;
                item.noUseGraphic = true;

                WeaponType = bow.WeaponType;
            }

            // Temporary Classification
            switch (item.type)
            {
                case ItemID.BeesKnees:
                case ItemID.HellwingBow:
                    WeaponType = BowType.Swarmbow;
                    break;
                case ItemID.BloodRainBow:
                case ItemID.DaedalusStormbow:
                    WeaponType = BowType.Stormbow;
                    break;
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            if (ModContent.GetModProjectile(item.shoot) is HeldBow && player.ownedProjectileCounts[player.HeldItem.shoot] < 1)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, item.shoot, item.damage, item.knockBack, player.whoAmI);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (OverridedBows.ContainsKey(weapon.type))
            {
                return false;
            }

            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (OverridedBows.ContainsKey(item.type))
            {
                return player.ownedProjectileCounts[item.shoot] <= 0;
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (OverridedBows.ContainsKey(item.type))
            {
                type = item.shoot;
            }
        }
    }
}
