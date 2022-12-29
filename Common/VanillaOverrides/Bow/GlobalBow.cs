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

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.DemonBow:
                    item.shoot = ModContent.ProjectileType<TestBow_Held>();
                    item.noUseGraphic = true;
                    break;
            }
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            switch (weapon.type)
            {
                case ItemID.DemonBow:
                    return false;
            }

            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.DemonBow:
                    return player.ownedProjectileCounts[item.shoot] <= 0;
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            switch (item.type)
            {
                case ItemID.DemonBow:
                    type = item.shoot;
                    break;
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
