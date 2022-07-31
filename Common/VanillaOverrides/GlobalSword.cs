using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Projectiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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
        public float HoldOffset;

        /// <summary>
        /// gnome
        /// </summary>
        /// <param name="itemID">The sword that is being overriden</param>
        /// <param name="chargeRate">Number that increments the charge amount for each usage</param>
        /// <param name="maxCharge">Maximum limit that the player can charge</param>
        /// <param name="chargeConsumption">Amount consumed in a right click</param>
        /// <param name="holdOffset"> the hold offset for the sword swing</param>
        /// <param name="chargeProjectile">Projectile fired when charge is consumed</param>
        public SwordOverride(int itemID, float chargeRate, float maxCharge, float chargeConsumption, int chargeProjectile, float holdOffset)
        {
            ItemID = itemID;
            ChargeRate = chargeRate;
            MaxCharge = maxCharge;
            ChargeConsumption = chargeConsumption;
            ChargeProjectile = chargeProjectile;
            HoldOffset = holdOffset;
        }

        public SwordOverride Clone()
        {
            return new SwordOverride(ItemID, ChargeRate, MaxCharge, ChargeConsumption, ProjectileID.EnchantedBeam, HoldOffset);
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

            return new SwordOverride(0, 0, 0, 0, 0, 0);
        }

        public static void LoadSwords()
        {
            SwordsToOverride = new List<SwordOverride>();

            // takes 2 seconds to charge, uses 25% charge to shoot
            SwordsToOverride.Add(new SwordOverride(ItemID.TerraBlade, 5f, 300f, 50f, ProjectileID.TerraBeam, 10f));
            SwordsToOverride.Add(new SwordOverride(ItemID.BeamSword, 1f, 120f, 30f, ProjectileID.EnchantedBeam, 20f));
        }
        public static void UnloadSwords()
        {
            SwordsToOverride = null;
        }

        public override void SetDefaults(Item item)
        {
            if (ShouldOverrideSword(item.type))
            {
                //item.noUseGraphic = true;
                SwordOverride = GetByID(item.type).Clone();
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            if (ShouldOverrideSword(item.type) && player.altFunctionUse == 2)
            {
                if (Main.mouseRight)
                {
                    player.channel = true;
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                    if (SwordOverride.ChargeAmount < SwordOverride.MaxCharge)
                    {
                        SwordOverride.ChargeAmount += SwordOverride.ChargeRate;
                        if (SwordOverride.ChargeAmount > SwordOverride.MaxCharge)
                        {
                            SwordOverride.ChargeAmount = SwordOverride.MaxCharge;
                        }
                    }
                }
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
                item.channel = player.altFunctionUse == 2;
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ShouldOverrideSword(item.type))
            {
                if (player.altFunctionUse == 2)
                {
                    return false;
                }
                if (SwordOverride.ChargeAmount >= SwordOverride.ChargeConsumption)
                {
                    SwordOverride.ChargeAmount -= SwordOverride.ChargeConsumption;
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            /*if (ShouldOverrideSword(item.type))
            {
                float progress = (float)player.itemAnimation / player.itemAnimationMax;
                float offset = player.direction == -1 ? MathHelper.Pi : 0;
                float rot = (float)Math.PI - (float)Math.PI * progress + offset;
                player.itemRotation = rot + offset - MathHelper.PiOver4;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + offset + (float)Math.PI);
                Vector2 pos = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rot + offset);
                Vector2 dir = (player.Center - pos).SafeNormalize(Vector2.Zero);
                // temp value
                player.itemLocation = pos + dir * 50f;
            }*/
        }
    }
}
