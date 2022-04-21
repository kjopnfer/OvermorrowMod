using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.Requirements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    [AutoloadBossHead]
    public class QuestMarker : ModNPC
    {
        public override bool CheckActive() => false;
        public override void SetDefaults()
        {
            npc.width = npc.height = 66;
            npc.lifeMax = 100;
            npc.dontTakeDamage = true;
            npc.friendly = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public ref float AICounter => ref npc.ai[0];
        public override void AI()
        {
            if (AICounter++ % 30 == 0)
            {
                Particle.CreateParticle(Particle.ParticleType<Pulse2>(), npc.Center, Vector2.Zero, Color.Yellow, 1, 0.55f, 0, 0, 480);
            }

            foreach (Player player in Main.player)
            {
                if (!player.active || npc.Distance(player.Center) > npc.width) continue;

                var modPlayer = player.GetModPlayer<QuestPlayer>();
                foreach (var quest in modPlayer.CurrentQuests)
                {
                    if (quest.QuestName != "Travel") continue;

                    npc.Kill();                    
                }
            }
        }

        public override void NPCLoot()
        {
            foreach (Player player in Main.player)
            {
                if (!player.active && npc.Distance(player.Center) > 100) continue;

                var modPlayer = player.GetModPlayer<QuestPlayer>();
                foreach (var quest in modPlayer.CurrentQuests)
                {
                    if (quest.QuestName != "Travel") continue;

                    foreach (IQuestRequirement requirement in quest.Requirements)
                    {
                        if (requirement is TravelRequirement travelRequirement)
                        {
                            travelRequirement.completed = true;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) => false;
    }
}