﻿using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    public class LightningPhaseChangeWarning : Deathray
    {
        public float RotateBy;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning";
        public LightningPhaseChangeWarning() : base(120, 3000f, 0f, Color.Blue, "Content/NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
            projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(RotateBy));
        }
        public override void Kill(int timeLeft)
        {
            int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning5>(), projectile.damage, projectile.knockBack, projectile.owner);
            ((TestLightning5)Main.projectile[proj].modProjectile).RotateBy = RotateBy;

        }
    }
}