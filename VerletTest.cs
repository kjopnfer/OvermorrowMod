using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Verlet;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod
{
    public class VerletTest : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("VerletTest");
        }
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 14;
            NPC.defense = 6;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
        }

        public VerletPoint[] points;
        public VerletPoint[] points2;
        public VerletStick[] sticks;
        public VerletStick[] sticks2;
        public override void AI()
        {
            Main.NewText(Main.windSpeedCurrent);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;

            if (NPC.ai[0]++ == 0)
            {
                //points = VerletPointsInLine_Even(NPC.Center, new Vector2(NPC.Center.X - 200, NPC.Center.Y + 240), 10, true, false);
                points = GenerateVerlet(texture, NPC.Center, NPC.Center + new Vector2(-200, 240), true, true);
                points2 = GenerateVerlet(texture, new Vector2(NPC.Center.X, NPC.Center.Y - 100), new Vector2(NPC.Center.X + 300, NPC.Center.Y - 400), true, false);
                sticks = GetVerletSticks(points);
                sticks2 = GetVerletSticks(points2);
            }
            points = SimulateVerlet(points, sticks, new Vector2(0, 1), 0.07f, 10, 100f);
            points2 = SimulateVerlet(points2, sticks2, Vector2.UnitY, 0.07f);
            //points2 = CalculateVerlet(points2, sticks2, new Vector2(0, 1), 0.07f, 10, 100f);
            DrawVerlet(points, spriteBatch);
            DrawVerlet(points2, spriteBatch);
            //DrawVerlet(points2, DustID.GemEmerald, Color.Red, spriteBatch);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}