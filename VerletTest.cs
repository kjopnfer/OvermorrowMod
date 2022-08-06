using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Verlet;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod
{
    // Party Zombie is a pretty basic clone of a vanilla NPC. To learn how to further adapt vanilla NPC behaviors, see https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-projectile-hoplite
    public class VerletTest : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("VerletTest");
            // Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Zombie];
            //Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

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
            //aiType = NPCID.Zombie;
           // animationType = NPCID.Zombie;
           // banner = Item.NPCtoBanner(NPCID.Zombie);
           // bannerItem = Item.BannerToItem(banner);
        }
        public VerletPoint[] points;
        public VerletPoint[] points2;
        public VerletStick[] sticks;
        public VerletStick[] sticks2;
        public override void AI()
        {
           
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] == 0)
            {
                /*points = new VerletPoint[4];
                points[0] = new VerletPoint(new Vector2(NPC.Center.X + 100, NPC.Center.Y - 250), new Vector2(NPC.Center.X + 100, NPC.Center.Y - 250), null, true);
                points[1] = new VerletPoint(new Vector2(NPC.Center.X + 0, NPC.Center.Y - 100), new Vector2(NPC.Center.X + 0, NPC.Center.Y - 100), null, false);
                points[2] = new VerletPoint(new Vector2(NPC.Center.X - 100, NPC.Center.Y - 50), new Vector2(NPC.Center.X - 100, NPC.Center.Y - 50),null, false);
                points[3] = new VerletPoint(new Vector2(NPC.Center.X - 140, NPC.Center.Y - 75), new Vector2(NPC.Center.X - 140, NPC.Center.Y - 75),null, false);
                points[0].connections = new VerletPoint[1];
                points[0].connections[0] = points[1];                
                points[1].connections = new VerletPoint[1];
                points[1].connections[0] = points[2];
                points[3].connections = new VerletPoint[1];
                points[3].connections[0] = points[2];
                */
                points = VerletPointsInLine_Even(NPC.Center, new Vector2(NPC.Center.X - 200, NPC.Center.Y + 250), 10, true, false);
                points2 = VerletPointsInLine_Offset(new Vector2(NPC.Center.X - 300, NPC.Center.Y - 400), new Vector2(NPC.Center.X + 300, NPC.Center.Y - 400), new float[] { 0f, 0.3f, .5f, .7f, 1f }, 5, true, true);
                sticks = GetVerletSticks(points);
                sticks2 = GetVerletSticks(points2);
            }
            points = CalculateVerlet(points, sticks, new Vector2(0, 1), 0.07f, 10, 100f);
            points2 = CalculateVerlet(points2, sticks2, new Vector2(0, 1), 0.07f, 10, 100f);
            NPC.ai[0]++;
            DrawVerletDust(points, DustID.GemEmerald, Color.White, spriteBatch);
            DrawVerletDust(points2, DustID.GemEmerald, Color.Red, spriteBatch);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }



    }
}