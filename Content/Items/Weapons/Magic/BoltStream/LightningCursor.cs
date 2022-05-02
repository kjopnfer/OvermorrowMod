using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BoltStream
{
    public class LightningCursor : ModProjectile, ITrailEntity
    {
        int DustRadius = 15;
        public Color TrailColor(float progress) => Color.Cyan;
        public float TrailSize(float progress) => 20;
        public Type TrailType() => typeof(LightningTrail);
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Cursor");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.LocalPlayer != player) return;

            Lighting.AddLight(Projectile.Center, 0f, 0f, 1f);

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel)
                {
                    // This enables the lightning arc shoot code for the next run
                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[0] = 1;
                    }

                    Projectile.localAI[0] += 10;
                    Vector2 Target = Projectile.Center - Main.MouseWorld;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Target.SafeNormalize(Vector2.UnitX) * -12, 0.1f);

                    if (DustRadius < 200) DustRadius += 5;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && Projectile.Hitbox.Intersects(npc.Hitbox)/*Projectile.Distance(npc.Center) < DustRadius*/)
                        {
                            npc.AddBuff(ModContent.BuffType<LightningMarked>(), 1200);
                        }
                    }

                    Projectile.timeLeft = 2;
                }
                else
                {
                    // This fires the lightning arc once
                    if (Projectile.ai[0] == 1)
                    {
                        Projectile.ai[0] = 0;

                        // Find nearest NPC that is marked
                        int StartingRadius = 0;
                        bool FoundTarget = false;
                        while (!FoundTarget)
                        {
                            if (StartingRadius >= 600) break;

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (npc.active && npc.HasBuff(ModContent.BuffType<LightningMarked>()) && Main.player[Projectile.owner].Distance(npc.Center) < StartingRadius)
                                {
                                    // Insert targeted npc in ai0, if jumping from npcs, input starting npc as ai0 and ending npc as ai1
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightningArc>(), Projectile.damage, 6f, Projectile.owner, npc.whoAmI, -1);
                                    FoundTarget = true;
                                    break;
                                }
                            }

                            StartingRadius += 15;
                        }
                    }

                    Projectile.Kill();
                }
            }
        }
    }
}