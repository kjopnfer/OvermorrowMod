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
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Cursor");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (Main.LocalPlayer != player) return;

            Lighting.AddLight(projectile.Center, 0f, 0f, 1f);

            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel)
                {
                    // This enables the lightning arc shoot code for the next run
                    if (projectile.ai[0] == 0)
                    {
                        projectile.ai[0] = 1;
                    }

                    projectile.localAI[0] += 10;
                    Vector2 Target = projectile.Center - Main.MouseWorld;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Target.SafeNormalize(Vector2.UnitX) * -12, 0.1f);

                    if (DustRadius < 200) DustRadius += 5;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && projectile.Hitbox.Intersects(npc.Hitbox)/*projectile.Distance(npc.Center) < DustRadius*/)
                        {
                            npc.AddBuff(ModContent.BuffType<LightningMarked>(), 1200);
                        }
                    }

                    projectile.timeLeft = 2;
                }
                else
                {
                    // This fires the lightning arc once
                    if (projectile.ai[0] == 1)
                    {
                        projectile.ai[0] = 0;

                        // Find nearest NPC that is marked
                        int StartingRadius = 0;
                        bool FoundTarget = false;
                        while (!FoundTarget)
                        {
                            if (StartingRadius >= 600) break;

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (npc.active && npc.HasBuff(ModContent.BuffType<LightningMarked>()) && Main.player[projectile.owner].Distance(npc.Center) < StartingRadius)
                                {
                                    // Insert targeted npc in ai0, if jumping from npcs, input starting npc as ai0 and ending npc as ai1
                                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightningArc>(), projectile.damage, 6f, projectile.owner, npc.whoAmI, -1);
                                    FoundTarget = true;
                                    break;
                                }
                            }

                            StartingRadius += 15;
                        }
                    }

                    projectile.Kill();
                }
            }
        }
    }
}