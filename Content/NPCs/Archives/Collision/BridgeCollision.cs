using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.CustomCollision;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class BridgeCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                var thirdEndpoint = NPC.TopRight + new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(-30));
                var fourthEndpoint = thirdEndpoint + new Vector2(54, 0);
                var fifthEndpoint = fourthEndpoint + new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(30));
                var sixthEndpoint = fifthEndpoint + new Vector2(64, 0);
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                    new CollisionSurface(NPC.TopRight, thirdEndpoint, new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                    new CollisionSurface(thirdEndpoint, fourthEndpoint, new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                    new CollisionSurface(fourthEndpoint, fifthEndpoint, new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                    new CollisionSurface(fifthEndpoint, sixthEndpoint, new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
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