using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.CustomCollision;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.NPCs
{
    public class LeftShelfCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                var firstEndpoint = NPC.TopLeft + new Vector2(8, -18);
                var secondEndpoint = NPC.TopRight + new Vector2(18 * 4 + 8, -18);

                var thirdEndpoint = firstEndpoint + new Vector2(0, 18 * 5 - 8);
                var fourthEndpoint = secondEndpoint + new Vector2(0, 18 * 5 - 8);
      
                colliders = new CollisionSurface[] {
                    new CollisionSurface(firstEndpoint, secondEndpoint, new int[] { CollisionID.Platform, 0, 0, 0 }, true),
                    new CollisionSurface(thirdEndpoint, fourthEndpoint, new int[] { CollisionID.Platform, 0, 0, 0 }, true),
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

    public class RightShelfCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                var firstEndpoint = NPC.TopLeft + new Vector2(8, -64);
                var secondEndpoint = NPC.TopRight + new Vector2(18 * 4 + 8, -64);

                var thirdEndpoint = firstEndpoint + new Vector2(0, 18 * 5 - 8);
                var fourthEndpoint = secondEndpoint + new Vector2(0, 18 * 5 - 8);

                colliders = new CollisionSurface[] {
                    new CollisionSurface(firstEndpoint, secondEndpoint, new int[] { 2, 0, 0, 0 }, true),
                    new CollisionSurface(thirdEndpoint, fourthEndpoint, new int[] { 2, 0, 0, 0 }, true),
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