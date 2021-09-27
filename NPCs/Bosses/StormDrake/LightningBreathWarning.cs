using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LaserBreathWarning : Deathray
    {
        public float RotateBy;
        private float flashTimer = 0;
        public int Direction = 1;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning";
        public LaserBreathWarning() : base(180, 3000f, 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(flashTimer / 5));
            flashTimer++;

            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
            NPC projectileowner = Main.npc[(int)projectile.ai[1]];
            projectile.position = projectileowner.Center + new Vector2(187 * Direction, -50);
            projectile.velocity = (Vector2.UnitX * Direction).RotatedBy(MathHelper.ToRadians((Direction == 1) ? 315 + RotateBy : 45 + -RotateBy));
        }
    }
}