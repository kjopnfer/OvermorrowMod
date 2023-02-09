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
            { ItemID.WoodenBow, new BowStats(ModContent.ProjectileType<WoodenBow_Held>(), 14) },
            { ItemID.BorealWoodBow, new BowStats(ModContent.ProjectileType<BorealWoodBow_Held>(), 16) },
            { ItemID.RichMahoganyBow, new BowStats(ModContent.ProjectileType<RichMahoganyBow_Held>(), 16) },
            { ItemID.PalmWoodBow, new BowStats(ModContent.ProjectileType<PalmWoodBow_Held>(), 16) },
            { ItemID.EbonwoodBow, new BowStats(ModContent.ProjectileType<EbonwoodBow_Held>(), 18) },
            { ItemID.ShadewoodBow, new BowStats(ModContent.ProjectileType<ShadewoodBow_Held>(), 18) },
            { ItemID.CopperBow, new BowStats(ModContent.ProjectileType<CopperBow_Held>(), 16) },
            { ItemID.TinBow, new BowStats(ModContent.ProjectileType<TinBow_Held>(), 17) },
            { ItemID.IronBow, new BowStats(ModContent.ProjectileType<IronBow_Held>(), 18) },
            { ItemID.LeadBow, new BowStats(ModContent.ProjectileType<LeadBow_Held>(), 19) },
            { ItemID.SilverBow, new BowStats(ModContent.ProjectileType<SilverBow_Held>(), 19) },
            { ItemID.TungstenBow, new BowStats(ModContent.ProjectileType<TungstenBow_Held>(), 20) },
            { ItemID.GoldBow, new BowStats(ModContent.ProjectileType<GoldBow_Held>(), 21) },
            { ItemID.PlatinumBow, new BowStats(ModContent.ProjectileType<PlatinumBow_Held>(), 23) },
            { ItemID.DemonBow, new BowStats(ModContent.ProjectileType<DemonBow_Held>(), 24) },
            { ItemID.TendonBow, new BowStats(ModContent.ProjectileType<TendonBow_Held>(), 29) },
            { ItemID.MoltenFury, new BowStats(ModContent.ProjectileType<MoltenFury_Held>(), 51) },
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
