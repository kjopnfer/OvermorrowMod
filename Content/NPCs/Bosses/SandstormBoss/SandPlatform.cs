using Microsoft.Xna.Framework;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class TestPlatform : MovingPlatform
    {
        public override string Texture => AssetDirectory.Boss + "SandstormBoss/SandPlatform";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AAH");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = 100;
            npc.height = 16;
            npc.noTileCollide = true;
            npc.dontCountMe = true;
        }

        public ref float AICounter => ref npc.ai[0];
        public ref float MiscCounter => ref npc.ai[1];
        public ref float RotationCounter => ref npc.ai[2];
        public override void AI()
        {
            base.AI();

            npc.velocity = -Vector2.UnitX;
        }
    }

    public class SandPlatform : MovingPlatform
    {
        public Vector2 RotationCenter;
        private bool RunOnce = true;

        public bool Rotate = false;

        private float RotationOffset;
        private float InitialRadius;
        private float Radius;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AA");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = 100;
            npc.height = 16;
            npc.noTileCollide = true;
            npc.dontCountMe = true;
        }

        public ref float AICounter => ref npc.ai[0];
        public ref float MiscCounter => ref npc.ai[1];
        public ref float RotationCounter => ref npc.ai[2];
        public override void AI()
        {
            base.AI();

            // Initialization step to save input into variables
            if (RunOnce)
            {
                RotationCenter = new Vector2(npc.ai[0], npc.ai[1]);
                RotationOffset = npc.ai[2];
                Radius = npc.ai[3];

                RunOnce = false;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
            }

            // Code that runs after the projectiles have spawned in
            if (AICounter++ > 90)
            {
                // Arena projectile tells it when to rotate
                if (Rotate)
                {
                    RotationCounter -= 0.0075f;
                }

                // Counter for the glowmask
                MiscCounter++;
            }

            npc.Center = RotationCenter + new Vector2(Radius, 0).RotatedBy(RotationOffset + RotationCounter);
        }
    }
}
