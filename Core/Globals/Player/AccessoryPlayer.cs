using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class AccessoryPlayer : ModPlayer
    {
        /// <summary>
        /// Used for calculating whether the player is in the NPC's aggro range.
        /// Higher values reduce the NPC's aggro range while increasing their alert threshold.
        /// </summary>
        public int AlertBonus = 0;

        /// <summary>
        /// Used for calculating how quickly the enemy loses aggro if the Player is their target.
        /// </summary>
        public float AggroLossBonus = 0;

        public bool CandlelitSanctuary = false;
        public int CandleCharges { get; private set; } = 0;
        private int CandleCounter = 0;
        public override void ResetEffects()
        {
            AlertBonus = 0;
            AggroLossBonus = 0;

            CandlelitSanctuary = false;
        }

        public override void PostUpdate()
        {
            if (CandleCharges < 3)
            {
                CandleCounter++;
                if (CandleCounter % ModUtils.SecondsToTicks(5) == 0)
                {
                    CandleCharges++;
                    CandleCounter = 0;
                    Main.NewText(CandleCharges);
                }
            }


            base.PostUpdate();
        }


        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            CandleCounter = 0;
            if (CandleCharges > 0)
            {
                var damageReduction = 15 * CandleCharges;
                hurtInfo.Damage -= damageReduction;
                CandleCharges = 0;

                var item = ItemLoader.GetItem(ModContent.ItemType<CandlelitSanctuary>()).Item;
                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(item, hurtInfo.DamageSource), Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleBurst>(), damageReduction, 6f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            CandleCounter = 0;
            if (CandleCharges > 0)
            {
                var damageReduction = 15 * CandleCharges;
                hurtInfo.Damage -= damageReduction;
                CandleCharges = 0;

                var item = ItemLoader.GetItem(ModContent.ItemType<CandlelitSanctuary>()).Item;
                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(item, hurtInfo.DamageSource), Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleBurst>(), damageReduction, 6f, Player.whoAmI);
            }
        }
    }
}