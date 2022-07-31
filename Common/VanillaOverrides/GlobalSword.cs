using OvermorrowMod.Content.Projectiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides
{
    public class SwordOverride
    {
        public int ItemID;
        public float ChargeRate;
        public float MaxCharge;
        public float ChargeConsumption;
        public float ChargeAmount;
        public int ChargeProjectile;

        /// <summary>
        /// gnome
        /// </summary>
        /// <param name="itemID">The sword that is being overriden</param>
        /// <param name="chargeRate">Number that increments the charge amount for each usage</param>
        /// <param name="maxCharge">Maximum limit that the player can charge</param>
        /// <param name="chargeConsumption">Amount consumed in a right click</param>
        /// <param name="chargeProjectile">Projectile fired when charge is consumed</param>
        public SwordOverride(int itemID, float chargeRate, float maxCharge, float chargeConsumption, int chargeProjectile)
        {
            ItemID = itemID;
            ChargeRate = chargeRate;
            MaxCharge = maxCharge;
            ChargeConsumption = chargeConsumption;
            ChargeProjectile = chargeProjectile;
        }

        public SwordOverride Clone()
        {
            return new SwordOverride(ItemID, ChargeRate, MaxCharge, ChargeConsumption, ProjectileID.EnchantedBeam);
        }
    }

    public class GlobalSword : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public static List<SwordOverride> SwordsToOverride;
        public SwordOverride SwordOverride;
        public static bool ShouldOverrideSword(int id)
        {
            foreach (SwordOverride swordOverride in SwordsToOverride)
            {
                if (swordOverride.ItemID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static SwordOverride GetByID(int id)
        {
            foreach (SwordOverride swordOverride in SwordsToOverride)
            {
                if (swordOverride.ItemID == id)
                {
                    return swordOverride;
                }
            }

            return new SwordOverride(0, 0, 0, 0, 0);
        }

        public static void LoadSwords()
        {
            SwordsToOverride = new List<SwordOverride>();

            // takes 2 seconds to charge, uses 25% charge to shoot
            SwordsToOverride.Add(new SwordOverride(ItemID.BeamSword, 1f, 120f, 30f, ProjectileID.EnchantedBeam));
        }
        public static void UnloadSwords()
        {
            SwordsToOverride = null;
        }

        public override void SetDefaults(Item item)
        {
            if (ShouldOverrideSword(item.type))
            {
                item.noUseGraphic = true;
                SwordOverride = GetByID(item.type).Clone();
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            if (ShouldOverrideSword(item.type) && player.channel)
            {
                if (SwordOverride.ChargeAmount < SwordOverride.MaxCharge)
                {
                    SwordOverride.ChargeAmount += SwordOverride.ChargeRate;
                    if (SwordOverride.ChargeAmount > SwordOverride.MaxCharge)
                    {
                        SwordOverride.ChargeAmount = SwordOverride.MaxCharge;
                    }
                }
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (ShouldOverrideSword(item.type)) return true;

            return base.AltFunctionUse(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (ShouldOverrideSword(item.type))
            {
                if (player.altFunctionUse == 0)
                {
                    item.channel = false;
                    item.shoot = SwordOverride.ChargeProjectile;
                }
                else
                {
                    item.channel = true;
                    item.shoot = ModContent.ProjectileType<ghostProjectile>();
                }
            }
            return base.UseItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ShouldOverrideSword(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "", $"Charge Rate: {SwordOverride.ChargeRate}"));
                tooltips.Add(new TooltipLine(Mod, "", $"Charge Max: {SwordOverride.MaxCharge}"));
                tooltips.Add(new TooltipLine(Mod, "", $"Charge Use: {SwordOverride.ChargeConsumption}"));
                tooltips.Add(new TooltipLine(Mod, "", $"Charge: {SwordOverride.ChargeAmount}"));
            }

            base.ModifyTooltips(item, tooltips);
        }
        
    }
}
