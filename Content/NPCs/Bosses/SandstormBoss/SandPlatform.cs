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

            NPC.width = 100;
            NPC.height = 16;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
        }

        public ref float AICounter => ref NPC.ai[0];
        public ref float MiscCounter => ref NPC.ai[1];
        public ref float RotationCounter => ref NPC.ai[2];
        public override void AI()
        {
            base.AI();

            NPC.velocity = -Vector2.UnitX;
        }
    }

    public class SandPlatform : MovingPlatform
    {
        public Vector2 RotationCenter;
        private bool RunOnce = true;

        public bool Rotate = false;

        private float RotationOffset;
        private float Radius;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AA");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 100;
            NPC.height = 16;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
        }

        public ref float AICounter => ref NPC.ai[0];
        public ref float MiscCounter => ref NPC.ai[1];
        public ref float RotationCounter => ref NPC.ai[2];
        public override void AI()
        {
            base.AI();

            // Initialization step to save input into variables
            if (RunOnce)
            {
                RotationCenter = new Vector2(NPC.ai[0], NPC.ai[1]);
                RotationOffset = NPC.ai[2];
                Radius = NPC.ai[3];

                RunOnce = false;
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
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

            NPC.Center = RotationCenter + new Vector2(Radius, 0).RotatedBy(RotationOffset + RotationCounter);
        }
    }
}
