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
            DisplayName.SetDefault("A");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 32;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            npc.dontCountMe = true;
            npc.chaseable = false;
        }

        public override void AI()
        {
            base.AI();

            if (npc.ai[0]++ == 60)
            {

                npc.velocity = Main.rand.NextBool() ? Vector2.UnitX : -Vector2.UnitX;

            }

            if (Grappled)
            {
                if (RunOnce)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)(npc.Center.Y - 75), ModContent.NPCType<MovingBlock>());
                    RunOnce = false;
                }
            }
        }
    }

    public class MovingBlock2 : CollideableNPC
    {
        bool RunOnce = true;
        public override string Texture => "OvermorrowMod/Common/NPCs/MovingBlock";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("A2");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 32;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            npc.dontCountMe = true;
            npc.chaseable = false;
        }

        public override void AI()
        {
            base.AI();

            if (npc.ai[0]++ == 120)
            {
                npc.velocity = new Vector2(1, -1) * 6;
            }

            npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(2f));
        }
    }
}
