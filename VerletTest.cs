using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Verlet;
using Microsoft.Xna.Framework;

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
        public override string Texture => "OvermorrowMod/Assets/Textures/Perlin";
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
        public override void AI()
        {
            if (NPC.ai[0] == 0)
            {
                points = new VerletPoint[3];
                points[0] = new VerletPoint(new Vector2(0, 0), new Vector2(NPC.Center.X + 100, NPC.Center.Y + 25), null, true);
                points[1] = new VerletPoint(new Vector2(0, 0), new Vector2(NPC.Center.X + 0, NPC.Center.Y + 25), null, false);
                points[2] = new VerletPoint(new Vector2(0, 0), new Vector2(NPC.Center.X - 100, NPC.Center.Y + 25),null, false);
                points[0].connections = new VerletPoint[1];
                points[0].connections[0] = points[1];                
                points[1].connections = new VerletPoint[1];
                points[1].connections[0] = points[2];
            }
            points = CalculateVerlet(points,new Vector2(0,1),10);
            NPC.ai[0]++;
            foreach(VerletPoint point in points)
            {
                Dust.NewDust(point.position, 0, 0, DustID.GemEmerald, 0f, 0f, 100, default(Color), 0.25f);
            }
        }



     
    }
}