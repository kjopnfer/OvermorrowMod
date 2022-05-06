using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class EyeOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private Vector2 InitialPosition;

        public override void SetDefaults(NPC npc)
        {
            if (npc.type != NPCID.EyeofCthulhu) return;

            npc.lifeMax = 3200;
        }

        public enum AIStates
        {
            Intro = -1,
            Selector = 0
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type != NPCID.EyeofCthulhu) return true;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            foreach (NPC minion in Main.npc)
            {
                if (minion.ModNPC is EyeStalk)
                {
                    npc.defense = 32;
                }
            }

            switch (npc.ai[0])
            {
                case (float)AIStates.Intro:
                    //npc.Center = player.Center - new Vector2(0, 50);
                    break;
                case (float)AIStates.Selector:
                    if (npc.ai[1]++ == 0)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            var entitySource = npc.GetSource_FromAI();
                            int index = NPC.NewNPC(entitySource, (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<EyeStalk>(), npc.whoAmI);

                            NPC minionNPC = Main.npc[index];
                            if (minionNPC.ModNPC is EyeStalk minion)
                            {
                                minion.StalkID = i;
                                minion.ParentIndex = npc.whoAmI;
                            }

                            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                            }
                        }
                    }
                    break;

            }

            npc.rotation = npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;

            return false;
        }

        /*public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 mountedCenter = npc.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;

            var drawPosition = npc.Center + new Vector2(0, 50);
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;
            float CHAIN_LENGTH = 8;

            float distance = Vector2.Distance(npc.Center, npc.Center + new Vector2(0, 50));
            float iterations = distance / CHAIN_LENGTH;

            Vector2 midPoint1 = npc.Center + new Vector2(25, 25).RotatedBy(NPC.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);
            Vector2 midPoint2 = drawPosition - new Vector2(-25, 25).RotatedBy(NPC.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);

            for (int i = 0; i < iterations; i++)
            {
                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                Vector2 position = ModUtils.Bezier(npc.Center, drawPosition, midPoint1, midPoint2, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }*/
    }
}