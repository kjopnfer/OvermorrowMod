using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class GlobalBow : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public Dictionary<int, int> OverridedBows = new Dictionary<int, int>()
        {
            { ItemID.DemonBow, ModContent.ProjectileType<TestBow_Held>() }
        };

        public override void SetDefaults(Item item)
        {
            if (OverridedBows.ContainsKey(item.type))
            {
                item.shoot = OverridedBows[item.type];
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
