using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class BridgeCollision : ModNPC
    {
        public Tile parentTile;
        public CollisionSurface[] colliders = null;

        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 11;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 1, 1, 0, 0 }, true) };
            }
            return true;
        }

        public override void AI()
        {
            if (colliders != null && colliders.Length == 1)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);

                var endPoint = Dust.NewDustDirect(colliders[0].endPoints[0], 1, 1, DustID.RedTorch);
                endPoint.noGravity = true;

                var endPoint2 = Dust.NewDustDirect(colliders[0].endPoints[1], 1, 1, DustID.RedTorch);
                endPoint.noGravity = true;
            }
        }

        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }

    }
}