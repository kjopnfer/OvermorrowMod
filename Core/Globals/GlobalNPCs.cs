using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Items.Archives.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalNPCs : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public Player ShadowBrandOwner = null;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                if (npc.buffTime[i] > 0)
                {
                    if (npc.buffType[i] == ModContent.BuffType<ShadowBrand>())
                    {
                        modifiers.FlatBonusDamage += ShadowBrand.TagDamage * projTagMultiplier;
                    }
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (WhitePage.HasStellarCorona(npc))
            {
                float brightnessFactor = 0.4f;

                drawColor = new Color(
                    (int)(drawColor.R * brightnessFactor),
                    (int)(drawColor.G * brightnessFactor),
                    (int)(drawColor.B * brightnessFactor),
                    drawColor.A
                );
            }

            for (int i = 0; i < npc.buffType.Length; i++)
            {
                if (npc.buffTime[i] > 0)
                {
                    if (npc.buffType[i] == ModContent.BuffType<ShadowBrand>())
                    {
                        ShadowBrand.DrawEffects(npc, ref drawColor);
                    }
                }
            }

            
            

            base.DrawEffects(npc, ref drawColor);
        }

        public override void OnKill(NPC npc)
        {
            ShadowBrand.OnKill(npc);
        }
    }
}