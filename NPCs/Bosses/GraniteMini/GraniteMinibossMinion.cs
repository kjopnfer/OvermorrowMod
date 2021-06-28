using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GraniteMinibossMinion : ModNPC
    {
        public override string Texture => "OvermorrowMod/Projectiles/Summon/GraniteSummon";

        private int moveSpeed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight Squire");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 38;
            npc.damage = 25;
            npc.defense = 15;
            npc.lifeMax = 150;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0.4f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void AI()
        {
            npc.TargetClosest(true);

            if (npc.ai[3] == 0)
            {
                moveSpeed = Main.rand.Next(7, 14);
                npc.ai[3]++;
            }

            Player player = Main.player[npc.target];

            Vector2 moveTo = player.Center;
            var move = moveTo - npc.Center;

            float length = move.Length();
            if (length > moveSpeed)
            {
                move *= moveSpeed / length;
            }
            var turnResistance = 45;
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if (length > 10)
            {
                move *= moveSpeed / length;
            }
            npc.velocity.X = move.X;
            npc.velocity.Y = move.Y * .98f;
        }

        public override void FindFrame(int frameHeight)
        {
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];

            npc.rotation = npc.velocity.X * 0.15f;
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y += num;
            }
            if (npc.frame.Y >= num * Main.npcFrameCount[npc.type])
            {
                npc.frame.Y = 0;
            }
        }
    }
}
