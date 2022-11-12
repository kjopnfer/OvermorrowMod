using OvermorrowMod.Common.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class AITest : ModNPC
    {
        private WalkPathFinder _pf = new WalkPathFinder(2, 2, 400, 100);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lil' Cubey");
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = 0;
            NPC.defense = 30;
            NPC.damage = 0;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.type == Type && npc != NPC)
                {
                    npc.life = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return _pf.State == AIState.Falling;
        }

        public override void AI()
        {
            _pf.SetTarget(Main.CurrentPlayer.position);
            var vel = _pf.GetVelocity(NPC.position, NPC.velocity);
            NPC.velocity = vel;
        }
    }
}
