using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace OvermorrowMod.Common.NPCs
{
    public class MovingBlock : CollideableNPC
    {
        bool RunOnce = true;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("A");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
        }

        public override void AI()
        {
            base.AI();

            if (NPC.ai[0]++ == 60)
            {

                NPC.velocity = Main.rand.NextBool() ? Vector2.UnitX : -Vector2.UnitX;

            }

            if (Grappled)
            {
                if (RunOnce)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y - 75), ModContent.NPCType<MovingBlock>());
                    RunOnce = false;
                }
            }
        }
    }

    public class MovingBlock2 : CollideableNPC
    {
        public override string Texture => "OvermorrowMod/Common/NPCs/MovingBlock";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("A2");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
        }

        public override void AI()
        {
            base.AI();

            if (NPC.ai[0]++ == 120)
            {
                NPC.velocity = new Vector2(1, -1) * 6;
            }

            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(2f));
        }
    }
}
