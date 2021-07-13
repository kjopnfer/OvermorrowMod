using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LaserBreathWarning : Deathray
    {
        public int direction = 1;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning";
        public LaserBreathWarning() : base(180, /*1000f*/ 750f, 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
        float length = (TRay.Cast(projectile.Center, projectile.velocity, /*2500f*/ 750f) - projectile.Center).Length();
        LaserLength = length;
        projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.25f;
        NPC projectileowner = Main.npc[(int)projectile.ai[1]];
        projectile.position = projectileowner.Center + new Vector2(187 * direction, -51);
        projectile.velocity = Vector2.UnitX * direction;
        }
    }
}