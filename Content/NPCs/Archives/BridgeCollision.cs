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
            NPC.width = 64;
            NPC.height = 10;
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
            if (colliders == null)
            {
                var thirdEndpoint = NPC.TopRight + new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(-30));
                var fourthEndpoint = thirdEndpoint + new Vector2(54, 0);
                var fifthEndpoint = fourthEndpoint + new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(30));
                var sixthEndpoint = fifthEndpoint + new Vector2(64, 0);
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 1, 1, 0, 0 }, true),
                    new CollisionSurface(NPC.TopRight, thirdEndpoint, new int[] { 1, 1, 0, 0 }, true),
                    new CollisionSurface(thirdEndpoint, fourthEndpoint, new int[] { 1, 1, 0, 0 }, true),
                    new CollisionSurface(fourthEndpoint, fifthEndpoint, new int[] { 1, 1, 0, 0 }, true),
                    new CollisionSurface(fifthEndpoint, sixthEndpoint, new int[] { 1, 1, 0, 0 }, true),
                };
            }
            return true;
        }

        public override void AI()
        {
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    collider.Update();

                    // Debugging
                    /*for (int i = 0; i < collider.endPoints.Length; i++)
                    {
                        var endPoint = Dust.NewDustDirect(collider.endPoints[i], 1, 1, DustID.RedTorch);
                        endPoint.noGravity = true;
                    }*/
                }
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