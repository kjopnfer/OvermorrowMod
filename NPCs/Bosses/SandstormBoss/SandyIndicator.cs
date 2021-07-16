using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class SandyIndicator : Deathray
    {
        public int waittime;
        public static int wait;

        public float length;
        public static float lengthtrue;

        public SandyIndicator() : base(1 + wait, lengthtrue, 0f, Color.SandyBrown, "NPCs/Bosses/SandstormBoss/SandyIndicator") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            wait = waittime;
            lengthtrue = length;
            LaserLength = lengthtrue;
            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
    }
}