using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Buffs.Debuffs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BoltStream
{
    public class LightningArc : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Arc");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }

        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 10f);

            if (Projectile.ai[0] != -1 && Projectile.ai[1] != -1)
            {
                Positions = CreateLightning(Main.npc[(int)Projectile.ai[0]].Center, Main.npc[(int)Projectile.ai[1]].Center, Projectile.width * 2, 80, 16f);
            }
            else if (Projectile.ai[1] == -1)
            {
                Positions = CreateLightning(Main.player[Projectile.owner].Center, Main.npc[(int)Projectile.ai[0]].Center, Projectile.width * 2, 80, 16f);
            }

            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 13; i++)
            {
                Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(2, 5);
                Dust.NewDust(target.Center, 2, 2, DustID.Electric, RandomVelocity.X, RandomVelocity.Y, 0, default, 0.5f);
            }

            // Remove debuff on this target
            int Index = target.FindBuffIndex(ModContent.BuffType<LightningMarked>());
            if (Index != -1)
                target.DelBuff(Index);

            // Find nearest NPC that is marked
            int StartingRadius = 0;
            bool FoundTarget = false;
            while (!FoundTarget)
            {
                if (StartingRadius >= 600)
                {
                    break;
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.HasBuff(ModContent.BuffType<LightningMarked>()) && target.Distance(npc.Center) < StartingRadius)
                    {
                        // Insert targeted npc in ai0, if jumping from npcs, input starting npc as ai0 and ending npc as ai1
                        Projectile.NewProjectile(Projectile.GetProjectileSource_OnHit(target, ProjectileSourceID.None), target.Center, Vector2.Zero, ModContent.ProjectileType<LightningArc>(), Projectile.damage, 6f, Projectile.owner, target.whoAmI, npc.whoAmI);
                        FoundTarget = true;
                        break;
                    }
                }

                StartingRadius += 15;
            }
        }
    }
}