using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.CustomCollision;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class StairCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                // This is so that the player doesn't bump into the colliders while at the top of the stairs when walking across
                // Doing so incorrectly causes the player to bump upwards
                int topOffset = 3;

                List<CollisionSurface> surfaces = new();

                Vector2 startLeft = NPC.TopLeft;
                Vector2 startRight = NPC.TopRight;
                int tileHeight = 36;
                int[] platformIDs = new int[] { CollisionID.Platform, 0, 0, 0 };

                for (int i = 0; i < 5; i++) // Create 10 layers
                {
                    Vector2 offset = new Vector2(0, i * tileHeight + topOffset);
                    Vector2 left = startLeft + offset;
                    Vector2 right = startRight + offset;

                    surfaces.Add(new CollisionSurface(left, right, platformIDs, true));
                }

                colliders = surfaces.ToArray();
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