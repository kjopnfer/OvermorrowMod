using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Core;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public partial class Moxley : ModNPC
    {
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool NeedSaving() => true;
        public override bool UsesPartyHat() => false;
        public override bool CheckActive() => false;
        public override bool CanChat() => true;
        public override bool CheckDead() => false;

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
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

            if (!OvermorrowWorld.savedFeyden) AIState = (int)AICase.Idle;
        }

        public enum AICase
        {
            Idle = 0,
            //Default = 0,
            Following = 1,
            Approach = 2,
            Fighting = 3,
            Dodge = 4,
        }

        private int AIState;
        private int AICounter = 0;
        private int AttackDelay = 0;

        public override void AI()
        {
            switch (AIState)
            {
                case (int)AICase.Idle:
                    break;
            }

            base.AI();

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AIState);
            writer.Write(AICounter);
            writer.Write(AttackDelay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIState = reader.ReadInt16();
            AICounter = reader.ReadInt16();
            AttackDelay = reader.ReadInt16();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTextureHandler(out Texture2D texture, out int yFrameCount);

            Rectangle drawRectangle = new Rectangle(0, texture.Height / yFrameCount * yFrame, texture.Width, texture.Height / yFrameCount);
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, NPC.Center - screenPos, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2f, NPC.scale, spriteEffects, 0);

            return false;
        }

        int frameCounter = 0;
        int yFrame = 0;
        private void NPCTextureHandler(out Texture2D texture, out int yFrameCount)
        {
            switch (AIState)
            {
                default:
                    texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "Town/Sojourn/Moxley").Value;
                    yFrameCount = 1;
                    break;
            }
        }
    }
}