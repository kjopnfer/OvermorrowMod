using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class BowStats
    {
        public int BowType;
        public int BowDamage;

        public BowStats(int BowType, int BowDamage)
        {
            this.BowType = BowType;
            this.BowDamage = BowDamage;
        }
    }

    public class GlobalBow : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public Dictionary<int, BowStats> OverridedBows = new Dictionary<int, BowStats>()
        {
            { ItemID.WoodenBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 4) },
            { ItemID.BorealWoodBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 6) },
            { ItemID.RichMahoganyBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 6) },
            { ItemID.PalmWoodBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 6) },
            { ItemID.EbonwoodBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 8) },
            { ItemID.ShadewoodBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 8) },
            { ItemID.CopperBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 6) },
            { ItemID.TinBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 7) },
            { ItemID.IronBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 8) },
            { ItemID.LeadBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 9) },
            { ItemID.SilverBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 9) },
            { ItemID.TungstenBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 10) },
            { ItemID.GoldBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 11) },
            { ItemID.PlatinumBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 13) },
            { ItemID.DemonBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 14) },
            { ItemID.TendonBow, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 19) },
            { ItemID.MoltenFury, new BowStats(ModContent.ProjectileType<TestBow_Held>(), 31) },
        };

        public override void SetDefaults(Item item)
        {
            if (OverridedBows.ContainsKey(item.type))
            {
                BowStats bow = OverridedBows[item.type];
                item.shoot = bow.BowType;
                item.damage = bow.BowDamage;
                item.noUseGraphic = true;
            }
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
        }
    }
}
