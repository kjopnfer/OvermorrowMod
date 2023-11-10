using OvermorrowMod.Common.Pathfinding;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public class Feyden : ModNPC
    {
        private WalkPathFinder _pf = new WalkPathFinder(SharedAIState.State1x2, new WalkPathFinderProperties
        {
            Acceleration = 1.0f,
            JumpSpeeds = new[] { 7f / 16f, 5.5f / 16f },
            MoveSpeed = 2f / 16f,
            MaxFallDepth = 50,
            NumPermutationSteps = 2,
            Timeout = 300,
            MaxDivergence = 100,
        });

        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool NeedSaving() => true;
        public override bool UsesPartyHat() => false;
        public override bool CheckActive() => false;
        public override bool CanChat() => true;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            /*NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;*/
            NPC.width = 16;
            NPC.height = 32;
            NPC.aiStyle = 7;
            NPC.defense = 30;
            NPC.damage = 0;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            AnimationType = NPCID.Guide;
        }

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.type == Type && npc != NPC) npc.life = 0;
            }
        }

        public Player followPlayer = null;
        private int AICounter = 0;
        public override void AI()
        {
            if (followPlayer != null)
            {
                _pf.SetTarget(followPlayer.position);
                _pf.GetVelocity(ref NPC.position, ref NPC.velocity);

                NPC.aiStyle = 0;
                if (++AICounter % 400 == 0)
                {
                    AICounter = 0;
                    SharedAIState.State1x2.CanFitInTile.Clear();
                    SharedAIState.State1x2.CanStandOnTile.Clear();
                    SharedAIState.State1x2.CanFallThroughTile.Clear();

                }
                return;
            }
            //_pf.SetTarget(Main.CurrentPlayer.position);
            //_pf.GetVelocity(ref NPC.position, ref NPC.velocity);

            NPC.aiStyle = 7;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return _pf.ShouldFallThroughPlatforms(NPC.velocity, NPC.position);
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }
    }
}