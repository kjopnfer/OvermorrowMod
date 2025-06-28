using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.DataStructures;
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
        public bool WarriorsEpic = false;
        public bool WhitePage = false;

        public int WarriorsEpicCooldown { get; private set; } = 0;
        public bool WarriorsResolveTriggered { get; private set; } = false;

        public int CandleCharges { get; private set; } = 0;
        private int CandleCounter = 0;
        public override void ResetEffects()
        {
            AlertBonus = 0;
            AggroLossBonus = 0;

            if (!Player.HasBuff<WarriorsResolve>())
                WarriorsResolveTriggered = false;

            CandlelitSanctuary = false;
            WarriorsEpic = false;
            WhitePage = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0)
            {
                if (Player.HasBuff<WarriorsResolve>() && !WarriorsResolveTriggered)
                {
                    Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/trace_05").Value;
                    var aura = new Spark(texture, ModUtils.SecondsToTicks(Main.rand.NextFloat(0.7f, 1f)), false) {
                        endColor = Color.DarkRed,
                        slowModifier = 0.98f,
                        squashHeight = false 
                    };

                    int delay = (int)(Main.rand.NextFloat(0.5f, 0.7f) * 15);
                    if (Main.GameUpdateCount % delay == 0 && !Main.gamePaused)
                    {
                        var widthRange = Player.width + 8;
                        var heightRange = Player.height - 4;

                        int randomIterations = 2;
                        for (int i = 0; i < randomIterations; i++)
                        {
                            float randomScale = Main.rand.NextFloat(0.1f, 0.2f);
                            int stepSize = 2; // Random step size between 4-5
                            int maxXSteps = widthRange / stepSize;
                            int maxYSteps = heightRange / stepSize;

                            Vector2 offset = new Vector2(
                                Main.rand.Next(-maxXSteps, maxXSteps + 1) * stepSize,
                                Main.rand.Next(-maxYSteps, 1) * stepSize
                            );

                            ParticleManager.CreateParticleDirect(aura, Player.Bottom + offset, -Vector2.UnitY * Main.rand.Next(3, 4), Color.Red, 1f, randomScale, 0f, useAdditiveBlending: true);
                        }
                    }
                }
            }
        }

        public override void UpdateEquips()
        {
            VigorEffects();

            if (Player.HasBuff<WarriorsResolve>() && !WarriorsResolveTriggered)
            {
                Player.GetCritChance(DamageClass.Melee) += 100; // 100% crit chance bonus
            }
        }

        public override void PostUpdateBuffs()
        {
            
        }

        public override void PostUpdate()
        {
            DeathsDoorEffects();

            if (WarriorsEpicCooldown > 0) WarriorsEpicCooldown--;

            if (CandlelitSanctuary && CandleCharges < 3)
            {
                CandleCounter++;
                if (CandleCounter % ModUtils.SecondsToTicks(15) == 0)
                {
                    CandleCharges++;
                    CandleCounter = 0;

                    Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleGain>(), 0, 0, Player.whoAmI);
                }
            }
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }

        private void VigorEffects()
        {
            if (Player.statLife < Player.statLifeMax2 * 0.8f) return;

            if (WarriorsEpic)
            {
                Player.GetDamage(DamageClass.Melee) += 0.15f; // 15% damage bonus if at or above 80% health
            }
        }

        private void OnHitEffects(NPC npc)
        {

        }

        private void OnKillEffects(NPC npc)
        {
            if (npc.life > 0) return;

            if (Player.HasBuff<WarriorsResolve>())
            {
                if (!WarriorsResolveTriggered)
                {
                    Player.Heal(40);
                    WarriorsResolveTriggered = true;
                }
            }
        }

        private void DeathsDoorEffects()
        {
            if (Player.statLife > Player.statLifeMax2 * 0.2f) return;

            if (WarriorsEpic && WarriorsEpicCooldown <= 0)
            {
                Player.AddBuff(ModContent.BuffType<WarriorsResolve>(), ModUtils.SecondsToTicks(10));
                WarriorsEpicCooldown = ModUtils.SecondsToTicks(30);
            }
        }
    }
}