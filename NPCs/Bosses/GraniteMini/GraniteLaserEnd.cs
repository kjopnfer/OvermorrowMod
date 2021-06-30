using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GraniteLaserEnd : Deathray
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/ParagonBeam";
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
        public GraniteLaserEnd() : base(10000f, 100f, 0f, Color.White, "OvermorrowMod/NPCs/Bosses/ParagonBeam") { }
        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
            NPC owner = Main.npc[(int)projectile.ai[1]];
            if (!owner.active || owner.type != ModContent.NPCType<AngryStone>())
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = owner.Center + Vector2.UnitY * 92;
            }
        }
    }
}