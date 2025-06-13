using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.CustomCollision;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class DiagonalStairsCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft + new Vector2(32, 2), NPC.BottomRight + new Vector2(-32, 0), new int[] { CollisionID.Platform, 0, 0, 0 }, true),
                    new CollisionSurface(NPC.TopLeft + new Vector2(0, 3), NPC.TopRight+ new Vector2(0, 3), new int[] { CollisionID.Platform, 0, 0, 0 }, true),
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
                    //for (int i = 0; i < collider.endPoints.Length; i++)
                    //{
                    //    var endPoint = Dust.NewDustDirect(collider.endPoints[i], 1, 1, DustID.RedTorch);
                    //    endPoint.noGravity = true;
                    //}
                }
            }
        }

        public override void PostAI()
        {
            //Main.NewText(TileLoader.GetTile(parentTile.TileType));
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }
    }
}